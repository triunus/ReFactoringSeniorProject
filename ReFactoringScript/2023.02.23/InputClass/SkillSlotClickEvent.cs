using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlotClickEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform skillSlot;
    private IInventoryMenuViewController inventoryMenuViewController = null;
    private SkillStruct skillStruct;
    private bool isSelected;
    private bool isClicked;

    private void Start()
    {
        skillSlot = this.GetComponent<RectTransform>();
        this.isClicked = false;
    }

    public void SetMouseClickEvent(IInventoryMenuViewController controller, SkillStruct skillStruct, bool isSelected)
    {
        this.inventoryMenuViewController = controller;
        this.skillStruct = skillStruct;

        this.isSelected = isSelected;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isClicked) skillSlot.GetChild(1).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClicked) skillSlot.GetChild(1).gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryMenuViewController == null) return;

        if (this.isSelected)
        {
            inventoryMenuViewController.OnClickedSelectedSkillSlot(skillStruct.SkillUniqueNumber);
        }
        else
        {
            inventoryMenuViewController.OnClickedUnSelectedSkillSlot(skillStruct.SkillUniqueNumber);
        }

        isClicked = true;
    }

    public void ResetisClicked()
    {
        this.isClicked = false;
    }

    public SkillStruct GetSkillStruct()
    {
        return this.skillStruct;
    }
}
