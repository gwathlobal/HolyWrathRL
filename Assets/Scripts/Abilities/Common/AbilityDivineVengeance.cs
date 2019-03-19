using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDivineVengeance : Ability
{
    public AbilityDivineVengeance()
    {
        id = AbilityTypeEnum.abilDivineVengeance;
        stdName = "Divine Vengeance";
        spd = 0;
        cost = 0;
        passive = true;
        slot = AbilitySlotCategoty.abilNone;
        category = AbilityPlayerCategory.abilCommon;
        doesMapCheck = false;
    }

    public override string Description(Mob mob)
    {
        return "Gain 3 WP per turn for 5 turns when inflicting or taking damage.";
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
        actor.AddEffect(EffectTypeEnum.effectDivineVengeance, actor, 5);
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        return;
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
