using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNone : Ability
{
    
	public AbilityNone()
    {
        id = AbilityTypeEnum.abilNone;
        stdName = "None";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilCommon;
    }

    public override string Name(Mob mob)
    {
        return "None";
    }

    public override string Description(Mob mob)
    {
        return "None";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        return 00;
    }

    public override bool DoesMapCheck(Mob mob)
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
    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        return false;
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
