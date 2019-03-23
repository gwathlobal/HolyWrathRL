using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityClaws : Ability {

    public AbilityClaws()
    {
        id = AbilityTypeEnum.abilClaws;
        stdName = "Claws";
        spd = MobType.NORMAL_AP;
        cost = 0;
        passive = true;
        slot = AbilitySlotCategoty.abilMelee;
        category = AbilityPlayerCategory.abilMobs;
        doesMapCheck = false;
    }

    public override string Description(Mob mob)
    {
        return "Attack the enemy in melee for 6 Physical dmg.";
    }

    public override string Name(Mob mob)
    {
        return "Claws";
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
        int dmg = 0;
        dmg += Mob.InflictDamage(actor, target.mob, 6, DmgTypeEnum.Physical, null);
        actor.mo.MeleeAttack(target.mob.x - actor.x, target.mob.y - actor.y, dmg + " <i>DMG</i>",
            () =>
            {
                BoardManager.instance.CreateBlooddrop(target.mob.x, target.mob.y);
            });
        if (target.mob.CheckDead())
        {
            target.mob.MakeDead(actor, true, true, false);
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
