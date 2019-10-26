using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NemesisPanelScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text panelTxt;
    private string titleTxt;
    private string descrTxt;
    private JournalDialogScript JournalDialog;
    public Color32 baseColor;
    public Nemesis nemesis;

    public void InitializeUI(JournalDialogScript dialog, Nemesis _nemsesis, string titleStr, string descrStr, Color32 color)
    {
        JournalDialog = dialog;
        panelTxt.text = titleStr;
        titleTxt = titleStr;
        descrTxt = descrStr;
        baseColor = color;
        panelTxt.color = baseColor;
        nemesis = _nemsesis;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnClick()
    {
        JournalDialog.DescrText.text = descrTxt;
        JournalDialog.selectedPanel = this;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        panelTxt.color = new Color32(255, 255, 0, 255);
        OnClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        panelTxt.color = baseColor;
    }
}
