using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate string OnHitProjectileMob(Mob attacker, Mob target);
public delegate void PostProjectileMobFunc(Mob attacker, Mob target);
public delegate string OnHitProjectileItem(Mob attacker, Item target);
public delegate void PostProjectileItemFunc(Mob attacker, Item target);


public struct TargetStruct {
    public Vector2Int loc;
    public Mob mob;

    public TargetStruct(Vector2Int _loc, Mob _mob)
    {
        loc = _loc;
        mob = _mob;
    }
}

public enum AbilitySlotEnum
{
    abilNormal, abilDodge, abilBlock, abilMelee, abilRanged, abilNone
}

public enum AbilityCostType
{
    fp, wp
}

public enum AbilityTypeEnum
{
    abilNone,
    abilVorpaniteClaws, abilHolySword, abilClaws, abilTarTentacles, abilFists, abilKnife,
    abilShootSpikes, abilLightBolt, abilShootRifle, abilShootSniperRifle, abilShootMachinegun,
    abilSprint,
    abilBlock,
    abilDodge, abilJump,
    abilDivineVengeance,
    abilHealSelf, 
    abilJudgement, abilAmbush, abilSweepAttack, abilInvisibility, abilBlindness, abilBurdenOfSins, abilSyphonLight,
    abilFireFists, abilFlamingArrow, abilFireAura, abilBreathOfFire, abilIncineration, abilWarmingLight, abilLeapOfStrength,
    abilMindBurn, abilFear, abilMeditate, abilDominateMind, abilTrapMind, abilSplitSoul, abilSphereOfSilence,
    abilAbsorbingShield, abilForceShot, abilReflectiveBlock, abilCallArchangel, abilHolyRune, abilPurgeRitual, abilSpearOfLight,
    abilCharge,  abilCannibalize, abilRegenerate, abilNamed, abilTeleportOnHit, abilCorpseExplosion, abilPowerWordImmobilize, abilDemonicPortal, abilSummonImp,
    abilMedkit, abilCallArtillery, abilMindless, abilAngel, abilDemon
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

    public AbilitySlotEnum slot;
    public AbilityPlayerCategoryEnum category;

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

        string _slot;
        switch (slot)
        {
            case AbilitySlotEnum.abilBlock:
                _slot = "Block Ability. ";
                break;
            case AbilitySlotEnum.abilDodge:
                _slot = "Dodge Ability. ";
                break;
            case AbilitySlotEnum.abilMelee:
                _slot = "Melee Ability. ";
                break;
            case AbilitySlotEnum.abilRanged:
                _slot = "Ranged Ability. ";
                break;
            default:
                _slot = "";
                break;
        }

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

        result = String.Format("{0}\n{1}{2}{3}{4}{5}\n{6}",
            stdName,
            _slot,
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

    public static void ShootProjectile(Mob actor, Mob target, Color32 color, OnHitProjectileMob onHitProjectile, PostProjectileMobFunc postProjectileFunc)
    {
        string str;

        Level level = BoardManager.instance.level;
        bool visibleStart = level.visible[actor.x, actor.y];
        bool visibleEnd = level.visible[target.x, target.y];

        Vector2 start = actor.go.transform.position;
        Vector2 end = target.go.transform.position;

        Vector2Int targetPos = new Vector2Int(target.x, target.y);

        bool reflect = false;
        foreach (Effect effect in target.effects.Values)
        {
            if (EffectTypes.effectTypes[effect.idType].reflectsProjectiles)
            {
                reflect = true;
                break;
            }
        }

        GameObject projectile = GameObject.Instantiate(UIManager.instance.projectilePrefab, new Vector3(actor.x, actor.y, 0), Quaternion.identity);
        projectile.GetComponent<SpriteRenderer>().color = color;

        BoardEventController.instance.AddEvent(new BoardEventController.Event(projectile,
            () =>
            {
                projectile.GetComponent<MovingObject>().Move(end, (visibleStart || visibleEnd),
                    () =>
                    {
                        return;
                    });
                BoardEventController.instance.RemoveFinishedEvent();
            }));

        BoardEventController.instance.AddEvent(new BoardEventController.Event(projectile,
            () =>
            {
                GameObject.Destroy(projectile);
                BoardEventController.instance.RemoveFinishedEvent();
            }));

        BoardEventController.instance.AddEvent(new BoardEventController.Event(actor.go,
            () =>
            {
                if (!reflect)
                {
                    str = onHitProjectile(actor, target);
                }
                else
                    str = "<i>REFLECT</i>";

                if (visibleEnd)
                {
                    UIManager.instance.CreateFloatingText(str, end);
                }

                if (!reflect)
                {
                    if (postProjectileFunc != null) postProjectileFunc(actor, target);
                }

                if (target.CheckDead())
                {
                    target.MakeDead(actor, true, true, false);
                }
                BoardEventController.instance.RemoveFinishedEvent();
            }));

        if (reflect)
        {
            str = String.Format("{0} reflects the projectile. ", target.name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(target.x, target.y, str);

            GameObject projectile2 = null;
            // fake object to be used in BoardEvents, if projectile game objects are used - the second, reflected projectile is seen immediately (not after the first projectile reaches its target)
            GameObject fakeProj2 = new GameObject();
            BoardEventController.instance.AddEvent(new BoardEventController.Event(fakeProj2,
                () =>
                {
                    projectile2 = GameObject.Instantiate(UIManager.instance.projectilePrefab, new Vector3(targetPos.x, targetPos.y, 0), Quaternion.identity);
                    projectile2.GetComponent<SpriteRenderer>().color = color;
                    projectile2.GetComponent<MovingObject>().Move(start, (visibleStart || visibleEnd),
                        () =>
                        {
                            return;
                        });
                    BoardEventController.instance.RemoveFinishedEvent();
                }));

            BoardEventController.instance.AddEvent(new BoardEventController.Event(fakeProj2,
                () =>
                {
                    GameObject.Destroy(projectile2);
                    GameObject.Destroy(fakeProj2);
                    BoardEventController.instance.RemoveFinishedEvent();
                }));

            BoardEventController.instance.AddEvent(new BoardEventController.Event(actor.go,
            () =>
            {
                str = onHitProjectile(target, actor);

                if (visibleEnd)
                {
                    UIManager.instance.CreateFloatingText(str, start);
                }

                if (postProjectileFunc != null) postProjectileFunc(target, actor);

                if (actor.CheckDead())
                {
                    actor.MakeDead(target, true, true, false);
                }

                if (target.GetEffect(EffectTypeEnum.effectReflectiveBlocking) != null)
                {
                    Effect eff = target.GetEffect(EffectTypeEnum.effectReflectiveBlocking);
                    eff.param1--;

                    if (eff.param1 <= 0)
                    {
                        target.RemoveEffect(EffectTypeEnum.effectReflectiveBlocking);
                    }
                }
                BoardEventController.instance.RemoveFinishedEvent();
            }));
        }
    }

    public static void ShootProjectile(Mob actor, Item target, Color32 color, OnHitProjectileItem onHitProjectile, PostProjectileItemFunc postProjectileFunc)
    {
        Level level = BoardManager.instance.level;
        bool visibleStart = level.visible[actor.x, actor.y];
        bool visibleEnd = level.visible[target.x, target.y];

        Vector2 start = actor.go.transform.position;
        Vector2 end = target.go.transform.position;

        Vector2Int targetPos = new Vector2Int(target.x, target.y);

        GameObject projectile = GameObject.Instantiate(UIManager.instance.projectilePrefab, new Vector3(actor.x, actor.y, 0), Quaternion.identity);
        projectile.GetComponent<SpriteRenderer>().color = color;

        BoardEventController.instance.AddEvent(new BoardEventController.Event(projectile,
            () =>
            {
                projectile.GetComponent<MovingObject>().Move(end, (visibleStart || visibleEnd),
                    () =>
                    {
                        return;
                    });
                BoardEventController.instance.RemoveFinishedEvent();
            }));

        BoardEventController.instance.AddEvent(new BoardEventController.Event(projectile,
            () =>
            {
                GameObject.Destroy(projectile);
                BoardEventController.instance.RemoveFinishedEvent();
            }));

        BoardEventController.instance.AddEvent(new BoardEventController.Event(actor.go,
            () =>
            {
                if (onHitProjectile != null) onHitProjectile(actor, target);
                if (postProjectileFunc != null) postProjectileFunc(actor, target);
                BoardEventController.instance.RemoveFinishedEvent();
            }));

        
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

        Add(new AbilityTarTentacles());

        Add(new AbilityFists());

        Add(new AbilityKnife());

        //============================
        //
        // RANGED ABILITIES
        // 
        //============================

        Add(new AbilityLightBolt());

        Add(new AbilityShootSpikes());

        Add(new AbilityShootRifle());

        Add(new AbilityShootSniperRifle());

        Add(new AbilityShootMachinegun());

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

        Add(new AbilityTeleportOnHit());

        Add(new AbilityCorpseExplosion());

        Add(new AbilityPowerWordImmobilize());

        Add(new AbilityDemonicPortal());

        Add(new AbilitySummonImp());

        Add(new AbilityMedkit());

        Add(new AbilityCallArtillery());

        Add(new AbilityMindless());

        Add(new AbilityDemon());

        //============================
        //
        // COMMON ABILITIES
        // 
        //============================

        Add(new AbilityDivineVengeance());

        Add(new AbilityAngel());

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

        //============================
        //
        // BASTION OF HOLINESS ABILITIES
        // 
        //============================

        Add(new AbilityAbsorbingShield());

        Add(new AbilityForceShot());

        Add(new AbilityReflectiveBlock());

        Add(new AbilityCallArchangel());

        Add(new AbilityHolyRune());

        Add(new AbilityPurgeRitual());

        Add(new AbilitySpearOfLight());
    }

    private static void Add(Ability ability)
    {
        abilTypes.Add(ability.id, ability);
    }
}