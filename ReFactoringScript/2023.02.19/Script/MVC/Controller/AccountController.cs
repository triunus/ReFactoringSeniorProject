public interface IAccountController : ILoginSceneController { }

public class AccountController : IAccountController, IConfirmMessageViewController, IYesOrNoMessageViewController
{
    private IAccountModel accountModel;

    private ILoginSceneView loginSceneView;
    private IConfirmMessageView confirmMessageView;
    private IYesOrNoMessageView yesOrNoMessageView;

    public AccountController(IAccountModel accountModel)
    {
        this.accountModel = accountModel;

        // ���� view ����
        loginSceneView = new LoginSceneView(this);

        confirmMessageView = new ConfirmMessageView(accountModel, this);
        yesOrNoMessageView = new YesOrNoMessageView(accountModel, this);
    }

    // 1. IConfirmMessageViewController Interface ����.
    public void OnConfirmButtonClicked(int messageCode)
    {
        confirmMessageView.DeleteConfirmMessagePanel();
        loginSceneView.ActivateButton();

        if (messageCode == 21)
        {
            accountModel.LoginSuccess();
        }
    }
    // ---------------------

    // 2. IYesOrNoMessageViewController Interface ����.
    public void OnYesButtonClicked(int messageCode)
    {
        yesOrNoMessageView.DeleteYesOrNoMessagePanel();
        loginSceneView.ActivateButton();

        if(messageCode == 4) accountModel.OnWithdrawYesButton();
    }

    public void OnNoButtonClicked(int messageCode)
    {
        yesOrNoMessageView.DeleteYesOrNoMessagePanel();
        loginSceneView.ActivateButton();
    }
    // ---------------------


    // 3. ILoginSceneController Interface ����.
    // Login Scene�� ��� Login Button�� Ŭ���� �߻�.
    public void LoginAccountButtonOnClicked()
    {
        loginSceneView.DisableButton();
        accountModel.OnLogin();
    }

    // Login Scene�� ������ �� Register Button�� Ŭ��.
    public void RegisterAccountButtonOnClicked()
    {
        loginSceneView.DisableButton();
        accountModel.OnRegister();
    }

    // ���� ��ư�� Ŭ�� ��, ���� Ȯ�� �޽��� ��� ��ư.
    public void WithdrawButtonOnClicked()
    {
        loginSceneView.DisableButton();
        accountModel.OnWithdrawButton();
    }
    // -----------------------------------------


    // 4. IGameLobbySceneController Interface ����
    // IGameLobbySceneController�� ������ �ʿ��� controller�� �ѿ��� �����ȴ�.
    public void GameLobbyMenuOnClicked() { }
    public void CharacterMenuOnClicked() { }
    public void InventoryMenuOnClicked() { }
    public void VenderMenuOnClicked() { }
    public void MarketMenuOnClicked() { }
    public void OptionMenuOnClicked() { }
}
