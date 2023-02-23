/// <summary>
/// GameManager�� AccountController�� ������ �� ����Ѵ�.
/// </summary>
public interface IAccountController { }

public class AccountController : IAccountController, ILoginSceneViewController, IConfirmMessageViewController, IYesOrNoMessageViewController
{
    private IAccountModel accountModel;

    private ILoginSceneView loginSceneView;
    private IConfirmMessageView confirmMessageView;
    private IYesOrNoMessageView yesOrNoMessageView;

    public AccountController(IAccountModel accountModel)
    {
        this.accountModel = accountModel;

        loginSceneView = new LoginSceneView(this);

        confirmMessageView = new ConfirmMessageView(accountModel.GetMessageClassifier(), this);
        yesOrNoMessageView = new YesOrNoMessageView(accountModel.GetMessageClassifier(), this);
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
}
