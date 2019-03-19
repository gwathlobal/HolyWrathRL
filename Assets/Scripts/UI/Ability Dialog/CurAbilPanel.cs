using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CurAbilPanel : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Text txt;
    public int curAbil;
    public AbilityTypeEnum abilType;
    public AbilitySlotCategoty abilSlot;
    public AbilityDialogScript AbilityDialog;

    private Vector3 startPosition;
    private Transform startParent;

    public void InitializeUI(string str)
    {
        txt.text = str;
    }

    public void ActivateText(bool active)
    {
        txt.gameObject.SetActive(active);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objectDragged = AbilityDialog.objectDragged;

        if (objectDragged == null) return;

        AbilityTypeEnum abilityType;

        if (objectDragged.GetComponent<AvailAbilPanel>())
        {
            objectDragged.GetComponent<AvailAbilPanel>().OnEndDrag(eventData);
            abilityType = objectDragged.GetComponent<AvailAbilPanel>().abilType;
        }
        else if (objectDragged.GetComponent<CurAbilPanel>())
        {
            objectDragged.GetComponent<CurAbilPanel>().OnEndDrag(eventData);
            abilityType = objectDragged.GetComponent<CurAbilPanel>().abilType;
        }
        else return;

        if (AbilityTypes.abilTypes[abilityType].slot != abilSlot) return;

        if (abilSlot == AbilitySlotCategoty.abilNormal)
            AbilityDialog.player.curAbils[curAbil] = abilityType;
        else if (abilSlot == AbilitySlotCategoty.abilSprint)
            AbilityDialog.player.sprintAbil = abilityType;
        else if (abilSlot == AbilitySlotCategoty.abilDodge)
            AbilityDialog.player.dodgeAbil = abilityType;
        else if (abilSlot == AbilitySlotCategoty.abilBlock)
            AbilityDialog.player.blockAbil = abilityType;
        else if (abilSlot == AbilitySlotCategoty.abilMelee)
            AbilityDialog.player.meleeAbil = abilityType;
        else if (abilSlot == AbilitySlotCategoty.abilRanged)
            AbilityDialog.player.rangedAbil = abilityType;

        AbilityDialog.InitializeUI(AbilityDialog.player);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (abilType == AbilityTypeEnum.abilNone) return;

        AbilityDialog.objectDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        transform.SetParent(AbilityDialog.canvasTrasform, true);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // make all eligible current ability panels green
        foreach (CurAbilPanel curAbilPanel in AbilityDialog.curAbilsPanels)
        {
            if (AbilityTypes.abilTypes[abilType].slot == AbilityTypes.abilTypes[curAbilPanel.abilType].slot)
                curAbilPanel.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        }
        if (AbilityTypes.abilTypes[abilType].slot == AbilityDialog.sprintAbilPanel.abilSlot)
            AbilityDialog.sprintAbilPanel.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        if (AbilityTypes.abilTypes[abilType].slot == AbilityDialog.dodgeAbilPanel.abilSlot)
            AbilityDialog.dodgeAbilPanel.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        if (AbilityTypes.abilTypes[abilType].slot == AbilityDialog.blockAbilPanel.abilSlot)
            AbilityDialog.blockAbilPanel.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        if (AbilityTypes.abilTypes[abilType].slot == AbilityDialog.meleeAbilPanel.abilSlot)
            AbilityDialog.meleeAbilPanel.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        if (AbilityTypes.abilTypes[abilType].slot == AbilityDialog.rangedAbilPanel.abilSlot)
            AbilityDialog.rangedAbilPanel.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (AbilityDialog.objectDragged == null) return;

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (AbilityDialog.objectDragged == null) return;

        AbilityDialog.objectDragged = null;
        transform.position = startPosition;
        transform.SetParent(startParent, true);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (abilSlot == AbilitySlotCategoty.abilNormal)
            AbilityDialog.player.curAbils[curAbil] = AbilityTypeEnum.abilNone;
        else if (abilSlot == AbilitySlotCategoty.abilSprint)
            AbilityDialog.player.sprintAbil = AbilityTypeEnum.abilNone;
        else if (abilSlot == AbilitySlotCategoty.abilDodge)
            AbilityDialog.player.dodgeAbil = AbilityTypeEnum.abilNone;
        else if (abilSlot == AbilitySlotCategoty.abilBlock)
            AbilityDialog.player.blockAbil = AbilityTypeEnum.abilNone;
        else if (abilSlot == AbilitySlotCategoty.abilMelee)
            AbilityDialog.player.meleeAbil = AbilityTypeEnum.abilNone;
        else if (abilSlot == AbilitySlotCategoty.abilRanged)
            AbilityDialog.player.rangedAbil = AbilityTypeEnum.abilNone;

        AbilityDialog.InitializeUI(AbilityDialog.player);
        
        foreach (CurAbilPanel curAbilPanel in AbilityDialog.curAbilsPanels)
        {
            curAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        AbilityDialog.sprintAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        AbilityDialog.dodgeAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        AbilityDialog.blockAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        AbilityDialog.meleeAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        AbilityDialog.rangedAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (abilType != AbilityTypeEnum.abilNone)
        {
            AbilityDialog.DescrText.text = AbilityTypes.abilTypes[abilType].GetFullDescription(AbilityDialog.player);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityDialog.DescrText.text = "";
    }
}
