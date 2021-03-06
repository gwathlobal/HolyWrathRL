﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DmgTypeEnum
{
    Physical, Fire, Holy, Vorpanite, Mind, Shadow, Acid 
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
        dmgTypes.Add(DmgTypeEnum.Acid, new DmgType(DmgTypeEnum.Acid, "acid"));
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
    public AbilityTypeEnum blockAbil;
    public AbilityTypeEnum dodgeAbil;

    public Dictionary<DmgTypeEnum, int> armorPR;
    public Dictionary<DmgTypeEnum, int> armorDR;

    public List<AiPackageEnum> aiPackages;

    public List<AbilityTypeEnum> abilities;
    public Dictionary<AbilityTypeEnum, int> probableAbilities;
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
    mobPCAngel, mobAngel, mobHomunculus, mobCrimsonImp, mobMachineImp, mobFiend, mobCrimsonDemon, mobMachineDemon, mobArchdemon, mobArchdevil, mobScavenger, mobSplitSoul, mobArchangel,
    mobSpearOfLight, mobTarDemon, mobDemonicPortal, mobSoldier, mobMachinegunman, mobScout, mobHuman
};

public class MobTypes
{
    public static GameObject mobAngel;
    public static GameObject mobHomunculus;
    public static GameObject mobCrimsonImp;
    public static GameObject mobMachineImp;
    public static GameObject mobFiend;
    public static GameObject mobCrimsonDemon;
    public static GameObject mobMachineDemon;
    public static GameObject mobArchdemon;
    public static GameObject mobArchdevil;
    public static GameObject mobScavenger;
    public static GameObject mobSplitSoul;
    public static GameObject mobArchangel;
    public static GameObject mobSpearOfLight;
    public static GameObject mobTarDemon;
    public static GameObject mobDemonicPortal;
    public static GameObject mobSoldier;
    public static GameObject mobHuman;
    public static GameObject mobMachinegunman;
    public static GameObject mobScout;

    public static Dictionary<MobTypeEnum, MobType> mobTypes;

    public static void InitializeMobTypes()
    {
        mobAngel = Resources.Load("Prefabs/Mobs/Angel") as GameObject;
        mobHomunculus = Resources.Load("Prefabs/Mobs/Homunculus") as GameObject;
        mobCrimsonImp = Resources.Load("Prefabs/Mobs/Crimson Imp") as GameObject;
        mobMachineImp = Resources.Load("Prefabs/Mobs/Machine Imp") as GameObject;
        mobFiend = Resources.Load("Prefabs/Mobs/Fiend") as GameObject;
        mobCrimsonDemon = Resources.Load("Prefabs/Mobs/Crimson Demon") as GameObject;
        mobMachineDemon = Resources.Load("Prefabs/Mobs/Machine Demon") as GameObject;
        mobArchdemon = Resources.Load("Prefabs/Mobs/Archdemon") as GameObject;
        mobArchdevil = Resources.Load("Prefabs/Mobs/Archdevil") as GameObject;
        mobScavenger = Resources.Load("Prefabs/Mobs/Scavenger") as GameObject;
        mobSplitSoul = Resources.Load("Prefabs/Mobs/Split Soul") as GameObject;
        mobArchangel = Resources.Load("Prefabs/Mobs/Archangel") as GameObject;
        mobSpearOfLight = Resources.Load("Prefabs/Mobs/Spear of Light") as GameObject;
        mobTarDemon = Resources.Load("Prefabs/Mobs/Tar Demon") as GameObject;
        mobDemonicPortal = Resources.Load("Prefabs/Mobs/Demonic Portal") as GameObject;
        mobSoldier = Resources.Load("Prefabs/Mobs/Soldier") as GameObject;
        mobHuman = Resources.Load("Prefabs/Mobs/Human") as GameObject;
        mobMachinegunman = Resources.Load("Prefabs/Mobs/Machinegunman") as GameObject;
        mobScout = Resources.Load("Prefabs/Mobs/Scout") as GameObject;

        mobTypes = new Dictionary<MobTypeEnum, MobType>();

        Dictionary<DmgTypeEnum, int> armorDR;
        Dictionary<DmgTypeEnum, int> armorPR;

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Fire, 80);
        armorPR.Add(DmgTypeEnum.Holy, 100);
        Add(MobTypeEnum.mobPCAngel, "Angel", mobAngel, 100, 100, 1, 5, MobType.NORMAL_AP, FactionEnum.factionAngels,
            AbilityTypeEnum.abilHolySword, AbilityTypeEnum.abilLightBolt, AbilityTypeEnum.abilDodge, AbilityTypeEnum.abilBlock,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilDivineVengeance, AbilityTypeEnum.abilAngel, AbilityTypeEnum.abilAbsorbMind },
            new Dictionary<AbilityTypeEnum, int>());

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Fire, 80);
        armorPR.Add(DmgTypeEnum.Holy, 100);
        Add(MobTypeEnum.mobAngel, "Angel", mobAngel, 100, 100, 1, 5, MobType.NORMAL_AP, FactionEnum.factionAngels,
            AbilityTypeEnum.abilHolySword, AbilityTypeEnum.abilLightBolt, AbilityTypeEnum.abilDodge, AbilityTypeEnum.abilBlock,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilAngel, AbilityTypeEnum.abilNamed },
            new Dictionary<AbilityTypeEnum, int>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Fire, 80);
        armorPR.Add(DmgTypeEnum.Holy, 100);
        Add(MobTypeEnum.mobArchangel, "Archangel", mobArchangel, 100, 100, 1, 5, MobType.NORMAL_AP, FactionEnum.factionAngels,
            AbilityTypeEnum.abilHolySword, AbilityTypeEnum.abilLightBolt, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilFireAura, AbilityTypeEnum.abilAngel },
            new Dictionary<AbilityTypeEnum, int>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Physical, 50);
        armorPR.Add(DmgTypeEnum.Mind, 100);
        armorPR.Add(DmgTypeEnum.Fire, 100);
        armorPR.Add(DmgTypeEnum.Holy, 100);
        Add(MobTypeEnum.mobSpearOfLight, "Spear of Light", mobSpearOfLight, 50, 100, 0, 5, MobType.NORMAL_AP, FactionEnum.factionAngels,
            AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { },
            new List<AbilityTypeEnum>() { },
            new Dictionary<AbilityTypeEnum, int>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Fire, 100);
        armorPR.Add(DmgTypeEnum.Holy, 100);
        armorPR.Add(DmgTypeEnum.Mind, 100);
        armorPR.Add(DmgTypeEnum.Physical, 100);
        Add(MobTypeEnum.mobSplitSoul, "Split Soul", mobSplitSoul, 10, 0, 0, 0, MobType.NORMAL_AP, FactionEnum.factionAngels,
            AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { },
            new List<AbilityTypeEnum>() { },
            new Dictionary<AbilityTypeEnum, int>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        Add(MobTypeEnum.mobHomunculus, "Homunculus", mobHomunculus, 30, 30, 1, 5, MobType.NORMAL_AP, FactionEnum.factionBeasts,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilMindless, AbilityTypeEnum.abilDemon },
            new Dictionary<AbilityTypeEnum, int>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        Add(MobTypeEnum.mobFiend, "Fiend", mobFiend, 25, 60, 1, 7, MobType.NORMAL_AP, FactionEnum.factionBeasts,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilJump, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilCharge, AbilityTypeEnum.abilMindless, AbilityTypeEnum.abilDemon },
            new Dictionary<AbilityTypeEnum, int>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        Add(MobTypeEnum.mobCrimsonImp, "Crimson imp", mobCrimsonImp, 40, 30, 2, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilVorpaniteClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilDemon },
            new Dictionary<AbilityTypeEnum, int>() {
                { AbilityTypeEnum.abilKnowledgeName, 15 },
                { AbilityTypeEnum.abilKnowledgeAbility, 15 },
                { AbilityTypeEnum.abilKnowledgeLocation, 15 }
            });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        Add(MobTypeEnum.mobScavenger, "Scavenger", mobScavenger, 40, 60, 2, 5, MobType.NORMAL_AP, FactionEnum.factionBeasts,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility, AiPackageEnum.aiFindCorpse },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilCannibalize, AbilityTypeEnum.abilMindless, AbilityTypeEnum.abilDemon },
            new Dictionary<AbilityTypeEnum, int>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        Add(MobTypeEnum.mobTarDemon, "Tar demon", mobTarDemon, 60, 60, 1, 5, MobType.NORMAL_AP * 2, FactionEnum.factionBeasts,
            AbilityTypeEnum.abilTarTentacles, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilMindless, AbilityTypeEnum.abilDemon },
            new Dictionary<AbilityTypeEnum, int>() { });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Physical, 20);
        Add(MobTypeEnum.mobCrimsonDemon, "Crimson demon", mobCrimsonDemon, 50, 60, 1, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilVorpaniteClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilRegenerate, AbilityTypeEnum.abilDemon },
            new Dictionary<AbilityTypeEnum, int>() {
                { AbilityTypeEnum.abilKnowledgeName, 15 },
                { AbilityTypeEnum.abilKnowledgeAbility, 15 },
                { AbilityTypeEnum.abilKnowledgeLocation, 15}
            });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Physical, 20);
        Add(MobTypeEnum.mobArchdemon, "Archdemon", mobArchdemon, 70, 70, 2, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilVorpaniteClaws, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilRegenerate, AbilityTypeEnum.abilCorpseExplosion, AbilityTypeEnum.abilDemon, AbilityTypeEnum.abilNamed },
            new Dictionary<AbilityTypeEnum, int>() {
                { AbilityTypeEnum.abilKnowledgeName, 15 },
                { AbilityTypeEnum.abilKnowledgeAbility, 15 },
                { AbilityTypeEnum.abilKnowledgeLocation, 15}
            });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        Add(MobTypeEnum.mobMachineImp, "Machine imp", mobMachineImp, 25, 30, 1, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilShootSpikes, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilTeleportOnHit, AbilityTypeEnum.abilDemon },
            new Dictionary<AbilityTypeEnum, int>() {
                { AbilityTypeEnum.abilKnowledgeName, 15 },
                { AbilityTypeEnum.abilKnowledgeAbility, 15 },
                { AbilityTypeEnum.abilKnowledgeLocation, 15}
            });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Physical, 20);
        Add(MobTypeEnum.mobMachineDemon, "Machine demon", mobMachineDemon, 40, 60, 1, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilShootSpikes, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilTeleportOnHit, AbilityTypeEnum.abilPowerWordImmobilize, AbilityTypeEnum.abilDemon },
            new Dictionary<AbilityTypeEnum, int>() {
                { AbilityTypeEnum.abilKnowledgeName, 15 },
                { AbilityTypeEnum.abilKnowledgeAbility, 15 },
                { AbilityTypeEnum.abilKnowledgeLocation, 15}
            });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Physical, 20);
        Add(MobTypeEnum.mobArchdevil, "Archdevil", mobArchdevil, 60, 70, 2, 5, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilClaws, AbilityTypeEnum.abilShootSpikes, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilTeleportOnHit, AbilityTypeEnum.abilPowerWordImmobilize, AbilityTypeEnum.abilDemonicPortal,
                AbilityTypeEnum.abilDemon, AbilityTypeEnum.abilNamed },
            new Dictionary<AbilityTypeEnum, int>() {
                { AbilityTypeEnum.abilKnowledgeName, 15 },
                { AbilityTypeEnum.abilKnowledgeAbility, 15 },
                { AbilityTypeEnum.abilKnowledgeLocation, 15}
            });

        armorDR = new Dictionary<DmgTypeEnum, int>();
        armorPR = new Dictionary<DmgTypeEnum, int>();
        armorPR.Add(DmgTypeEnum.Physical, 70);
        armorPR.Add(DmgTypeEnum.Mind, 100);
        armorPR.Add(DmgTypeEnum.Fire, 70);
        Add(MobTypeEnum.mobDemonicPortal, "Demonic Portal", mobDemonicPortal, 20, 10, 0, 1, MobType.NORMAL_AP, FactionEnum.factionDemons,
            AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            armorPR, armorDR,
            new List<AiPackageEnum>() { AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilSummonImp },
            new Dictionary<AbilityTypeEnum, int>() {
                { AbilityTypeEnum.abilKnowledgeName, 15 },
                { AbilityTypeEnum.abilKnowledgeAbility, 15 },
                { AbilityTypeEnum.abilKnowledgeLocation, 15}
            });

        Add(MobTypeEnum.mobSoldier, "Soldier", mobSoldier, 25, 30, 0, 5, MobType.NORMAL_AP, FactionEnum.factionSoldiers,
            AbilityTypeEnum.abilKnife, AbilityTypeEnum.abilShootRifle, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Holy, 100 }
            }, 
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Physical, 1 }
            },
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilMedkit },
            new Dictionary<AbilityTypeEnum, int>() { });

        Add(MobTypeEnum.mobMachinegunman, "Machinegunman", mobMachinegunman, 25, 30, 0, 5, MobType.NORMAL_AP, FactionEnum.factionSoldiers,
            AbilityTypeEnum.abilKnife, AbilityTypeEnum.abilShootMachinegun, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Holy, 100 }
            },
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Physical, 1 }
            },
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilMedkit },
            new Dictionary<AbilityTypeEnum, int>() { });

        Add(MobTypeEnum.mobScout, "Scout", mobScout, 25, 40, 0, 5, MobType.NORMAL_AP * 0.75f, FactionEnum.factionSoldiers,
            AbilityTypeEnum.abilKnife, AbilityTypeEnum.abilShootSniperRifle, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Holy, 100 }
            },
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Physical, 1 }
            },
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiMeleeEnemy, AiPackageEnum.aiUseAbility },
            new List<AbilityTypeEnum>() { AbilityTypeEnum.abilMedkit, AbilityTypeEnum.abilCallArtillery },
            new Dictionary<AbilityTypeEnum, int>() { });

        Add(MobTypeEnum.mobHuman, "Human", mobHuman, 25, 25, 0, 5, MobType.NORMAL_AP, FactionEnum.factionHumans,
            AbilityTypeEnum.abilFists, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone, AbilityTypeEnum.abilNone,
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Holy, 100 }
            },
            new Dictionary<DmgTypeEnum, int>(),
            new List<AiPackageEnum>() { AiPackageEnum.aiFindRandomLocation, AiPackageEnum.aiFleeEnemy },
            new List<AbilityTypeEnum>(),
            new Dictionary<AbilityTypeEnum, int>() { });
    }

    private static void Add(MobTypeEnum _id, string _name, GameObject _prefab, int _HP, int _FP, int _regenHP, int _regenFP, float _moveSpeed, 
        FactionEnum _faction,
        AbilityTypeEnum _meleeAbil, AbilityTypeEnum _rangedAbil, AbilityTypeEnum _dodgeAbil, AbilityTypeEnum _blockAbil,
        Dictionary<DmgTypeEnum, int> armorPR, Dictionary<DmgTypeEnum, int> armorDR,
        List<AiPackageEnum> _aiPackages, List<AbilityTypeEnum> _abilities, Dictionary<AbilityTypeEnum, int> _probableAbilities)
    {
        MobType mt = new MobType(_name, _prefab, _HP, _FP, _regenHP, _regenFP, _moveSpeed)
        {
            id = _id,
            meleeAbil = _meleeAbil,
            rangedAbil = _rangedAbil,
            blockAbil = _blockAbil,
            dodgeAbil = _dodgeAbil,
            aiPackages = _aiPackages,
            abilities = _abilities,
            faction = _faction,
            probableAbilities = _probableAbilities
        };
        mobTypes.Add(_id, mt);

        if (!mt.abilities.Contains(mt.meleeAbil)) mt.abilities.Add(mt.meleeAbil);
        if (!mt.abilities.Contains(mt.rangedAbil)) mt.abilities.Add(mt.rangedAbil);
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

    public static void UpgradeAngel(Mob angel)
    {
        List<Ability> availAbilities = new List<Ability>();
        foreach (Ability ability in AbilityTypes.abilTypes.Values)
        {
            if (ability.CheckRequirements(angel, new List<AbilityTypeEnum>()) && 
                !angel.abilities.ContainsKey(ability.id))
                availAbilities.Add(ability);
        }
        if (availAbilities.Count > 0) {
            int r = Random.Range(0, availAbilities.Count);
            angel.abilities.Add(availAbilities[r].id, true);

            if (availAbilities[r].slot == AbilitySlotEnum.abilMelee) angel.meleeAbil = availAbilities[r].id;
            if (availAbilities[r].slot == AbilitySlotEnum.abilRanged) angel.rangedAbil = availAbilities[r].id;
            if (availAbilities[r].slot == AbilitySlotEnum.abilDodge) angel.dodgeAbil = availAbilities[r].id;
            if (availAbilities[r].slot == AbilitySlotEnum.abilBlock) angel.blockAbil = availAbilities[r].id;
        }
    }
}