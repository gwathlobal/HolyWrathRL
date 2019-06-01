using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AbilityAddedStatus
{
    selected, available, added, unavailable
}

public class AvailAbilPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public AbilityDialogScript AbilityDialog;
    public Text abilName;
    public AbilityTypeEnum abilType;
    public Button button;
    public Text buttonTxt;

    private Vector3 startPosition;
    private Transform startParent;
    public AbilityAddedStatus status;
    private Color32 colorName; 

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
                colorName = new Color32(255, 255, 255, 255);
                abilName.color = colorName;
                buttonTxt.text = "-";
                break;
            case AbilityAddedStatus.available:
                button.gameObject.SetActive(true);
                colorName = new Color32(150, 150, 150, 255);
                abilName.color = colorName;
                buttonTxt.text = "+";
                break;
            case AbilityAddedStatus.added:
                button.gameObject.SetActive(false);
                colorName = new Color32(255, 255, 255, 255);
                abilName.color = colorName;
                break;
            default:
                button.gameObject.SetActive(false);
                colorName = new Color32(150, 150, 150, 255);
                abilName.color = colorName;
                break;
        }

        if (AbilityDialog.curUnspentTP == 0 && status == AbilityAddedStatus.available) button.gameObject.SetActive(false);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (status == AbilityAddedStatus.unavailable || status == AbilityAddedStatus.available || 
            AbilityTypes.abilTypes[abilType].slot == AbilitySlotEnum.abilNone)
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
            abilName.color = new Color32(255, 255, 0, 255);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityDialog.DescrText.text = AbilityDialog.defaultHint;
        abilName.color = colorName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            MoveToSelected();
        }
    }

    private void MoveToSelected()
    {
        if ((status == AbilityAddedStatus.selected || status == AbilityAddedStatus.added) &&
            AbilityTypes.abilTypes[abilType].slot != AbilitySlotEnum.abilNone)
        {
            switch (AbilityTypes.abilTypes[abilType].slot)
            {
                case AbilitySlotEnum.abilBlock:
                    AbilityDialog.player.blockAbil = abilType;
                    break;
                case AbilitySlotEnum.abilDodge:
                    AbilityDialog.player.dodgeAbil = abilType;
                    break;
                case AbilitySlotEnum.abilMelee:
                    AbilityDialog.player.meleeAbil = abilType;
                    break;
                case AbilitySlotEnum.abilRanged:
                    AbilityDialog.player.rangedAbil = abilType;
                    break;
                case AbilitySlotEnum.abilNormal:
                    for (int i = 0; i < AbilityDialog.curAbilsPanels.Count; i++)
                    {
                        if (AbilityDialog.curAbilsPanels[i].abilType == AbilityTypeEnum.abilNone)
                        {
                            AbilityDialog.player.curAbils[i] = abilType;
                            break;
                        }
                    }
                    break;
            }
            AbilityDialog.InitializeUI(AbilityDialog.player);
        }
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

            MoveToSelected();
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
        AbilityDialog.InitializeUI(AbilityDialog.player);
    }


    void RemoveAbility(AbilityTypeEnum abilT)
    {
        for (int i = 0; i < AbilityDialog.player.curAbils.Count; i++)
        {
            if (AbilityDialog.player.curAbils[i] == abilT)
            {
                AbilityDialog.player.curAbils[i] = AbilityTypeEnum.abilNone;
                AbilityDialog.curAbilsPanels[i].abilType = AbilityTypeEnum.abilNone;
                //AbilityDialog.curAbilsPanels[i].ActivateText(false);
            }
        }

        if (AbilityDialog.player.meleeAbil == abilT)
        {
            AbilityDialog.player.meleeAbil = MobTypes.mobTypes[AbilityDialog.player.idType].meleeAbil;
            AbilityDialog.meleeAbilPanel.abilType = AbilityDialog.player.meleeAbil;
            //AbilityDialog.meleeAbilPanel.ActivateText(false);
        }

        if (AbilityDialog.player.rangedAbil == abilT)
        {
            AbilityDialog.player.rangedAbil = MobTypes.mobTypes[AbilityDialog.player.idType].rangedAbil;
            AbilityDialog.rangedAbilPanel.abilType = AbilityDialog.player.rangedAbil;
            //AbilityDialog.rangedAbilPanel.ActivateText(false);
        }

        if (AbilityDialog.player.dodgeAbil == abilT)
        {
            AbilityDialog.player.dodgeAbil = MobTypes.mobTypes[AbilityDialog.player.idType].dodgeAbil;
            AbilityDialog.dodgeAbilPanel.abilType = AbilityDialog.player.dodgeAbil;
            //AbilityDialog.dodgeAbilPanel.ActivateText(false);
        }

        if (AbilityDialog.player.blockAbil == abilT)
        {
            AbilityDialog.player.blockAbil = MobTypes.mobTypes[AbilityDialog.player.idType].blockAbil;
            AbilityDialog.blockAbilPanel.abilType = AbilityDialog.player.blockAbil;
            //AbilityDialog.blockAbilPanel.ActivateText(false);
        }
    }
}
