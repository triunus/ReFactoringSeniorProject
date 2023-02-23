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

// View와 Controller가 Model에 접근하기위해 사용하는 Interface입니다.
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

    // Observer 등록.
    public void RegisterMessageObserver(MessageObserver observer)
    {
        messageCodeObservers.Add(observer);
    }

    // Message 리턴
    public string[] GetMessage()
    {
        return messageRow;
    }

    // 등록된 Observer에게 메시지 변경 통보.
    public void NotifyMessageObservers(int messageCode)
    {
        // 메시지 내용 객체에 저장.
        GetMessageContent(messageCode);

        // Observer에게 통보 -> View들은 Content를 요청해서 가져감.
        for (int i = 0; i < messageCodeObservers.Count; i++)
        {
            messageCodeObservers[i].UpdateMessageObserver();
        }
    }

    // 메시지 코드를 통해, 메시지 테이블에서 해당 행을 추출 및 저장.
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
