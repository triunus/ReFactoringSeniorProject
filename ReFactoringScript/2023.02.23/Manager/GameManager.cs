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
/// GameManager가 타 모델의 정보를 Get, Set 하기 위한 인터페이스.
/// IGameManagerForModel과 관계되어 같이 사용됩니다.
/// </summary>
public interface IModel
{
    /// <summary>
    /// GameManager가 Model에서 데이터 가져오는 함수.
    /// </summary>
    public string GetModelData();
    /// <summary>
    /// GameManager가 Model의 데이터 수정하는 함수.
    /// </summary>
    public void SetModelData(string modelData);
}

/// <summary>
/// 서버 요청과 응답에 의하여 Model의 필드가 변경되는 경우 Model에서 사용하는 Interface입니다.
/// IModel과 관계되어 같이 사용됩니다.
/// </summary>
public interface IGameManagerForModel
{
    /// <summary>
    /// Model에서 GameManager에 자신의 정보를 등록할 때 사용합니다.
    /// </summary>
    public void RegisterModel(IModel model);
    /// <summary>
    /// Model에서 GameManager에 자신의 정보를 제거할 때 사용합니다.
    /// </summary>
    public void RemoveModel(IModel model);
    /// <summary>
    /// Server 요청을 할때 사용합니다. (차후 다른 인터페이스로 이동될 수 있습니다.)
    /// </summary>
    /// <param name="path"> 요청을 보낼 서버 경로를 명시합니다. </param>
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
    private IAccountController accountController = null;

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

    // 2. Server 요청을 위해, Model들이 GameManager에 자신을 등록 및 취소하는 부분이다.
    // 하위 모델들이 GameManager가 자신의 데이터를 수월하게 가져올 수 있도록, List<IModel>에 자신을 등록하는 부분이다.
    public void RegisterModel(IModel model)
    {
        models.Add(model);
    }
    
    // Scene 변경이 발생하면서 삭제되는 Model들이 GameManager에서 자신의 등록 정보를 없애는 메소드이다.
    public void RemoveModel(IModel model)
    {
        models.Remove(model);
    }

    // 하위 모델들이 서버요청을 위해 호출하는 함수이다.
    // 파라미터로는 요청하고자하는 경로(요청 구분 담당)를 전달 받는다.
    public bool ServerRequest(string path)
    {
        List<string> modelData = new List<string>();

        // 현재 등록된 모델들의 데이터를 모아온다.
        for (int i =0; i< models.Count; i++)
        {
            modelData.Add(models[i].GetModelData());
        }

        // List<string> 형식의 Json String하여 하나의 Json String으로 Merge 해주는 함수이다.
        string requestData = converterJsonType.MergeJObjectAndJObject(modelData);
        // 서버 요청 후 반환값이 저장된다. 'serverConnecter.Request()'은 동기적으로 작동한다.
        string responseData = serverConnecter.Request(requestData, path);

        Debug.Log(responseData);    //응답 Json String 확인.

        // 성공 시, 각 Model에 데이터 전달한다. -> 이후, 각 Model은 정보를 갱신하고 View에 Notify 해준다.
        if ((bool)JObject.Parse(responseData)["success"])
        {
            for (int i = 0; i < models.Count; i++)
            {
                models[i].SetModelData(responseData);
            }
        }

        modelData.Clear();
        GC.Collect();

        // 실패 시, Server 요청을 요청한 Model에서 오류 처리를 수행한다.
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