using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNamed : Ability
{

    public AbilityNamed()
    {
        id = AbilityTypeEnum.abilNamed;
        stdName = "Named";
        passive = true;
        slot = AbilitySlotEnum.abilNone;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "This is a prominent creature worth having a name.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
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
        return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        Nemesis nemesis = null;
        foreach (Nemesis nem in GameManager.instance.nemeses)
        {
            if (nem.mob == actor)
            {
                nemesis = nem;
                break;
            }
        }

        if (nemesis.personalStatus == Nemesis.PersonalStatusEnum.hidden)
        {
            nemesis.personalStatus = Nemesis.PersonalStatusEnum.revealedName;
            if (BoardManager.instance.player.faction == actor.faction)
            {
                nemesis.personalStatus = Nemesis.PersonalStatusEnum.revealedAbils;
            }
        }

        if (actor.CheckDead())
        {
            nemesis.deathStatus = Nemesis.DeathStatusEnum.deceased;
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
