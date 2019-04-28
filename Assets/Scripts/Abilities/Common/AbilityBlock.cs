using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBlock : Ability
{
    public AbilityBlock()
    {
        id = AbilityTypeEnum.abilBlock;
        stdName = "Block";
        passive = false;
        slot = AbilitySlotCategoty.abilBlock;
        category = AbilityPlayerCategory.abilCommon;
    }

    public override string Description(Mob mob)
    {
        string str = "Blocking increases the %DR by 50. While blocking FP regeneration is set to zero.";
        if (mob.GetEffect(EffectTypeEnum.effectBlock) == null) return "Start blocking. " + str;
        else return "Stop blocking. " + str;
    }

    public override string Name(Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectBlock) == null) return "Block";
        else return "Stop block";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectBlock) == null) return 10;
        else return 0;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return false;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) &&
                    ((nearestEnemy != null && actor.GetEffect(EffectTypeEnum.effectBlock) == null && ((double)actor.curFP / actor.maxFP >= 0.5) &&
                    Level.GetDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) < 2)
                    ||
                    (nearestEnemy == null && actor.GetEffect(EffectTypeEnum.effectBlock) != null)))
            return true;
        else return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        if (actor.GetEffect(EffectTypeEnum.effectBlock) == null)
        {
            string str = String.Format("{0} starts to block. ", actor.name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

            actor.AddEffect(EffectTypeEnum.effectBlock, actor, Effect.CD_UNLIMITED);
        }
        else
        {
            string str = String.Format("{0} stops blocking. ", actor.name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

            actor.RemoveEffect(EffectTypeEnum.effectBlock);
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
        return true;
    }

}
