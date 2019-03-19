﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFlamingSword : Ability
{
    public AbilityFlamingSword()
    {
        id = AbilityTypeEnum.abilFlamingSword;
        stdName = "Flaming sword";
        spd = MobType.NORMAL_AP;
        cost = 0;
        passive = true;
        slot = AbilitySlotCategoty.abilMelee;
        category = AbilityPlayerCategory.abilCommon;
        doesMapCheck = false;
    }

    public override string Description(Mob mob)
    {
        return "Attack the enemy in melee for 5 Physical dmg and 5 Fire dmg. Can sever limbs on target dearh.";
    }

    public override string Name(Mob mob)
    {
        return "Flaming sword";
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        return true;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        int dmg = 0;
        dmg += Mob.InflictDamage(actor, target.mob, 5, DmgTypeEnum.Physical);
        dmg += Mob.InflictDamage(actor, target.mob, 5, DmgTypeEnum.Fire);
        actor.mo.MeleeAttack(target.mob.x - actor.x, target.mob.y - actor.y, dmg + " <i>DMG</i>",
            () =>
            {
                BoardManager.instance.CreateBlooddrop(target.mob.x, target.mob.y);
            });
        if (target.mob.CheckDead())
        {
            target.mob.MakeDead(actor, true, true, true);
        }
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        
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
