using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct TargetStruct {
    public Vector2Int loc;
    public Mob mob;

    public TargetStruct(Vector2Int _loc, Mob _mob)
    {
        loc = _loc;
        mob = _mob;
    }
}

public enum AbilityPlayerCategory
{
    abilMobs, abilCommon, abilDeadlyRays, abilFieryRage, abilBrilliantMind, abilBastionHoly
}

public enum AbilitySlotCategoty
{
    abilNormal, abilSprint, abilDodge, abilBlock, abilMelee, abilRanged, abilNone
}

public enum AbilityCostType
{
    fp, wp
}

public enum AbilityTypeEnum
{
    abilNone,
    abilVorpaniteClaws, abilHolySword, abilClaws,
    abilShootSpikes, abilLightBolt,
    abilSprint,
    abilBlock,
    abilDodge, abilJump,
    abilDivineVengeance,
    abilHealSelf, 
    abilJudgement, abilAmbush, abilSweepAttack, abilInvisibility, abilBlindness, abilBurdenOfSins, abilSyphonLight,
    abilFireFists, abilFlamingArrow, abilFireAura, abilBreathOfFire, abilIncineration, abilWarmingLight, abilLeapOfStrength,
    abilMindBurn, abilFear, abilMeditate, abilDominateMind, abilTrapMind, abilSplitSoul, abilSphereOfSilence,
    abilCharge,  abilCannibalize, abilRegenerate, abilNamed
}


public abstract class Ability {
    public AbilityTypeEnum id;
    public string stdName;
    public abstract string Name(Mob mob);
    public abstract string Description(Mob mob);
    public abstract float Spd(Mob mob);
    public bool passive;
    public abstract int Cost(Mob mob);
    public AbilityCostType costType = AbilityCostType.fp;

    public AbilitySlotCategoty slot;
    public AbilityPlayerCategory category;

    public abstract bool AbilityCheckApplic(Ability ability, Mob mob);
    public abstract void AbilityInvoke(Mob actor, TargetStruct target);
    public abstract bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly);
    public abstract void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly);
    public abstract bool DoesMapCheck(Mob mob);
    public abstract bool AbilityMapCheck(Ability ability);
    public abstract bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils);

    public string GetFullDescription(Mob mob)
    {
        string result = "";

        float _spd = Spd(mob) / MobType.NORMAL_AP;

        string turnStr = (_spd > 1) ? " turns. " : " turn. ";
        turnStr = (_spd != 0) ? _spd + turnStr : "";

        string _ultimate = (costType == AbilityCostType.wp) ? "Wrath Ability. " : "";

        string _costType;
        switch (costType)
        {
            case AbilityCostType.wp:
                _costType = "WP";
                break;
            default:
                _costType = "FP";
                break;
        }

        string _cost = (Cost(mob) != 0) ? String.Format("{0} {1}, ", Cost(mob), _costType) : "";

        string _passive = (passive == true) ? "Passive. " : ""; 

        result = String.Format("{0}\n{1}{2}{3}{4}\n{5}",
            stdName,
            _ultimate,
            _cost,
            turnStr,
            _passive,
            Description(mob));

        return result;
    }

    public bool IsApplicByCost(Mob actor)
    {
        switch (costType)
        {
            case AbilityCostType.wp:
                if (actor.curWP >= Cost(actor)) return true;
                else return false;
            default:
                if (actor.curFP >= Cost(actor)) return true;
                else return false;
        }
        
    }

}

public class AbilityTypes
{
    public static Dictionary<AbilityTypeEnum, Ability> abilTypes;

    public static void InitializeAbilTypes()
    {

        abilTypes = new Dictionary<AbilityTypeEnum, Ability>();

        Add(new AbilityNone());

        //============================
        //
        // MELEE ABILITIES
        // 
        //============================

        Add(new AbilityClaws());

        Add(new AbilityHolySword());

        Add(new AbilityVorpaniteClaws());

        //============================
        //
        // RANGED ABILITIES
        // 
        //============================

        Add(new AbilityLightBolt());

        Add(new AbilityShootSpikes());

        //============================
        //
        // DODGE ABILITIES
        // 
        //============================

        Add(new AbilityDodge());

        Add(new AbilityJump());

        //============================
        //
        // BLOCK ABILITIES
        // 
        //============================

        Add(new AbilityBlock());

        //============================
        //
        // SPRINT ABILITIES
        // 
        //============================

        Add(new AbilitySprint());



        //============================
        //
        // MOB ABILITIES
        // 
        //============================

        Add(new AbilityCharge());

        Add(new AbilityCannibalize());

        Add(new AbilityRegenerate());

        Add(new AbilityNamed());

        //============================
        //
        // COMMON ABILITIES
        // 
        //============================

        Add(new AbilityDivineVengeance());

        //============================
        //
        // PLAYER ABILITIES
        // 
        //============================

        Add(new AbilityHealSelf());

        //============================
        //
        // DEADLY RAYS ABILITIES
        // 
        //============================

        Add(new AbilityJudgement());

        Add(new AbilityAmbush());

        Add(new AbilitySweepAttack());

        Add(new AbilityInvisibility());

        Add(new AbilityBlindness());

        Add(new AbilityBurdenOfSins());

        Add(new AbilitySyphonLight());

        //============================
        //
        // FIERY RAGE ABILITIES
        // 
        //============================

        Add(new AbilityFireFists());

        Add(new AbilityFlamingArrow());

        Add(new AbilityFireAura());

        Add(new AbilityBreathOfFire());

        Add(new AbilityIncineration());

        Add(new AbilityWarmingLight());

        Add(new AbilityLeapOfStrength());

        //============================
        //
        // FIERY RAGE ABILITIES
        // 
        //============================

        Add(new AbilityMindBurn());

        Add(new AbilityFear());

        Add(new AbilityMeditate());

        Add(new AbilityDominateMind());

        Add(new AbilityTrapMind());

        Add(new AbilitySplitSoul());

        Add(new AbilitySphereOfSilence());
    }

    private static void Add(Ability ability)
    {
        abilTypes.Add(ability.id, ability);
    }
}