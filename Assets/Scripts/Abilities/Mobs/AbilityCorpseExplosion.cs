﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCorpseExplosion : Ability
{
    public AbilityCorpseExplosion()
    {
        id = AbilityTypeEnum.abilCorpseExplosion;
        stdName = "Corpse Explosion";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Explode a corpse that creates a 3x3 cloud that inflict 10 Acid dmg to all characters inside it. The cloud lasts for 5 turns.";
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
        return 50;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return true;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (nearestEnemy == null) return false;

        Level level = BoardManager.instance.level;
        Item corpse = null;
        foreach (Item item in level.items[nearestEnemy.x, nearestEnemy.y])
        {
            if (item.corpsePwr > 0)
            {
                corpse = item;
                break;
            }
        }
        if (actor.CanInvokeAbility(ability) && corpse != null) return true;
        else return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} makes a corpse burst into acidic fumes. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Level level = BoardManager.instance.level;
        level.CheckSurroundings(target.loc.x, target.loc.y, true,
            (int x, int y) =>
            {
                Feature cloud = new Feature(FeatureTypeEnum.featAcidCloud, x, y);
                cloud.counter = 5;
                BoardManager.instance.level.AddFeatureToLevel(cloud, cloud.x, cloud.y);
            });

        Item corpse = null;
        foreach (Item item in level.items[target.loc.x, target.loc.y])
        {
            if (item.corpsePwr > 0)
            {
                corpse = item;
                break;
            }
        }
        level.RemoveItemFromLevel(corpse);
        BoardManager.instance.RemoveItemFromWorld(corpse);

        
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
        return false;
    }
}