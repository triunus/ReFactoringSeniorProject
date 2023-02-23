using UnityEngine;

public interface IVenderMenuView_VenderModelController
{
    public void UpdateButtonOnClicked();
    public void BuyButtonOnClicked(int selectedVenderProductNumber);

    public void OnClickedVenderProductSlot(int productName);
}

public interface IVenderMenuView_VenderModel
{
    public void UnMarkSelectedProductSlot();
    public void DisableButton();
    public void ActivateButton();
}

public class VenderMenuView_VenderModel : IVenderMenuView_VenderModel, VenderProductStructObserver, SelectedVenderProductNumberObserver, NPCMessageObserver
{
    private ISelectedVenderProductNumberObserver selectedVenderProductNumberModel;
    private IVenderProductStructObserver venderProductStructModel;
    private INPCMessageViewClassifier NPCMessageViewMediator;

    private IVenderMenuView_VenderModelController venderMenuView_VenderModelController;

    private RectTransform canvas = null;

    private TMPro.TextMeshProUGUI venderLine;

    private RectTransform venderProductsPanel;
    private RectTransform UpdateAndPurchasePanel;

    private Newtonsoft.Json.Linq.JArray skillInformationTable = null;

    private System.Collections.Generic.List<VenderProductStruct> venderProductStructs = new System.Collections.Generic.List<VenderProductStruct>();
    private int selectedVenderProductNumber;
    private string venderNPCLine = null;

    public VenderMenuView_VenderModel(IVenderModel model, IVenderMenuView_VenderModelController controller)
    {
        this.selectedVenderProductNumberModel = model;
        this.venderProductStructModel = model;
        this.NPCMessageViewMediator = model;

        this.venderMenuView_VenderModelController = controller;

        this.selectedVenderProductNumberModel.RegisterSelectedVenderProductNumberObserver(this);
        this.venderProductStructModel.RegisterVenderProductStructObserver(this);
        this.NPCMessageViewMediator.RegisterNPCMessageObserver(this);

        SetView();
    }

    // 0. 사용되는 Panel들의 위치를 명시.
    private void SetView()
    {
        this.canvas = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();

//        venderBackGroundImage = canvas.GetChild(0).GetChild(1).GetChild(3).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>();
//        venderImage = canvas.GetChild(0).GetChild(1).GetChild(3).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Image>();

        venderLine = canvas.GetChild(0).GetChild(1).GetChild(3).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();

        venderProductsPanel = canvas.GetChild(0).GetChild(1).GetChild(3).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        UpdateAndPurchasePanel = canvas.GetChild(0).GetChild(1).GetChild(3).GetChild(1).GetChild(1).GetChild(2).GetComponent<RectTransform>();

        // update
        UpdateAndPurchasePanel.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { venderMenuView_VenderModelController.UpdateButtonOnClicked(); });
        //purchase
        UpdateAndPurchasePanel.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { venderMenuView_VenderModelController.BuyButtonOnClicked(selectedVenderProductNumber); });

        selectedVenderProductNumber = -1;

        TextAsset skillInformation = Resources.Load<TextAsset>("GameData/SkillInformationTable");
        skillInformationTable = Newtonsoft.Json.Linq.JArray.Parse(skillInformation.ToString());
    }
    // -------------------------

    public void ActivateButton()
    {
        canvas.GetChild(0).GetChild(1).GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void DisableButton()
    {
        canvas.GetChild(0).GetChild(1).GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    // -------------------------


    // 2_1. Observer
    public void UpdateVenderProductStructObserver()
    {
        venderProductStructs = this.venderProductStructModel.GetVenderProductStruct();

        RemoveVenderProductSlotPanel();
        SetVenderProductSlotPanel();
        MarkSoldOutVenderProduct();
    }

    public void UpdateSelectedVenderProductNumberObserver()
    {
        selectedVenderProductNumber = this.selectedVenderProductNumberModel.GetSelectedVenderProductNumber();

        MarkSelectedProductSlot();
    }

    public void UpdateNPCMessageObserver()
    {
        venderNPCLine = this.NPCMessageViewMediator.GetNPCMessage();

        SetVenderNPCLine();
    }

    // 2_1. Update에 대한 설정
    private void SetVenderProductSlotPanel()
    {
        for (int i=0; i< this.venderProductStructs.Count; i++)
        {
            RectTransform venderProductPrefab = MonoBehaviour.Instantiate(Resources.Load<RectTransform>("Prefab/ProductPrefab/Vender/VenderProduct"));
            venderProductPrefab.SetParent(venderProductsPanel);

            venderProductPrefab.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = (string)skillInformationTable[venderProductStructs[i].SkillNumber]["skillName"];
            venderProductPrefab.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>((string)skillInformationTable[venderProductStructs[i].SkillNumber]["skillImagePath"]);
            venderProductPrefab.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(venderProductStructs[i].Price);

            venderProductPrefab.GetComponent<VenderProductSlotClickEvent>().SetMouseClickEvent(this.venderMenuView_VenderModelController, venderProductStructs[i]);
        }
    }
    private void SetVenderNPCLine()
    {
        venderLine.text = this.venderNPCLine;
    }

    // 2_2. 클릭 정보 표시
    private void MarkSelectedProductSlot()
    {
        if (selectedVenderProductNumber == -1) return;

        for (int i = 0; i < venderProductsPanel.childCount; i++)
        {
            if (selectedVenderProductNumber == venderProductsPanel.GetChild(i).GetComponent<VenderProductSlotClickEvent>().GetVenderProductStruct().ProductNumber)
            {
                venderProductsPanel.GetChild(i).GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<RectTransform>().gameObject.SetActive(true);
            }
        }
    }
    public void UnMarkSelectedProductSlot()
    {
        for (int i = 0; i < venderProductsPanel.childCount; i++)
        {
            if (selectedVenderProductNumber == venderProductsPanel.GetChild(i).GetComponent<VenderProductSlotClickEvent>().GetVenderProductStruct().ProductNumber)
            {
                venderProductsPanel.GetChild(i).GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<RectTransform>().gameObject.SetActive(false);
                venderProductsPanel.GetChild(i).GetComponent<VenderProductSlotClickEvent>().ResetIsClickedInfo();
            }
        }
    }

    // 2_3. 판매 완료된 상품 표시.
    private void MarkSoldOutVenderProduct()
    {
        if (venderProductsPanel.childCount == 0) return;

        for (int i = 0; i < venderProductsPanel.childCount; i++)
        {            
            if (venderProductsPanel.GetChild(i).GetComponent<VenderProductSlotClickEvent>().GetVenderProductStruct().IsBye)
            {
                venderProductsPanel.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
        }
    }
    // -----------------------------

    // 3. 삭제
    private void RemoveVenderProductSlotPanel()
    {
        if (venderProductsPanel.childCount == 0) return;

        for (int i = 0; i < venderProductsPanel.childCount; i++)
        {
            UnityEngine.Object.Destroy(venderProductsPanel.GetChild(i).gameObject);
        }

        venderProductsPanel.DetachChildren();
    }

}
