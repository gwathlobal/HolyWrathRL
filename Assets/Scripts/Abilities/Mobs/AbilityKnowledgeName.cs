using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityKnowledgeName : Ability
{

    public AbilityKnowledgeName()
    {
        id = AbilityTypeEnum.abilKnowledgeName;
        stdName = "Knowledge: Prominent Character Name";
        passive = true;
        slot = AbilitySlotEnum.abilNone;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "This is creature knows the name of a prominent character.";
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
        Level level = BoardManager.instance.level;
        level.AddFeatureToLevel(new Feature(FeatureTypeEnum.featKnowledgeName, target.loc.x, target.loc.y)
        {
            counter = 5
        },
        target.loc.x, target.loc.y);
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
        return false;
    }
}
