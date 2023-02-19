using UnityEngine;
using UnityEngine.EventSystems;

public class VenderProductSlotClickEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform venderSlot;
    private IVenderMenuView_VenderModelController venderMenuView_VenderModelController = null;
    private VenderProductStruct venderProductStruct;
    private bool isClicked;

    private void Start()
    {
        this.venderSlot = this.GetComponent<RectTransform>();

        this.isClicked = false;
    }

    public void SetMouseClickEvent(IVenderMenuView_VenderModelController controller, VenderProductStruct venderProductStruct)
    {
        this.venderMenuView_VenderModelController = controller;
        this.venderProductStruct = venderProductStruct;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isClicked)
        {
            venderSlot.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<RectTransform>().gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClicked)
        {
            venderSlot.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<RectTransform>().gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (venderMenuView_VenderModelController == null) return;

        venderMenuView_VenderModelController.OnClickedVenderProductSlot(venderProductStruct.ProductNumber);
        isClicked = true;
    }

    public void ResetIsClickedInfo()
    {
        isClicked = false;
    }

    public VenderProductStruct GetVenderProductStruct()
    {
        return this.venderProductStruct;
    }
}
