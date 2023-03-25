**筆者:**
こんにちは、GPT-4。今日はあなたにUnityにおけるゲーム開発（プログラミング言語はC#を使用）をサポートしていただきたく思います。
わたしがこれから質問をするので、答えて下さい。

**GPT-4:**
こんにちは！UnityとC#に関する質問があれば、どんどんお聞きください。喜んでお手伝いいたします。

**筆者:**
それでは早速質問します。

【目的】
現在、UnityのEditor拡張からGPTのAPIにアクセスして質問をすることができるシステムを作りたいと思っています。
TextFieldに質問文を打ち込み、OKボタンをクリックすることでGPT APIにアクセスし回答を受け取れるEditor拡張のコードを書いていただけますでしょうか？

【要件】
GPTのAPIのエンドポイントは「https://api.openai.com/v1/chat/completions」です。
また必要なヘッダー情報として「"Content-Type: application/json"」と「"Authorization: Bearer $OPENAI_API_KEY"」があります。
加えて送信情報としては使用したいモデルを表す「"model"」と会話履歴のリストである「"messages"」があり、messages内の各会話履歴はその種別を表す「role」と内容を表す「content」の2つの要素を持っています。「messages」の最初の要素としては「role」を「assistant」として、「content」を「You are AI system which supports unity game development」としてほしいです。

よろしくお願いします。

**GPT-4:**
了解しました。以下に、UnityのEditor拡張を作成するためのC#コードを示します。このコードは、Unityのエディター内で動作し、質問を入力し、OKボタンをクリックすることでGPT APIにアクセスして回答を受け取ることができます。

まず、Unityプロジェクトに以下のパッケージをインストールしてください。

Newtonsoft.Json（Json.NET）：Jsonデータのシリアル化・デシリアル化のために使用します。
UnityWebRequest：UnityでHTTP通信を行うためのパッケージです。
次に、下記のコードを「GptApiEditorWindow.cs」という名前でEditorフォルダ内に保存してください。

~~~
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GptApiEditorWindow : EditorWindow
{
    private string _question = string.Empty;
    private string _responseText = string.Empty;
    private const string ApiEndpoint = "https://api.openai.com/v1/chat/completions";
    private const string OpenAiApiKey = "your_api_key_here";

    [MenuItem("Window/GPT-4 API")]
    public static void ShowWindow()
    {
        GetWindow<GptApiEditorWindow>("GPT-4 API");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("質問を入力してください:", EditorStyles.boldLabel);
        _question = EditorGUILayout.TextField("質問:", _question);

        if (GUILayout.Button("OK"))
        {
            StartCoroutine(SendRequestToGptApi(_question));
        }

        EditorGUILayout.LabelField("回答:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(_responseText, GUILayout.Height(100));
    }

    private IEnumerator SendRequestToGptApi(string question)
    {
        var request = new UnityWebRequest(ApiEndpoint, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + OpenAiApiKey);

        var requestBody = new GptApiRequestBody
        {
            model = "text-davinci-002",
            messages = new List<Message>
            {
                new Message { role = "assistant", content = "You are AI system which supports unity game development" },
                new Message { role = "user", content = question }
            }
        };

        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonRequestBody));
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            _responseText = "Error: " + request.error;
        }
        else
        {
            GptApiResponse response = JsonConvert.DeserializeObject<GptApiResponse>(request.downloadHandler.text);
            _responseText = response.choices[0].message.content;
        }
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
~~~

`OpenAi

**筆者:**
続けて下さい。

**GPT-4:**
すみません。誤って送信してしまいました。

OpenAiApiKeyの部分には、実際のAPIキーを指定してください。APIキーをハードコードするのはセキュリティ上のリスクがあるため、実際にはUnityプロジェクトの設定や環境変数からAPIキーを読み込むようにすることをお勧めします。

このコードを使って、Unityエディター内にGPT-4 APIウィンドウが表示されます。質問を入力してOKボタンをクリックすることで、指定されたAPIエンドポイントにリクエストを送信し、回答を受け取ることができます。回答はウィンドウ内の「回答」の部分に表示されます。

このコードがお望みの機能を満たしていることを願っています。何か質問や追加のサポートが必要な場合は、お気軽にお知らせください。

**筆者:**
頂いたコードを実行した所、下記のエラーが発生しました。
Assets/Editor/GptApiEditorWindow.cs(29,13): error CS0103: The name 'StartCoroutine' does not exist in the current context

Editor拡張のため、MonobehaviorのメソッドであるStartCoroutineは実装されていないようです。
Editor拡張としても動作するように修正をお願いします。

**GPT-4:**
すみません、その点を見落としていました。Editor拡張ではStartCoroutineを使用することができませんので、async/awaitを利用して非同期処理を行うように修正します。以下が修正したコードです：

~~~
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GptApiEditorWindow : EditorWindow
{
    private string _question = string.Empty;
    private string _responseText = string.Empty;
    private const string ApiEndpoint = "https://api.openai.com/v1/chat/completions";
    private const string OpenAiApiKey = "your_api_key_here";

    [MenuItem("Window/GPT-4 API")]
    public static void ShowWindow()
    {
        GetWindow<GptApiEditorWindow>("GPT-4 API");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("質問を入力してください:", EditorStyles.boldLabel);
        _question = EditorGUILayout.TextField("質問:", _question);

        if (GUILayout.Button("OK"))
        {
            SendRequestToGptApi(_question);
        }

        EditorGUILayout.LabelField("回答:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(_responseText, GUILayout.Height(100));
    }

    private async void SendRequestToGptApi(string question)
    {
        using (UnityWebRequest request = new UnityWebRequest(ApiEndpoint, "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + OpenAiApiKey);

            var requestBody = new GptApiRequestBody
            {
                model = "text-davinci-002",
                messages = new List<Message>
                {
                    new Message { role = "assistant", content = "You are AI system which supports unity game development" },
                    new Message { role = "user", content = question }
                }
            };

            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonRequestBody));
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                _responseText = "Error: " + request.error;
            }
            else
            {
                GptApiResponse response = JsonConvert.DeserializeObject<GptApiResponse>(request.downloadHandler.text);
                _responseText = response.choices[0].message.content;
            }
        }
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
~~~

この修正で、エディタ拡張が正常に動作するはずです。もし何か問題があれば、お気軽にお知らせください。

**筆者:**
すみません、今度は
Assets/Editor/GptApiEditorWindow.cs(57,13): error CS1061: 'UnityWebRequestAsyncOperation' does not contain a definition for 'GetAwaiter' and no accessible extension method 'GetAwaiter' accepting a first argument of type 'UnityWebRequestAsyncOperation' could be found (are you missing a using directive or an assembly reference?)
というエラーが出てしまいました。
対処をお願いできますでしょうか。

**GPT-4:**
すみません、そのエラーは非同期処理に関連するものです。UnityWebRequest.SendWebRequest()はUnityWebRequestAsyncOperationを返しますが、GetAwaiter()が定義されていないためawaitを直接使用することはできません。

その問題を解決するために、以下のようにSystem.Threading.Tasks.TaskCompletionSourceを使用してTaskを作成し、そのTaskをawaitするように修正しました。

~~~
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GptApiEditorWindow : EditorWindow
{
    private string _question = string.Empty;
    private string _responseText = string.Empty;
    private const string ApiEndpoint = "https://api.openai.com/v1/chat/completions";
    private const string OpenAiApiKey = "your_api_key_here";

    [MenuItem("Window/GPT-4 API")]
    public static void ShowWindow()
    {
        GetWindow<GptApiEditorWindow>("GPT-4 API");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("質問を入力してください:", EditorStyles.boldLabel);
        _question = EditorGUILayout.TextField("質問:", _question);

        if (GUILayout.Button("OK"))
        {
            SendRequestToGptApi(_question);
        }

        EditorGUILayout.LabelField("回答:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(_responseText, GUILayout.Height(100));
    }

    private async void SendRequestToGptApi(string question)
    {
        using (UnityWebRequest request = new UnityWebRequest(ApiEndpoint, "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + OpenAiApiKey);

            var requestBody = new GptApiRequestBody
            {
                model = "text-davinci-002",
                messages = new List<Message>
                {
                    new Message { role = "assistant", content = "You are AI system which supports unity game development" },
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
            }
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
~~~

**筆者:**
続けて下さい。

**GPT-4:**
すみません、再び誤って送信してしまいました。続きを記載します。

~~~
{
    public Message message;
}
~~~

この修正により、エラーなくエディタ拡張が動作するはずです。再度、このコードで問題が解決するかどうかお試しください。何か他に質問やサポートが必要な場合は、お気軽にお知らせください。

**筆者:**
ありがとうございます。UnityからGPTに質問して、答えてくれました！
いくつか修正をお願いしても良いでしょうか。以下の点についてお願いします。

・質問を複数行入力できた方が良いので、フォームをTextAreaにしてください。
・回答について、ウィンドウを広げたらその分枠が広がるようにしてください。
・OKボタンを押したら、回答が来るまでボタンを押せなくして、ボタンのラベルを「回答待機中」に切り替えて下さい。回答が来たら「OK」に戻して再び押せるようにしてください。
・質問の上に、「使用するAPIキーを入力するテキストフォーム」と「モデルの種類（GptApiRequestBodyのmodel）を"gpt-3.5-turbo"と"gpt-4"のいずれかから選択できるプルダウンメニュー」を追加して下さい。

すべてのコードを出すと返答が長くなってしまうと思うので、修正すべき箇所だけ教えていただければ大丈夫です。

**GPT-4:**
ご要望に合わせて、修正すべき箇所を以下に示します。

1. 質問を複数行入力できるようにするために、_questionのEditorGUILayout.TextFieldをEditorGUILayout.TextAreaに変更します。
~~~
_question = EditorGUILayout.TextArea(_question, GUILayout.Height(100));
~~~

2. 回答のウィンドウが広げたら枠が広がるようにするために、GUILayout.HeightをGUILayout.ExpandHeightに変更します。
~~~
EditorGUILayout.TextArea(_responseText, GUILayout.ExpandHeight(true));
~~~

3. OKボタンの制御を行うために、bool _isRequestInProgressフィールドを追加し、以下のようにSendRequestToGptApiメソッドを修正します。
~~~
private async void SendRequestToGptApi(string question)
{
    _isRequestInProgress = true;

    // ... その他のコード ...

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
        _responseText = "Error: " + request.error;
    }
    else
    {
        GptApiResponse response = JsonConvert.DeserializeObject<GptApiResponse>(request.downloadHandler.text);
        _responseText = response.choices[0].message.content;
    }

    _isRequestInProgress = false;
}
~~~

また、OnGUIメソッド内でOKボタンの表示を切り替えるようにします。

~~~
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
~~~

4. APIキーの入力とモデル選択のプルダウンメニューを追加するために、OnGUIメソッドに以下のコードを追加します。また、GptApiRequestBodyのmodelを削除し、メソッド内で直接セットするようにします。

~~~
private string _apiKey = string.Empty;
private int _selectedModel = 0;
private string[] _modelOptions = { "gpt-3.5-turbo", "gpt-4" };

// ...

_apiKey = EditorGUILayout.TextField("API Key:", _apiKey);

_selectedModel = EditorGUILayout.Popup("Model:", _selectedModel, _modelOptions);

// ...

var requestBody = new GptApiRequestBody
{
    model = _modelOptions[_selectedModel],
    messages = new List<Message>
    {
        // ...
    }
};
~~~

これらの修正を加えることで、ご要望に沿ったエディタ拡張が実現されるはずです。もし何か質問やサポートが必要な場合は、お気軽にお知らせください。

**筆者:**
コードを実行したのですが、ボタンが「回答待機中」のままになっていてクリックできません。回答待機中か否かの判定がうまくいっておらず、起動した直後から回答待機中になってしまっているようです。
修正をお願いします。

**GPT-4:**
すみません、その問題を解決するために、_isRequestInProgressを初期化する方法を変更します。_isRequestInProgressの初期値をfalseに設定し、OnEnableメソッドを使用して_isRequestInProgressをfalseにリセットします。

以下に修正箇所を示します。

~~~
private bool _isRequestInProgress = false;

private void OnEnable()
{
    _isRequestInProgress = false;
}
~~~

これで、エディタウィンドウが開いたときにボタンが「回答待機中」の状態にならないようになります。もし何か質問やサポートが必要な場合は、お気軽にお知らせください。

---

