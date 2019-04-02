using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AbilityAddedStatus
{
    selected, available, added, unavailable
}

public class AvailAbilPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public AbilityDialogScript AbilityDialog;
    public Text abilName;
    public AbilityTypeEnum abilType;
    public Button button;
    public Text buttonTxt;

    private Vector3 startPosition;
    private Transform startParent;
    public AbilityAddedStatus status;

    public void InitializeUI(AbilityDialogScript _abilityDialog, string name, AbilityTypeEnum _abilType, AbilityAddedStatus _status)
    {
        AbilityDialog = _abilityDialog;
        abilName.text = name;
        abilType = _abilType;
        status = _status;
        switch (status)
        {
            case AbilityAddedStatus.selected:
                button.gameObject.SetActive(true);
                abilName.color = new Color32(255, 255, 255, 255);
                buttonTxt.text = "-";
                break;
            case AbilityAddedStatus.available:
                button.gameObject.SetActive(true);
                abilName.color = new Color32(150, 150, 150, 255);
                buttonTxt.text = "+";
                break;
            case AbilityAddedStatus.added:
                button.gameObject.SetActive(false);
                abilName.color = new Color32(255, 255, 255, 255);
                break;
            default:
                button.gameObject.SetActive(false);
                abilName.color = new Color32(150, 150, 150, 255);
                break;
        }

        if (AbilityDialog.curUnspentTP == 0 && status == AbilityAddedStatus.available) button.gameObject.SetActive(false);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (status == AbilityAddedStatus.unavailable || status == AbilityAddedStatus.available || 
            AbilityTypes.abilTypes[abilType].slot == AbilitySlotCategoty.abilNone)
        {
            eventData.pointerDrag = null;
            return;
        }

        AbilityDialog.objectDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        transform.SetParent(AbilityDialog.canvasTrasform, true);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // make all eligible current ability panels green
        foreach (CurAbilPanel curAbilPanel in AbilityDialog.curAbilsPanels)
        {
            if (AbilityTypes.abilTypes[abilType].slot == curAbilPanel.abilSlot)
                curAbilPanel.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        }
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
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        AbilityDialog.objectDragged = null;
        transform.position = startPosition;
        transform.SetParent(startParent, true);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        foreach (CurAbilPanel curAbilPanel in AbilityDialog.curAbilsPanels)
        {
            curAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        AbilityDialog.dodgeAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        AbilityDialog.blockAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        AbilityDialog.meleeAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        AbilityDialog.rangedAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AbilityTypes.abilTypes[abilType].id != AbilityTypeEnum.abilNone)
        {
            AbilityDialog.DescrText.text = AbilityTypes.abilTypes[abilType].GetFullDescription(AbilityDialog.player);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityDialog.DescrText.text = "";
    }

    public void ClickSelectAbility()
    {
        if (status == AbilityAddedStatus.available)
        {
            status = AbilityAddedStatus.selected;
            AbilityDialog.addedAbils.Add(abilType);
            AbilityDialog.curUnspentTP--;
            AbilityDialog.SetUnspentTPTxt();

            foreach (GameObject abilPanel in AbilityDialog.availAbilPanels)
            {
                AbilityAddedStatus status = abilPanel.GetComponent<AvailAbilPanel>().status;
                AbilityTypeEnum abilT = abilPanel.GetComponent<AvailAbilPanel>().abilType;
                if (AbilityTypes.abilTypes[abilT].CheckRequirements(AbilityDialog.player, AbilityDialog.addedAbils) && status == AbilityAddedStatus.unavailable)
                {
                    status = AbilityAddedStatus.available;
                }

                abilPanel.GetComponent<AvailAbilPanel>().InitializeUI(AbilityDialog, AbilityTypes.abilTypes[abilT].stdName, AbilityTypes.abilTypes[abilT].id, status);
            }
        }
        else if (status == AbilityAddedStatus.selected)
        {
            status = AbilityAddedStatus.available;
            AbilityDialog.addedAbils.Remove(abilType);
            AbilityDialog.curUnspentTP++;
            AbilityDialog.SetUnspentTPTxt();
            abilName.color = new Color32(150, 150, 150, 255);

            bool wasRemoved;
            do
            {
                wasRemoved = false;
                foreach (GameObject abilPanel in AbilityDialog.availAbilPanels)
                {
                    AbilityAddedStatus status = abilPanel.GetComponent<AvailAbilPanel>().status;
                    AbilityTypeEnum abilT = abilPanel.GetComponent<AvailAbilPanel>().abilType;
                    if (status != AbilityAddedStatus.unavailable && !AbilityTypes.abilTypes[abilT].CheckRequirements(AbilityDialog.player, AbilityDialog.addedAbils))
                    {
                        status = AbilityAddedStatus.unavailable;
                        wasRemoved = true;

                        if (AbilityDialog.addedAbils.Contains(abilT))
                        {
                            AbilityDialog.addedAbils.Remove(abilT);
                            AbilityDialog.curUnspentTP++;
                            AbilityDialog.SetUnspentTPTxt();
                            RemoveAbility(abilT);
                        }
                    }

                    abilPanel.GetComponent<AvailAbilPanel>().InitializeUI(AbilityDialog, AbilityTypes.abilTypes[abilT].stdName, AbilityTypes.abilTypes[abilT].id, status);
                }
            } while (wasRemoved);

            RemoveAbility(abilType);
        }
    }


    void RemoveAbility(AbilityTypeEnum abilT)
    {
        for (int i = 0; i < AbilityDialog.player.curAbils.Count; i++)
        {
            if (AbilityDialog.player.curAbils[i] == abilT)
            {
                AbilityDialog.player.curAbils[i] = AbilityTypeEnum.abilNone;
                AbilityDialog.curAbilsPanels[i].ActivateText(false);
            }
        }

        if (AbilityDialog.player.meleeAbil == abilT)
        {
            AbilityDialog.meleeAbilPanel.abilType = AbilityTypeEnum.abilNone;
            AbilityDialog.meleeAbilPanel.ActivateText(false);
        }

        if (AbilityDialog.player.rangedAbil == abilT)
        {
            AbilityDialog.rangedAbilPanel.abilType = AbilityTypeEnum.abilNone;
            AbilityDialog.rangedAbilPanel.ActivateText(false);
        }

        if (AbilityDialog.player.dodgeAbil == abilT)
        {
            AbilityDialog.dodgeAbilPanel.abilType = AbilityTypeEnum.abilNone;
            AbilityDialog.dodgeAbilPanel.ActivateText(false);
        }

        if (AbilityDialog.player.blockAbil == abilT)
        {
            AbilityDialog.blockAbilPanel.abilType = AbilityTypeEnum.abilNone;
            AbilityDialog.blockAbilPanel.ActivateText(false);
        }
    }
}
