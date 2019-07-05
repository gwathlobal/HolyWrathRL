using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterLayoutEnum
{
    levelTest, levelBeastsOnly, levelCrimsonDemons, levelMachineDemons, levelBeastsAndDemons, levelHumansVsDemons,
    levelSoldiersVsDemons
}

public abstract class MonsterLayout {

    public MonsterLayoutEnum layoutId;
    public string name;

    public abstract void PlaceMobs(Level level, LevelGeneratorResult levelGeneratorResult);

    protected void PlacePlayer(Level level)
    {

        bool createPlayer = false;
        PlayerMob player;

        if (GameManager.instance == null) createPlayer = true;
        if (GameManager.instance != null && GameManager.instance.player == null) createPlayer = true;

        if (createPlayer)
        {
            player = new PlayerMob(MobTypeEnum.mobPCAngel, 1, 1);
            GameObject selector = GameObject.Instantiate(UIManager.instance.selectorPrefab, player.go.transform) as GameObject;
            // NOTE: for unknown reason it gets shift -1,-1 so I cant put it to 0, 0
            selector.transform.position = new Vector3(1, 1, 0);
            selector.GetComponent<SpriteRenderer>().color = new Color32(0, 100, 0, 255);

        }
        else
        {
            player = GameManager.instance.player;
            player.go = GameObject.Instantiate(MobTypes.mobTypes[player.idType].prefab, new Vector3(player.x, player.y, 0f), Quaternion.identity);
            player.mo = player.go.GetComponent<MovingObject>();

            GameObject selector = GameObject.Instantiate(UIManager.instance.selectorPrefab, player.go.transform) as GameObject;
            // NOTE: for unknown reason it gets shift -1,-1 so I cant put it to 0, 0
            selector.transform.position = new Vector3(1, 1, 0);
            selector.GetComponent<SpriteRenderer>().color = new Color32(0, 100, 0, 255);

            player.curHP = player.maxHP;
            player.curFP = player.maxFP;
            player.curWP = 0;
            player.curAP = MobType.NORMAL_AP;

            player.effects = new Dictionary<EffectTypeEnum, Effect>();
        }

        BoardManager.instance.gameObject.AddComponent<PlayerInput>();
        player.go.tag = "Player";
        BoardManager.instance.mobs.Add(player.id, player);
        BoardManager.instance.player = player;

        Vector2Int loc;
        if (level.FindFreeSpotAtBorder(out loc))
        {
            player.x = loc.x;
            player.y = loc.y;
        }

        level.AddMobToLevel(player, player.x, player.y);
    }

    protected void PlaceLevelLayoutMobs(Level level, LevelGeneratorResult levelGeneratorResult, Dictionary<MobTypeEnum, int> mobsToSpawn)
    {
        foreach (BuildingLayoutResult br in levelGeneratorResult.buildingLayoutResults)
        {
            if (br.mobPlacements != null)
            {
                foreach (BuildingLayoutResult.MobPlacement mobPlacement in br.mobPlacements)
                {
                    if (mobsToSpawn.ContainsKey(mobPlacement.mobType) && mobsToSpawn[mobPlacement.mobType] > 0)
                    {
                        if (mobPlacement.buildingPlaceMob != null)
                            mobPlacement.buildingPlaceMob(level, mobPlacement.mobType, br.sx, br.sy);
                        mobsToSpawn[mobPlacement.mobType]--;
                    }
                }
            }
        }
    }
}

public static class MonsterLayouts
{
    public static Dictionary<MonsterLayoutEnum, MonsterLayout> monsterLayouts;

    public static void InitializeLayouts()
    {
        monsterLayouts = new Dictionary<MonsterLayoutEnum, MonsterLayout>();

        monsterLayouts.Add(MonsterLayoutEnum.levelTest, new MonsterLayoutTest());
        monsterLayouts.Add(MonsterLayoutEnum.levelBeastsOnly, new MonsterLayoutBeastsOnly());
        monsterLayouts.Add(MonsterLayoutEnum.levelCrimsonDemons, new MonsterLayoutCrimsonDemons());
        monsterLayouts.Add(MonsterLayoutEnum.levelMachineDemons, new MonsterLayoutMachineDemons());
        monsterLayouts.Add(MonsterLayoutEnum.levelBeastsAndDemons, new MonsterLayoutBeastsAndDemons());
        monsterLayouts.Add(MonsterLayoutEnum.levelHumansVsDemons, new MonsterLayoutHumansVsDemons());
        monsterLayouts.Add(MonsterLayoutEnum.levelSoldiersVsDemons, new MonsterLayoutSoldiersVsDemons());
    }

    static void Add(MonsterLayoutEnum _id, MonsterLayout ml)
    {
        monsterLayouts.Add(_id, ml);
    }
}


public class MonsterLayoutBeastsOnly : MonsterLayout
{
    public MonsterLayoutBeastsOnly()
    {
        name = "Mindless beasts";
    }

    public override void PlaceMobs(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        int numHomunculus = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numFiend = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numScavenger = 2 + Random.Range(0, GameManager.instance.levelNum);
        int numTarDemon = GameManager.instance.levelNum;

        Dictionary<MobTypeEnum, int> mobsToSpawn = new Dictionary<MobTypeEnum, int>();
        mobsToSpawn.Add(MobTypeEnum.mobHomunculus, numHomunculus);
        mobsToSpawn.Add(MobTypeEnum.mobFiend, numFiend);
        mobsToSpawn.Add(MobTypeEnum.mobScavenger, numScavenger);

        PlacePlayer(level);

        Vector2Int loc;
        Mob mob;
        foreach (MobTypeEnum mobType in mobsToSpawn.Keys)
        {
            for (int i = 0; i < mobsToSpawn[mobType]; i++)
            {
                if (level.FindFreeSpotInside(out loc))
                {
                    mob = new Mob(mobType, loc.x, loc.y);
                    mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                    BoardManager.instance.mobs.Add(mob.id, mob);
                    level.AddMobToLevel(mob, mob.x, mob.y);
                }
            }
        }

        Dictionary<MobTypeEnum, int> addMobsToSpawn = new Dictionary<MobTypeEnum, int>();
        addMobsToSpawn.Add(MobTypeEnum.mobTarDemon, numTarDemon);

        PlaceLevelLayoutMobs(level, levelGeneratorResult, addMobsToSpawn);
    }
}

public class MonsterLayoutCrimsonDemons : MonsterLayout
{
    public MonsterLayoutCrimsonDemons()
    {
        name = "Crimson demons";
    }

    public override void PlaceMobs(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        int numCrimsonImp = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numCrimsonDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numTarDemon = GameManager.instance.levelNum;

        Dictionary<MobTypeEnum, int> mobsToSpawn = new Dictionary<MobTypeEnum, int>();
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonImp, numCrimsonImp);
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonDemon, numCrimsonDemon);

        PlacePlayer(level);

        Vector2Int loc;
        Mob mob;
        foreach (MobTypeEnum mobType in mobsToSpawn.Keys)
        {
            for (int i = 0; i < mobsToSpawn[mobType]; i++)
            {
                if (level.FindFreeSpotInside(out loc))
                {
                    mob = new Mob(mobType, loc.x, loc.y);
                    mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                    BoardManager.instance.mobs.Add(mob.id, mob);
                    level.AddMobToLevel(mob, mob.x, mob.y);
                }
            }
        }

        Dictionary<MobTypeEnum, int> addMobsToSpawn = new Dictionary<MobTypeEnum, int>();
        addMobsToSpawn.Add(MobTypeEnum.mobTarDemon, numTarDemon);

        PlaceLevelLayoutMobs(level, levelGeneratorResult, addMobsToSpawn);
    }
}

public class MonsterLayoutMachineDemons : MonsterLayout
{
    public MonsterLayoutMachineDemons()
    {
        name = "Machine demons";
    }

    public override void PlaceMobs(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        int numMachineImp = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numMachineDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numTarDemon = GameManager.instance.levelNum;

        Dictionary<MobTypeEnum, int> mobsToSpawn = new Dictionary<MobTypeEnum, int>();
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonImp, numMachineImp);
        mobsToSpawn.Add(MobTypeEnum.mobMachineDemon, numMachineDemon);

        PlacePlayer(level);

        Vector2Int loc;
        Mob mob;
        foreach (MobTypeEnum mobType in mobsToSpawn.Keys)
        {
            for (int i = 0; i < mobsToSpawn[mobType]; i++)
            {
                if (level.FindFreeSpotInside(out loc))
                {
                    mob = new Mob(mobType, loc.x, loc.y);
                    mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                    BoardManager.instance.mobs.Add(mob.id, mob);
                    level.AddMobToLevel(mob, mob.x, mob.y);
                }
            }
        }

        Dictionary<MobTypeEnum, int> addMobsToSpawn = new Dictionary<MobTypeEnum, int>();
        addMobsToSpawn.Add(MobTypeEnum.mobTarDemon, numTarDemon);

        PlaceLevelLayoutMobs(level, levelGeneratorResult, addMobsToSpawn);
    }
}

public class MonsterLayoutBeastsAndDemons : MonsterLayout
{
    public MonsterLayoutBeastsAndDemons()
    {
        name = "Demons & beasts";
    }

    public override void PlaceMobs(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        int numCrimsonImp = 0;
        int numCrimsonDemon = 0;
        int numMachineImp = 0;
        int numMachineDemon = 0;
        int numHomunculus = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numFiend = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numScavenger = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numTarDemon = GameManager.instance.levelNum;

        int r = Random.Range(0, 3);
        switch (r)
        {
            // crimson demons
            case 1:
                numCrimsonImp = 4 + Random.Range(0, GameManager.instance.levelNum);
                numCrimsonDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
                break;
            // shadow demons
            case 2:
                numMachineImp = 4 + Random.Range(0, GameManager.instance.levelNum);
                numMachineDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
                break;
            // both crimson & shadow demons
            default:
                numCrimsonImp = 2 + Random.Range(0, GameManager.instance.levelNum);
                numCrimsonDemon = 2 + Random.Range(0, GameManager.instance.levelNum);
                numMachineImp = 2 + Random.Range(0, GameManager.instance.levelNum);
                numMachineDemon = 2 + Random.Range(0, GameManager.instance.levelNum);
                break;
        }

        Dictionary<MobTypeEnum, int> mobsToSpawn = new Dictionary<MobTypeEnum, int>();
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonImp, numCrimsonImp);
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonDemon, numCrimsonDemon);
        mobsToSpawn.Add(MobTypeEnum.mobMachineImp, numMachineImp);
        mobsToSpawn.Add(MobTypeEnum.mobMachineDemon, numMachineDemon);
        mobsToSpawn.Add(MobTypeEnum.mobHomunculus, numHomunculus);
        mobsToSpawn.Add(MobTypeEnum.mobFiend, numFiend);
        mobsToSpawn.Add(MobTypeEnum.mobScavenger, numScavenger);

        PlacePlayer(level);

        Vector2Int loc;
        Mob mob;
        foreach (MobTypeEnum mobType in mobsToSpawn.Keys)
        {
            for (int i = 0; i < mobsToSpawn[mobType]; i++)
            {
                if (level.FindFreeSpotInside(out loc))
                {
                    mob = new Mob(mobType, loc.x, loc.y);
                    mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                    BoardManager.instance.mobs.Add(mob.id, mob);
                    level.AddMobToLevel(mob, mob.x, mob.y);
                }
            }
        }

        Dictionary<MobTypeEnum, int> addMobsToSpawn = new Dictionary<MobTypeEnum, int>();
        addMobsToSpawn.Add(MobTypeEnum.mobTarDemon, numTarDemon);

        PlaceLevelLayoutMobs(level, levelGeneratorResult, addMobsToSpawn);
    }
}

public class MonsterLayoutHumansVsDemons : MonsterLayout
{
    public MonsterLayoutHumansVsDemons()
    {
        name = "Demons vs Humans";
    }

    public override void PlaceMobs(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        int numCrimsonImp = 0;
        int numCrimsonDemon = 0;
        int numMachineImp = 0;
        int numMachineDemon = 0;
        int r = Random.Range(0, 3);
        switch (r)
        {
            // crimson demons
            case 1:
                numCrimsonImp = 4 + Random.Range(0, GameManager.instance.levelNum);
                numCrimsonDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
                break;
            // shadow demons
            case 2:
                numMachineImp = 4 + Random.Range(0, GameManager.instance.levelNum);
                numMachineDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
                break;
            // both crimson & shadow demons
            default:
                numCrimsonImp = 2 + Random.Range(0, GameManager.instance.levelNum);
                numCrimsonDemon = 2 + Random.Range(0, GameManager.instance.levelNum);
                numMachineImp = 2 + Random.Range(0, GameManager.instance.levelNum);
                numMachineDemon = 2 + Random.Range(0, GameManager.instance.levelNum);
                break;
        }

        Dictionary<MobTypeEnum, int> mobsToSpawn = new Dictionary<MobTypeEnum, int>();
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonImp, numCrimsonImp);
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonDemon, numCrimsonDemon);
        mobsToSpawn.Add(MobTypeEnum.mobMachineImp, numMachineImp);
        mobsToSpawn.Add(MobTypeEnum.mobMachineDemon, numMachineDemon);

        PlacePlayer(level);

        Vector2Int loc;
        Mob mob;
        foreach (MobTypeEnum mobType in mobsToSpawn.Keys)
        {
            for (int i = 0; i < mobsToSpawn[mobType]; i++)
            {
                if (level.FindFreeSpotInside(out loc))
                {
                    mob = new Mob(mobType, loc.x, loc.y);
                    mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                    BoardManager.instance.mobs.Add(mob.id, mob);
                    level.AddMobToLevel(mob, mob.x, mob.y);
                }
            }
        }

        int numHumans = 100;

        Dictionary<MobTypeEnum, int> addMobsToSpawn = new Dictionary<MobTypeEnum, int>();
        addMobsToSpawn.Add(MobTypeEnum.mobHuman, numHumans);

        PlaceLevelLayoutMobs(level, levelGeneratorResult, addMobsToSpawn);
    }
}

public class MonsterLayoutSoldiersVsDemons : MonsterLayout
{
    public MonsterLayoutSoldiersVsDemons()
    {
        name = "Demons vs Soldiers";
    }

    public override void PlaceMobs(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        int numCrimsonImp = 0;
        int numCrimsonDemon = 0;
        int numMachineImp = 0;
        int numMachineDemon = 0;
        int r = Random.Range(0, 3);
        switch (r)
        {
            // crimson demons
            case 1:
                numCrimsonImp = 4 + Random.Range(0, GameManager.instance.levelNum);
                numCrimsonDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
                break;
            // shadow demons
            case 2:
                numMachineImp = 4 + Random.Range(0, GameManager.instance.levelNum);
                numMachineDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
                break;
            // both crimson & shadow demons
            default:
                numCrimsonImp = 2 + Random.Range(0, GameManager.instance.levelNum);
                numCrimsonDemon = 2 + Random.Range(0, GameManager.instance.levelNum);
                numMachineImp = 2 + Random.Range(0, GameManager.instance.levelNum);
                numMachineDemon = 2 + Random.Range(0, GameManager.instance.levelNum);
                break;
        }

        Dictionary<MobTypeEnum, int> mobsToSpawn = new Dictionary<MobTypeEnum, int>();
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonImp, numCrimsonImp);
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonDemon, numCrimsonDemon);
        mobsToSpawn.Add(MobTypeEnum.mobMachineImp, numMachineImp);
        mobsToSpawn.Add(MobTypeEnum.mobMachineDemon, numMachineDemon);

        PlacePlayer(level);

        Vector2Int loc;
        Mob mob;
        foreach (MobTypeEnum mobType in mobsToSpawn.Keys)
        {
            for (int i = 0; i < mobsToSpawn[mobType]; i++)
            {
                if (level.FindFreeSpotInside(out loc))
                {
                    mob = new Mob(mobType, loc.x, loc.y);
                    mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                    BoardManager.instance.mobs.Add(mob.id, mob);
                    level.AddMobToLevel(mob, mob.x, mob.y);
                }
            }
        }

        int numHumans = 100;
        int numSoldiers = 100;
        int numMachinegunners = 100;
        int numScouts = 100;

        Dictionary<MobTypeEnum, int> addMobsToSpawn = new Dictionary<MobTypeEnum, int>();
        addMobsToSpawn.Add(MobTypeEnum.mobHuman, numHumans);
        addMobsToSpawn.Add(MobTypeEnum.mobSoldier, numSoldiers);
        addMobsToSpawn.Add(MobTypeEnum.mobMachinegunman, numMachinegunners);
        addMobsToSpawn.Add(MobTypeEnum.mobScout, numScouts);

        PlaceLevelLayoutMobs(level, levelGeneratorResult, addMobsToSpawn);
    }
}

public class MonsterLayoutTest : MonsterLayout
{
    public MonsterLayoutTest()
    {
        name = "Test";
    }

    public override void PlaceMobs(Level level, LevelGeneratorResult levelGeneratorResult)
    {

        PlacePlayer(level);

        BoardManager.instance.player.curAbils[0] = AbilityTypeEnum.abilWarmingLight;
        //player.curAbils[1] = AbilityTypeEnum.abilMindBurn;
        //player.meleeAbil = AbilityTypeEnum.abilFireFists;
        //player.abilities.Add(AbilityTypeEnum.abilFireFists, true);
        BoardManager.instance.player.abilities.Add(AbilityTypeEnum.abilWarmingLight, true);
        //player.blockAbil = AbilityTypeEnum.abilReflectiveBlock;
        //UIManager.instance.LeftPanel.blockAbilPanel.abilType = AbilityTypeEnum.abilReflectiveBlock;

        level.mobs[BoardManager.instance.player.x, BoardManager.instance.player.y] = null;
        level.mobs[5, 5] = BoardManager.instance.player;
        BoardManager.instance.player.x = 5;
        BoardManager.instance.player.y = 5;
        BoardManager.instance.player.go.GetComponent<Rigidbody2D>().MovePosition(new Vector2(5, 5));

        Mob mob;

        //BoardManager.instance.player.curFP = 10;

        /*
        mob = new Mob(MobTypeEnum.mobCrimsonImp, 16, 5);
        mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
        BoardManager.instance.mobs.Add(mob.id, mob);
        level.AddMobToLevel(mob, mob.x, mob.y);
        */
        /*
        mob = new Mob(MobTypeEnum.mobScout, 21, 5);
        mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
        BoardManager.instance.mobs.Add(mob.id, mob);
        level.AddMobToLevel(mob, mob.x, mob.y);
        //mob.curHP = 25;
        */
        /*
        mob = new Mob(MobTypeEnum.mobScout, 12, 5);
        mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
        BoardManager.instance.mobs.Add(mob.id, mob);
        level.AddMobToLevel(mob, mob.x, mob.y);
        */
    }
}

