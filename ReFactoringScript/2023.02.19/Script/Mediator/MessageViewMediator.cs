using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

public enum MessageViewType
{
    ConfirmMessageView,
    YesOrNoMessageView
}

public interface MessageObserver
{
    public void UpdateMessageObserver();
}

// View�� Controller�� Model�� �����ϱ����� ����ϴ� Interface�Դϴ�.
public interface IMessageViewMediator
{
    public void RegisterMessageObserver(MessageObserver observer);
    public string[] GetMessage();
    public void NotifyMessageObservers(int messageCode);
}

public class MessageViewMediator : IMessageViewMediator
{
    private List<MessageObserver> messageCodeObservers = new List<MessageObserver>();
    
    private string[] messageRow = new string[4];

    // Observer ���.
    public void RegisterMessageObserver(MessageObserver observer)
    {
        messageCodeObservers.Add(observer);
    }

    // Message ����
    public string[] GetMessage()
    {
        return messageRow;
    }

    // ��ϵ� Observer���� �޽��� ���� �뺸.
    public void NotifyMessageObservers(int messageCode)
    {
        // �޽��� ���� ��ü�� ����.
        GetMessageContent(messageCode);

        // Observer���� �뺸 -> View���� Content�� ��û�ؼ� ������.
        for (int i = 0; i < messageCodeObservers.Count; i++)
        {
            messageCodeObservers[i].UpdateMessageObserver();
        }
    }

    // �޽��� �ڵ带 ����, �޽��� ���̺��� �ش� ���� ���� �� ����.
    private void GetMessageContent(int messageCode)
    {
        TextAsset messageText = Resources.Load<TextAsset>("MessageData/MessageData");
        JArray messageTable = JArray.Parse(messageText.ToString());

        for (int i = 0; i < messageTable.Count; i++)
        {
            if (Convert.ToInt32(messageTable[i]["MessageCode"]) == messageCode)
            {
                messageRow[0] = (string)messageTable[i]["MessageCode"];
                messageRow[1] = (string)messageTable[i]["MessageViewType"];
                messageRow[2] = (string)messageTable[i]["Title"];
                messageRow[3] = (string)messageTable[i]["Detail"];
            }
        }
    }
}
