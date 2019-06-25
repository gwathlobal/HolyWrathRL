using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLocDialogScript : MonoBehaviour
{

    public IntermissionScript intermissionScript;
    public GameObject NextLocPanelPrefab;
    public Image NextLocScrollPanel;
    public Text locationTxt;
    public Text descrTxt;
    public Text missionCompletedTxt;

    public List<GameObject> nextLocPanels;


    // Use this for initialization
    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);
        nextLocPanels = new List<GameObject>();

        missionCompletedTxt.text = "Missions Completed: " + GameManager.instance.levelNum;

        SetUpScrollablePanels();

        nextLocPanels[0].GetComponent<NextLocPanelDialog>().OnClick();
    }

    public void SetUpScrollablePanels()
    {
        foreach (GameObject go in nextLocPanels)
        {
            Destroy(go);
        }
        nextLocPanels.Clear();

        int i = 0;
        List<LevelLayoutEnum> layoutList = new List<LevelLayoutEnum>();
        foreach (LevelLayoutEnum ll in System.Enum.GetValues(typeof(LevelLayoutEnum)))
        {
            if (ll != LevelLayoutEnum.levelTest && GameManager.instance.levelNum >= LevelLayouts.levelLayouts[ll].minLvl)
                layoutList.Add(ll);
        }
        
        if (GameManager.instance.levelNum == 0)
        {
            LevelLayoutEnum levelLayout = LevelLayoutEnum.levelDesolatePlains;
            MonsterLayoutEnum monsterLayout = MonsterLayoutEnum.levelBeastsOnly;
            ObjectiveLayoutEnum objectiveLayout = ObjectiveLayoutEnum.levelKillAllEnemies;

            GameObject nextLocPanel = GameObject.Instantiate(NextLocPanelPrefab);
            nextLocPanel.transform.SetParent(NextLocScrollPanel.transform, false);
            nextLocPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1, 0 + i * -30);

            List<LevelModifier> levelModifiers = new List<LevelModifier>();

            /*
            // prepare a list of available level modifiers
            List<LevelModifierTypes.LevelModifierEnum> availLevelModifiers = new List<LevelModifierTypes.LevelModifierEnum>();
            foreach (LevelModifierTypes.LevelModifierEnum lme in LevelLayouts.levelLayouts[levelLayout].levelModifiers)
            {
                if (LevelModifierTypes.levelModifiers[lme].CheckRequirements())
                    availLevelModifiers.Add(lme);
            }

            // 25% to pick a random level modifier, if picked - try to pick a next one until no more are available
            bool oneMoreLM = false;
            do
            {
                if (availLevelModifiers.Count > 0)
                {
                    if (Random.Range(0, 100) <= 100)
                    {
                        LevelModifierTypes.LevelModifierEnum lme = availLevelModifiers[Random.Range(0, availLevelModifiers.Count)];

                        System.Type t = LevelModifierTypes.levelModifiers[lme].GetType();

                        LevelModifier lm = (LevelModifier)System.Activator.CreateInstance(t);

                        lm.Initialize();

                        levelModifiers.Add(lm);
                        availLevelModifiers.Remove(lme);

                        oneMoreLM = true;
                    }
                    else
                        oneMoreLM = false;
                }
                else
                    oneMoreLM = false;

            } while (oneMoreLM);
            */

            string str = "";
            str += System.String.Format("Location: {0}\nPopulation: {1}\nObjective: {2}", LevelLayouts.levelLayouts[levelLayout].name,
                    MonsterLayouts.monsterLayouts[monsterLayout].name, ObjectiveLayouts.objectiveLayouts[objectiveLayout].name);

            bool firstLM = true;
            foreach (LevelModifier lm in levelModifiers)
            {
                if (firstLM)
                {
                    str += "\n\nLevel features:\n\n";
                    firstLM = false;
                }
                str += lm.Description();
            }

            nextLocPanel.GetComponent<NextLocPanelDialog>().InitializeUI(this, LevelLayouts.levelLayouts[levelLayout].name,
                str,
                () =>
                {
                    GameManager.instance.monsterLayout = monsterLayout;
                    GameManager.instance.levelLayout = levelLayout;
                    GameManager.instance.objectiveLayout = objectiveLayout;
                    GameManager.instance.levelModifiers = levelModifiers;
                });
            i++;
            nextLocPanels.Add(nextLocPanel);
        }
        else
        {
            for (i = 0; i < 3; i++)
            {
                if (layoutList.Count <= 0) break;

                int r = Random.Range(0, layoutList.Count);
                LevelLayoutEnum levelLayout = layoutList[r];
                layoutList.RemoveAt(r);

                ObjectiveLayoutEnum objectiveLayout = (ObjectiveLayoutEnum)Random.Range(1, System.Enum.GetValues(typeof(ObjectiveLayoutEnum)).Length);

                MonsterLayoutEnum monsterLayout = LevelLayouts.levelLayouts[levelLayout].monsterLayouts[Random.Range(0, LevelLayouts.levelLayouts[levelLayout].monsterLayouts.Count)];

                GameObject nextLocPanel = GameObject.Instantiate(NextLocPanelPrefab);
                nextLocPanel.transform.SetParent(NextLocScrollPanel.transform, false);
                nextLocPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1, 0 + i * -30);

                List<LevelModifier> levelModifiers = new List<LevelModifier>();

                // prepare a list of available level modifiers
                List<LevelModifierTypes.LevelModifierEnum> availLevelModifiers = new List<LevelModifierTypes.LevelModifierEnum>();
                foreach (LevelModifierTypes.LevelModifierEnum lme in LevelLayouts.levelLayouts[levelLayout].levelModifiers)
                {
                    if (LevelModifierTypes.levelModifiers[lme].CheckRequirements())
                        availLevelModifiers.Add(lme);
                }

                // 25% to pick a random level modifier, if picked - try to pick a next one until no more are available
                bool oneMoreLM = false;
                do
                {
                    if (availLevelModifiers.Count > 0)
                    {
                        if (Random.Range(0, 100) <= 25)
                        {
                            LevelModifierTypes.LevelModifierEnum lme = availLevelModifiers[Random.Range(0, availLevelModifiers.Count)];

                            System.Type t = LevelModifierTypes.levelModifiers[lme].GetType();

                            LevelModifier lm = (LevelModifier)System.Activator.CreateInstance(t);

                            lm.Initialize();

                            levelModifiers.Add(lm);
                            availLevelModifiers.Remove(lme);

                            oneMoreLM = true;
                        }
                        else
                            oneMoreLM = false;
                    }
                    else
                        oneMoreLM = false;

                } while (oneMoreLM);

                string str = "";
                str += System.String.Format("Location: {0}\nPopulation: {1}\nObjective: {2}", LevelLayouts.levelLayouts[levelLayout].name,
                        MonsterLayouts.monsterLayouts[monsterLayout].name, ObjectiveLayouts.objectiveLayouts[objectiveLayout].name);

                bool firstLM = true;
                foreach (LevelModifier lm in levelModifiers)
                {
                    if (firstLM)
                    {
                        str += "\n\nLevel features:\n";
                        firstLM = false;
                    }
                    str += lm.Description();
                }

                nextLocPanel.GetComponent<NextLocPanelDialog>().InitializeUI(this, LevelLayouts.levelLayouts[levelLayout].name,
                    str,
                    () =>
                    {
                        GameManager.instance.monsterLayout = monsterLayout;
                        GameManager.instance.levelLayout = levelLayout;
                        GameManager.instance.objectiveLayout = objectiveLayout;
                        GameManager.instance.levelModifiers = levelModifiers;
                    });
                nextLocPanels.Add(nextLocPanel);
            }
        }
        /*
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            GameObject nemesisPanel = GameObject.Instantiate(NextLocPanelPrefab);
            nemesisPanel.transform.SetParent(NextLocScrollPanel.transform, false);
            nemesisPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1, 0 + i * -30);

            nemesisPanel.GetComponent<NextLocPanelDialog>().InitializeUI(this, nemesis.mob.name, nemesis.GetNemesisDescription());
            i++;
            nextLocPanels.Add(nemesisPanel);
        }
        */
        NextLocScrollPanel.rectTransform.sizeDelta = new Vector2(158, i * 30);
    }
}
