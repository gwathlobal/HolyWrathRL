using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAbsorbMind : Ability
{

    public AbilityAbsorbMind()
    {
        id = AbilityTypeEnum.abilAbsorbMind;
        stdName = "Absorb Mind";
        passive = true;
        slot = AbilitySlotEnum.abilNone;
        category = AbilityPlayerCategoryEnum.abilCommon;
    }

    public override string Description(Mob mob)
    {
        return "This one as able to absorb remnants of minds. To absorb mind, stand on the tile with the remnant for the whole turn.";
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
