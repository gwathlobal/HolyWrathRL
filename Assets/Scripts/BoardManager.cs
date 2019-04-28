using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour {

    public GameObject[,] tiles;
    public GameObject[,] fog;
    
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

    public int levelNum = 0;

    private List<Mob> mobList;

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
        if (GameManager.instance != null)
        {
            ll = GameManager.instance.levelLayout;
            ml = GameManager.instance.monsterLayout;
            ol = GameManager.instance.objectiveLayout;
        }
        level = new Level(ll, ml, ol);

        if (GameManager.instance != null && GameManager.instance.player == null)
            GameManager.instance.player = player;


        tiles = new GameObject[level.maxX, level.maxY];
        fog = new GameObject[level.maxX, level.maxY];
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
            }
        }

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().player = player.go;
        
        playersTurn = false;

        UIManager.instance.InitializeUI(player);

        mobList = new List<Mob>();
    }

    // Update is called once per frame
    void Update() {

        if (playersTurn || BoardAnimationController.instance.toProccessQueue.Count > 0 || BoardAnimationController.instance.inProcessCount > 0)
            //If any of these are true, return and do not start MoveEnemies.
            return;

        mobList = mobs.Values.ToList();

        foreach (Mob mob in mobList)
        {
            if (mob.curAP > 0 && !mob.CheckDead())
            {
                msgLog.SetHasMessageThisTurn(false);
                mob.AiFunction();
                mobActed = true;
                if (playersTurn)
                {
                    msgLog.SetPlayerStartedTurn(true);
                    return;
                }

                msgLog.FinalizeMsg();
            }
        }

        if (mobActed)
        {
            mobActed = false;
            return;
        }
        else
        {
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
        }

        if (ObjectiveLayouts.objectiveLayouts[level.objectiveType].CheckObjective() &&
            FinalObjectives.finalObjectives[FinalObjectiveEnum.objWin10Levels].CheckObjective())
        {
            SceneManager.LoadScene("WinScene");
        }

        if (ObjectiveLayouts.objectiveLayouts[level.objectiveType].CheckObjective())
        {
            GameManager.instance.levelNum++;
            UIManager.instance.ShowMissionWonWindow();
            playersTurn = true;
        }
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
        player.GetFOV();
        playersTurn = false;
        msgLog.FinalizeMsg();
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
        if (r <= 30)
        {
            Feature bloodPool = new Feature(FeatureTypeEnum.featBloodDrop, (int)poolPos.x, (int)poolPos.y);
            level.AddFeatureToLevel(bloodPool, bloodPool.x, bloodPool.y);
            //if (!level.visible[bloodPool.x, bloodPool.y] && bloodPool.go != null) bloodPool.go.GetComponent<Renderer>().enabled = false;
        }
    }
}
