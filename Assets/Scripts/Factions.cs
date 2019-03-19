using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactionEnum
{
    factionAngels, factionDemons, factionHumans, factionBeasts
}

public class Faction {
    public FactionEnum factionType;
    public Dictionary<FactionEnum, bool> relations;

    public bool GetRelation(FactionEnum faction)
    {
        return relations[faction];
    }
}

public static class Factions
{
    public static Dictionary<FactionEnum, Faction> factions;

    public static void InitializeFactions()
    {
        factions = new Dictionary<FactionEnum, Faction>();

        Dictionary<FactionEnum, bool> relations;

        relations = new Dictionary<FactionEnum, bool>();
        relations.Add(FactionEnum.factionAngels, true);
        relations.Add(FactionEnum.factionDemons, false);
        relations.Add(FactionEnum.factionHumans, false);
        relations.Add(FactionEnum.factionBeasts, false);
        Add(FactionEnum.factionAngels, relations);

        relations = new Dictionary<FactionEnum, bool>();
        relations.Add(FactionEnum.factionAngels, false);
        relations.Add(FactionEnum.factionDemons, true);
        relations.Add(FactionEnum.factionHumans, false);
        relations.Add(FactionEnum.factionBeasts, false);
        Add(FactionEnum.factionDemons, relations);

        relations = new Dictionary<FactionEnum, bool>();
        relations.Add(FactionEnum.factionAngels, false);
        relations.Add(FactionEnum.factionDemons, false);
        relations.Add(FactionEnum.factionHumans, true);
        relations.Add(FactionEnum.factionBeasts, false);
        Add(FactionEnum.factionHumans, relations);

        relations = new Dictionary<FactionEnum, bool>();
        relations.Add(FactionEnum.factionAngels, false);
        relations.Add(FactionEnum.factionDemons, false);
        relations.Add(FactionEnum.factionHumans, false);
        relations.Add(FactionEnum.factionBeasts, true);
        Add(FactionEnum.factionBeasts, relations);
    }

    public static void Add(FactionEnum _faction, Dictionary<FactionEnum, bool> _relations)
    {
        Faction f = new Faction()
        {
            factionType = _faction,
            relations = _relations
        };
        factions.Add(_faction, f);
    }
}