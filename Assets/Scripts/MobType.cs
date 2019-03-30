using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DmgTypeEnum
{
    Physical, Fire, Holy, Vorpanite, Mind, Shadow 
}

public struct DmgType
{
    public DmgTypeEnum dmgType;
    public string name;

    public DmgType(DmgTypeEnum _dmgType, string _name)
    {
        dmgType = _dmgType;
        name = _name;
    }
}

public static class DmgTypes
{
    public static Dictionary<DmgTypeEnum, DmgType> dmgTypes;

    public static void InitializeDmgTypes()
    {
        dmgTypes = new Dictionary<DmgTypeEnum, DmgType>();

        dmgTypes.Add(DmgTypeEnum.Fire, new DmgType(DmgTypeEnum.Fire, "fire"));
        dmgTypes.Add(DmgTypeEnum.Physical, new DmgType(DmgTypeEnum.Physical, "physical"));
        dmgTypes.Add(DmgTypeEnum.Holy, new DmgType(DmgTypeEnum.Holy, "holy"));
        dmgTypes.Add(DmgTypeEnum.Vorpanite, new DmgType(DmgTypeEnum.Vorpanite, "vorpanite"));
        dmgTypes.Add(DmgTypeEnum.Mind, new DmgType(DmgTypeEnum.Mind, "mind"));
        dmgTypes.Add(DmgTypeEnum.Shadow, new DmgType(DmgTypeEnum.Shadow, "shadow"));
    }
}

public class MobType {

    public const float NORMAL_AP = 10;

    public MobTypeEnum id;
    public string name = "Mob Template";
    public GameObject prefab;
    public int maxHP = 1;
    public int maxFP = 1;
    public int regenFP = 10;
    public int regenHP = 1;
    public float maxAP = NORMAL_AP;
    public float moveSpeed = NORMAL_AP;

    public AbilityTypeEnum meleeAbil;
    public AbilityTypeEnum rangedAbil;
    public AbilityTypeEnum sprintAbil;
    public AbilityTypeEnum blockAbil;
    public AbilityTypeEnum dodgeAbil;

    public Dictionary<DmgTypeEnum, int> armorPR;
    public Dictionary<DmgTypeEnum, int> armorDR;

    public List<AiPackageEnum> aiPackages;

    public List<AbilityTypeEnum> abilities;
    public FactionEnum faction;

    public MobType(string _name, GameObject _prefab, int _HP, int _FP, int _regenHP, int _regenFP, float _moveSpeed)
    {
        name = _name;
        prefab = _prefab;
        maxHP = _HP;
        maxFP = _FP;
        regenHP = _regenHP;
        regenFP = _regenFP;
        moveSpeed = _moveSpeed;
    }
}

public enum MobTypeEnum
{
    mobAngel, mobHomunculus, mobCrimsonImp, mobShadowImp, mobFiend, mobCrimsonDemon, mobShadowDemon, mobArchdemon, mobArchdevil, mobScavenger, mobSplitSoul, mobArchangel
};

public class MobTypes
{
    public static GameObject mobAngel;
    public static GameObject mobHomunculus;
    public static GameObject mobCrimsonImp;
    public static GameObject mobShadowImp;
    public static GameObject mobFiend;
    public static GameObject mobCrimsonDemon;
    public static GameObject mobShadowDemon;
    public static GameObject mobArchdemon;
    public static GameObject mobArchdevil;
    public static GameObject mobScavenger;
    public static GameObject mobSplitSoul;
    public static GameObject mobArchangel;

    public static Dictionary<MobTypeEnum, MobType> mobTypes;

    public static void InitializeMobTypes()
    {
        mobAngel = Resources.Load("Prefabs/Mobs/Angel") as GameObject;
        mobHomunculus = Resources.Load("Prefabs/Mobs/Homunculus") as GameObject;
        mobCrimsonImp = Resources.Load("Prefabs/Mobs/Crimson Imp") as GameObject;
        mobShadowImp = Resources.Load("Prefabs/Mobs/Shadow Imp") as GameObject;
        mobFiend = Resources.Load("Prefabs/Mobs/Fiend") as GameObject;
        mobCrimsonDemon = Resources.Load("Prefabs/Mobs/Crimson Demon") as GameObject;
        mobShadowDemon = Resources.Load("Prefabs/Mobs/Shadow Demon") as GameObject;
        mobArchdemon = Resources.Load("Prefabs/Mobs/Archdemon") as GameObject;
        mobArchdevil = Resources.Load("Prefabs/Mobs/Archdevil") as GameObject;
        mobScavenger = Resources.Load("Prefabs/Mobs/Scavenger") as GameObject;
        mobSplitSoul = Resources.Load("Prefabs/Mobs/Split Soul") as GameObject;
        mobArchangel = Resources.Load("Prefabs/Mobs/Archangel") as GameObject;

        mobTypes = new Dictionary<MobTypeEnum, MobType>();

        Dictionary<DmgTypeEnum, int> armorDR;
        Dictionary<DmgTypeEnum, int> armorPR;

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Fire, 80);
        armorPR.Add(DmgTypeEnum.Holy, 100);
        Add(MobTypeEnum.mobAngel, "Angel", mobAngel, 100, 100, 1, 5, MobType.NORMAL_AP, FactionEnum.factionAngels,
            AbilityTypeEnum.abilHolySword, AbilityTypeEnum.abilLightBolt, AbilityTypeEnum.abilSprint, AbilityTypeEnum.abilDodge, AbilityTypeEnum.abilBlock,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilDivineVengeance});

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Fire, 80);
        armorPR.Add(DmgTypeEnum.Holy, 100);
        Add(MobTypeEnum.mobArchangel, "Archangel", mobArchangel, 100, 100, 1, 5, MobType.NORMAL_AP, FactionEnum.factionAngels,
            AbilityTypeEnum.abilHolySword, AbilityTypeEnum.abilLightBolt, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilFireAura });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Fire, 100);
        armorPR.Add(DmgTypeEnum.Holy, 100);
        armorPR.Add(DmgTypeEnum.Mind, 100);
        armorPR.Add(DmgTypeEnum.Physical, 100);
        Add(MobTypeEnum.mobSplitSoul, "Split Soul", mobSplitSoul, 10, 0, 0, 0, MobType.NORMAL_AP, FactionEnum.factionAngels,
            AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { },
            new List<AbilityTypeEnum>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Mind, 50);
        Add(MobTypeEnum.mobHomunculus, "Homunculus", mobHomunculus, 30, 30, 1, 5, MobType.NORMAL_AP, FactionEnum.factionBeasts,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy },
            new List<AbilityTypeEnum>());

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Mind, 50);
        Add(MobTypeEnum.mobFiend, "Fiend", mobFiend, 30, 60, 1, 7, MobType.NORMAL_AP, FactionEnum.factionBeasts,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilJump, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilCharge });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        Add(MobTypeEnum.mobCrimsonImp, "Crimson imp", mobCrimsonImp, 40, 30, 1, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilVorpaniteClaws, AbilityTypeEnum.abilShootSpikes, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>());

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Mind, 50);
        Add(MobTypeEnum.mobScavenger, "Scavenger", mobScavenger, 40, 60, 5, 5, MobType.NORMAL_AP, FactionEnum.factionBeasts,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility, AiPackageEnum.aiFindCorpse },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilCannibalize });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Physical, 20);
        Add(MobTypeEnum.mobCrimsonDemon, "Crimson demon", mobCrimsonDemon, 50, 60, 1, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilVorpaniteClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilRegenerate });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Physical, 20);
        Add(MobTypeEnum.mobArchdemon, "Archdemon", mobArchdemon, 70, 60, 2, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilVorpaniteClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilRegenerate, AbilityTypeEnum.abilNamed });
    }

    private static void Add(MobTypeEnum _id, string _name, GameObject _prefab, int _HP, int _FP, int _regenHP, int _regenFP, float _moveSpeed, 
        FactionEnum _faction,
        AbilityTypeEnum _meleeAbil, AbilityTypeEnum _rangedAbil, AbilityTypeEnum _sprintAbil, AbilityTypeEnum _dodgeAbil, AbilityTypeEnum _blockAbil,
        Dictionary<DmgTypeEnum, int> armorPR, Dictionary<DmgTypeEnum, int> armorDR,
        List<AiPackageEnum> _aiPackages, List<AbilityTypeEnum> _abilities)
    {
        MobType mt = new MobType(_name, _prefab, _HP, _FP, _regenHP, _regenFP, _moveSpeed)
        {
            id = _id,
            meleeAbil = _meleeAbil,
            rangedAbil = _rangedAbil,
            sprintAbil = _sprintAbil,
            blockAbil = _blockAbil,
            dodgeAbil = _dodgeAbil,
            aiPackages = _aiPackages,
            abilities = _abilities,
            faction = _faction
        };
        mobTypes.Add(_id, mt);

        if (!mt.abilities.Contains(mt.meleeAbil)) mt.abilities.Add(mt.meleeAbil);
        if (!mt.abilities.Contains(mt.rangedAbil)) mt.abilities.Add(mt.rangedAbil);
        if (!mt.abilities.Contains(mt.sprintAbil)) mt.abilities.Add(mt.sprintAbil);
        if (!mt.abilities.Contains(mt.blockAbil)) mt.abilities.Add(mt.blockAbil);
        if (!mt.abilities.Contains(mt.dodgeAbil)) mt.abilities.Add(mt.dodgeAbil);

        mt.armorDR = new Dictionary<DmgTypeEnum, int>();
        foreach (DmgType dmgType in DmgTypes.dmgTypes.Values)
        {
            mt.armorDR[dmgType.dmgType] = 0;
        }
        foreach (DmgTypeEnum dmgType in armorDR.Keys)
        {
            mt.armorDR[dmgType] = armorDR[dmgType];
        }

        mt.armorPR = new Dictionary<DmgTypeEnum, int>();
        foreach (DmgType dmgType in DmgTypes.dmgTypes.Values)
        {
            mt.armorPR[dmgType.dmgType] = 0;
        }
        foreach (DmgTypeEnum dmgType in armorPR.Keys)
        {
            mt.armorPR[dmgType] = armorPR[dmgType];
        }
    }
}