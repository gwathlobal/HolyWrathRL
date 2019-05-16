using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharAbilPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Text txt;
    public int curAbil;
    public AbilityTypeEnum abilType;
    public AbilitySlotEnum abilSlot;
    public CharacterDialogScript CharacterDialog;

    public void InitializeUI(string str)
    {
        txt.text = str;
    }

    public void ActivateText(bool active)
    {
        txt.gameObject.SetActive(active);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (abilType != AbilityTypeEnum.abilNone)
        {
            CharacterDialog.DescrText.text = AbilityTypes.abilTypes[abilType].GetFullDescription(CharacterDialog.player);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CharacterDialog.DescrText.text = "";
    }
}
