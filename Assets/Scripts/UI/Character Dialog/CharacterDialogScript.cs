using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogScript : MonoBehaviour {

    public PlayerMob player;

    public Text DescrText;

    public List<CharAbilPanel> charAbilsPanels;
    public List<Text> charAbilShortcuts;
    public CharAbilPanel sprintAbilPanel;
    public CharAbilPanel dodgeAbilPanel;
    public CharAbilPanel blockAbilPanel;
    public CharAbilPanel meleeAbilPanel;
    public CharAbilPanel rangedAbilPanel;
    public Text sprintAbilShortcut;
    public Text dodgeAbilShortcut;
    public Text blockAbilShortcut;
    public Text rangedAbilShortcut;

    public Image PassiveScrollPanel;
    public GameObject passivePanelPrefab;
    private List<GameObject> passivePanels;

    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);
        passivePanels = new List<GameObject>();
    }

    public void InitializeUI(PlayerMob _player)
    {
        player = _player;

        int i;

        for (i = 0; i < charAbilsPanels.Count; i++)
        {

            charAbilsPanels[i].ActivateText(false);
            charAbilsPanels[i].curAbil = i;
            charAbilsPanels[i].abilType = AbilityTypeEnum.abilNone;
            charAbilShortcuts[i].gameObject.SetActive(false);
        }

        for (i = 0; i < player.curAbils.Count; i++)
        {
            charAbilsPanels[i].abilType = player.curAbils[i];
            if (player.curAbils[i] != AbilityTypeEnum.abilNone)
            {
                charAbilsPanels[i].ActivateText(true);
                charAbilsPanels[i].InitializeUI(AbilityTypes.abilTypes[player.curAbils[i]].stdName);
                charAbilShortcuts[i].gameObject.SetActive(true);
            }
            else
            {
                charAbilsPanels[i].ActivateText(false);
                charAbilShortcuts[i].gameObject.SetActive(false);
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

        foreach (GameObject go in passivePanels)
            Destroy(go);
        passivePanels.Clear();

        i = 0;
        foreach (AbilityTypeEnum abilType in player.abilities.Keys)
        {
            if (AbilityTypes.abilTypes[abilType].passive && AbilityTypes.abilTypes[abilType].slot == AbilitySlotCategoty.abilNone)
            {
                GameObject ep = Instantiate(passivePanelPrefab);
                ep.transform.SetParent(PassiveScrollPanel.transform, false);
                ep.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0 + i * -30);

                PassiveScrollPanel.rectTransform.sizeDelta = new Vector2(290, i * 30);

                ep.GetComponent<CharAbilPanel>().InitializeUI(AbilityTypes.abilTypes[abilType].stdName);
                ep.GetComponent<CharAbilPanel>().ActivateText(true);
                ep.GetComponent<CharAbilPanel>().CharacterDialog = this;
                ep.GetComponent<CharAbilPanel>().abilType = abilType;

                passivePanels.Add(ep);
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
