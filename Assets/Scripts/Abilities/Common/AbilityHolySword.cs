﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolySword : Ability
{
    public AbilityHolySword()
    {
        id = AbilityTypeEnum.abilHolySword;
        stdName = "Holy Sword";
        passive = true;
        slot = AbilitySlotEnum.abilMelee;
        category = AbilityPlayerCategoryEnum.abilCommon;
    }

    public override string Description(Mob mob)
    {
        return "Attack the enemy in melee for 5 physical dmg and 5 holy dmg. Can sever limbs on target dearh.";
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
        return true;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        int dmg = 0;
        Level level = BoardManager.instance.level;
        bool visibleStart = level.visible[actor.x, actor.y];
        bool visibleEnd = level.visible[target.mob.x, target.mob.y];

        Vector3 start = actor.go.transform.position;
        Vector3 end = target.mob.go.transform.position;

        actor.mo.MeleeAttack(start, end, (visibleStart || visibleEnd),
            () =>
            {
                dmg += Mob.InflictDamage(actor, target.mob,
                    new Dictionary<DmgTypeEnum, int>()
                    {
                        { DmgTypeEnum.Physical, 5 },
                        { DmgTypeEnum.Holy, 5 }
                    },
                    null,
                    true);

                if (target.mob.CheckDead())
                {
                    target.mob.MakeDead(actor, true, true, true, "");
                }

                if (visibleEnd)
                {
                    string str = dmg + " <i>DMG</i>";
                    UIManager.instance.CreateFloatingText(str, end);
                }
            });
    }

    private void Melee(Mob actor, TargetStruct target)
    {
        
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
