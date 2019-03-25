using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMeditate : Ability
{
    public AbilityMeditate()
    {
        id = AbilityTypeEnum.abilMeditate;
        stdName = "Meditate";
        spd = MobType.NORMAL_AP;
        cost = 0;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBrilliantMind;
        doesMapCheck = false;
    }

    public override string Description(Mob mob)
    {
        return "Meditate to regenerate double amount of HP and FP for 1 turn.";
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
        string str = String.Format("{0} starts to meditate. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.AddEffect(EffectTypeEnum.effectMeditate, actor, 1);
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilMindBurn) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMindBurn)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFear) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFear)))
            return true;
        else return false;
    }
}
