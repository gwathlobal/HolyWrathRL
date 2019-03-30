using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolyRune : Ability
{
    public AbilityHolyRune()
    {
        id = AbilityTypeEnum.abilHolyRune;
        stdName = "Holy Runes";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBastionHoly;
    }

    public override string Description(Mob mob)
    {
        return "Cover a 3x3 area around you with holy runes. If an angel stands on a holy rune, the rune will heal it for 3 HP and restore 3 FP. If a demon or a beast stands on a holy rune, the rune will deal 3 holy dmg to it. Runes last for 5 turns.";
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
        return 40;
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
        string str = String.Format("{0} invokes holy runes. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Level level = BoardManager.instance.level;
        level.CheckSurroundings(actor.x, actor.y, true,
            (int x, int y) =>
            {
                if (!TerrainTypes.terrainTypes[level.terrain[x, y]].blocksMovement) 
                {
                    Feature rune = new Feature(FeatureTypeEnum.featHolyRune, x, y)
                    {
                        counter = 5
                    };
                    BoardManager.instance.level.AddFeatureToLevel(rune, rune.x, rune.y);
                }
            });
        
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        TargetStruct target = new TargetStruct(new Vector2Int(nearestEnemy.x, nearestEnemy.y), nearestEnemy);
        actor.InvokeAbility(ability, target);
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        if ((addedAbils.Contains(AbilityTypeEnum.abilAbsorbingShield) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAbsorbingShield)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilForceShot) || mob.abilities.ContainsKey(AbilityTypeEnum.abilForceShot)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilReflectiveBlock) || mob.abilities.ContainsKey(AbilityTypeEnum.abilReflectiveBlock)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilCallArchangel) || mob.abilities.ContainsKey(AbilityTypeEnum.abilCallArchangel)))
            return true;
        else return false;
    }
}
