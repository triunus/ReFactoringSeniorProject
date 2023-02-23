using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

/// <summary>
/// Message에 따라 출력하고자 하는 View를 구분하는데 사용한다.
/// </summary>
public enum MessageViewType
{
    ConfirmMessageView,
    YesOrNoMessageView
}

/// <summary>
/// Observer 패턴의 Subscriber들이 상속받아 사용하는 Interface이다.
/// </summary>
public interface MessageObserver
{
    /// <summary>
    /// Model의 정보가 변경되면 호출된다.
    /// </summary>
    public void UpdateMessageObserver();
}

/// <summary>
/// Observer 패턴의 Subject들이 상속받아 사용하는 Interface이다.
/// </summary>
public interface IMessageObserver
{
    /// <summary>
    /// Subscriber인 View들이 Subject를 구독할 때 사용하는 메소드입니다.
    /// </summary>
    public void RegisterMessageObserver(MessageObserver observer);
    /// <summary>
    /// Model의 정보를 가져올 때 사용하는 메소드입니다.
    /// </summary>
    public string[] GetMessage();
}

/// <summary>
/// Model에서 MessageClassifier의 기능을 필요로 할 때 사용하는 Interface입니다.
/// </summary>
public interface IMessageClassifier : IMessageObserver
{
    /// <summary>
    /// MessageClassifier 객체를 리턴해줍니다.
    /// </summary>
    public IMessageClassifier GetMessageClassifier();
    /// <summary>
    /// 출력하고 싶은 MessageCode를 인자로 보내, 메시지를 출력하도록 합니다.
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

        GetMessageContent();                // 메시지 내용(messageRow) 갱신.        
        NotifyMessageObservers();           // Subscriber들에게 통보.
    }

    // 2. IMessageObserver 구현.
    public void RegisterMessageObserver(MessageObserver observer)
    {
        messageCodeObservers.Add(observer);
    }

    public string[] GetMessage()
    {
        return messageRow;
    }

    // 3. Subject가 Subscriber들에게 변경 여부를 통보하는 메소드입니다.
    private void NotifyMessageObservers()
    {
        for (int i = 0; i < messageCodeObservers.Count; i++)
        {
            messageCodeObservers[i].UpdateMessageObserver();
        }
    }

    // 메시지 코드를 통해, 메시지 테이블에서 해당 행을 추출 및 저장.
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
