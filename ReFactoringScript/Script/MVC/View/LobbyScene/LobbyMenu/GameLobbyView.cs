using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public interface IGameLobbyViewController
{
    public void OnShowSelectionInformationButtonClicked();
}

public interface IGameLobbyView
{
    public void ControllerSelectionInformationPanel();
}

public class GameLobbyView : IGameLobbyView, UserInformationObserver, SelectedCharacterNumberObserver, SelectedSkillNumberObserver
{
    private IUserInformationObserver userInformationModel;
    private ISelectedCharacterNumberObserver selectedCharacterNumberModel;
    private ISelectedSkillNumberObserver selectedSkillNumberModel;

    private IGameLobbyViewController controller;

    private RectTransform canvas = null;

    // 유저 기본 정보, 재화 정보
    private Image userImage = null;
    private TextMeshProUGUI userNickname = null;
    private TextMeshProUGUI coin = null;
    private TextMeshProUGUI paidCoid = null;

    // 선택한 정보
    private RectTransform selectedCharacterPanel= null;
    private RectTransform charcterSkillPanel = null;
    private RectTransform selectedSkillPanel = null;

    // GameLobby 좌측 상단의 유저 선택정보 Panel과 보기/숨기기 버튼.
    private RectTransform SelectionInformationPanel = null;
    private Button ShowSelectionInformationButton;

    // Character와 Skill 정보가 들어있는 Json 데이터
    private JArray characterInformationTable = null;
    private JArray skillInformationTable = null;

    private UserData userData = null;
    private int selectedCharacterNumber;
    private List<SkillStruct> selectedSkillNumber = new List<SkillStruct>();

    private bool isActive = false;

    public GameLobbyView(IUserModel model, IGameLobbyViewController controller)
    {
        this.userInformationModel = model;
        this.selectedCharacterNumberModel = model;
        this.selectedSkillNumberModel = model;

        this.controller = controller;

        this.userInformationModel.RegisterUserInformationObserver(this);
        this.selectedCharacterNumberModel.RegisterSelectedCharacterNumberObserver(this);
        this.selectedSkillNumberModel.RegisterSelectedSkillNumberObserver(this);

        SetView();
    }


    // 1. IView 인터페이스의 메소드를 구현한 것이다.
    // View에서 공통적으로 구현해야 할 '초기 설정, 갱신, 삭제, 버튼 제어, 객체 (비)활성화'를 구현한다.
    private void SetView()
    {
        this.canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();

        userImage = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        userNickname = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        coin = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        paidCoid = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        selectedCharacterPanel = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        charcterSkillPanel = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<RectTransform>(); ;
        selectedSkillPanel = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<RectTransform>(); ;

        SelectionInformationPanel = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<RectTransform>();
        ShowSelectionInformationButton = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>();
        
        ShowSelectionInformationButton.onClick.AddListener(delegate { controller.OnShowSelectionInformationButtonClicked(); });

        TextAsset characterInformation = Resources.Load<TextAsset>("GameData/CharacterInformationTable");
        characterInformationTable = JArray.Parse(characterInformation.ToString());

        TextAsset skillInformation = Resources.Load<TextAsset>("GameData/SkillInformationTable");
        skillInformationTable = JArray.Parse(skillInformation.ToString());
    }

    // 2_1. 옵저버를 통해, Model의 데이터를 얻어온다.
    public void UpdateUserInformationObserver()
    {
        userData = userInformationModel.GetUserInformation();

        SetUserInformation();
    }

    public void UpdateSelectedCharacterNumberObserver()
    {
        selectedCharacterNumber = selectedCharacterNumberModel.GetSelectedCharacterNumber();

        RemveCharacterPrefab();
        RemoveCharacterSkills();
        SetSelectedCharacter();
        SetCharacterSkills();
    }

    public void UpdateSelectedSkillNumberObserver()
    {
        selectedSkillNumber = selectedSkillNumberModel.GetSelectedSkillNumber();

        RemoveSelectedSkills();
        SetSelectedSkill();
    }
    // -----------------------------------------

    // 2_2. Model의 정보를 받아오면, 정보를 갱신한다.
    // 각 정보를 갱신하는 곳.
    private void SetUserInformation()
    {
        userImage.sprite = Resources.Load<Sprite>("Image/UserImage/UserImage" + this.userData.UserImageNumber);
        userNickname.text = this.userData.UserNickname;

        coin.text = Convert.ToString(this.userData.Coin);

        paidCoid.text = Convert.ToString(this.userData.PaidCoin);
    }

    private void SetSelectedCharacter()
    {
        RectTransform selectedCharacterPrefab = MonoBehaviour.Instantiate(Resources.Load<RectTransform>((string)characterInformationTable[selectedCharacterNumber]["characterLobbyPrefabAnimationPath"]));
        selectedCharacterPrefab.SetParent(selectedCharacterPanel);

        selectedCharacterPrefab.GetComponent<Animator>().SetBool("isClicked", false);
    }
    private void SetCharacterSkills()
    {
        int[] characterSkill = new int[] {
            (int)characterInformationTable[selectedCharacterNumber]["baseSkill01"],
            (int)characterInformationTable[selectedCharacterNumber]["baseSkill02"] 
        };

        // 선택 캐릭터의 스킬 이미지 프리펩 등록.
        for(int i = 0; i < 2; i++)
        {
            RectTransform skillImagePanel = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/SkillImagePrefab/GameLobby/SkillImagePanel"));

            skillImagePanel.transform.SetParent(charcterSkillPanel);

            skillImagePanel.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[characterSkill[i]]["skillImagePath"]);
        }
    }

    private void SetSelectedSkill()
    {
        // 선택 스킬 이미지 프리펩 등록.
        for (int i = 0; i < selectedSkillNumber.Count; i++)
        {
            RectTransform skillImagePanel = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/SkillImagePrefab/GameLobby/SkillImagePanel"));

            skillImagePanel.transform.SetParent(selectedSkillPanel);

            skillImagePanel.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[selectedSkillNumber[i].SkillNumber]["skillImagePath"]);
        }
    }
    // --------------------------------------

    // 3. GameLobbyView를 비활성화 시킬 때, Model과 상호작용을 하여 정의한 값들 중 동적으로 생성한 Prefab을 삭제한다. (다음에 GameLobbyView를 활성화할때 중복됨을 방지.)
    private void RemveCharacterPrefab()
    {
        if (selectedCharacterPanel.childCount == 0) return;

        for (int i = 0; i < selectedCharacterPanel.childCount; i++)
        {
            UnityEngine.Object.Destroy(selectedCharacterPanel.GetChild(i).gameObject);
        }

        // Destroy가 시행 되어도, 1프레임 지나기 전에는 아직 해당 오브젝트는 사라지지 않는다.
        // 따라서 해당 패널의 부모에서 제거하여, 부모의 차일드를 사용하는 코드에 영향을 미치지 못하도록 한다.
        selectedCharacterPanel.DetachChildren();
    }

    private void RemoveCharacterSkills()
    {
        if (charcterSkillPanel.childCount == 0) return;

        for(int i =0; i< charcterSkillPanel.childCount; i++)
        {
            UnityEngine.Object.Destroy(charcterSkillPanel.GetChild(i).gameObject);
        }

        // Destroy가 시행 되어도, 1프레임 지나기 전에는 아직 해당 오브젝트는 사라지지 않는다.
        // 따라서 해당 패널의 부모에서 제거하여, 부모의 차일드를 사용하는 코드에 영향을 미치지 못하도록 한다.
        charcterSkillPanel.DetachChildren();
    }

    private void RemoveSelectedSkills()
    {
        if (selectedSkillPanel.childCount == 0) return;

        for (int i = 0; i < selectedSkillPanel.childCount; i++)
        {
            UnityEngine.Object.Destroy(selectedSkillPanel.GetChild(i).gameObject);
        }

        selectedSkillPanel.DetachChildren();
    }
    // ----------------------------------------

    // 4. GameLobbyView만 갖는 특정 기능들을 정의한다.
    // GameLobby 좌측 상단 패널 활성화 / 비활성화
    public void ControllerSelectionInformationPanel()
    {
        if (isActive)
        {
            this.SelectionInformationPanel.gameObject.SetActive(false);
            isActive = false;
        }
        else
        {
            this.SelectionInformationPanel.gameObject.SetActive(true);
            isActive = true;
        }
    }
    // ---------------------------------
}
