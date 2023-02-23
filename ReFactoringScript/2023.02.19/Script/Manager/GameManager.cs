using System;
using System.Collections.Generic;

using UnityEngine;

using Newtonsoft.Json.Linq;

public enum ModelType
{
    AccountModel,
    ViewManagerModel,
    UserModel,
    VenderModel,
    UserTransactionModel,
    SceneModel,
    TransactionModel,
    PlayerModel,
    SkillModel,
    EnemyModel
}


/// <summary>
/// Scene 모델을 제외한 다른 모델과 상호작용하기 위한 인터페이스
/// </summary>
public interface IModel
{
    // Model에서 데이터 가져오는 함수.
    public string GetModelData();
    // Model의 데이터 수정하는 함수.
    public void SetModelData(string modelData);
}

public interface ILoginSceneController
{
    public void LoginAccountButtonOnClicked();
    public void RegisterAccountButtonOnClicked();
    public void WithdrawButtonOnClicked();
}
/*
public interface IGameLobbySceneController
{
    public void GameLobbyMenuOnClicked();
    public void CharacterMenuOnClicked();
    public void InventoryMenuOnClicked();
    public void VenderMenuOnClicked();
    public void MarketMenuOnClicked();
    public void OptionMenuOnClicked();
}*/

/// <summary>
/// Model에서 GameManager에 접근하기 위해 사용하는 인터페이스
/// </summary>
public interface IGameManagerForModel
{
    public void RegisterModel(IModel model);
    public bool ServerRequest(string path);
}

/// <summary>
/// Scene 모델과 상호작용하기 위한 인터페이스
/// </summary>
public interface IGameManagerForSceneManager
{
    public void CreateAndDeleteModel(string[] createModel, string[] deleteModel);
    public bool ServerRequest(string path);
}

public class GameManager : MonoBehaviour, IGameManagerForModel, IGameManagerForSceneManager
{
    private static GameManager gameManager = null;

    private IServerConnecter serverConnecter = null;
    private IConverterJsonType converterJsonType = null;

    private List<IModel> models = new List<IModel>();

    private IViewManagerModel viewManagerModel = null;
    private IViewManagerController viewManagerController = null;

    private IAccountModel accountModel = null;
    private ILoginSceneController accountController = null;

    private IUserModel userModel = null;
    private IUserController userController = null;

    private IVenderModel venderModel = null;
    private IVenderController venderController = null;

    private void Awake()
    {
        SetGameManager();
        serverConnecter = new ServerConnecter();
        converterJsonType = new ConverterJsonType();

        accountModel = new AccountModel();
        accountController = new AccountController(accountModel);
    }

    // 1. GameManager GameObject가 갖고 있는 GameManager 인스턴스를 가져옵니다.
    public static GameManager GetGameManager()
    {
        // "GameManager" 클래스가 포함된 게임 오브젝트가 존재하는지 확인하고, 없으면 생성한다.
        if (GameObject.FindWithTag("GameManager") == null) {
            GameObject _gameManagerObject = new GameObject();
            _gameManagerObject.name = "GameManager";
            _gameManagerObject.tag = "GameManager";
        }

        // 기존에 존재 or 생성된 "GameManager" 게임 오브젝트를 명시한다.
        GameObject gameManagerObject = GameObject.FindWithTag("GameManager");

        // "GameManager" 게임 오브젝트가 GameManager.cs 클래스를 포함하고 있는지 확인하고, 없으면 포함시킨다.
        if (gameManagerObject.GetComponent<GameManager>() == null) gameManagerObject.AddComponent<GameManager>();

        DontDestroyOnLoad(gameManagerObject);            // "GameManager" 클래스가 포함된 GameObject가 삭제되지 않도록 명시.

        // 전역변수로 추가한 GameManager 클래스 연결.
        gameManager = gameManagerObject.GetComponent<GameManager>();

        return gameManager;             // Get의 리턴으로 "GameManager" GameObject가 포함한 GameManager 클래스를 리턴해준다.
    }

    private void SetGameManager()
    {
        GameObject gameManagerObject = this.gameObject;

        // "GameManager" 클래스가 포함된 GameObject가 삭제되지 않도록 명시.
        DontDestroyOnLoad(gameManagerObject);

        // 전역변수로 추가한 GameManager 클래스 연결.'
        gameManager = gameManagerObject.GetComponent<GameManager>();
    }

    // 2. Server 요청을 위해, Model들이 GameManager에 접근하는 부분이다.
    // 하위 모델들이 GameManager가 자신의 데이터를 수월하게 가져올 수 있도록, List<IModel>에 자신을 등록하는 부분이다.
    public void RegisterModel(IModel model)
    {
        models.Add(model);
    }

    // 하위 모델들이 서버요청을 위해 호출하는 함수이다.
    public bool ServerRequest(string path)
    {
        // 현재 사용하는 모델들의 데이터를 모아온다.
        List<string> modelData = new List<string>(); 

        for(int i =0; i< models.Count; i++)
        {
            modelData.Add(models[i].GetModelData());

//            Debug.Log(i + " : "+ modelData[i]);
        }

        string requestData = converterJsonType.MergeJObjectAndJObject(modelData);
        string responseData = serverConnecter.Request(requestData, path);

        Debug.Log(responseData);

        // 성공 시, 각 Model에 데이터 전달.
        if ((bool)JObject.Parse(responseData)["success"])
        {
            for (int i = 0; i < models.Count; i++)
            {
                //            Debug.Log("set : " + i);
                // 모델에 값 설정
                models[i].SetModelData(responseData);
            }
        }

        modelData.Clear();
        GC.Collect();

        return (bool)JObject.Parse(responseData)["success"];
    }

    // 파라미터로 전달 받은 모델이름을 통해 Model 생성 및 삭제.
    public void CreateAndDeleteModel(string[] createModel, string[] deleteModel)
    {
        for (int i = 0; i < deleteModel.Length; i++)
        {
            switch ((ModelType)Enum.Parse(typeof(ModelType), deleteModel[i]))
            {
                case ModelType.AccountModel:
                    accountModel = null;
                    accountController = null;
                    break;
                case ModelType.ViewManagerModel :
                    viewManagerModel = null;
                    viewManagerController = null;
                    break;
                case ModelType.UserModel:
                    userModel = null;
                    userController = null;
                    break;
                case ModelType.VenderModel:
                    venderModel = null;
                    venderController = null;
                    break;
                default:
                    break;
            }
        }

        GC.Collect();

        for (int i =0; i< createModel.Length; i++)
        {
            switch((ModelType)Enum.Parse(typeof(ModelType), createModel[i]))
            {
                case ModelType.AccountModel:
                    accountModel = new AccountModel();
                    accountController = new AccountController(accountModel);
                    break;
                case ModelType.ViewManagerModel:
                    viewManagerModel = new ViewManagerModel();
                    viewManagerController = new ViewManagerController(viewManagerModel);
                    break;
                case ModelType.UserModel:
                    userModel = new UserModel();
                    userController = new UserController(userModel);
                    break;
                case ModelType.VenderModel:
                    venderModel = new VenderModel();
                    venderController = new VenderController(venderModel);
                    break;
                default:
                    break;
            }
        }
    }
}


/*
 * 
                case ModelType.UserTransactionModel:
                    break;
                case ModelType.TransactionModel:
                    break;
                case ModelType.PlayerModel:
                    break;
                case ModelType.SkillModel:
                    break;
                case ModelType.EnemyModel:
                    break;
 * case ModelType.UserTransactionModel:
                    break;
                    break;
                case ModelType.TransactionModel:
                    break;
                case ModelType.PlayerModel:
                    break;
                case ModelType.SkillModel:
                    break;
                case ModelType.EnemyModel:
                    break;
*/