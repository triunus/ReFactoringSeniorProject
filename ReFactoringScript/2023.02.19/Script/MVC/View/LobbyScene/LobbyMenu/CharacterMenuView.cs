using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public interface ICharacterMenuViewController
{
    public void OnClickedCharacterSlot(int selectedCharacterNumber);
}

public interface ICharacterMenuView 
{
    public void UnMarkSelectedCharacterSlot();
}

public class CharacterMenuView : ICharacterMenuView, SelectedCharacterNumberObserver, OwnedCharacterNumberObserver
{
    private ISelectedCharacterNumberObserver selectedCharacterNumberModel;
    private IOwnedCharacterNumberObserver ownedCharacterNumberModel;

    private ICharacterMenuViewController characterMenuViewController;

    private RectTransform canvas = null;

    private RectTransform characterPanel = null;

    private RectTransform rightCharacterPrefabPanel = null;
    private RectTransform rightCharacterDescriptionPanel = null;
    private RectTransform rightCharacterSkillPanel = null;

    // Character와 Skill 정보가 들어있는 Json 데이터
    private JArray characterInformationTable = null;
    private JArray skillInformationTable = null;

    private int selectedCharacterNumber;
    private System.Collections.Generic.List<int> ownedCharacterNumber = new System.Collections.Generic.List<int>();

    public CharacterMenuView(IUserModel model, ICharacterMenuViewController controller)
    {
        this.selectedCharacterNumberModel = model;
        this.ownedCharacterNumberModel = model;

        this.characterMenuViewController = controller;

        this.selectedCharacterNumberModel.RegisterSelectedCharacterNumberObserver(this);
        this.ownedCharacterNumberModel.RegisterOwnedCharacterNumberObserver(this);

        SetView();
    }

    // 1. IView 인터페이스의 메소드를 구현한 것이다.
    // View에서 공통적으로 구현해야 할 '초기 설정, 갱신, 삭제, 버튼 제어, 객체 (비)활성화'를 구현한다.
    private void SetView()
    {
        this.canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();

        characterPanel = canvas.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();

        rightCharacterPrefabPanel = canvas.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        rightCharacterDescriptionPanel = canvas.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<RectTransform>();
        rightCharacterSkillPanel = canvas.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<RectTransform>();

        TextAsset characterInformation = Resources.Load<TextAsset>("GameData/CharacterInformationTable");
        characterInformationTable = JArray.Parse(characterInformation.ToString());

        TextAsset skillInformation = Resources.Load<TextAsset>("GameData/SkillInformationTable");
        skillInformationTable = JArray.Parse(skillInformation.ToString());
    }

    // 2_1. Observer를 통해 Model에서 데이터를 전달받는 Update 함수이다.
    public void UpdateSelectedCharacterNumberObserver()
    {
        selectedCharacterNumber = this.selectedCharacterNumberModel.GetSelectedCharacterNumber();

        RemoveSelectedCharacterPrafab_Right();
        MarkSelectedCharacterSlot();
        SetSelectedCharacterPrafab_Right();
        SetSelectedCharacterInformation();
    }

    public void UpdateOwnedCharacterNumberObserver()
    {
        ownedCharacterNumber = this.ownedCharacterNumberModel.GetOwnedCharacterNumber();

        RemoveOwnedCharacterSlotPanel();
        SetOwnedCharacterSlotPanel();
    }
    // -------------------------------


    // 2_2. CharacterMenuView의 정보들을 갱신해준다.
    // 소유하고 있는 캐릭터들을 보여준다.
    private void SetOwnedCharacterSlotPanel()
    {
        for (int i = 0; i < this.ownedCharacterNumber.Count; i++)
        {
            RectTransform characterPanelPrefab = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/CharacterPrefab/CharacterImagePrefab"));
            RectTransform characterPrefab = MonoBehaviour.Instantiate(Resources.Load<RectTransform>((string)characterInformationTable[ownedCharacterNumber[i]]["characterLobbyPrefabAnimationPath"]));
            characterPanelPrefab.SetParent(characterPanel);
            characterPrefab.SetParent(characterPanelPrefab.GetChild(0));

            // 프리팹 클릭 관련 메소드 연결
            characterPrefab.GetComponent<CharacterSlotClickEvent>().SetMouseClickEvent(characterMenuViewController, ownedCharacterNumber[i]);
        }
    }

    // 소유 캐릭터가 존재하는 패널에서 사용자가 클릭한 GameObject에 색의 변화를 주어 나타낸다.
    private void MarkSelectedCharacterSlot()
    {
        for (int i = 0; i < this.characterPanel.childCount; i++)
        {
            if (selectedCharacterNumber == characterPanel.GetChild(i).GetChild(0).GetChild(0).GetComponent<CharacterSlotClickEvent>().GetSelectedCharacterNumber())
            {
                characterPanel.GetChild(i).GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("isClicked", true);
            }
        }
    }

    private void SetSelectedCharacterPrafab_Right()
    {
        RectTransform selectedCharacterPrefab_Right = MonoBehaviour.Instantiate(Resources.Load<RectTransform>((string)characterInformationTable[selectedCharacterNumber]["characterLobbyPrefabAnimationPath"]));
        selectedCharacterPrefab_Right.SetParent(rightCharacterPrefabPanel);

        selectedCharacterPrefab_Right.GetComponent<Animator>().SetBool("isClicked", true);
    }

    // 사용자가 클릭한 Character의 상세 정보를 나타낸다.
    private void SetSelectedCharacterInformation()
    {
        rightCharacterDescriptionPanel.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)characterInformationTable[selectedCharacterNumber]["characterName"];
        rightCharacterDescriptionPanel.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)characterInformationTable[selectedCharacterNumber]["characterDescription"];

        int characterSkill01 = (int)characterInformationTable[selectedCharacterNumber]["baseSkill01"];
        int characterSkill02 = (int)characterInformationTable[selectedCharacterNumber]["baseSkill02"];

        rightCharacterSkillPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[characterSkill01]["skillImagePath"]);
        rightCharacterSkillPanel.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)skillInformationTable[characterSkill01]["skillName"];
        rightCharacterSkillPanel.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)skillInformationTable[characterSkill01]["skillDescription"];

        rightCharacterSkillPanel.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[characterSkill02]["skillImagePath"]);
        rightCharacterSkillPanel.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)skillInformationTable[characterSkill02]["skillName"];
        rightCharacterSkillPanel.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)skillInformationTable[characterSkill02]["skillDescription"];
    }
    // --------------------------------------------

    // 3. GameLobbyView를 비활성화 시킬 때, Model과 상호작용을 하여 정의한 값들 중 동적으로 생성한 Prefab을 삭제한다. (다음에 GameLobbyView를 활성화할때 중복됨을 방지.)
    private void RemoveOwnedCharacterSlotPanel()
    {
        if (characterPanel.childCount == 0) return;

        for (int i = 0; i < characterPanel.childCount; i++)
        {
            Object.Destroy(characterPanel.GetChild(i).gameObject);
        }

        characterPanel.DetachChildren();
    }

    private void RemoveSelectedCharacterPrafab_Right()
    {
        if (rightCharacterPrefabPanel.childCount == 0) return;

        UnityEngine.Object.Destroy(rightCharacterPrefabPanel.GetChild(0).gameObject);

        rightCharacterPrefabPanel.DetachChildren();
    }

    // 4. Controller에서 접근하는 부분.
    // 캐릭터를 선택했을 때, 기존에 선택한 캐릭터의 행동을 원래대로 돌린다.
    public void UnMarkSelectedCharacterSlot()
    {
        for (int i = 0; i < this.characterPanel.childCount; i++)
        {
            if (selectedCharacterNumber == characterPanel.GetChild(i).GetChild(0).GetChild(0).GetComponent<CharacterSlotClickEvent>().GetSelectedCharacterNumber())
            {
                characterPanel.GetChild(i).GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("isClicked", false);
                characterPanel.GetChild(i).GetChild(0).GetChild(0).GetComponent<CharacterSlotClickEvent>().ResetIsClickedInfo();
            }
        }
    }

}
