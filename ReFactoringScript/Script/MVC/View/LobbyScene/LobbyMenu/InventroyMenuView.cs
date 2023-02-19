

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IInventoryMenuViewController
{
    public void OnClickedAllButton();
    public void OnClickedNotNFTButton();
    public void OnClickedNFTButton();

    public void OnClickedUnSelectedSkillSlot(string skillUniqueNumber);
    public void OnClickedSelectedSkillSlot(string skillUniqueNumber);
}

public interface IInventoryMenuView
{
    public void UnMarkSelectedSkillSlot();
    public void DisplayAllSkill();
    public void DisplayNFTSkill();
    public void DisplayNotNFTSkill();
}

public class InventoryMenuView : IInventoryMenuView, SelectedCharacterNumberObserver, SelectedSkillNumberObserver, OwnedSkillNumberObserver
{
    private ISelectedCharacterNumberObserver selectedCharacterNumberModel;
    private ISelectedSkillNumberObserver selectedSkillNumberModel;
    private IOwnedSkillNumberObserver ownedSkillNumberModel;

    private IInventoryMenuViewController inventoryMenuViewController;

    private RectTransform canvas = null;

    private RectTransform buttonTypePanel = null;
    private RectTransform inventoryPanel = null;

    private RectTransform selectedCharacterPanel = null;
    private RectTransform selectedCharacterSkillPanel = null;

    private RectTransform selectedSkillPanel = null;

    // Character와 Skill 정보가 들어있는 Json 데이터
    private JArray characterInformationTable = null;
    private JArray skillInformationTable = null;

    private int selectedCharacterNumber;
    private System.Collections.Generic.List<SkillStruct> selectedSkillNumber = new System.Collections.Generic.List<SkillStruct>();
    private System.Collections.Generic.List<SkillStruct> ownedSkillNumber = new System.Collections.Generic.List<SkillStruct>();

    public InventoryMenuView(IUserModel model, IInventoryMenuViewController controller)
    {
        this.selectedCharacterNumberModel = model;
        this.selectedSkillNumberModel = model;
        this.ownedSkillNumberModel = model;

        this.inventoryMenuViewController = controller;

        this.selectedCharacterNumberModel.RegisterSelectedCharacterNumberObserver(this);
        this.selectedSkillNumberModel.RegisterSelectedSkillNumberObserver(this);
        this.ownedSkillNumberModel.RegisterOwnedSkillNumberObserver(this);

        SetView();
    }

    private void SetView()
    {
        this.canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();

        buttonTypePanel = canvas.GetChild(0).GetChild(1).GetChild(2).GetChild(0).GetChild(1).GetComponent<RectTransform>();
        inventoryPanel = canvas.GetChild(0).GetChild(1).GetChild(2).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>();

        selectedCharacterPanel = canvas.GetChild(0).GetChild(1).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        selectedCharacterSkillPanel = canvas.GetChild(0).GetChild(1).GetChild(2).GetChild(1).GetChild(1).GetComponent<RectTransform>();

        selectedSkillPanel = canvas.GetChild(0).GetChild(1).GetChild(2).GetChild(1).GetChild(2).GetComponent<RectTransform>();

        buttonTypePanel.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { inventoryMenuViewController.OnClickedAllButton(); });
        buttonTypePanel.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { inventoryMenuViewController.OnClickedNotNFTButton(); });
        buttonTypePanel.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { inventoryMenuViewController.OnClickedNFTButton(); });

        TextAsset characterInformation = Resources.Load<TextAsset>("GameData/CharacterInformationTable");
        characterInformationTable = JArray.Parse(characterInformation.ToString());

        TextAsset skillInformation = Resources.Load<TextAsset>("GameData/SkillInformationTable");
        skillInformationTable = JArray.Parse(skillInformation.ToString());
    }

    // 2. Model의 정보가 바뀌면 업데이트 해주는 부분.
    public void UpdateOwnedSkillNumberObserver()
    {
        ownedSkillNumber = ownedSkillNumberModel.GetOwnedSkillNumber();

        RemoveOwnedSkillSlot();
        SetOwnedSkillNumberObserver();
    }

    public void UpdateSelectedCharacterNumberObserver()
    {
        selectedCharacterNumber = selectedCharacterNumberModel.GetSelectedCharacterNumber();

        RemoveSelectedCharacterPrafab();
        SetSelectedCharacterPrafab();
        SetSelectedCharacterInformation();
    }

    public void UpdateSelectedSkillNumberObserver()
    {
        selectedSkillNumber = selectedSkillNumberModel.GetSelectedSkillNumber();

        RemoveSelectedSkillSlot();
        MarkSelectedSkillSlot();
        SetSelectedSkillNumberObserver();
    }

    // 3. Model의 정보가 바뀜에 따라 호출되는 부분.
    private void SetOwnedSkillNumberObserver()
    {
        for(int i=0; i< this.ownedSkillNumber.Count; i++)
        {
            RectTransform skillPanelPrefab = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/SkillImagePrefab/Inventory/SkillImagePanel"));
            skillPanelPrefab.transform.SetParent(inventoryPanel);

            skillPanelPrefab.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[ownedSkillNumber[i].SkillNumber]["skillImagePath"]);
            skillPanelPrefab.GetChild(1).gameObject.SetActive(false);

            skillPanelPrefab.GetComponent<SkillSlotClickEvent>().SetMouseClickEvent(inventoryMenuViewController, ownedSkillNumber[i], false);
            skillPanelPrefab.name = ownedSkillNumber[i].SkillUniqueNumber;
        }
    }

    private void MarkSelectedSkillSlot()
    {
        if (inventoryPanel.childCount == 0) return;

        for (int i=0; i< this.inventoryPanel.childCount; i++)
        {
            for (int j = 0; j < this.selectedSkillNumber.Count; j++)
            {
                if (selectedSkillNumber[j].SkillUniqueNumber.Equals(inventoryPanel.GetChild(i).GetComponent<SkillSlotClickEvent>().GetSkillStruct().SkillUniqueNumber))
                {
                    inventoryPanel.GetChild(i).GetChild(1).gameObject.SetActive(true);
                }
            }
        }
    }

    public void UnMarkSelectedSkillSlot()
    {
        if (inventoryPanel.childCount == 0) return;

        for (int i = 0; i < this.inventoryPanel.childCount; i++)
        {
            for (int j = 0; j < this.selectedSkillNumber.Count; j++)
            {
                if (selectedSkillNumber[j].SkillUniqueNumber.Equals(inventoryPanel.GetChild(i).GetComponent<SkillSlotClickEvent>().GetSkillStruct().SkillUniqueNumber))
                {
                    inventoryPanel.GetChild(i).GetChild(1).gameObject.SetActive(false);
                    inventoryPanel.GetChild(i).GetComponent<SkillSlotClickEvent>().ResetisClicked();
                }
            }
        }
    }

    private void SetSelectedCharacterPrafab()
    {
        RectTransform selectedCharacterPrefab_Right = MonoBehaviour.Instantiate(Resources.Load<RectTransform>((string)characterInformationTable[selectedCharacterNumber]["characterLobbyPrefabAnimationPath"]));
        selectedCharacterPrefab_Right.SetParent(selectedCharacterPanel);

        selectedCharacterPrefab_Right.GetComponent<Animator>().SetBool("isClicked", false);
    }

    private void SetSelectedCharacterInformation()
    {
        int characterSkill01 = (int)characterInformationTable[selectedCharacterNumber]["baseSkill01"];
        int characterSkill02 = (int)characterInformationTable[selectedCharacterNumber]["baseSkill02"];

        selectedCharacterSkillPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[characterSkill01]["skillImagePath"]); ;
        selectedCharacterSkillPanel.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)skillInformationTable[characterSkill01]["skillName"]; ;
        selectedCharacterSkillPanel.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)skillInformationTable[characterSkill01]["skillDescription"];

        selectedCharacterSkillPanel.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[characterSkill02]["skillImagePath"]); ;
        selectedCharacterSkillPanel.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)skillInformationTable[characterSkill02]["skillName"];
        selectedCharacterSkillPanel.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)skillInformationTable[characterSkill02]["skillDescription"];
    }

    private void SetSelectedSkillNumberObserver()
    {
        for (int i = 0; i < this.selectedSkillNumber.Count; i++)
        {
            RectTransform skillPanelPrefab = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/SkillImagePrefab/Inventory/SkillImagePanel"));
            skillPanelPrefab.transform.SetParent(selectedSkillPanel);

            skillPanelPrefab.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[selectedSkillNumber[i].SkillNumber]["skillImagePath"]);
            skillPanelPrefab.GetChild(1).gameObject.SetActive(false);

            skillPanelPrefab.GetComponent<SkillSlotClickEvent>().SetMouseClickEvent(inventoryMenuViewController, selectedSkillNumber[i], true);
            skillPanelPrefab.name = selectedSkillNumber[i].SkillUniqueNumber;
        }
    }

    private void RemoveOwnedSkillSlot()
    {
        if (inventoryPanel.childCount == 0) return;

        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            UnityEngine.Object.Destroy(inventoryPanel.GetChild(i).gameObject);
        }

        inventoryPanel.DetachChildren();
    }

    private void RemoveSelectedSkillSlot()
    {
        if (selectedSkillPanel.childCount == 0) return;

        for (int i = 0; i < selectedSkillPanel.childCount; i++)
        {
            UnityEngine.Object.Destroy(selectedSkillPanel.GetChild(i).gameObject);
        }

        selectedSkillPanel.DetachChildren();
    }

    private void RemoveSelectedCharacterPrafab()
    {
        if (selectedCharacterPanel.childCount == 0) return;

        UnityEngine.Object.Destroy(selectedCharacterPanel.GetChild(0).gameObject);

        selectedCharacterPanel.DetachChildren();
    }

    // 4. Model의 정보는 변경되지 않지만, 사용자의 입력으로 인해 보이는 View가 변경되는 부분.
    public void DisplayAllSkill()
    {
        if (inventoryPanel.childCount == 0) return;

        for(int i = 0; i < inventoryPanel.childCount; i++)
        {
            inventoryPanel.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void DisplayNFTSkill()
    {
        if (inventoryPanel.childCount == 0) return;

        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            if (string.IsNullOrEmpty(inventoryPanel.GetChild(i).GetComponent<SkillSlotClickEvent>().GetSkillStruct().NFTNumber))
            {
                inventoryPanel.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                inventoryPanel.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void DisplayNotNFTSkill()
    {
        if (inventoryPanel.childCount == 0) return;

        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            if (string.IsNullOrEmpty(inventoryPanel.GetChild(i).GetComponent<SkillSlotClickEvent>().GetSkillStruct().NFTNumber))
            {
                inventoryPanel.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                inventoryPanel.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
