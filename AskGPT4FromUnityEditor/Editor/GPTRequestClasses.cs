using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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