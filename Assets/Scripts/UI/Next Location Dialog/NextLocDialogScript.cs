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

    public List<GameObject> nextLocPanels;


    // Use this for initialization
    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);
        nextLocPanels = new List<GameObject>();

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
            if (ll != LevelLayoutEnum.levelTest)
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

            nextLocPanel.GetComponent<NextLocPanelDialog>().InitializeUI(this, LevelLayouts.levelLayouts[levelLayout].name,
                System.String.Format("Location: {0}\nPopulation: {1}\nObjective: {2}", LevelLayouts.levelLayouts[levelLayout].name,
                    MonsterLayouts.monsterLayouts[monsterLayout].name, ObjectiveLayouts.objectiveLayouts[objectiveLayout].name),
                () =>
                {
                    GameManager.instance.monsterLayout = monsterLayout;
                    GameManager.instance.levelLayout = levelLayout;
                    GameManager.instance.objectiveLayout = objectiveLayout;
                });
            i++;
            nextLocPanels.Add(nextLocPanel);
        }
        else
        {
            do
            {
                int r = Random.Range(0, layoutList.Count);
                LevelLayoutEnum levelLayout = layoutList[r];
                layoutList.RemoveAt(r);
                MonsterLayoutEnum monsterLayout = (MonsterLayoutEnum)Random.Range(1, System.Enum.GetValues(typeof(MonsterLayoutEnum)).Length);
                ObjectiveLayoutEnum objectiveLayout = (ObjectiveLayoutEnum)Random.Range(1, System.Enum.GetValues(typeof(ObjectiveLayoutEnum)).Length);

                GameObject nextLocPanel = GameObject.Instantiate(NextLocPanelPrefab);
                nextLocPanel.transform.SetParent(NextLocScrollPanel.transform, false);
                nextLocPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1, 0 + i * -30);

                nextLocPanel.GetComponent<NextLocPanelDialog>().InitializeUI(this, LevelLayouts.levelLayouts[levelLayout].name,
                    System.String.Format("Location: {0}\nPopulation: {1}\nObjective: {2}", LevelLayouts.levelLayouts[levelLayout].name,
                        MonsterLayouts.monsterLayouts[monsterLayout].name, ObjectiveLayouts.objectiveLayouts[objectiveLayout].name),
                    () =>
                    {
                        GameManager.instance.monsterLayout = monsterLayout;
                        GameManager.instance.levelLayout = levelLayout;
                        GameManager.instance.objectiveLayout = objectiveLayout;
                    });
                i++;
                nextLocPanels.Add(nextLocPanel);
            } while (layoutList.Count > 0 && i < 3);
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
