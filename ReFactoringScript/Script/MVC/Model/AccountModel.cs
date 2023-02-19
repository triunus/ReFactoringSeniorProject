using System.Net.NetworkInformation;                    // MAC Address

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public interface IAccountModel : IModel, IMessageViewMediator
{
    // Controller가 사용하는 Model 부분.     
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

    // Local에 저장된 유저 정보 가져오는 클래스
    private IGetSetAccountData getSetAccountData = null;

    // AccountData는 Json String으로 저장되어 있다.
    private AccountData accountData = null;

    public AccountModel()
    {
        gameManager = GameManager.GetGameManager();
        sceneManager = SceneManager.GetSceneModel();
        messageMediator = new MessageViewMediator();

        gameManager.RegisterModel(this);

        getSetAccountData = new GetSetAccountData();
    }

    // 1. Message 관련 Observer 기능을 MessageViewMediator에 위임하였다.
    public void RegisterMessageObserver(MessageObserver observer)
    {
        messageMediator.RegisterMessageObserver(observer);
    }

    public string[] GetMessage()
    {
        return messageMediator.GetMessage();
    }

    // GM에서, AccountModel의 MessageMediator를 통해 메시지를 출력하고 싶을 때 사용한다.
    public void NotifyMessageObservers(int messageCode)
    {
        messageMediator.NotifyMessageObservers(messageCode);
    }
    // ---------------------------------------------------------------

    // 2. GameManager에서 Model의 데이터를 Get, Set하는 부분.
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

    // 3. Controller에서 사용하는 로그인, 회원가입 기능 구현부분.
    public void OnLogin()
    {
        accountData = getSetAccountData.GetAccountData();

        if (accountData == null)    // 로컬 파일이 없음 ( 기존 로그인 기록이 없음 )
        {
            messageMediator.NotifyMessageObservers(23);         // 출력할 messageCode 입력 후, View에 전달.
        }
        else
        {
            // 서버 요청의 성공과 실패를 가져온다.
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

        if (accountData != null)    // 로컬 파일이 있는 상태 ( 기존 로그인 기록이 있음 )
        {
            accountData = null;
            messageMediator.NotifyMessageObservers(12);         // 출력할 messageCode 입력 후, View에 전달.
        }
        else
        {
            accountData = new AccountData(null, GetMACAddress(), null, null);

            // 서버 요청의 성공과 실패를 가져온다.
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
            messageMediator.NotifyMessageObservers(2);         // 출력할 messageCode 입력 후, View에 전달.
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

    // MAC Address를 리턴한다.
    private string GetMACAddress()
    {
        return NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
    }
    // ---------------------------------
}
