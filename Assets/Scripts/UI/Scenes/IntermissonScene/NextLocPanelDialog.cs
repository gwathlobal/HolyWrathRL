using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void NextLocFunc();

public class NextLocPanelDialog : MonoBehaviour, IPointerClickHandler
{

    public Text panelTxt;
    public string titleTxt;
    public string descrTxt;
    public NextLocFunc NextLocFunc;

    public NextLocDialogScript NextLocDialog;

    public void InitializeUI(NextLocDialogScript dialog, string titleStr, string descrStr, NextLocFunc func)
    {
        NextLocDialog = dialog;
        panelTxt.text = titleStr;
        titleTxt = titleStr;
        descrTxt = descrStr;
        NextLocFunc = func;
    }

    public void ActivateText(bool active)
    {
        panelTxt.gameObject.SetActive(active);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnClick()
    {
        NextLocDialog.locationTxt.text = titleTxt;
        NextLocDialog.descrTxt.text = descrTxt;
        NextLocDialog.intermissionScript.func = NextLocFunc;
    }
}
