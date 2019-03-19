using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveLayoutEnum
{
    levelTest, levelKillAllEnemies
}


public abstract class ObjectiveLayout {

    ObjectiveLayoutEnum layoutId;
    public string name;

    public abstract bool CheckObjective();
}

public static class ObjectiveLayouts
{
    public static Dictionary<ObjectiveLayoutEnum, ObjectiveLayout> objectiveLayouts;

    public static void InitializeLayouts()
    {
        objectiveLayouts = new Dictionary<ObjectiveLayoutEnum, ObjectiveLayout>();

        objectiveLayouts.Add(ObjectiveLayoutEnum.levelTest, new ObjectiveLayoutTest());
        objectiveLayouts.Add(ObjectiveLayoutEnum.levelKillAllEnemies, new ObjectiveLayoutKillAllEnemies());
    }

    static void Add(ObjectiveLayoutEnum _id, ObjectiveLayout ml)
    {
        objectiveLayouts.Add(_id, ml);
    }
}

public class ObjectiveLayoutTest : ObjectiveLayout
{
    public ObjectiveLayoutTest()
    {
        name = "Indefinite gameplay";
    }

    public override bool CheckObjective()
    {
        return false;
    }
}

public class ObjectiveLayoutKillAllEnemies : ObjectiveLayout
{
    public ObjectiveLayoutKillAllEnemies()
    {
        name = "Destroy all enemies";
    }

    public override bool CheckObjective()
    {
        Level level = BoardManager.instance.level;
        PlayerMob player = BoardManager.instance.player;
        int enemiesCount = 0;

        foreach (Mob mob in level.mobList)
        {
            if (!mob.CheckDead() && mob != player && !player.GetFactionRelation(mob.faction)) enemiesCount++;
        }

        if (enemiesCount == 0) return true;
        else return false;
    }
}