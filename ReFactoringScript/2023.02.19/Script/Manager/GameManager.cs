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
/// Scene ���� ������ �ٸ� �𵨰� ��ȣ�ۿ��ϱ� ���� �������̽�
/// </summary>
public interface IModel
{
    // Model���� ������ �������� �Լ�.
    public string GetModelData();
    // Model�� ������ �����ϴ� �Լ�.
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
/// Model���� GameManager�� �����ϱ� ���� ����ϴ� �������̽�
/// </summary>
public interface IGameManagerForModel
{
    public void RegisterModel(IModel model);
    public bool ServerRequest(string path);
}

/// <summary>
/// Scene �𵨰� ��ȣ�ۿ��ϱ� ���� �������̽�
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

    // 1. GameManager GameObject�� ���� �ִ� GameManager �ν��Ͻ��� �����ɴϴ�.
    public static GameManager GetGameManager()
    {
        // "GameManager" Ŭ������ ���Ե� ���� ������Ʈ�� �����ϴ��� Ȯ���ϰ�, ������ �����Ѵ�.
        if (GameObject.FindWithTag("GameManager") == null) {
            GameObject _gameManagerObject = new GameObject();
            _gameManagerObject.name = "GameManager";
            _gameManagerObject.tag = "GameManager";
        }

        // ������ ���� or ������ "GameManager" ���� ������Ʈ�� ����Ѵ�.
        GameObject gameManagerObject = GameObject.FindWithTag("GameManager");

        // "GameManager" ���� ������Ʈ�� GameManager.cs Ŭ������ �����ϰ� �ִ��� Ȯ���ϰ�, ������ ���Խ�Ų��.
        if (gameManagerObject.GetComponent<GameManager>() == null) gameManagerObject.AddComponent<GameManager>();

        DontDestroyOnLoad(gameManagerObject);            // "GameManager" Ŭ������ ���Ե� GameObject�� �������� �ʵ��� ���.

        // ���������� �߰��� GameManager Ŭ���� ����.
        gameManager = gameManagerObject.GetComponent<GameManager>();

        return gameManager;             // Get�� �������� "GameManager" GameObject�� ������ GameManager Ŭ������ �������ش�.
    }

    private void SetGameManager()
    {
        GameObject gameManagerObject = this.gameObject;

        // "GameManager" Ŭ������ ���Ե� GameObject�� �������� �ʵ��� ���.
        DontDestroyOnLoad(gameManagerObject);

        // ���������� �߰��� GameManager Ŭ���� ����.'
        gameManager = gameManagerObject.GetComponent<GameManager>();
    }

    // 2. Server ��û�� ����, Model���� GameManager�� �����ϴ� �κ��̴�.
    // ���� �𵨵��� GameManager�� �ڽ��� �����͸� �����ϰ� ������ �� �ֵ���, List<IModel>�� �ڽ��� ����ϴ� �κ��̴�.
    public void RegisterModel(IModel model)
    {
        models.Add(model);
    }

    // ���� �𵨵��� ������û�� ���� ȣ���ϴ� �Լ��̴�.
    public bool ServerRequest(string path)
    {
        // ���� ����ϴ� �𵨵��� �����͸� ��ƿ´�.
        List<string> modelData = new List<string>(); 

        for(int i =0; i< models.Count; i++)
        {
            modelData.Add(models[i].GetModelData());

//            Debug.Log(i + " : "+ modelData[i]);
        }

        string requestData = converterJsonType.MergeJObjectAndJObject(modelData);
        string responseData = serverConnecter.Request(requestData, path);

        Debug.Log(responseData);

        // ���� ��, �� Model�� ������ ����.
        if ((bool)JObject.Parse(responseData)["success"])
        {
            for (int i = 0; i < models.Count; i++)
            {
                //            Debug.Log("set : " + i);
                // �𵨿� �� ����
                models[i].SetModelData(responseData);
            }
        }

        modelData.Clear();
        GC.Collect();

        return (bool)JObject.Parse(responseData)["success"];
    }

    // �Ķ���ͷ� ���� ���� ���̸��� ���� Model ���� �� ����.
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