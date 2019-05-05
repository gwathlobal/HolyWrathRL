using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanelScript : MonoBehaviour {

    public PlayerMob player;
    public List<GameObject> abilNameTxts;
    public List<AbilityPanelScript> abilPanels;
    public List<GameObject> abilShortcutTxts;

    public GameObject meleeAbilTxt;
    public AbilityPanelScript meleeAbilPanel;

    public GameObject rangedAbilTxt;
    public AbilityPanelScript rangedAbilPanel;
    public GameObject rangedAbilShortcut;

    public GameObject dodgeAbilTxt;
    public AbilityPanelScript dodgeAbilPanel;
    public GameObject dodgeAbilShortcut;

    public GameObject blockAbilTxt;
    public AbilityPanelScript blockAbilPanel;
    public GameObject blockAbilShortcut;

    public void RecreateInterface()
    {
        Text text;

        for (int i = 0; i < abilPanels.Count; i++)
        {
            abilPanels[i].gameObject.SetActive(false);
            abilShortcutTxts[i].SetActive(false);
            abilPanels[i].abilType = AbilityTypeEnum.abilNone;
        }

        for (int i = 0; i < player.curAbils.Count; i++)
        {

            if (AbilityTypes.abilTypes[player.curAbils[i]].id != AbilityTypeEnum.abilNone)
            {
                abilPanels[i].gameObject.SetActive(true);
                abilShortcutTxts[i].SetActive(true);
                abilPanels[i].abilType = player.curAbils[i];
                text = abilNameTxts[i].GetComponent<Text>();

                text.text = AbilityTypes.abilTypes[player.curAbils[i]].Name(player);
            }
        }

        if (player.meleeAbil != AbilityTypeEnum.abilNone)
        {
            meleeAbilPanel.gameObject.SetActive(true);
            meleeAbilPanel.abilType = player.meleeAbil;

            text = meleeAbilTxt.GetComponent<Text>();
            text.text = AbilityTypes.abilTypes[player.meleeAbil].Name(player);
        }
        else
        {
            meleeAbilPanel.gameObject.SetActive(false);
            meleeAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }

        if (player.rangedAbil != AbilityTypeEnum.abilNone)
        {
            rangedAbilPanel.gameObject.SetActive(true);
            rangedAbilShortcut.SetActive(true);
            rangedAbilPanel.abilType = player.rangedAbil;

            text = rangedAbilTxt.GetComponent<Text>();
            text.text = AbilityTypes.abilTypes[player.rangedAbil].Name(player);
        }
        else
        {
            rangedAbilPanel.gameObject.SetActive(false);
            rangedAbilShortcut.SetActive(false);
            rangedAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }

        if (player.dodgeAbil != AbilityTypeEnum.abilNone)
        {
            dodgeAbilPanel.gameObject.SetActive(true);
            dodgeAbilShortcut.SetActive(true);
            dodgeAbilPanel.abilType = player.dodgeAbil;

            text = dodgeAbilTxt.GetComponent<Text>();
            text.text = AbilityTypes.abilTypes[player.dodgeAbil].Name(player);
        }
        else
        {
            dodgeAbilPanel.gameObject.SetActive(false);
            dodgeAbilShortcut.SetActive(false);
            dodgeAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }

        if (player.blockAbil != AbilityTypeEnum.abilNone)
        {
            blockAbilPanel.gameObject.SetActive(true);
            blockAbilShortcut.SetActive(true);
            blockAbilPanel.abilType = player.blockAbil;

            text = blockAbilTxt.GetComponent<Text>();
            text.text = AbilityTypes.abilTypes[player.blockAbil].Name(player);
        }
        else
        {
            blockAbilPanel.gameObject.SetActive(false);
            blockAbilShortcut.SetActive(false);
            blockAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }
    }

    public void UpdateLeftPanel()
    {
        PlayerMob player = BoardManager.instance.player;

        if (player == null) return;

        for (int i = 0; i < player.curAbils.Count; i++)
        {
            if (AbilityTypes.abilTypes[player.curAbils[i]].id != AbilityTypeEnum.abilNone)
            {
                Ability ability = player.GetAbility(player.curAbils[i]);

                if (UIManager.instance.CheckApplicAbility(ability))
                {
                    abilNameTxts[i].GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                    abilShortcutTxts[i].GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                    abilPanels[i].blocked = false;
                }
                else
                {
                    abilNameTxts[i].GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                    abilShortcutTxts[i].GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                    abilPanels[i].blocked = true;
                }
                abilNameTxts[i].GetComponent<Text>().text = ability.Name(player);
            }
        }

        if (player.meleeAbil != AbilityTypeEnum.abilNone)
        {
            Ability ability = player.GetAbility(player.meleeAbil);

            if (UIManager.instance.CheckApplicAbility(ability))
            {
                meleeAbilTxt.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                meleeAbilPanel.blocked = false;
            }
            else
            {
                meleeAbilTxt.GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                meleeAbilPanel.blocked = true;
            }
            meleeAbilTxt.GetComponent<Text>().text = ability.Name(player);
        }

        if (player.rangedAbil != AbilityTypeEnum.abilNone)
        {
            Ability ability = player.GetAbility(player.rangedAbil);

            if (UIManager.instance.CheckApplicAbility(ability))
            {
                rangedAbilTxt.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                rangedAbilShortcut.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                rangedAbilPanel.blocked = false;
            }
            else
            {
                rangedAbilTxt.GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                rangedAbilShortcut.GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                rangedAbilPanel.blocked = true;
            }
            rangedAbilTxt.GetComponent<Text>().text = ability.Name(player);
        }

        if (player.dodgeAbil != AbilityTypeEnum.abilNone)
        {
            Ability ability = player.GetAbility(player.dodgeAbil);

            if (UIManager.instance.CheckApplicAbility(ability))
            {
                dodgeAbilTxt.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                dodgeAbilShortcut.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                dodgeAbilPanel.blocked = false;
            }
            else
            {
                dodgeAbilTxt.GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                dodgeAbilShortcut.GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                dodgeAbilPanel.blocked = true;
            }
            dodgeAbilTxt.GetComponent<Text>().text = ability.Name(player);
        }

        if (player.blockAbil != AbilityTypeEnum.abilNone)
        {
            Ability ability = player.GetAbility(player.blockAbil);

            if (UIManager.instance.CheckApplicAbility(ability))
            {
                blockAbilTxt.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                blockAbilShortcut.GetComponent<Text>().color = new Color32(255, 255, 255, 255);
                blockAbilPanel.blocked = false;
            }
            else
            {
                blockAbilTxt.GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                blockAbilShortcut.GetComponent<Text>().color = new Color32(100, 100, 100, 255);
                blockAbilPanel.blocked = true;
            }
            blockAbilTxt.GetComponent<Text>().text = ability.Name(player);
        }
    }
}
