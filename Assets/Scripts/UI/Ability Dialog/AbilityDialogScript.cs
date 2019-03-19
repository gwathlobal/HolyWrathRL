﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum AbilityDialogStatusEnum
{
    nemesis, ability
}

public class AbilityDialogScript : MonoBehaviour {

    public PlayerMob player;

    public IntermissionScript intermissionScript;
    public Image AbilityScrollPanel;
    public GameObject AbiltyPanelPrefab;
    public Text DescrText;

    public List<CurAbilPanel> curAbilsPanels;
    public List<GameObject> curAbilShortcuts;
    public CurAbilPanel sprintAbilPanel;
    public CurAbilPanel dodgeAbilPanel;
    public CurAbilPanel blockAbilPanel;
    public CurAbilPanel meleeAbilPanel;
    public CurAbilPanel rangedAbilPanel;

    public GameObject objectDragged;
    public Transform canvasTrasform;

    public List<GameObject> availAbilPanels;
    public AbilityPlayerCategory curCategory;
    public Text UnspentTPTxt;

    public int curUnspentTP;
    public int maxUnspentTP;
    public List<AbilityTypeEnum> addedAbils;

    private void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);

        addedAbils = new List<AbilityTypeEnum>();
        curUnspentTP = 2;
        maxUnspentTP = 2;
        curCategory = AbilityPlayerCategory.abilCommon;
        availAbilPanels = new List<GameObject>();

        InitializeUI(GameManager.instance.player);
        SetUnspentTPTxt();
    }

    public void InitializeUI(PlayerMob _player)
    {
        player = _player;

        int i;

        SetUpScrollablePanels(curCategory);

        for (i = 0; i < curAbilsPanels.Count; i++)
        {

            curAbilsPanels[i].ActivateText(false);
            curAbilsPanels[i].curAbil = i;
            curAbilsPanels[i].abilType = AbilityTypeEnum.abilNone;
        }

        for (i = 0; i < player.curAbils.Count; i++)
        {
            curAbilsPanels[i].abilType = player.curAbils[i];
            if (player.curAbils[i] != AbilityTypeEnum.abilNone)
            {
                curAbilsPanels[i].ActivateText(true);
                curAbilsPanels[i].InitializeUI(AbilityTypes.abilTypes[player.curAbils[i]].stdName);
            }
            else
            {
                curAbilsPanels[i].ActivateText(false);
            }
        }

        if (player.meleeAbil != AbilityTypeEnum.abilNone)
        {
            meleeAbilPanel.abilType = player.meleeAbil;
            meleeAbilPanel.ActivateText(true);
            meleeAbilPanel.InitializeUI(AbilityTypes.abilTypes[player.meleeAbil].stdName);
        }
        else
        {
            meleeAbilPanel.ActivateText(false);
            meleeAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }

        if (player.rangedAbil != AbilityTypeEnum.abilNone)
        {
            rangedAbilPanel.abilType = player.rangedAbil;
            rangedAbilPanel.ActivateText(true);
            rangedAbilPanel.InitializeUI(AbilityTypes.abilTypes[player.rangedAbil].stdName);
        }
        else
        {
            rangedAbilPanel.ActivateText(false);
            rangedAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }

        if (player.sprintAbil != AbilityTypeEnum.abilNone)
        {
            sprintAbilPanel.abilType = player.sprintAbil;
            sprintAbilPanel.ActivateText(true);
            sprintAbilPanel.InitializeUI(AbilityTypes.abilTypes[player.sprintAbil].stdName);
        }
        else
        {
            sprintAbilPanel.ActivateText(false);
            sprintAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }

        if (player.dodgeAbil != AbilityTypeEnum.abilNone)
        {
            dodgeAbilPanel.abilType = player.dodgeAbil;
            dodgeAbilPanel.ActivateText(true);
            dodgeAbilPanel.InitializeUI(AbilityTypes.abilTypes[player.dodgeAbil].stdName);
        }
        else
        {
            dodgeAbilPanel.ActivateText(false);
            dodgeAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }

        if (player.blockAbil != AbilityTypeEnum.abilNone)
        {
            blockAbilPanel.abilType = player.blockAbil;
            blockAbilPanel.ActivateText(true);
            blockAbilPanel.InitializeUI(AbilityTypes.abilTypes[player.blockAbil].stdName);
        }
        else
        {
            blockAbilPanel.ActivateText(false);
            blockAbilPanel.abilType = AbilityTypeEnum.abilNone;
        }
    }

    public void GoToNextLevel()
    {
        foreach (AbilityTypeEnum ability in addedAbils)
        {
            if (!player.abilities.ContainsKey(ability)) player.abilities.Add(ability, true);
        }

        /*
        if (GameManager.instance.levelNum > 0)
        {
            MonsterLayoutEnum monsterLayout = (MonsterLayoutEnum)Random.Range(1, System.Enum.GetValues(typeof(MonsterLayoutEnum)).Length);
            LevelLayoutEnum levelLayout = (LevelLayoutEnum)Random.Range(1, System.Enum.GetValues(typeof(LevelLayoutEnum)).Length);
            ObjectiveLayoutEnum objectiveLayout = (ObjectiveLayoutEnum)Random.Range(1, System.Enum.GetValues(typeof(ObjectiveLayoutEnum)).Length);

            GameManager.instance.monsterLayout = monsterLayout;
            GameManager.instance.levelLayout = levelLayout;
            GameManager.instance.objectiveLayout = objectiveLayout;
        }
        */

        intermissionScript.func();

        SceneManager.LoadScene("LevelScene");
    }

    public void SetUpScrollablePanels(AbilityPlayerCategory abilCategory)
    {
        foreach (GameObject go in availAbilPanels)
        {
            Destroy(go);
        }
        availAbilPanels.Clear();

        int i = 0;
        foreach (AbilityTypeEnum abil in AbilityTypes.abilTypes.Keys)
        {
            if (AbilityTypes.abilTypes[abil].id != AbilityTypeEnum.abilNone && AbilityTypes.abilTypes[abil].category == abilCategory)
            {
                GameObject abilPanel = GameObject.Instantiate(AbiltyPanelPrefab);
                abilPanel.transform.SetParent(AbilityScrollPanel.transform, false);
                abilPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1, 0 + i * -30);

                AbilityAddedStatus status;
                bool added = false;
                if (player.abilities.ContainsKey(abil))
                {
                    status = AbilityAddedStatus.selected;
                    added = true;
                }
                else if (AbilityTypes.abilTypes[abil].CheckRequirements(player, addedAbils)) {
                    status = AbilityAddedStatus.available;
                }
                else status = AbilityAddedStatus.unavailable;
                if (addedAbils.Contains(abil))
                {
                    added = true;
                }

                abilPanel.GetComponent<AvailAbilPanel>().InitializeUI(this, AbilityTypes.abilTypes[abil].stdName, AbilityTypes.abilTypes[abil].id, added, status);
                i++;
                availAbilPanels.Add(abilPanel);
            }
        }

        AbilityScrollPanel.rectTransform.sizeDelta = new Vector2(158, i * 30);
    }

    public void SetUnspentTPTxt()
    {
        UnspentTPTxt.text = "Unspent Talent Points: " + curUnspentTP;
    }
}
