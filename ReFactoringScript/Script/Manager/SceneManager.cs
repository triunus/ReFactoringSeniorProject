using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public enum SceneType
{
    LoginScene,
    GameLobbyScene,
    InGameScene
}

public interface ISceneManager
{
    public void ChangeScene(string nextSceneState);
}

// LoginScene - GameLobbyScene - InGame
public class SceneManager : MonoBehaviour, ISceneManager
{
    private static ISceneManager sceneManager = null;

    private IGameManagerForSceneManager gameManager = null;

    private string currentSceneState = null;

    private void Awake()
    {
        gameManager = GameManager.GetGameManager();
    }

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += sceneLoadedEvent;
    }

    private void sceneLoadedEvent(UnityEngine.SceneManagement.Scene nextScene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        ChangeSceneOperation(currentSceneState, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // 1. Singleton ����.
    public static ISceneManager GetSceneModel()
    {
        // "GameManager" Ŭ������ ���Ե� ���� ������Ʈ�� �����ϴ��� Ȯ���ϰ�, ������ �����Ѵ�.
        if (GameObject.FindWithTag("GameManager") == null)
        {
            GameObject _gameManagerObject = new GameObject();
            _gameManagerObject.name = "GameManager";
            _gameManagerObject.tag = "GameManager";
        }

        // ������ ���� or ������ "GameManager" ���� ������Ʈ�� ����Ѵ�.
        GameObject gameManagerObject = GameObject.FindWithTag("GameManager");

        // "GameManager" ���� ������Ʈ�� GameManager.cs Ŭ������ �����ϰ� �ִ��� Ȯ���ϰ�, ������ ���Խ�Ų��.
        if (gameManagerObject.GetComponent<SceneManager>() == null) gameManagerObject.AddComponent<SceneManager>();

        DontDestroyOnLoad(gameManagerObject);            // "GameManager" Ŭ������ ���Ե� GameObject�� �������� �ʵ��� ���.

        // ���������� �߰��� GameManager Ŭ���� ����.
        sceneManager = gameManagerObject.GetComponent<SceneManager>();

        return sceneManager;             // Get�� �������� "GameManager" GameObject�� ������ GameManager Ŭ������ �������ش�.
    }


    // 2.  �ܺο��� Scene ������ ���� �����ϴ� �޼ҵ�.
    public void ChangeScene(string nextSceneState)
    {
        currentSceneState = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneState, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    // ---------------------------------------------------------


    // �� ������ �Ͼ ��, ���� �Ǵ� ������ model�� �����ϴ� ���̴�.
    // ������ �� ������ GameManager���� �����Ͽ� ���� ���� �� �����Ѵ�.
    private void ChangeSceneOperation(string currentSceneState, string nextSceneState)
    {
        if (currentSceneState.Equals(nextSceneState)) return;

        // �� Scene ���� �ʿ�� �ϴ� ��ü�������� �����´�.
        string[] currentSceneModels = SpecifyModelUsedInScene(currentSceneState);
        string[] nextSceneModels = SpecifyModelUsedInScene(nextSceneState);

        // ���� ������ ����Ǵ� ����.        // ������ ������ ���� �ʿ��� ��ü ����.
        IEnumerable<string>  modelToCreate = nextSceneModels.Except(currentSceneModels);
        IEnumerable<string>  modelToDelete = currentSceneModels.Except(nextSceneModels);

        // GameMangaer�� ���� �� ���� ���� ���� �� �۾� ���� ��û.
        gameManager.CreateAndDeleteModel(modelToCreate.Cast<string>().ToArray(), modelToDelete.Cast<string>().ToArray());

        // �۾� ���� ��, �ش� Scene���� �ʿ�� �ϴ� ������ ������ ��û.
        switch((SceneType)Enum.Parse(typeof(SceneType), nextSceneState))
        {
            case SceneType.LoginScene:
                break;
            case SceneType.GameLobbyScene:
                if (gameManager.ServerRequest("getGameLobbySceneInformationOperation")) { }
                else { }
                break;
            case SceneType.InGameScene:
                break;
            default:
                break;
        }
    }

    // �� Scene���� �ʿ���ϴ� ��ü�������� ��ȯ���ش�.
    private string[] SpecifyModelUsedInScene(string SceneName)
    {
        switch ((SceneType)Enum.Parse(typeof(SceneType), SceneName))
        {
            case SceneType.LoginScene:
                return new string[] { "AccountModel" };
            case SceneType.GameLobbyScene:
                return new string[] { "AccountModel", "ViewManagerModel", "UserModel", "VenderModel", "UserTransactionModel", "TransactionModel" };
            case SceneType.InGameScene:
                return new string[] { "AccountModel", "PlayerModel", "SkillModel", "EnemyModel" };
            default:
                return null;
        }
    }
}