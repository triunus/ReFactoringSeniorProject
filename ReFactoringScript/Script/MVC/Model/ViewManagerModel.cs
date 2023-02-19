using System.Collections.Generic;

public enum MenuName
{
    GameLobbyMenu,
    CharacterMenu,
    InventoryMenu,
    VenderMenu,
    GambleMenu,
    MarketMenu,
    ProductRegistrationMenu,
    MyTransactionMenu,
    OptionMenu
}

// 1. Observer 인터페이스.
///<summary>
/// ViewManagerModel의 정보를 필요로하는 View가 아래 인터페이스를 상속받아 사용한다. 
///</summary>
public interface CurrentMenuNameObserver
{
    public void UpdateCurrentMenuNameObserver();
}

/// <summary>
/// View가 Model에 자신을 등록하는데 사용하는 인터페이스이다.
/// Model이 정보의 갱신을 View들에게 알리면, View들은 아래의 인터페이스를 통해 Model의 정보를 가져온다.
/// </summary>
public interface ICurrentMenuNameObserver
{
    public void RegisterCurrentMenuNameObserver(CurrentMenuNameObserver observer);
    public MenuName GetCurrentMenuName();
}

/// <summary>
/// 사용자의 입력에 대응하는 Controller에서 Model에 접근하는 부분이다.
/// </summary>
public interface IViewManagerModel : ICurrentMenuNameObserver, INPCMessageViewMediator
{
    public void ChangeMenuOperation(MenuName nextMenuName);
}

public class ViewManagerModel : IViewManagerModel
{
    private INPCMessageViewMediator NPCMessageMediator = null;

    private List<CurrentMenuNameObserver> currentMenuNameObservers = new List<CurrentMenuNameObserver>();

    private MenuName currentMenuName;

    public ViewManagerModel()
    {
        NPCMessageMediator = new NPCMessageViewMediator();
    }

    // Observer 구현.
    public void RegisterCurrentMenuNameObserver(CurrentMenuNameObserver observer)
    {
        currentMenuNameObservers.Add(observer);
    }

    public MenuName GetCurrentMenuName()
    {
        return currentMenuName;
    }

    public void NotifyCurrentMenuNameObservers()
    {
        for(int i = 0; i < currentMenuNameObservers.Count; i++)
        {
            currentMenuNameObservers[i].UpdateCurrentMenuNameObserver();
        }
    }


    public void ChangeMenuOperation(MenuName nextMenuName)
    {
        this.currentMenuName = nextMenuName;

        if(currentMenuName.Equals(MenuName.VenderMenu))
        {
            this.NotifyNPCMessageObservers("VenderNPC", "Greeting");
        }

        NotifyCurrentMenuNameObservers();
    }

    public void RegisterNPCMessageObserver(NPCMessageObserver observer)
    {
        NPCMessageMediator.RegisterNPCMessageObserver(observer);
    }

    public string GetNPCMessage()
    {
        return NPCMessageMediator.GetNPCMessage();
    }

    public void NotifyNPCMessageObservers(string NPCName, string messageContentType)
    {
        NPCMessageMediator.NotifyNPCMessageObservers(NPCName, messageContentType);
    }
}
