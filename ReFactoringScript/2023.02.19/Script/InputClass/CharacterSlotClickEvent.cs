using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSlotClickEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform characterSlot;
    private ICharacterMenuViewController characterMenuViewController = null;
    private int selectedCharacterNumber;
    private Animator animator;
    private bool isClicked;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        characterSlot = this.GetComponent<RectTransform>();
        isClicked = false;
    }

    public void SetMouseClickEvent(ICharacterMenuViewController controller, int selectedCharacterNumber)
    {
        this.characterMenuViewController = controller;
        this.selectedCharacterNumber = selectedCharacterNumber;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isClicked) this.animator.SetBool("isClicked", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClicked) this.animator.SetBool("isClicked", false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (characterMenuViewController == null) return;

        characterMenuViewController.OnClickedCharacterSlot(selectedCharacterNumber);
        isClicked = true;
    }

    public int GetSelectedCharacterNumber()
    {
        return selectedCharacterNumber;
    }

    public void ResetIsClickedInfo()
    {
        isClicked = false;
    }
}
