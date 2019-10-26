using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostMissionDialog : MonoBehaviour
{
    public Text descrText;
    public Text postMissionNextBtnText;
    public Text namesNextBtnText;
    public Text abilitiesNextBtnTxt;
    public enum dialogStatus { postMisssion, namesRevealed, abilitiesRevealed, locationsRevealed };

    public Image postMissionPanel;
    public Image namesPanel;
    public Image abilitiesPanel;
    public Image locationsPanel;

    public JournalDialogScript NamesJournalDialog;
    public JournalDialogScript AbilitiesJournalDialog;
    public JournalDialogScript LocationsJournalDialog;
    
    public Text abilPointsLeftTxt;
    public Button revealSelectedBtn;
    public Button revealRandomBtn;

    private bool availableNames;
    private bool availableAbilities;
    private bool availableLocations;

    void Start()
    {
        namesPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        abilitiesPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        locationsPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        HideNamesPanel();
        HideAbilitiesPanel();
        HideLocationsPanel();

        availableNames = false;
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.personalStatus == Nemesis.PersonalStatusEnum.hidden && nemesis.deathStatus == Nemesis.DeathStatusEnum.alive)
            {
                availableNames = true;
            }
        }

        availableAbilities = false;
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden && nemesis.personalStatus != Nemesis.PersonalStatusEnum.revealedAbils && 
                nemesis.deathStatus == Nemesis.DeathStatusEnum.alive)
            {
                availableAbilities = true;
            }
        }

        availableLocations = false;
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if(nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden && !nemesis.revealedLocation &&
                nemesis.deathStatus == Nemesis.DeathStatusEnum.alive)
            {
                availableLocations = true;
            }
        }

        if (GameManager.instance.learntNames > 0 && availableNames)
            postMissionNextBtnText.text = "Reveal names";
        else if (GameManager.instance.learntAbilities > 0 && availableAbilities)
            postMissionNextBtnText.text = "Reveal abilities";
        else if (GameManager.instance.learntLocations > 0 && availableLocations)
            postMissionNextBtnText.text = "Reveal locations";
        else
            postMissionNextBtnText.text = "OK";

        if (GameManager.instance.learntAbilities > 0 && availableAbilities)
            namesNextBtnText.text = "Reveal abilities";
        else if (GameManager.instance.learntLocations > 0 && availableLocations)
            namesNextBtnText.text = "Reveal locations";
        else
            namesNextBtnText.text = "OK";

        if (GameManager.instance.learntLocations > 0 && availableLocations)
            abilitiesNextBtnTxt.text = "Reveal locations";
        else
            abilitiesNextBtnTxt.text = "OK";

        string str = "";

        str += "You gained 1 ability point.\n";

        foreach (Nemesis nemesis in GameManager.instance.nemesesPresent)
        {
            if (nemesis.mob.CheckDead() && nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden)
            {
                str += String.Format("{0} the {1} has died!\n", nemesis.mob.name, MobTypes.mobTypes[nemesis.mob.idType].name);
            }
            if (!nemesis.mob.CheckDead() && nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden)
            {
                str += String.Format("{0} the {1} has survived!\n", nemesis.mob.name, MobTypes.mobTypes[nemesis.mob.idType].name);
            }          
        }

        str += "\n";

        if (GameManager.instance.learntNames > 0)
            str += String.Format("You have learnt {0} {1} of prominent characters!\n", GameManager.instance.learntNames, 
                (GameManager.instance.learntNames == 1) ? "name" : "names");

        if (GameManager.instance.learntAbilities > 0)
            str += String.Format("You have learnt {0} {1} about abilities of prominent characters!\n", GameManager.instance.learntAbilities, 
                (GameManager.instance.learntAbilities == 1) ? "piece of knowledge" : "pieces of knowledge");

        if (GameManager.instance.learntLocations > 0)
            str += String.Format("You have learnt {0} {1} of prominent characters!\n", GameManager.instance.learntLocations,
                (GameManager.instance.learntLocations == 1) ? "location" : "locations");

        str += "\n";

        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            string str1 = "";
            if (!nemesis.mob.CheckDead() && nemesis.activity != NemesisActivityTypes.ActivityEnum.none)
            {
                str1 = String.Format("{0}\n", NemesisActivityTypes.nemesisActivities[nemesis.activity].ProcessActivity(nemesis, nemesis.activityTarget));
            }

            if (nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden)
            {
                str += str1;
            }
        }

        descrText.text = str;
    }

    public void ShowNamesPanel()
    {
        List<Nemesis> nemeses = new List<Nemesis>();
        for (int i = 0; i < GameManager.instance.learntNames; i++)
        {
            foreach (Nemesis nemesis in GameManager.instance.nemeses)
            {
                if (nemesis.personalStatus == Nemesis.PersonalStatusEnum.hidden && nemesis.deathStatus == Nemesis.DeathStatusEnum.alive)
                {
                    nemesis.personalStatus = Nemesis.PersonalStatusEnum.revealedName;
                    nemeses.Add(nemesis);
                    break;
                }
            }
        }

        availableAbilities = false;
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden && nemesis.personalStatus != Nemesis.PersonalStatusEnum.revealedAbils &&
                nemesis.deathStatus == Nemesis.DeathStatusEnum.alive)
            {
                availableAbilities = true;
            }
        }
        if (GameManager.instance.learntAbilities > 0 && availableAbilities)
            namesNextBtnText.text = "Reveal abilities";
        else if (GameManager.instance.learntLocations > 0 && availableLocations)
            namesNextBtnText.text = "Reveal locations";
        else
            namesNextBtnText.text = "OK";

        availableLocations = false;
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden && !nemesis.revealedLocation &&
                nemesis.deathStatus == Nemesis.DeathStatusEnum.alive)
            {
                availableLocations = true;
            }
        }
        if (GameManager.instance.learntLocations > 0 && availableLocations)
            abilitiesNextBtnTxt.text = "Reveal locations";
        else
            abilitiesNextBtnTxt.text = "OK";

        NamesJournalDialog.InitializeNemesis(nemeses);

        namesPanel.gameObject.SetActive(true);
    }

    public void HideNamesPanel()
    {
        namesPanel.gameObject.SetActive(false);
    }

    public void ShowAbilitiesPanel()
    {

        List<Nemesis> nemeses = new List<Nemesis>();
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.personalStatus != Nemesis.PersonalStatusEnum.revealedAbils && nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden && 
                nemesis.deathStatus == Nemesis.DeathStatusEnum.alive)
            {
                nemeses.Add(nemesis);
            }
        }

        AbilitiesJournalDialog.InitializeNemesis(nemeses);

        abilPointsLeftTxt.text = String.Format("You can learn about {0} more characters", GameManager.instance.learntAbilities);

        abilitiesPanel.gameObject.SetActive(true);
    }

    public void HideAbilitiesPanel()
    {
        abilitiesPanel.gameObject.SetActive(false);
    }

    public void RevealAbilitySelectedBtn()
    {

        Nemesis nemesis = AbilitiesJournalDialog.selectedPanel.nemesis;
        if (nemesis.personalStatus == Nemesis.PersonalStatusEnum.revealedAbils) return;

        nemesis.personalStatus = Nemesis.PersonalStatusEnum.revealedAbils;
        
        AbilitiesJournalDialog.selectedPanel.GetComponent<NemesisPanelScript>().InitializeUI(AbilitiesJournalDialog, 
            nemesis, 
            System.String.Format("{0}\n{1}", nemesis.mob.name, MobTypes.mobTypes[nemesis.mob.idType].name), nemesis.GetNemesisDescription(),
            new Color32(255, 255, 255, 255));
        AbilitiesJournalDialog.selectedPanel.OnClick();
        GameManager.instance.learntAbilities--;

        bool notRevealedLeft = true;
        foreach (GameObject nemesisPanel in AbilitiesJournalDialog.nemesisPanels)
        {
            if (nemesisPanel.GetComponent<NemesisPanelScript>().nemesis.personalStatus != Nemesis.PersonalStatusEnum.revealedAbils)
            {
                notRevealedLeft = false;
                break;
            }
        }
        if (GameManager.instance.learntAbilities <= 0) notRevealedLeft = true;
        if (notRevealedLeft)
        {
            revealSelectedBtn.gameObject.SetActive(false);
            revealRandomBtn.gameObject.SetActive(false);
        }

        abilPointsLeftTxt.text = String.Format("You can learn about {0} more characters", GameManager.instance.learntAbilities);
    }

    public void RevealAbilityRandomBtn()
    {
        GameManager.instance.learntAbilities--;

        List<NemesisPanelScript> availableNemeses = new List<NemesisPanelScript>();
        foreach (GameObject nemesisPanel in AbilitiesJournalDialog.nemesisPanels)
        {
            if (nemesisPanel.GetComponent<NemesisPanelScript>().nemesis.personalStatus != Nemesis.PersonalStatusEnum.revealedAbils)
            {
                availableNemeses.Add(nemesisPanel.GetComponent<NemesisPanelScript>());
            }
        }

        if (availableNemeses.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, availableNemeses.Count);

            availableNemeses[r].nemesis.personalStatus = Nemesis.PersonalStatusEnum.revealedAbils;
            Nemesis nemesis = availableNemeses[r].nemesis;
            nemesis.personalStatus = Nemesis.PersonalStatusEnum.revealedAbils;

            availableNemeses[r].GetComponent<NemesisPanelScript>().InitializeUI(AbilitiesJournalDialog,
                nemesis,
                System.String.Format("{0}\n{1}", nemesis.mob.name, MobTypes.mobTypes[nemesis.mob.idType].name), nemesis.GetNemesisDescription(),
                new Color32(255, 255, 255, 255));
            availableNemeses[r].OnClick();
        }

        bool notRevealedLeft = true;
        foreach (GameObject nemesisPanel in AbilitiesJournalDialog.nemesisPanels)
        {
            if (nemesisPanel.GetComponent<NemesisPanelScript>().nemesis.personalStatus != Nemesis.PersonalStatusEnum.revealedAbils)
            {
                notRevealedLeft = false;
                break;
            }
        }
        if (GameManager.instance.learntAbilities <= 0) notRevealedLeft = true;
        if (notRevealedLeft)
        {
            revealSelectedBtn.gameObject.SetActive(false);
            revealRandomBtn.gameObject.SetActive(false);
        }

        List<Nemesis> nemeses = new List<Nemesis>();
        foreach (GameObject nemesisPanel in AbilitiesJournalDialog.nemesisPanels)
        {
            nemeses.Add(nemesisPanel.GetComponent<NemesisPanelScript>().nemesis);
        }
        abilPointsLeftTxt.text = String.Format("You can learn about {0} more characters", GameManager.instance.learntAbilities);
    }

    public void ShowLocationsPanel()
    {
        List<Nemesis> nemeses = new List<Nemesis>();
        for (int i = 0; i < GameManager.instance.learntLocations; i++)
        {
            foreach (Nemesis nemesis in GameManager.instance.nemeses)
            {
                if (nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden && !nemesis.revealedLocation && nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null &&
                    nemesis.deathStatus == Nemesis.DeathStatusEnum.alive)
                {
                    nemesis.revealedLocation = true;
                    nemeses.Add(nemesis);
                    break;
                }
            }
        }

        LocationsJournalDialog.InitializeNemesis(nemeses);

        locationsPanel.gameObject.SetActive(true);
    }

    public void HideLocationsPanel()
    {
        locationsPanel.gameObject.SetActive(false);
    }

    public void PostMissionGoToButton()
    {
        if (GameManager.instance.learntNames > 0 && availableNames)
            ShowNamesPanel();
        else if (GameManager.instance.learntAbilities > 0 && availableAbilities)
            ShowAbilitiesPanel();
        else if (GameManager.instance.learntLocations > 0 && availableLocations)
            ShowLocationsPanel();
        else
            GoToIntermissionScene();
    }

    public void RevealNamesGoToButton()
    {
        if (GameManager.instance.learntAbilities > 0 && availableAbilities)
            ShowAbilitiesPanel();
        else if (GameManager.instance.learntLocations > 0 && availableLocations)
            ShowLocationsPanel();
        else
            GoToIntermissionScene();
    }

    public void RevealAbilitiesGoToButton()
    {
        if (GameManager.instance.learntLocations > 0 && availableLocations)
            ShowLocationsPanel();
        else
            GoToIntermissionScene();
    }

    public void RevealSelectedAbils()
    {
        if (AbilitiesJournalDialog.selectedPanel.nemesis.personalStatus != Nemesis.PersonalStatusEnum.revealedAbils)
            AbilitiesJournalDialog.selectedPanel.nemesis.personalStatus = Nemesis.PersonalStatusEnum.revealedAbils;

        PostRevealAbilities();
    }

    public void PostRevealAbilities()
    {
        GameManager.instance.learntAbilities--;
        abilPointsLeftTxt.text = String.Format("You can learn about {0} more characters", GameManager.instance.learntAbilities);

        if (GameManager.instance.learntAbilities == 0)
        {
            revealSelectedBtn.gameObject.SetActive(false);
            revealRandomBtn.gameObject.SetActive(false);
        }
        AbilitiesJournalDialog.selectedPanel.OnClick();
    }

    public void GoToIntermissionScene()
    {
        int aliveAngels = 0;
        int aliveDemons = 0;

        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (!nemesis.mob.CheckDead() && nemesis.activity == NemesisActivityTypes.ActivityEnum.none)
            {
                nemesis.AssignRandomActivity();
            }

            if (nemesis.deathStatus == Nemesis.DeathStatusEnum.alive && nemesis.mob.GetAbility(AbilityTypeEnum.abilAngel) != null)
                aliveAngels++;
            if (nemesis.deathStatus == Nemesis.DeathStatusEnum.alive && nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null)
                aliveDemons++;

            
        }

        if (aliveAngels < Nemesis.MAX_ANGEL_NEMESIS)
        {
            for (int i = 0; i < Nemesis.MAX_ANGEL_NEMESIS - aliveAngels; i++)
            {
                if (UnityEngine.Random.Range(0, 100) < 25)
                {
                    Nemesis nemesis = Nemesis.CreateAngelNemesis();

                    GameManager.instance.nemeses.Add(nemesis);
                }
            }
        }

        if (aliveDemons < Nemesis.MAX_DEMON_NEMESIS)
        {
            for (int i = 0; i < Nemesis.MAX_DEMON_NEMESIS - aliveAngels; i++)
            {
                if (UnityEngine.Random.Range(0, 100) < 25)
                {
                    Nemesis nemesis = Nemesis.CreateAngelNemesis();

                    GameManager.instance.nemeses.Add(nemesis);
                }
            }
        }

        foreach (Nemesis angel in GameManager.instance.nemesesPresent)
        {
            if (angel.mob.idType == MobTypeEnum.mobAngel && !angel.mob.CheckDead())
            {
                MobTypes.UpgradeAngel(angel.mob);
            }
        }

        UIManager.instance.GoToIntermissionScene();
    }
}
