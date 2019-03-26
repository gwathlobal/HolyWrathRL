using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySyphonLight : Ability
{
    public AbilitySyphonLight()
    {
        id = AbilityTypeEnum.abilSyphonLight;
        stdName = "Syphon Light";
        costType = AbilityCostType.wp;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilDeadlyRays;
    }

    public override string Description(Mob mob)
    {
        return "Places the Syphon Light debuff on all visible units. When a unit with Syphon Light is inflicted damage, half of the amount is transferred to you as health. Syphon Light lasts for 5 turns.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        return 10;
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
        string str = String.Format("{0} invokes Syphon Light. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
        foreach (Mob mob in actor.visibleMobs)
        {
            if (mob != actor)
            {
                str = String.Format("{0} is affected. ", mob.name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(mob.x, mob.y, str);
                mob.AddEffect(EffectTypeEnum.effectSyphonLight, actor, 5);
            }
        }
        

        
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        TargetStruct target = new TargetStruct(new Vector2Int(nearestEnemy.x, nearestEnemy.y), nearestEnemy);
        actor.InvokeAbility(ability, target);
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        if ((addedAbils.Contains(AbilityTypeEnum.abilJudgement) || mob.abilities.ContainsKey(AbilityTypeEnum.abilJudgement)) && 
            (addedAbils.Contains(AbilityTypeEnum.abilAmbush) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAmbush)) && 
            (addedAbils.Contains(AbilityTypeEnum.abilSweepAttack) || mob.abilities.ContainsKey(AbilityTypeEnum.abilSweepAttack)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilInvisibility) || mob.abilities.ContainsKey(AbilityTypeEnum.abilInvisibility)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilBlindness) || mob.abilities.ContainsKey(AbilityTypeEnum.abilBlindness)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilBurdenOfSins) || mob.abilities.ContainsKey(AbilityTypeEnum.abilBurdenOfSins)))
            return true;
        else return false;
    }
}
