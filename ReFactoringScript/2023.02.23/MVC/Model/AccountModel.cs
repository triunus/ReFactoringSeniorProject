using System.Net.NetworkInformation;                    // MAC Address

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Controller�� Model�� ����� ȣ���Ҷ� ����ϴ� Interface�̴�.
/// </summary>
public interface IAccountModel : IModel
{
    /// <summary>
    /// ������ �Է¿� ����, Login �۾��� �����մϴ�.
    /// </summary>
    public void OnLogin();
    /// <summary>
    /// �α��� ���� �޽����� ����ϰ�, Scene ���� ������ �����մϴ�.
    /// </summary>
    public void LoginSuccess();
    /// <summary>
    /// ������ �Է¿� ����, Register �۾��� �����մϴ�.
    /// </summary>
    public void OnRegister();
    /// <summary>
    /// ȸ�� Ż�� �۾��� �����մϴ�.
    /// </summary>
    public void OnWithdrawYesButton();
    /// <summary>
    /// ȸ�� Ż�� �۾��� ���� Ȯ�� �޽����� ����մϴ�.
    /// </summary>
    public void OnWithdrawButton();

    /// <summary>
    /// MessageClassifier ��ü�� �����ϱ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    public IMessageClassifier GetMessageClassifier();
}

public class AccountModel : IAccountModel
{
    private IGameManagerForModel gameManager = null;
    private ISceneManager sceneManager = null;
    private IMessageClassifier messageClassifier = null;

    // Local�� ����� ���� ���� �������� Ŭ����
    private IGetSetAccountData getSetAccountData = null;

    // AccountData�� Json String���� ����Ǿ� �ִ�.
    private AccountStruct accountStruct = null;

    public AccountModel()
    {
        gameManager = GameManager.GetGameManager();
        sceneManager = SceneManager.GetSceneModel();
        messageClassifier = new MessageClassifier();

        gameManager.RegisterModel(this);

        getSetAccountData = new GetSetAccountData();
    }

    // 2. IModel�� ����, GameManager���� Model�� �����͸� Get, Set�ϴ� �κ�.
    public string GetModelData()
    {
        // ������ ���޵Ǵ� ����� �����ֱ� ���� �ڵ��̴�.
        // AccountModel�� �ʵ� accountData�� ���ϴ� JsonString �������� �ٲپ� GameManager�� �����Ѵ�.
        JObject tempJObject = new JObject();
        tempJObject["AccountData"] = JObject.Parse(JsonConvert.SerializeObject(accountStruct, Formatting.Indented));

        return JsonConvert.SerializeObject(tempJObject, Formatting.Indented);
    }

    public void SetModelData(string JString)
    {
        // �������� ���޵� JString ��ü�� �޾ƿ�, JObject�� ��ȯ�Ѵ�.
        JObject responseData = JObject.Parse(JString);

        // AccountModel���� �ʿ��� �������� �Ľ��Ͽ� ���� �����Ѵ�.
        this.accountStruct = JsonConvert.DeserializeObject<AccountStruct>(JsonConvert.SerializeObject(responseData["accountData"], Formatting.Indented));

        // ������ AccountModel�� �ʵ尪�� ���濡 ���� View�� Notify ���־�� �ϴµ�, ���� View���� AccountModel�� ������ ����ϴ� ���� ����.
    }

    // 3. IAccountModel ����, Controller���� ����ϴ� �α���, ȸ������ ��� �����κ�.
    public void OnLogin()
    {
        this.accountStruct = getSetAccountData.GetAccountData();

        if (this.accountStruct == null)    // ���� ������ ���� ( ���� �α��� ����� ���� )
        {
            messageClassifier.ChangeMessageCode(23);         // ����� messageCode �Է� ��, View�� ����.
        }
        else
        {
            // ���� ��û�� ������ ���и� �����´�.
            if (gameManager.ServerRequest("login_test"))
            {
                messageClassifier.ChangeMessageCode(21);     // �α��� ���� �ȳ� �޽��� ���.
            }
            else
            {
                this.accountStruct = null;
                messageClassifier.ChangeMessageCode(22);     // �α��� ���� �ȳ� �޽��� ���.
            }
        }
    }

    public void LoginSuccess()
    {
        sceneManager.ChangeScene("GameLobbyScene");
    }

    public void OnRegister()
    {
        this.accountStruct = getSetAccountData.GetAccountData();

        if (this.accountStruct != null)    // ���� ������ �ִ� ���� ( ���� �α��� ����� ���� )
        {
            this.accountStruct = null;
            messageClassifier.ChangeMessageCode(12);         // ����� messageCode �Է� ��, View�� ����.
        }
        else
        {
            this.accountStruct = new AccountStruct(null, GetMACAddress(), null, null);

            // ���� ��û�� ������ ���и� �����´�.
            if (gameManager.ServerRequest("register_test"))
            {
                getSetAccountData.SaveAccountData(this.accountStruct);
                messageClassifier.ChangeMessageCode(11);
            }
            else
            {
                this.accountStruct = null;
                messageClassifier.ChangeMessageCode(13);
            }
        }
    }

    public void OnWithdrawYesButton()
    {
        this.accountStruct = getSetAccountData.GetAccountData();

        if (this.accountStruct == null)
        {
            messageClassifier.ChangeMessageCode(2);         // ����� messageCode �Է� ��, View�� ����.
        }
        else
        {
            if (gameManager.ServerRequest("delete_test"))
            {
                this.accountStruct = null;
                getSetAccountData.DeleteAccountData();
                messageClassifier.ChangeMessageCode(1);
            }
            else
            {
                messageClassifier.ChangeMessageCode(3);
            }
        }
    }

    public void OnWithdrawButton()
    {
        messageClassifier.ChangeMessageCode(4);
    }

    public IMessageClassifier GetMessageClassifier()
    {
        return messageClassifier.GetMessageClassifier();
    }

    // 4. Model ���ο��� �ʿ�� �ϴ� ��� �κ�.
    // MAC Address�� �����Ѵ�.
    private string GetMACAddress()
    {
        return NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
    }
}
