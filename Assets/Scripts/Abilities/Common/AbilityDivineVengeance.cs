using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDivineVengeance : Ability
{
    public AbilityDivineVengeance()
    {
        id = AbilityTypeEnum.abilDivineVengeance;
        stdName = "Divine Vengeance";
        passive = true;
        slot = AbilitySlotEnum.abilNone;
        category = AbilityPlayerCategoryEnum.abilCommon;
    }

    public override string Description(Mob mob)
    {
        return "Gain 3 WP per turn for 5 turns when inflicting or taking damage.";
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
