public interface NPCMessageObserver
{
    public void UpdateNPCMessageObserver();
}

public interface INPCMessageViewClassifier
{
    public void RegisterNPCMessageObserver(NPCMessageObserver observer);
    public string GetNPCMessage();
    public void NotifyNPCMessageObservers(string NPCName, string messageContentType);
}

public class NPCMessageViewClassifier : INPCMessageViewClassifier
{
    private System.Random random = new System.Random();

    private System.Collections.Generic.List<NPCMessageObserver> NPCMessageObservers = new System.Collections.Generic.List<NPCMessageObserver>();

    private string message;

    // Observer µî·Ï.
    public void RegisterNPCMessageObserver(NPCMessageObserver observer)
    {
        NPCMessageObservers.Add(observer);
    }

    public string GetNPCMessage()
    {
        return message;
    }

    public void NotifyNPCMessageObservers(string NPCName, string messageContentType)
    {
        GetMessageContent(NPCName, messageContentType);

        for (int i = 0; i < NPCMessageObservers.Count; i++)
        {
            NPCMessageObservers[i].UpdateNPCMessageObserver();
        }
    }

    private void GetMessageContent(string name, string messageContentType)
    {
        UnityEngine.TextAsset messageText = UnityEngine.Resources.Load<UnityEngine.TextAsset>("GameData/NPCMessageInformationTable");
        Newtonsoft.Json.Linq.JArray messageTable = Newtonsoft.Json.Linq.JArray.Parse(messageText.ToString());

        Newtonsoft.Json.Linq.JArray contents = new Newtonsoft.Json.Linq.JArray();

        for (int i = 0; i < messageTable.Count; i++)
        {
            if (name.Equals((string)messageTable[i]["NPCName"]))
            {
                contents = (Newtonsoft.Json.Linq.JArray)messageTable[i]["MessageContent"][(string)messageContentType];
            }
        }

        this.message = (string)contents[random.Next(contents.Count)];
    }

}
