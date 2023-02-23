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

// 1. Observer �������̽�.
///<summary>
/// ViewManagerModel�� ������ �ʿ���ϴ� View�� �Ʒ� �������̽��� ��ӹ޾� ����Ѵ�. 
///</summary>
public interface CurrentMenuNameObserver
{
    public void UpdateCurrentMenuNameObserver();
}

/// <summary>
/// View�� Model�� �ڽ��� ����ϴµ� ����ϴ� �������̽��̴�.
/// Model�� ������ ������ View�鿡�� �˸���, View���� �Ʒ��� �������̽��� ���� Model�� ������ �����´�.
/// </summary>
public interface ICurrentMenuNameObserver
{
    public void RegisterCurrentMenuNameObserver(CurrentMenuNameObserver observer);
    public MenuName GetCurrentMenuName();
}

/// <summary>
/// ������� �Է¿� �����ϴ� Controller���� Model�� �����ϴ� �κ��̴�.
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

    // Observer ����.
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
