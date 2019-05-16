using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCallArtillery : Ability
{
    public AbilityCallArtillery()
    {
        id = AbilityTypeEnum.abilCallArtillery;
        stdName = "Call Artillery";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Call an artillery strike to bombard the 5x5 area around the target cell 1 turn later. The bombardment deals 10 physical dmg and 10 fire dmg to all units inside the area of effect.";
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
        return true;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && Level.GetSimpleDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) > 3)
            return true;
        else
            return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} calls for an artillery strike. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Level level = BoardManager.instance.level;
        level.AddFeatureToLevel(new Feature(FeatureTypeEnum.featArtilleryTarget, target.loc.x, target.loc.y)
            {
                counter = 2
            }, 
            target.loc.x, target.loc.y);
        
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        TargetStruct target = new TargetStruct(new Vector2Int(nearestEnemy.x, nearestEnemy.y), nearestEnemy);
        actor.InvokeAbility(ability, target);
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        Level level = BoardManager.instance.level;
        PlayerMob player = BoardManager.instance.player;
        Vector2Int pos = UIManager.instance.selectorPos;

        if (level.visible[pos.x, pos.y] &&
            TerrainTypes.terrainTypes[level.terrain[pos.x, pos.y]].blocksMovement == false)
        {
            BoardManager.instance.msgLog.ClearCurMsg();
            TargetStruct target = new TargetStruct(new Vector2Int(pos.x, pos.y), null);
            player.InvokeAbility(ability, target);
            return true;
        }
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        return false;
    }
}
