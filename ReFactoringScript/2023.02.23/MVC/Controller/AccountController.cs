/// <summary>
/// GameManager가 AccountController를 참조할 때 사용한다.
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

    // 1. IConfirmMessageViewController Interface 구현.
    public void OnConfirmButtonClicked(int messageCode)
    {
        confirmMessageView.DeleteConfirmMessagePanel();
        loginSceneView.ActivateButton();

        if (messageCode == 21)
        {
            accountModel.LoginSuccess();
        }
    }

    // 2. IYesOrNoMessageViewController Interface 구현.
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

    // 3. ILoginSceneController Interface 구현.
    // Login Scene의 가운데 Login Button을 클릭시 발생.
    public void LoginAccountButtonOnClicked()
    {
        loginSceneView.DisableButton();
        accountModel.OnLogin();
    }

    // Login Scene의 오른쪽 밑 Register Button을 클릭.
    public void RegisterAccountButtonOnClicked()
    {
        loginSceneView.DisableButton();
        accountModel.OnRegister();
    }

    // 삭제 버튼을 클릭 시, 삭제 확인 메시지 출력 버튼.
    public void WithdrawButtonOnClicked()
    {
        loginSceneView.DisableButton();
        accountModel.OnWithdrawButton();
    }
}
