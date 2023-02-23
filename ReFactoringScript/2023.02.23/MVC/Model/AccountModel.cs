using System.Net.NetworkInformation;                    // MAC Address

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Controller가 Model의 기능을 호출할때 사용하는 Interface이다.
/// </summary>
public interface IAccountModel : IModel
{
    /// <summary>
    /// 유저의 입력에 따른, Login 작업을 수행합니다.
    /// </summary>
    public void OnLogin();
    /// <summary>
    /// 로그인 성공 메시지를 출력하고, Scene 변경 로직을 시작합니다.
    /// </summary>
    public void LoginSuccess();
    /// <summary>
    /// 유저의 입력에 따른, Register 작업을 수행합니다.
    /// </summary>
    public void OnRegister();
    /// <summary>
    /// 회원 탈퇴 작업을 수행합니다.
    /// </summary>
    public void OnWithdrawYesButton();
    /// <summary>
    /// 회원 탈퇴 작업에 대한 확인 메시지를 출력합니다.
    /// </summary>
    public void OnWithdrawButton();

    /// <summary>
    /// MessageClassifier 객체에 접근하기 위한 메소드입니다.
    /// </summary>
    public IMessageClassifier GetMessageClassifier();
}

public class AccountModel : IAccountModel
{
    private IGameManagerForModel gameManager = null;
    private ISceneManager sceneManager = null;
    private IMessageClassifier messageClassifier = null;

    // Local에 저장된 유저 정보 가져오는 클래스
    private IGetSetAccountData getSetAccountData = null;

    // AccountData는 Json String으로 저장되어 있다.
    private AccountStruct accountStruct = null;

    public AccountModel()
    {
        gameManager = GameManager.GetGameManager();
        sceneManager = SceneManager.GetSceneModel();
        messageClassifier = new MessageClassifier();

        gameManager.RegisterModel(this);

        getSetAccountData = new GetSetAccountData();
    }

    // 2. IModel의 구현, GameManager에서 Model의 데이터를 Get, Set하는 부분.
    public string GetModelData()
    {
        // 서버에 전달되는 양식을 지켜주기 위한 코드이다.
        // AccountModel의 필드 accountData를 원하는 JsonString 형식으로 바꾸어 GameManager에 전달한다.
        JObject tempJObject = new JObject();
        tempJObject["AccountData"] = JObject.Parse(JsonConvert.SerializeObject(accountStruct, Formatting.Indented));

        return JsonConvert.SerializeObject(tempJObject, Formatting.Indented);
    }

    public void SetModelData(string JString)
    {
        // 응답으로 전달된 JString 전체를 받아와, JObject로 변환한다.
        JObject responseData = JObject.Parse(JString);

        // AccountModel에서 필요한 정보만을 파싱하여 값을 변경한다.
        this.accountStruct = JsonConvert.DeserializeObject<AccountStruct>(JsonConvert.SerializeObject(responseData["accountData"], Formatting.Indented));

        // 원래는 AccountModel의 필드값의 변경에 대해 View에 Notify 해주어야 하는데, 아직 View에서 AccountModel의 정보를 사용하는 곳이 없다.
    }

    // 3. IAccountModel 구현, Controller에서 사용하는 로그인, 회원가입 기능 구현부분.
    public void OnLogin()
    {
        this.accountStruct = getSetAccountData.GetAccountData();

        if (this.accountStruct == null)    // 로컬 파일이 없음 ( 기존 로그인 기록이 없음 )
        {
            messageClassifier.ChangeMessageCode(23);         // 출력할 messageCode 입력 후, View에 전달.
        }
        else
        {
            // 서버 요청의 성공과 실패를 가져온다.
            if (gameManager.ServerRequest("login_test"))
            {
                messageClassifier.ChangeMessageCode(21);     // 로그인 성공 안내 메시지 출력.
            }
            else
            {
                this.accountStruct = null;
                messageClassifier.ChangeMessageCode(22);     // 로그인 실패 안내 메시지 출력.
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

        if (this.accountStruct != null)    // 로컬 파일이 있는 상태 ( 기존 로그인 기록이 있음 )
        {
            this.accountStruct = null;
            messageClassifier.ChangeMessageCode(12);         // 출력할 messageCode 입력 후, View에 전달.
        }
        else
        {
            this.accountStruct = new AccountStruct(null, GetMACAddress(), null, null);

            // 서버 요청의 성공과 실패를 가져온다.
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
            messageClassifier.ChangeMessageCode(2);         // 출력할 messageCode 입력 후, View에 전달.
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

    // 4. Model 내부에서 필요로 하는 기능 부분.
    // MAC Address를 리턴한다.
    private string GetMACAddress()
    {
        return NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
    }
}
