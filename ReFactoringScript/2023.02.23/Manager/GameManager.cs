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
/// GameManager�� Ÿ ���� ������ Get, Set �ϱ� ���� �������̽�.
/// IGameManagerForModel�� ����Ǿ� ���� ���˴ϴ�.
/// </summary>
public interface IModel
{
    /// <summary>
    /// GameManager�� Model���� ������ �������� �Լ�.
    /// </summary>
    public string GetModelData();
    /// <summary>
    /// GameManager�� Model�� ������ �����ϴ� �Լ�.
    /// </summary>
    public void SetModelData(string modelData);
}

/// <summary>
/// ���� ��û�� ���信 ���Ͽ� Model�� �ʵ尡 ����Ǵ� ��� Model���� ����ϴ� Interface�Դϴ�.
/// IModel�� ����Ǿ� ���� ���˴ϴ�.
/// </summary>
public interface IGameManagerForModel
{
    /// <summary>
    /// Model���� GameManager�� �ڽ��� ������ ����� �� ����մϴ�.
    /// </summary>
    public void RegisterModel(IModel model);
    /// <summary>
    /// Model���� GameManager�� �ڽ��� ������ ������ �� ����մϴ�.
    /// </summary>
    public void RemoveModel(IModel model);
    /// <summary>
    /// Server ��û�� �Ҷ� ����մϴ�. (���� �ٸ� �������̽��� �̵��� �� �ֽ��ϴ�.)
    /// </summary>
    /// <param name="path"> ��û�� ���� ���� ��θ� ����մϴ�. </param>
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

    // 2. Server ��û�� ����, Model���� GameManager�� �ڽ��� ��� �� ����ϴ� �κ��̴�.
    // ���� �𵨵��� GameManager�� �ڽ��� �����͸� �����ϰ� ������ �� �ֵ���, List<IModel>�� �ڽ��� ����ϴ� �κ��̴�.
    public void RegisterModel(IModel model)
    {
        models.Add(model);
    }
    
    // Scene ������ �߻��ϸ鼭 �����Ǵ� Model���� GameManager���� �ڽ��� ��� ������ ���ִ� �޼ҵ��̴�.
    public void RemoveModel(IModel model)
    {
        models.Remove(model);
    }

    // ���� �𵨵��� ������û�� ���� ȣ���ϴ� �Լ��̴�.
    // �Ķ���ͷδ� ��û�ϰ����ϴ� ���(��û ���� ���)�� ���� �޴´�.
    public bool ServerRequest(string path)
    {
        List<string> modelData = new List<string>();

        // ���� ��ϵ� �𵨵��� �����͸� ��ƿ´�.
        for (int i =0; i< models.Count; i++)
        {
            modelData.Add(models[i].GetModelData());
        }

        // List<string> ������ Json String�Ͽ� �ϳ��� Json String���� Merge ���ִ� �Լ��̴�.
        string requestData = converterJsonType.MergeJObjectAndJObject(modelData);
        // ���� ��û �� ��ȯ���� ����ȴ�. 'serverConnecter.Request()'�� ���������� �۵��Ѵ�.
        string responseData = serverConnecter.Request(requestData, path);

        Debug.Log(responseData);    //���� Json String Ȯ��.

        // ���� ��, �� Model�� ������ �����Ѵ�. -> ����, �� Model�� ������ �����ϰ� View�� Notify ���ش�.
        if ((bool)JObject.Parse(responseData)["success"])
        {
            for (int i = 0; i < models.Count; i++)
            {
                models[i].SetModelData(responseData);
            }
        }

        modelData.Clear();
        GC.Collect();

        // ���� ��, Server ��û�� ��û�� Model���� ���� ó���� �����Ѵ�.
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