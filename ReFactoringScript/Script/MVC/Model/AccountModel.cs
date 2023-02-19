using System.Net.NetworkInformation;                    // MAC Address

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public interface IAccountModel : IModel, IMessageViewMediator
{
    // Controller�� ����ϴ� Model �κ�.     
    public void OnLogin();
    public void LoginSuccess();
    public void OnRegister();
    public void OnWithdrawYesButton();
    public void OnWithdrawButton();
}

public class AccountModel : IAccountModel
{
    private IGameManagerForModel gameManager = null;
    private ISceneManager sceneManager = null;
    private IMessageViewMediator messageMediator = null;

    // Local�� ����� ���� ���� �������� Ŭ����
    private IGetSetAccountData getSetAccountData = null;

    // AccountData�� Json String���� ����Ǿ� �ִ�.
    private AccountData accountData = null;

    public AccountModel()
    {
        gameManager = GameManager.GetGameManager();
        sceneManager = SceneManager.GetSceneModel();
        messageMediator = new MessageViewMediator();

        gameManager.RegisterModel(this);

        getSetAccountData = new GetSetAccountData();
    }

    // 1. Message ���� Observer ����� MessageViewMediator�� �����Ͽ���.
    public void RegisterMessageObserver(MessageObserver observer)
    {
        messageMediator.RegisterMessageObserver(observer);
    }

    public string[] GetMessage()
    {
        return messageMediator.GetMessage();
    }

    // GM����, AccountModel�� MessageMediator�� ���� �޽����� ����ϰ� ���� �� ����Ѵ�.
    public void NotifyMessageObservers(int messageCode)
    {
        messageMediator.NotifyMessageObservers(messageCode);
    }
    // ---------------------------------------------------------------

    // 2. GameManager���� Model�� �����͸� Get, Set�ϴ� �κ�.
    public string GetModelData()
    {
        JObject tempJObject = new JObject();
        tempJObject["AccountData"] = JObject.Parse(JsonConvert.SerializeObject(accountData, Formatting.Indented));

        return JsonConvert.SerializeObject(tempJObject, Formatting.Indented);
    }

    public void SetModelData(string JString)
    {
        JObject responseData = JObject.Parse(JString);

        this.accountData = JsonConvert.DeserializeObject<AccountData>(JsonConvert.SerializeObject(responseData["accountData"], Formatting.Indented));

//        UnityEngine.Debug.Log(accountData);
    }

    // 3. Controller���� ����ϴ� �α���, ȸ������ ��� �����κ�.
    public void OnLogin()
    {
        accountData = getSetAccountData.GetAccountData();

        if (accountData == null)    // ���� ������ ���� ( ���� �α��� ����� ���� )
        {
            messageMediator.NotifyMessageObservers(23);         // ����� messageCode �Է� ��, View�� ����.
        }
        else
        {
            // ���� ��û�� ������ ���и� �����´�.
            if (gameManager.ServerRequest("login_test"))
            {
                messageMediator.NotifyMessageObservers(21);
            }
            else
            {
                accountData = null;
                messageMediator.NotifyMessageObservers(22);
            }
        }
    }

    public void LoginSuccess()
    {
        sceneManager.ChangeScene("GameLobbyScene");
    }

    public void OnRegister()
    {
        accountData = getSetAccountData.GetAccountData();

        if (accountData != null)    // ���� ������ �ִ� ���� ( ���� �α��� ����� ���� )
        {
            accountData = null;
            messageMediator.NotifyMessageObservers(12);         // ����� messageCode �Է� ��, View�� ����.
        }
        else
        {
            accountData = new AccountData(null, GetMACAddress(), null, null);

            // ���� ��û�� ������ ���и� �����´�.
            if (gameManager.ServerRequest("register_test"))
            {
                getSetAccountData.SaveAccountData(accountData);
                messageMediator.NotifyMessageObservers(11);
            }
            else
            {
                accountData = null;
                messageMediator.NotifyMessageObservers(13);
            }
        }
    }

    public void OnWithdrawYesButton()
    {
        accountData = getSetAccountData.GetAccountData();

        if (accountData == null)
        {
            messageMediator.NotifyMessageObservers(2);         // ����� messageCode �Է� ��, View�� ����.
        }
        else
        {
            if (gameManager.ServerRequest("delete_test"))
            {
                accountData = null;
                getSetAccountData.DeleteAccountData();
                messageMediator.NotifyMessageObservers(1);
            }
            else
            {
                messageMediator.NotifyMessageObservers(3);
            }
        }
    }

    public void OnWithdrawButton()
    {
        messageMediator.NotifyMessageObservers(4);
    }

    // MAC Address�� �����Ѵ�.
    private string GetMACAddress()
    {
        return NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
    }
    // ---------------------------------
}
