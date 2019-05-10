using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HintPanelScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  {

    public string hintStr;
    public bool isBlocked;

    private GameObject panel;
    private Text txt;
    private string panelName;

    // Use this for initialization
    void Start () {
        panel = new GameObject((panelName != "") ? panelName : "Panel Hint");
        panel.transform.SetParent(this.transform);
        panel.layer = 5;

        RectTransform panelRectTransform = panel.AddComponent<RectTransform>();
        panelRectTransform.anchoredPosition = new Vector2(0, -85);
        panelRectTransform.sizeDelta = new Vector2(230, 140);

        panel.transform.SetParent(UIManager.instance.canvasTrasform, true);
        
        if (panel.transform.position.y - panelRectTransform.sizeDelta.y <= 0)
        {
            panel.transform.position = new Vector2(panel.transform.position.x, panel.transform.position.y + panelRectTransform.sizeDelta.y + 30);
        }

        if (panel.transform.position.x + panelRectTransform.sizeDelta.x >= Screen.width)
        {
            panel.transform.position = new Vector2(Screen.width - panelRectTransform.sizeDelta.x / 2 - 10, panel.transform.position.y);
        }
        
        Image imagePanel = panel.AddComponent<Image>();
        imagePanel.color = new Color(0.1f, 0.1f, 0.1f);

        GameObject textGO = new GameObject("Panel Hint Txt");
        textGO.transform.SetParent(panel.transform);
        textGO.layer = 5;

        RectTransform txtRectTransform = textGO.AddComponent<RectTransform>();
        txtRectTransform.anchoredPosition = new Vector2(0, 0);
        txtRectTransform.sizeDelta = new Vector2(220, 130);

        Font font = Font.CreateDynamicFontFromOSFont("Arial", 14);

        txt = textGO.AddComponent<Text>();
        txt.font = font;
        txt.fontStyle = FontStyle.Normal;
        txt.supportRichText = false;
        txt.fontSize = font.fontSize;
        txt.color = new Color(255, 255, 255);
        txt.alignment = TextAnchor.UpperLeft;

        panel.SetActive(false);

        isBlocked = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isBlocked) return;
        txt.text = hintStr;
        panel.SetActive(true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(txt);
        Destroy(panel);
    }

    public void SetPanelName(string str)
    {
        if (panel != null) panel.name = str;
        panelName = str;
    }
}
