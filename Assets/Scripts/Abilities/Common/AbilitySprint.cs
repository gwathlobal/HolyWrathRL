using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySprint : Ability
{
    public AbilitySprint()
    {
        id = AbilityTypeEnum.abilSprint;
        stdName = "Sprint";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Start to sprint making you move x1.5 faster. If you are already sprinting, stop it. Sprinting costs 20 FP each move. While sprinting your FP regeneration is set to zero. You cannot start sprinting if you have less than 20 FP.";
    }

    public override string Name(Mob mob)
    {
        return "Sprint";
    }

    public override float Spd(Mob mob)
    {
        return 0;
    }

    public override int Cost(Mob mob)
    {
        return 0;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return false;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && actor.GetEffect(EffectTypeEnum.effectSprint) == null &&
                    Level.GetDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) > 5)
            return true;
        else return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectSprint) == null)
            if (mob.curFP >= 20) return true;
            else return false;
        else return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        if (actor.GetEffect(EffectTypeEnum.effectSprint) == null)
        {
            string str = String.Format("{0} starts sprinting. ", actor.name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

            actor.AddEffect(EffectTypeEnum.effectSprint, actor, Effect.CD_UNLIMITED);
        }
        else
        {
            string str = String.Format("{0} stops sprinting. ", actor.name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

            actor.RemoveEffect(EffectTypeEnum.effectSprint);
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
