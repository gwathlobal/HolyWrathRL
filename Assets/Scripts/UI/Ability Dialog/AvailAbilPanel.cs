using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AbilityAddedStatus
{
    selected, available, unavailable
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
    public  bool added;
    public AbilityAddedStatus status;

    public void InitializeUI(AbilityDialogScript _abilityDialog, string name, AbilityTypeEnum _abilType, bool _added, AbilityAddedStatus _status)
    {
        AbilityDialog = _abilityDialog;
        abilName.text = name;
        abilType = _abilType;
        status = _status;
        added = _added;
        switch (status)
        {
            case AbilityAddedStatus.selected:
                button.gameObject.SetActive(false);
                added = true;
                break;
            case AbilityAddedStatus.unavailable:
                added = false;
                button.gameObject.SetActive(false);
                break;
            default:
                button.gameObject.SetActive(true);
                break;
        }

        if (AbilityDialog.curUnspentTP == 0 && added == false && status == AbilityAddedStatus.available) button.gameObject.SetActive(false);

        if (added)
        {
            buttonTxt.text = "-";
            abilName.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            buttonTxt.text = "+";
            abilName.color = new Color32(150, 150, 150, 255);
        }
        

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (status == AbilityAddedStatus.unavailable || (status == AbilityAddedStatus.available && added == false) || 
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
        AbilityDialog.sprintAbilPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
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
        added = !added;
        if (added)
        {
            AbilityDialog.addedAbils.Add(abilType);
            AbilityDialog.curUnspentTP--;
            buttonTxt.text = "-";
            abilName.color = new Color32(255, 255, 255, 255);
            AbilityDialog.SetUnspentTPTxt();
            foreach (GameObject abilPanel in AbilityDialog.availAbilPanels)
            {
                AbilityAddedStatus status = abilPanel.GetComponent<AvailAbilPanel>().status;
                bool added = abilPanel.GetComponent<AvailAbilPanel>().added;
                AbilityTypeEnum abilT = abilPanel.GetComponent<AvailAbilPanel>().abilType;
                if (AbilityTypes.abilTypes[abilT].CheckRequirements(AbilityDialog.player, AbilityDialog.addedAbils))
                {
                    status = AbilityAddedStatus.available;
                }
                
                abilPanel.GetComponent<AvailAbilPanel>().InitializeUI(AbilityDialog, AbilityTypes.abilTypes[abilT].stdName, AbilityTypes.abilTypes[abilT].id, added, status);
            }
            
        }
        else
        {
            AbilityDialog.addedAbils.Remove(abilType);
            AbilityDialog.curUnspentTP++;
            buttonTxt.text = "+";
            AbilityDialog.SetUnspentTPTxt();
            abilName.color = new Color32(150, 150, 150, 255);

            bool wasRemoved;
            do
            {
                wasRemoved = false;
                foreach (GameObject abilPanel in AbilityDialog.availAbilPanels)
                {
                    AbilityAddedStatus status = abilPanel.GetComponent<AvailAbilPanel>().status;
                    bool added = abilPanel.GetComponent<AvailAbilPanel>().added;
                    AbilityTypeEnum abilT = abilPanel.GetComponent<AvailAbilPanel>().abilType;
                    if (status != AbilityAddedStatus.unavailable && !AbilityTypes.abilTypes[abilT].CheckRequirements(AbilityDialog.player, AbilityDialog.addedAbils))
                    {
                        status = AbilityAddedStatus.unavailable;
                        added = false;
                        wasRemoved = true;

                        if (AbilityDialog.addedAbils.Contains(abilT))
                        {
                            AbilityDialog.addedAbils.Remove(abilT);
                            AbilityDialog.curUnspentTP++;
                            AbilityDialog.SetUnspentTPTxt();
                            RemoveAbility(abilT);
                        }
                    }

                    abilPanel.GetComponent<AvailAbilPanel>().InitializeUI(AbilityDialog, AbilityTypes.abilTypes[abilT].stdName, AbilityTypes.abilTypes[abilT].id, added, status);
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

        if (AbilityDialog.player.sprintAbil == abilT)
        {
            AbilityDialog.sprintAbilPanel.abilType = AbilityTypeEnum.abilNone;
            AbilityDialog.sprintAbilPanel.ActivateText(false);
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
