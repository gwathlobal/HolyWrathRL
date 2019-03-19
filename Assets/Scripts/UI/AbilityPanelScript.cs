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

    private bool pointerInside;
    private GameObject panel;
    private Text txt;

    private void Start()
    {
        panel = new GameObject("Abil Hover Panel " + curAbil);
        panel.transform.SetParent(this.transform);
        panel.layer = 5;

        RectTransform panelRectTransform = panel.AddComponent<RectTransform>();
        panelRectTransform.anchoredPosition = new Vector2(80, -85);
        panelRectTransform.sizeDelta = new Vector2(230, 140);

        panel.transform.SetParent(UIManager.instance.canvasTrasform, true);

        Image imagePanel = panel.AddComponent<Image>();
        imagePanel.color = new Color(0.1f, 0.1f, 0.1f);

        GameObject textGO  = new GameObject("Abil Hover Txt " + curAbil);
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

        pointerInside = false;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UIManager.instance.screenStatus != MainScreenStatus.statusNormal) return;

        if (abilType != AbilityTypeEnum.abilNone)
        {
            txt.text = AbilityTypes.abilTypes[abilType].GetFullDescription(BoardManager.instance.player);
                        
            panel.SetActive(true);

            pointerInside = true;
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
        panel.SetActive(false);
        pointerInside = false;
    }

    void Update()
    {
        PlayerMob player = BoardManager.instance.player;

        if (player == null) return;

        Ability ability = player.GetAbility(abilType);

        if (UIManager.instance.CheckApplicAbility(ability))
        {
            if (pointerInside)
            {
                abilText.color = new Color32(255, 222, 0, 255);
                shortcutText.color = new Color32(255, 222, 0, 255);
            }
            else
            {
                abilText.color = new Color32(255, 255, 255, 255);
                shortcutText.color = new Color32(255, 255, 255, 255);
            }
        }
        else
        {
            abilText.color = new Color32(100, 100, 100, 255);
            shortcutText.color = new Color32(100, 100, 100, 255);
        }
    }
}
