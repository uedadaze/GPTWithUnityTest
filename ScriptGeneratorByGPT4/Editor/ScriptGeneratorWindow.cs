using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

public class ScriptGeneratorWindow : EditorWindow
{
    private string _apiKey = string.Empty;
    private string _question = string.Empty;
    private string _responseText = string.Empty;
    private bool _isRequestInProgress = false;
    private int _selectedModel = 0;
    private string[] _modelOptions = { "gpt-3.5-turbo", "gpt-4" };
    private const string ApiEndpoint = "https://api.openai.com/v1/chat/completions";

    private string scriptFileName = "Editor/GeneratedClass.cs";

    [MenuItem("Window/GPT Script Generator")]
    public static void ShowWindow()
    {
        GetWindow<ScriptGeneratorWindow>("GPT Script Generator");
    }

    private void OnEnable()
    {
        _isRequestInProgress = false;
    }

    private void OnGUI()
    {
        _apiKey = EditorGUILayout.TextField("API Key:", _apiKey);

        _selectedModel = EditorGUILayout.Popup("Model:", _selectedModel, _modelOptions);

        EditorGUILayout.LabelField("やらせたいことを入力して下さい:", EditorStyles.boldLabel);
        _question = EditorGUILayout.TextArea(_question, GUILayout.Height(100));

        if (_isRequestInProgress)
        {
            GUI.enabled = false;
            GUILayout.Button("回答待機中");
            GUI.enabled = true;
        }
        else
        {
            if (GUILayout.Button("OK"))
            {
                SendRequestToGptApi(_question);
            }
        }

        EditorGUILayout.LabelField("回答:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(_responseText, GUILayout.ExpandHeight(true));
    }

    private async void SendRequestToGptApi(string question)
    {
        _isRequestInProgress = true;

        string scriptAssistant =
            "You are AI system which supports unity game development. Please write a Unity editor script which fulfill the task I tell.\n" +
            "- It must be an unity editor script. Don't use MonoBehavior.\n" +
            "- It provides its functionality as a menu item. placed \"Edit\" > \"Do Generated Task\".\n" +
            "- It must does the task when it called. Don't include any editor window.\n" +
            "- Only include the script code. Don't add any explanation, block for markdown.\n" +
            "I'll tell a task, you output a script which fulfill the task after that.\n";

        using (UnityWebRequest request = new UnityWebRequest(ApiEndpoint, "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            var requestBody = new GptApiRequestBody
            {
                model = _modelOptions[_selectedModel],
                messages = new List<Message>
                {
                    new Message { role = "assistant", content = scriptAssistant },
                    new Message { role = "user", content = question }
                }
            };

            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonRequestBody));
            request.downloadHandler = new DownloadHandlerBuffer();

            await SendWebRequest(request);

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                _responseText = "Error: " + request.error;
            }
            else
            {
                GptApiResponse response = JsonConvert.DeserializeObject<GptApiResponse>(request.downloadHandler.text);
                _responseText = response.choices[0].message.content;

                SaveGeneratedCodeAsScript(response.choices[0].message.content, scriptFileName);
            }

            _isRequestInProgress = false;
        }
    }

    private Task SendWebRequest(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<object>();
        request.SendWebRequest().completed += operation =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                tcs.SetResult(null);
            }
            else
            {
                tcs.SetException(new Exception(request.error));
            }
        };
        return tcs.Task;
    }

    private void SaveGeneratedCodeAsScript(string code, string fileName)
    {
        Debug.Log("Generate.");

        string path = Path.Combine(Application.dataPath, fileName);

        File.WriteAllText(path, code);

        AssetDatabase.Refresh();
        Debug.Log("Generated script saved.");
    }
}

[Serializable]
public class GptApiRequestBody
{
    public string model;
    public List<Message> messages;
}

[Serializable]
public class Message
{
    public string role;
    public string content;
}

[Serializable]
public class GptApiResponse
{
    public List<Choice> choices;
}

[Serializable]
public class Choice
{
    public Message message;
}