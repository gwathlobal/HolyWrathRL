using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFists : Ability {

    public AbilityFists()
    {
        id = AbilityTypeEnum.abilFists;
        stdName = "Fists";
        passive = true;
        slot = AbilitySlotEnum.abilMelee;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Attack the enemy in melee for 2 physical dmg.";
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
                        { DmgTypeEnum.Physical, 2 }
                    },
                    null,
                    true);

                if (target.mob.CheckDead())
                {
                    target.mob.MakeDead(actor, true, true, false);
                }

                if (visibleEnd)
                {
                    string str = dmg + " <i>DMG</i>";
                    UIManager.instance.CreateFloatingText(str, end);
                }
            });
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
