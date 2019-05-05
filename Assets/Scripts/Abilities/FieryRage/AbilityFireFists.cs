using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFireFists : Ability
{
    public AbilityFireFists()
    {
        id = AbilityTypeEnum.abilFireFists;
        stdName = "Fire Fists";
        passive = true;
        slot = AbilitySlotCategoty.abilMelee;
        category = AbilityPlayerCategory.abilFieryRage;
    }

    public override string Description(Mob mob)
    {
        return "Attack the enemy in melee for 5 Physical dmg and 5 Fire dmg. Places a Burning effect on the target that deals 3 Fire dmg for 5 turns.";
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
        return 0;
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
        int dmg = 0;
        dmg += Mob.InflictDamage(actor, target.mob,
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Physical, 5 },
                { DmgTypeEnum.Fire, 5 }
            }, 
            null);

        target.mob.AddEffect(EffectTypeEnum.effectBurning, actor, 5);
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
