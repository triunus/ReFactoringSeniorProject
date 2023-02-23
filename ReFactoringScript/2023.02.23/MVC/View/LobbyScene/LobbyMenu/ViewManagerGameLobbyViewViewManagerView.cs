using UnityEngine;
using UnityEngine.UI;

public interface IViewManagerGameLobbyViewController
{
    public void OnClickedChangeMenuButton(MenuName nextMenuName);
}

public interface IViewManagerGameLobbyView
{
    public void DisableCurrentMenuPanel();
}

// GameLobby
public class ViewManagerGameLobbyView : IViewManagerGameLobbyView, CurrentMenuNameObserver, NPCMessageObserver
{
    private ICurrentMenuNameObserver currentMenuNameModel;
    private INPCMessageViewClassifier NPCMessageViewMediator;

    private IViewManagerGameLobbyViewController viewManagerViewController;

    private RectTransform canvas;

    private RectTransform gameLobbyPanel;

    private RectTransform subPanel;

    private RectTransform subPanelTitle;

    private TMPro.TextMeshProUGUI venderLine;

    private MenuName currentMenuName;
    private string venderNPCLine = null;

    public ViewManagerGameLobbyView(IViewManagerModel model, IViewManagerGameLobbyViewController controller)
    {
        this.currentMenuNameModel = model;
        this.NPCMessageViewMediator = model;

        this.viewManagerViewController = controller;

        currentMenuNameModel.RegisterCurrentMenuNameObserver(this);
        NPCMessageViewMediator.RegisterNPCMessageObserver(this);

        currentMenuName = MenuName.GameLobbyMenu;

        SetView();
    }

    private void SetView()
    {
        canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();

        gameLobbyPanel = canvas.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        gameLobbyPanel.GetChild(2).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { viewManagerViewController.OnClickedChangeMenuButton(MenuName.CharacterMenu); });    // Characdter
        gameLobbyPanel.GetChild(2).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { viewManagerViewController.OnClickedChangeMenuButton(MenuName.InventoryMenu); });    // Inventory
        gameLobbyPanel.GetChild(2).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { viewManagerViewController.OnClickedChangeMenuButton(MenuName.VenderMenu); });    // Vender
//        gameLobbyPanel.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { });    // Market
//        gameLobbyPanel.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { });    // Option

        subPanel = canvas.GetChild(0).GetChild(1).GetComponent<RectTransform>();
        subPanel.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { viewManagerViewController.OnClickedChangeMenuButton(MenuName.GameLobbyMenu); }); // GameLobby
        subPanel.GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { viewManagerViewController.OnClickedChangeMenuButton(MenuName.VenderMenu); });  // Vender
                                                                                                                                                                                                                  //        subPanel.GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { });  // Gamble
        subPanelTitle = subPanel.GetChild(0).GetChild(1).GetChild(0).GetComponent<RectTransform>();

        venderLine = canvas.GetChild(0).GetChild(1).GetChild(3).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void UpdateCurrentMenuNameObserver()
    {
        currentMenuName = currentMenuNameModel.GetCurrentMenuName();

        ChangeTitle();
        ActivateNextMenuPanel();
    }

    public void UpdateNPCMessageObserver()
    {
        venderNPCLine = this.NPCMessageViewMediator.GetNPCMessage();

        SetVenderNPCLine();
    }

    private void SetVenderNPCLine()
    {
        venderLine.text = this.venderNPCLine;
    }

    public void DisableCurrentMenuPanel()
    {
        switch (currentMenuName)
        {
            case MenuName.GameLobbyMenu:
                gameLobbyPanel.gameObject.SetActive(false);
                break;
            case MenuName.CharacterMenu:
                subPanel.gameObject.SetActive(false);
                subPanel.GetChild(0).gameObject.SetActive(false);
                subPanel.GetChild(1).gameObject.SetActive(false);
                break;
            case MenuName.InventoryMenu:
                subPanel.gameObject.SetActive(false);
                subPanel.GetChild(0).gameObject.SetActive(false);
                subPanel.GetChild(2).gameObject.SetActive(false);
                break;
            case MenuName.VenderMenu:
                subPanel.gameObject.SetActive(false);
                subPanel.GetChild(3).gameObject.SetActive(false);
                subPanel.GetChild(0).gameObject.SetActive(false);
                subPanel.GetChild(3).GetChild(1).gameObject.SetActive(false);
                break;
/*            case MenuName.GambleMenu:
                subPanel.gameObject.SetActive(false);
                subPanel.GetChild(3).gameObject.SetActive(false);
                subPanel.GetChild(3).GetChild(2).gameObject.SetActive(false);
                break;
            case MenuName.MarketMenu:
                subPanel.gameObject.SetActive(false);
                subPanel.GetChild(4).gameObject.SetActive(false);
                subPanel.GetChild(4).GetChild(1).gameObject.SetActive(false);
                break;
            case MenuName.ProductRegistrationMenu:
                subPanel.gameObject.SetActive(false);
                subPanel.GetChild(4).gameObject.SetActive(false);
                subPanel.GetChild(4).GetChild(2).gameObject.SetActive(false);
                break;
            case MenuName.MyTransactionMenu:
                subPanel.gameObject.SetActive(false);
                subPanel.GetChild(4).gameObject.SetActive(false);
                subPanel.GetChild(4).GetChild(3).gameObject.SetActive(false);
                break;
            case MenuName.OptionMenu:
                subPanel.gameObject.SetActive(false);
                subPanel.GetChild(5).gameObject.SetActive(false);
                break;*/
            default:
                break;
        }

    }

    private void ChangeTitle()
    {
        subPanelTitle.GetComponent<TMPro.TextMeshProUGUI>().text = currentMenuName.ToString().Replace("Menu", "");
    }

    private void ActivateNextMenuPanel()
    {
        switch (currentMenuName)
        {
            case MenuName.GameLobbyMenu:
                gameLobbyPanel.gameObject.SetActive(true);
                break;
            case MenuName.CharacterMenu:
                subPanel.gameObject.SetActive(true);
                subPanel.GetChild(0).gameObject.SetActive(true);
                subPanel.GetChild(1).gameObject.SetActive(true);
                break;
            case MenuName.InventoryMenu:
                subPanel.gameObject.SetActive(true);
                subPanel.GetChild(0).gameObject.SetActive(true);
                subPanel.GetChild(2).gameObject.SetActive(true);
                break;
            case MenuName.VenderMenu:

                subPanel.gameObject.SetActive(true);
                subPanel.GetChild(0).gameObject.SetActive(true);
                subPanel.GetChild(3).gameObject.SetActive(true);
                subPanel.GetChild(3).GetChild(1).gameObject.SetActive(true);
                break;
/*            case MenuName.GambleMenu:
                subPanel.gameObject.SetActive(true);
                subPanel.GetChild(3).gameObject.SetActive(true);
                subPanel.GetChild(3).GetChild(2).gameObject.SetActive(true);
                break;
            case MenuName.MarketMenu:
                subPanel.gameObject.SetActive(true);
                subPanel.GetChild(4).gameObject.SetActive(true);
                subPanel.GetChild(4).GetChild(1).gameObject.SetActive(true);
                break;
            case MenuName.ProductRegistrationMenu:
                subPanel.gameObject.SetActive(true);
                subPanel.GetChild(4).gameObject.SetActive(true);
                subPanel.GetChild(4).GetChild(2).gameObject.SetActive(true);
                break;
            case MenuName.MyTransactionMenu:
                subPanel.gameObject.SetActive(true);
                subPanel.GetChild(4).gameObject.SetActive(true);
                subPanel.GetChild(4).GetChild(3).gameObject.SetActive(true);
                break;
            case MenuName.OptionMenu:
                subPanel.gameObject.SetActive(true);
                subPanel.GetChild(5).gameObject.SetActive(true);
                break;*/
            default:
                break;
        }
    }
}
