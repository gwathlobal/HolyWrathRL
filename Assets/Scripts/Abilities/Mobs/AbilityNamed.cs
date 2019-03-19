using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNamed : Ability
{

    public AbilityNamed()
    {
        id = AbilityTypeEnum.abilNamed;
        stdName = "Named";
        spd = MobType.NORMAL_AP;
        cost = 0;
        passive = true;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilMobs;
        doesMapCheck = false;
    }

    public override string Description(Mob mob)
    {
        return "This is a prominent creature worth having a name.";
    }

    public override string Name(Mob mob)
    {
        return "Named";
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
