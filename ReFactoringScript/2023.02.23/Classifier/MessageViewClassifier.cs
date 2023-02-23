using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

/// <summary>
/// Message�� ���� ����ϰ��� �ϴ� View�� �����ϴµ� ����Ѵ�.
/// </summary>
public enum MessageViewType
{
    ConfirmMessageView,
    YesOrNoMessageView
}

/// <summary>
/// Observer ������ Subscriber���� ��ӹ޾� ����ϴ� Interface�̴�.
/// </summary>
public interface MessageObserver
{
    /// <summary>
    /// Model�� ������ ����Ǹ� ȣ��ȴ�.
    /// </summary>
    public void UpdateMessageObserver();
}

/// <summary>
/// Observer ������ Subject���� ��ӹ޾� ����ϴ� Interface�̴�.
/// </summary>
public interface IMessageObserver
{
    /// <summary>
    /// Subscriber�� View���� Subject�� ������ �� ����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public void RegisterMessageObserver(MessageObserver observer);
    /// <summary>
    /// Model�� ������ ������ �� ����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public string[] GetMessage();
}

/// <summary>
/// Model���� MessageClassifier�� ����� �ʿ�� �� �� ����ϴ� Interface�Դϴ�.
/// </summary>
public interface IMessageClassifier : IMessageObserver
{
    /// <summary>
    /// MessageClassifier ��ü�� �������ݴϴ�.
    /// </summary>
    public IMessageClassifier GetMessageClassifier();
    /// <summary>
    /// ����ϰ� ���� MessageCode�� ���ڷ� ����, �޽����� ����ϵ��� �մϴ�.
    /// </summary>
    public void ChangeMessageCode(int messageCode);
}

public class MessageClassifier : IMessageClassifier
{
    private List<MessageObserver> messageCodeObservers = new List<MessageObserver>();

    private int messageCode;
    private string[] messageRow = new string[4];

    public IMessageClassifier GetMessageClassifier()
    {
        return this;
    }

    public void ChangeMessageCode(int messageCode)
    {
        this.messageCode = messageCode;

        GetMessageContent();                // �޽��� ����(messageRow) ����.        
        NotifyMessageObservers();           // Subscriber�鿡�� �뺸.
    }

    // 2. IMessageObserver ����.
    public void RegisterMessageObserver(MessageObserver observer)
    {
        messageCodeObservers.Add(observer);
    }

    public string[] GetMessage()
    {
        return messageRow;
    }

    // 3. Subject�� Subscriber�鿡�� ���� ���θ� �뺸�ϴ� �޼ҵ��Դϴ�.
    private void NotifyMessageObservers()
    {
        for (int i = 0; i < messageCodeObservers.Count; i++)
        {
            messageCodeObservers[i].UpdateMessageObserver();
        }
    }

    // �޽��� �ڵ带 ����, �޽��� ���̺��� �ش� ���� ���� �� ����.
    private void GetMessageContent()
    {
        TextAsset messageText = Resources.Load<TextAsset>("MessageData/MessageData");
        JArray messageTable = JArray.Parse(messageText.ToString());

        for (int i = 0; i < messageTable.Count; i++)
        {
            if (Convert.ToInt32(messageTable[i]["MessageCode"]) == this.messageCode)
            {
                messageRow[0] = (string)messageTable[i]["MessageCode"];
                messageRow[1] = (string)messageTable[i]["MessageViewType"];
                messageRow[2] = (string)messageTable[i]["Title"];
                messageRow[3] = (string)messageTable[i]["Detail"];
            }
        }
    }
}
