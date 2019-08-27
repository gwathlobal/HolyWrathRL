using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NemesisActivity
{
    public NemesisActivityTypes.ActivityEnum idType;

    public abstract string DescrLineShort(Nemesis target);
    public abstract bool CheckRequirements(Nemesis nemesis);
    public abstract Nemesis FindTarget(Nemesis nemesis);
    public abstract string ProcessActivity(Nemesis nemesis, Nemesis target);
    public abstract void AssignActivity(Nemesis nemesis, Nemesis target);
}

public static class NemesisActivityTypes
{
    public enum ActivityEnum
    {
        none, duel, assassinate, becomeSubordinate, leaveSubordinate, huntDemons
    }

    public static Dictionary<ActivityEnum, NemesisActivity> nemesisActivities;

    public static void InitNemesisActivityTypes()
    {
        nemesisActivities = new Dictionary<ActivityEnum, NemesisActivity>();

        Add(new NemesisActivityDuel());
        Add(new NemActAssassinate());
        Add(new NemActBecomeSubordinate());
        Add(new NemActLeaveSubordinate());
        Add(new NemActHuntDemons());
    }

    public static void Add(NemesisActivity na)
    {
        nemesisActivities.Add(na.idType, na);
    }
}
