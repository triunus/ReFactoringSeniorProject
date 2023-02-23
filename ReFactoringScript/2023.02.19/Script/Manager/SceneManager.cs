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

    // 1. Singleton 설정.
    public static ISceneManager GetSceneModel()
    {
        // "GameManager" 클래스가 포함된 게임 오브젝트가 존재하는지 확인하고, 없으면 생성한다.
        if (GameObject.FindWithTag("GameManager") == null)
        {
            GameObject _gameManagerObject = new GameObject();
            _gameManagerObject.name = "GameManager";
            _gameManagerObject.tag = "GameManager";
        }

        // 기존에 존재 or 생성된 "GameManager" 게임 오브젝트를 명시한다.
        GameObject gameManagerObject = GameObject.FindWithTag("GameManager");

        // "GameManager" 게임 오브젝트가 GameManager.cs 클래스를 포함하고 있는지 확인하고, 없으면 포함시킨다.
        if (gameManagerObject.GetComponent<SceneManager>() == null) gameManagerObject.AddComponent<SceneManager>();

        DontDestroyOnLoad(gameManagerObject);            // "GameManager" 클래스가 포함된 GameObject가 삭제되지 않도록 명시.

        // 전역변수로 추가한 GameManager 클래스 연결.
        sceneManager = gameManagerObject.GetComponent<SceneManager>();

        return sceneManager;             // Get의 리턴으로 "GameManager" GameObject가 포함한 GameManager 클래스를 리턴해준다.
    }


    // 2.  외부에서 Scene 변경을 위해 접근하는 메소드.
    public void ChangeScene(string nextSceneState)
    {
        currentSceneState = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneState, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    // ---------------------------------------------------------


    // 씬 변경이 일어날 때, 생성 또는 삭제될 model을 결정하는 곳이다.
    // 결정된 모델 정보를 GameManager에게 전달하여 모델을 생성 및 삭제한다.
    private void ChangeSceneOperation(string currentSceneState, string nextSceneState)
    {
        if (currentSceneState.Equals(nextSceneState)) return;

        // 각 Scene 마다 필요로 하는 객체정보들을 가져온다.
        string[] currentSceneModels = SpecifyModelUsedInScene(currentSceneState);
        string[] nextSceneModels = SpecifyModelUsedInScene(nextSceneState);

        // 집합 연산이 저장되는 형태.        // 차집합 연산을 통해 필요한 객체 도출.
        IEnumerable<string>  modelToCreate = nextSceneModels.Except(currentSceneModels);
        IEnumerable<string>  modelToDelete = currentSceneModels.Except(nextSceneModels);

        // GameMangaer에 생성 및 삭제 정보 전달 및 작업 수행 요청.
        gameManager.CreateAndDeleteModel(modelToCreate.Cast<string>().ToArray(), modelToDelete.Cast<string>().ToArray());

        // 작업 수행 후, 해당 Scene에서 필요로 하는 정보들 서버에 요청.
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

    // 각 Scene마다 필요로하는 객체정보들을 반환해준다.
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