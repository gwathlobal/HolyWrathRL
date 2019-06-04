using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NemesisPanelScript : MonoBehaviour, IPointerClickHandler
{
    public Text panelTxt;
    private string titleTxt;
    private string descrTxt;
    private JournalDialogScript JournalDialog;

    public void InitializeUI(JournalDialogScript dialog, string titleStr, string descrStr)
    {
        JournalDialog = dialog;
        panelTxt.text = titleStr;
        titleTxt = titleStr;
        descrTxt = descrStr;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnClick()
    {
        JournalDialog.DescrText.text = descrTxt;
    }
}
