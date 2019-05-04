using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityReflectiveBlock : Ability
{
    public AbilityReflectiveBlock()
    {
        id = AbilityTypeEnum.abilReflectiveBlock;
        stdName = "Reflective Block";
        passive = false;
        slot = AbilitySlotCategoty.abilBlock;
        category = AbilityPlayerCategory.abilBastionHoly;
    }

    public override string Description(Mob mob)
    {
        string str = "Blocking increases the %DR by 50. While blocking FP regeneration is set to zero. Reflective blocking returns all projectiles to the originator. Each projectile reduces your current FP by 10.";
        if (mob.GetEffect(EffectTypeEnum.effectReflectiveBlocking) == null) return "Start blocking. " + str;
        else return "Stop blocking. " + str;
    }

    public override string Name(Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectReflectiveBlocking) == null) return stdName;
        else return "Stop block";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectReflectiveBlocking) == null) return 10;
        else return 0;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return false;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        if (actor.GetEffect(EffectTypeEnum.effectReflectiveBlocking) == null)
        {
            string str = String.Format("{0} starts to block and reflect projectiles. ", actor.name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

            actor.AddEffect(EffectTypeEnum.effectReflectiveBlocking, actor, Effect.CD_UNLIMITED);
        }
        else
        {
            actor.RemoveEffect(EffectTypeEnum.effectReflectiveBlocking);
        }
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        TargetStruct target = new TargetStruct(new Vector2Int(actor.x, actor.y), actor);
        actor.InvokeAbility(ability, target);
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        if ((addedAbils.Contains(AbilityTypeEnum.abilAbsorbingShield) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAbsorbingShield)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilForceShot) || mob.abilities.ContainsKey(AbilityTypeEnum.abilForceShot)))
            return true;
        else return false;
    }

}
