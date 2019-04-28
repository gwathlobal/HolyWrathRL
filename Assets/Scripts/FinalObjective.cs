using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FinalObjectiveEnum
{
    objWin10Levels
}

public abstract class FinalObjective
{

    FinalObjectiveEnum objId;

    public abstract bool CheckObjective();
}

public static class FinalObjectives
{
    public static Dictionary<FinalObjectiveEnum, FinalObjective> finalObjectives;

    public static void InitializeObjectives()
    {
        finalObjectives = new Dictionary<FinalObjectiveEnum, FinalObjective>();

        finalObjectives.Add(FinalObjectiveEnum.objWin10Levels, new FinalObjective10Wins());
    }

    static void Add(FinalObjectiveEnum _id, FinalObjective ml)
    {
        finalObjectives.Add(_id, ml);
    }
}

public class FinalObjective10Wins : FinalObjective
{
    public override bool CheckObjective()
    {
        if (GameManager.instance.levelNum >= 9) return true;
        else return false;
    }
}
