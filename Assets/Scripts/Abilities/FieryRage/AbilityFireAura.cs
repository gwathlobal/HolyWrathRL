﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFireAura : Ability
{
    public AbilityFireAura()
    {
        id = AbilityTypeEnum.abilFireAura;
        stdName = "Fire Aura";
        spd = MobType.NORMAL_AP;
        cost = 30;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilFieryRage;
        doesMapCheck = false;
    }

    public override string Description(Mob mob)
    {
        return "Create a fire aura that damages all enemies around your for 10 Fire dmg and applies the Burning effect to them. Burning deals 3 Fire dmg for 5 turns. The fire aura lasts for 5 turns.";
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
        string str = String.Format("{0} invokes a fire aura. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.AddEffect(EffectTypeEnum.effectFireAura, actor, 5);

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
        if ((addedAbils.Contains(AbilityTypeEnum.abilFireFists) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireFists)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFlamingArrow) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFlamingArrow)))
            return true;
        else return false;
    }
}
