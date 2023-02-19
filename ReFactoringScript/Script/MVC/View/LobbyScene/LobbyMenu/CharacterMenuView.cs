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

    // Character�� Skill ������ ����ִ� Json ������
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

    // 1. IView �������̽��� �޼ҵ带 ������ ���̴�.
    // View���� ���������� �����ؾ� �� '�ʱ� ����, ����, ����, ��ư ����, ��ü (��)Ȱ��ȭ'�� �����Ѵ�.
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

    // 2_1. Observer�� ���� Model���� �����͸� ���޹޴� Update �Լ��̴�.
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


    // 2_2. CharacterMenuView�� �������� �������ش�.
    // �����ϰ� �ִ� ĳ���͵��� �����ش�.
    private void SetOwnedCharacterSlotPanel()
    {
        for (int i = 0; i < this.ownedCharacterNumber.Count; i++)
        {
            RectTransform characterPanelPrefab = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/CharacterPrefab/CharacterImagePrefab"));
            RectTransform characterPrefab = MonoBehaviour.Instantiate(Resources.Load<RectTransform>((string)characterInformationTable[ownedCharacterNumber[i]]["characterLobbyPrefabAnimationPath"]));
            characterPanelPrefab.SetParent(characterPanel);
            characterPrefab.SetParent(characterPanelPrefab.GetChild(0));

            // ������ Ŭ�� ���� �޼ҵ� ����
            characterPrefab.GetComponent<CharacterSlotClickEvent>().SetMouseClickEvent(characterMenuViewController, ownedCharacterNumber[i]);
        }
    }

    // ���� ĳ���Ͱ� �����ϴ� �гο��� ����ڰ� Ŭ���� GameObject�� ���� ��ȭ�� �־� ��Ÿ����.
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

    // ����ڰ� Ŭ���� Character�� �� ������ ��Ÿ����.
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

    // 3. GameLobbyView�� ��Ȱ��ȭ ��ų ��, Model�� ��ȣ�ۿ��� �Ͽ� ������ ���� �� �������� ������ Prefab�� �����Ѵ�. (������ GameLobbyView�� Ȱ��ȭ�Ҷ� �ߺ����� ����.)
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

    // 4. Controller���� �����ϴ� �κ�.
    // ĳ���͸� �������� ��, ������ ������ ĳ������ �ൿ�� ������� ������.
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
