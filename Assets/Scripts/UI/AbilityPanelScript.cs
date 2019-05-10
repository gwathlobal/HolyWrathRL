using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AbilityPanelScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public int curAbil;
    public AbilitySlotCategoty abilSlot;
    public AbilityTypeEnum abilType;

    public Text abilText;
    public Text shortcutText;
    public bool blocked;

    private void Start()
    {
        GetComponent<HintPanelScript>().SetPanelName("Hint Panel Abil " + abilSlot.ToString());
        GetComponent<HintPanelScript>().hintStr = AbilityTypes.abilTypes[abilType].GetFullDescription(BoardManager.instance.player);
        if (abilType == AbilityTypeEnum.abilNone)
        {
            GetComponent<HintPanelScript>().isBlocked = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (abilType != AbilityTypeEnum.abilNone)
        {
            GetComponent<HintPanelScript>().hintStr = AbilityTypes.abilTypes[abilType].GetFullDescription(BoardManager.instance.player);

            if (!blocked)
            {
                abilText.color = new Color32(255, 222, 0, 255);
                shortcutText.color = new Color32(255, 222, 0, 255);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!BoardManager.instance.playersTurn) return;
        if (BoardManager.instance.player.GetAbility(abilType).passive) return;


        bool turnEnded = UIManager.instance.InvokeAbility(BoardManager.instance.player.GetAbility(abilType));
        if (turnEnded)
        {
            BoardManager.instance.FinalizePlayerTurn();
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!blocked)
        {
            abilText.color = new Color32(255, 255, 255, 255);
            shortcutText.color = new Color32(255, 255, 255, 255);
        }
    }

}
