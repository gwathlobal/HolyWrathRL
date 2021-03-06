﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour {

    public GameObject[,] tiles;
    public GameObject[,] fog;
    public GameObject[,] unexplored;
    
    public static BoardManager instance = null;
    
    public Level level;
    public Dictionary<int, Mob> mobs;
    public Dictionary<int, Item> items;
    public Dictionary<int, Feature> features;
    public Dictionary<int, Effect> effects;
    public PlayerMob player;
    public float prevTime = 0;

    public List<Feature> featuresToRemove;

    public MessageLog msgLog;

    private bool mobActed = false;

    public bool playersTurn = false;

    public int turnNum = 0;

    private List<Mob> mobList;

    //public List<Nemesis> nemesesPresent;

    public GameObject unexploredPrefab;

    public int curMobId;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {

        Nemesis.InitializeNames();
        DmgTypes.InitializeDmgTypes();
        AIs.InitializeAIPackages();
        Factions.InitializeFactions();
        LevelLayouts.InitializeLayouts();
        MonsterLayouts.InitializeLayouts();
        ObjectiveLayouts.InitializeLayouts();
        FinalObjectives.InitializeObjectives();

        TerrainTypes.InitializeTerrainTypes();
        MobTypes.InitializeMobTypes();
        ItemTypes.InitializeItemTypes();
        FeatureTypes.InitializeFeatureTypes();
        AbilityTypes.InitializeAbilTypes();
        EffectTypes.InitializeEffects();
        BuildingLayouts.InitializeLayouts();

        mobs = new Dictionary<int, Mob>();
        items = new Dictionary<int, Item>();
        features = new Dictionary<int, Feature>();
        effects = new Dictionary<int, Effect>();

        featuresToRemove = new List<Feature>();

        msgLog = new MessageLog();

        LevelLayoutEnum ll = LevelLayoutEnum.levelTest;
        MonsterLayoutEnum ml = MonsterLayoutEnum.levelTest;
        ObjectiveLayoutEnum ol = ObjectiveLayoutEnum.levelTest;
        List<LevelModifier> lm = new List<LevelModifier>();
        if (GameManager.instance != null)
        {
            ll = GameManager.instance.levelLayout;
            ml = GameManager.instance.monsterLayout;
            ol = GameManager.instance.objectiveLayout;
            lm = GameManager.instance.levelModifiers;
        }
        level = new Level(ll, ml, ol, lm);

        if (GameManager.instance != null && GameManager.instance.player == null)
            GameManager.instance.player = player;


        tiles = new GameObject[level.maxX, level.maxY];
        fog = new GameObject[level.maxX, level.maxY];
        unexplored = new GameObject[level.maxX, level.maxY];
        for (int y=0;y<level.maxY; y++)
        {
            for(int x=0;x<level.maxX; x++)
            {
                tiles[x, y] = Instantiate(TerrainTypes.terrainTypes[level.terrain[x, y]].prefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                tiles[x, y].GetComponent<SpriteRenderer>().color = TerrainTypes.terrainTypes[level.terrain[x, y]].color;
                tiles[x, y].transform.SetParent(GameObject.Find("TilesParent").transform);
                fog[x, y] = Instantiate(TerrainTypes.terrainTypes[level.terrain[x, y]].prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                fog[x, y].GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 255);
                fog[x, y].GetComponent<SpriteRenderer>().sortingOrder = 10;
                fog[x, y].transform.SetParent(GameObject.Find("FogParent").transform);
                unexplored[x, y] = Instantiate(unexploredPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                unexplored[x, y].GetComponent<SpriteRenderer>().sortingOrder = 15;
                unexplored[x, y].transform.SetParent(GameObject.Find("UnexploredParent").transform);
            }
        }

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().player = player.go;
        
        playersTurn = false;

        UIManager.instance.InitializeUI(player);

        mobList = new List<Mob>();

        if (GameManager.instance.levelNum == 0)
        {
            msgLog.AddMsg("Welcome to Holy Wrath RL. To view help, press '?'.");
            msgLog.FinalizeMsg();
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.learntAbilities = 0;
            GameManager.instance.learntLocations = 0;
            GameManager.instance.learntNames = 0;
        }

        UIManager.instance.RightPanel.ShowStatus();
    }

    // Update is called once per frame
    void Update() {

        if (playersTurn || BoardEventController.instance.toProccessQueue.Count > 0 || BoardEventController.instance.inProcessCount > 0 ||
            BoardEventController.instance.coroutinesInProcess > 0)
            return;

        mobList = mobs.Values.ToList();

        foreach (Mob mob in mobList)
        {
            if (mob.curAP > 0 && !mob.CheckDead())
            {
                //Debug.Log(mob.name + " FINALIZE MSG, msgLog.hasMsg = " + msgLog.HasMessageThisTurn());
                msgLog.FinalizeMsg();

                //msgLog.SetHasMessageThisTurn(false);
                mob.AiFunction();
                mobActed = true;
                if (playersTurn)
                {
                    msgLog.SetPlayerStartedTurn(true);
                    UIManager.instance.LeftPanel.UpdateLeftPanel();
                    return;
                }
                else
                {
                    //msgLog.FinalizeMsg();
                    return;
                }
            }
        }

        if (mobActed)
        {
            mobActed = false;
            return;
        }

        msgLog.FinalizeMsg();

        /*
        for (int id = mobs.Count - 1; id >= 0; id--) 
        {
            if (mobs.ContainsKey(id) && mobs[id].CheckDead()) mobs.Remove(id);
        }
        */

        foreach (Feature feature in features.Values.ToList())
        {
            msgLog.SetHasMessageThisTurn(false);
            if (FeatureTypes.featureTypes[feature.idType].FeatOnTick != null)
                FeatureTypes.featureTypes[feature.idType].FeatOnTick(level, feature);
            msgLog.FinalizeMsg();
        }

        for (int i = featuresToRemove.Count - 1; i >= 0; i--)
        {
            RemoveFeatureFromWorld(featuresToRemove[i]);
        }
        featuresToRemove.Clear();

        foreach (int mobId in mobs.Keys)
        {
            msgLog.SetHasMessageThisTurn(false);
            if (!mobs[mobId].CheckDead()) mobs[mobId].OnTick();
            msgLog.FinalizeMsg();
        }

        /*
        for (int id = 0; id < mobs.Count; id++) 
        {
            if (mobs.ContainsKey(id) && !mobs[id].CheckDead()) mobs[id].OnTick();
        }
        */

        if (ObjectiveLayouts.objectiveLayouts[level.objectiveType].CheckObjective() &&
            FinalObjectives.finalObjectives[FinalObjectiveEnum.objWin10Levels].CheckObjective())
        {
            GameManager.instance.msgLog = BoardManager.instance.msgLog.GetLastMessagesAsText(BoardManager.instance.msgLog.MsgLength());
            SceneManager.LoadScene("WinScene");
        }

        if (ObjectiveLayouts.objectiveLayouts[level.objectiveType].CheckObjective())
        {
            GameManager.instance.levelNum++;
            UIManager.instance.ShowMissionWonWindow();
            playersTurn = true;
            return;
        }

        foreach (GameEvent gameEvent in level.gameEvents)
        {
            if (gameEvent.CheckEvent())
            {
                gameEvent.Activate();
            }
        }

        turnNum++;
        UIManager.instance.RightPanel.ShowStatus();
    }

    public void RemoveMobFromWorld(Mob mob)
    {
        level.RemoveMobFromLevel(mob);
        mobs.Remove(mob.id);
        if (mob.go != null) Destroy(mob.go);
    }

    public void RemoveItemFromWorld(Item item)
    {
        level.RemoveItemFromLevel(item);
        items.Remove(item.id);
        if (item.go != null) Destroy(item.go);
    }

    public void RemoveFeatureFromWorld(Feature feature)
    {
        level.RemoveFeatureFromLevel(feature);
        features.Remove(feature.id);
        if (feature.go != null) Destroy(feature.go);
    }

    public int FindFreeID(IDictionary dict)
    {
        int i = -1;
        do
        {
            i++;
        } while (dict.Contains(i) == true);
        return i;
    }

    public void FinalizePlayerTurn()
    {
        playersTurn = false;
        BoardEventController.instance.AddEvent(new BoardEventController.Event(player.go,
            () =>
            {
                player.GetFOV();
                BoardEventController.instance.RemoveFinishedEvent();
            }));
    }

    public void CreateBlooddrop(int x, int y)
    {
        // create a blood pool
        int r = Random.Range(1, 9);
        Vector2 poolPos = new Vector2(x, y);
        switch (r)
        {
            case 1:
                poolPos.x -= 1; poolPos.y -= 1;
                break;
            case 2:
                poolPos.y -= 1;
                break;
            case 3:
                poolPos.x += 1; poolPos.y -= 1;
                break;
            case 4:
                poolPos.x -= 1;
                break;
            case 6:
                poolPos.x += 1;
                break;
            case 7:
                poolPos.x -= 1; poolPos.y += 1;
                break;
            case 8:
                poolPos.y += 1;
                break;
            case 9:
                poolPos.x += 1; poolPos.y += 1;
                break;
        }
        r = Random.Range(0, 100);
        if (r <= 30 && TerrainTypes.terrainTypes[level.terrain[(int)poolPos.x,(int)poolPos.y]].takesBlood)
        {
            Feature bloodPool = new Feature(FeatureTypeEnum.featBloodDrop, (int)poolPos.x, (int)poolPos.y);
            level.AddFeatureToLevel(bloodPool, bloodPool.x, bloodPool.y);
            //if (!level.visible[bloodPool.x, bloodPool.y] && bloodPool.go != null) bloodPool.go.GetComponent<Renderer>().enabled = false;
        }
    }
}
