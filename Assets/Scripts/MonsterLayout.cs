using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterLayoutEnum
{
    levelTest, levelBeastsOnly, levelDemonsOnly, levelBeastsAndDemons
}

public abstract class MonsterLayout {

    public MonsterLayoutEnum layoutId;
    public string name;

    public abstract void PlaceMobs(Level level);

    protected void PlacePlayer(Level level)
    {

        bool createPlayer = false;
        PlayerMob player;

        if (GameManager.instance == null) createPlayer = true;
        if (GameManager.instance != null && GameManager.instance.player == null) createPlayer = true;

        if (createPlayer)
        {
            player = new PlayerMob(MobTypeEnum.mobAngel, 1, 1);
            
            player.curAbils[0] = AbilityTypeEnum.abilHealSelf;
            player.curAbils[1] = AbilityTypeEnum.abilMindBurn;
            
        }
        else
        {
            player = GameManager.instance.player;
            player.go = GameObject.Instantiate(MobTypes.mobTypes[player.idType].prefab, new Vector3(player.x, player.y, 0f), Quaternion.identity);
            player.mo = player.go.GetComponent<MovingObject>();
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
}

public static class MonsterLayouts
{
    public static Dictionary<MonsterLayoutEnum, MonsterLayout> monsterLayouts;

    public static void InitializeLayouts()
    {
        monsterLayouts = new Dictionary<MonsterLayoutEnum, MonsterLayout>();

        monsterLayouts.Add(MonsterLayoutEnum.levelTest, new MonsterLayoutTest());
        monsterLayouts.Add(MonsterLayoutEnum.levelBeastsOnly, new MonsterLayoutBeastsOnly());
        monsterLayouts.Add(MonsterLayoutEnum.levelDemonsOnly, new MonsterLayoutDemonsOnly());
        monsterLayouts.Add(MonsterLayoutEnum.levelBeastsAndDemons, new MonsterLayoutBeastsAndDemons());
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

    public override void PlaceMobs(Level level)
    {
        int numHomunculus = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numFiend = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numScavenger = 4 + Random.Range(0, GameManager.instance.levelNum);

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
    }
}

public class MonsterLayoutDemonsOnly : MonsterLayout
{
    public MonsterLayoutDemonsOnly()
    {
        name = "Demons";
    }

    public override void PlaceMobs(Level level)
    {
        int numCrimsonImp = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numCrimsonDemon = 4 + Random.Range(0, GameManager.instance.levelNum);

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
    }
}

public class MonsterLayoutBeastsAndDemons : MonsterLayout
{
    public MonsterLayoutBeastsAndDemons()
    {
        name = "Demons & beasts";
    }

    public override void PlaceMobs(Level level)
    {
        int numCrimsonImp = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numCrimsonDemon = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numHomunculus = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numFiend = 4 + Random.Range(0, GameManager.instance.levelNum);
        int numScavenger = 4 + Random.Range(0, GameManager.instance.levelNum);

        Dictionary<MobTypeEnum, int> mobsToSpawn = new Dictionary<MobTypeEnum, int>();
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonImp, numCrimsonImp);
        mobsToSpawn.Add(MobTypeEnum.mobCrimsonDemon, numCrimsonDemon);
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
    }
}

public class MonsterLayoutTest : MonsterLayout
{
    public MonsterLayoutTest()
    {
        name = "Test";
    }

    public override void PlaceMobs(Level level)
    {

        PlacePlayer(level);

        Mob mob;

        mob = new Mob(MobTypeEnum.mobCrimsonImp, 14, 1);
        mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
        BoardManager.instance.mobs.Add(mob.id, mob);
        level.AddMobToLevel(mob, mob.x, mob.y);
    }
}