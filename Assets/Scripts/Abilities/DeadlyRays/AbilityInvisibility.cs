using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInvisibility : Ability
{
    public AbilityInvisibility()
    {
        id = AbilityTypeEnum.abilInvisibility;
        stdName = "Invisibility";
        spd = MobType.NORMAL_AP / 2;
        cost = 80;
        costType = AbilityCostType.wp;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilDeadlyRays;
        doesMapCheck = false;
    }

    public override string Description(Mob mob)
    {
        return "Render yourself invisible to enemies for 4 turns.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
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
        string str = String.Format("{0} invokes Invisibility. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.AddEffect(EffectTypeEnum.effectInvisibility, actor, 4);
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilJudgement) || mob.abilities.ContainsKey(AbilityTypeEnum.abilJudgement)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilAmbush) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAmbush)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilSweepAttack) || mob.abilities.ContainsKey(AbilityTypeEnum.abilSweepAttack)))
            return true;
        else return false;
    }
}
