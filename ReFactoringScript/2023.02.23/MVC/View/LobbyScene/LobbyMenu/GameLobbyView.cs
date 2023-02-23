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

    // ���� �⺻ ����, ��ȭ ����
    private Image userImage = null;
    private TextMeshProUGUI userNickname = null;
    private TextMeshProUGUI coin = null;
    private TextMeshProUGUI paidCoid = null;

    // ������ ����
    private RectTransform selectedCharacterPanel= null;
    private RectTransform charcterSkillPanel = null;
    private RectTransform selectedSkillPanel = null;

    // GameLobby ���� ����� ���� �������� Panel�� ����/����� ��ư.
    private RectTransform SelectionInformationPanel = null;
    private Button ShowSelectionInformationButton;

    // Character�� Skill ������ ����ִ� Json ������
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


    // 1. IView �������̽��� �޼ҵ带 ������ ���̴�.
    // View���� ���������� �����ؾ� �� '�ʱ� ����, ����, ����, ��ư ����, ��ü (��)Ȱ��ȭ'�� �����Ѵ�.
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

    // 2_1. �������� ����, Model�� �����͸� ���´�.
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

    // 2_2. Model�� ������ �޾ƿ���, ������ �����Ѵ�.
    // �� ������ �����ϴ� ��.
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

        // ���� ĳ������ ��ų �̹��� ������ ���.
        for(int i = 0; i < 2; i++)
        {
            RectTransform skillImagePanel = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/SkillImagePrefab/GameLobby/SkillImagePanel"));

            skillImagePanel.transform.SetParent(charcterSkillPanel);

            skillImagePanel.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[characterSkill[i]]["skillImagePath"]);
        }
    }

    private void SetSelectedSkill()
    {
        // ���� ��ų �̹��� ������ ���.
        for (int i = 0; i < selectedSkillNumber.Count; i++)
        {
            RectTransform skillImagePanel = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/SkillImagePrefab/GameLobby/SkillImagePanel"));

            skillImagePanel.transform.SetParent(selectedSkillPanel);

            skillImagePanel.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[selectedSkillNumber[i].SkillNumber]["skillImagePath"]);
        }
    }
    // --------------------------------------

    // 3. GameLobbyView�� ��Ȱ��ȭ ��ų ��, Model�� ��ȣ�ۿ��� �Ͽ� ������ ���� �� �������� ������ Prefab�� �����Ѵ�. (������ GameLobbyView�� Ȱ��ȭ�Ҷ� �ߺ����� ����.)
    private void RemveCharacterPrefab()
    {
        if (selectedCharacterPanel.childCount == 0) return;

        for (int i = 0; i < selectedCharacterPanel.childCount; i++)
        {
            UnityEngine.Object.Destroy(selectedCharacterPanel.GetChild(i).gameObject);
        }

        // Destroy�� ���� �Ǿ, 1������ ������ ������ ���� �ش� ������Ʈ�� ������� �ʴ´�.
        // ���� �ش� �г��� �θ𿡼� �����Ͽ�, �θ��� ���ϵ带 ����ϴ� �ڵ忡 ������ ��ġ�� ���ϵ��� �Ѵ�.
        selectedCharacterPanel.DetachChildren();
    }

    private void RemoveCharacterSkills()
    {
        if (charcterSkillPanel.childCount == 0) return;

        for(int i =0; i< charcterSkillPanel.childCount; i++)
        {
            UnityEngine.Object.Destroy(charcterSkillPanel.GetChild(i).gameObject);
        }

        // Destroy�� ���� �Ǿ, 1������ ������ ������ ���� �ش� ������Ʈ�� ������� �ʴ´�.
        // ���� �ش� �г��� �θ𿡼� �����Ͽ�, �θ��� ���ϵ带 ����ϴ� �ڵ忡 ������ ��ġ�� ���ϵ��� �Ѵ�.
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

    // 4. GameLobbyView�� ���� Ư�� ��ɵ��� �����Ѵ�.
    // GameLobby ���� ��� �г� Ȱ��ȭ / ��Ȱ��ȭ
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
