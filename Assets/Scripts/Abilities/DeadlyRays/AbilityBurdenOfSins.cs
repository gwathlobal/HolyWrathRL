﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBurdenOfSins : Ability
{
    public AbilityBurdenOfSins()
    {
        id = AbilityTypeEnum.abilBurdenOfSins;
        stdName = "Burden of Sins";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilDeadlyRays;
    }

    public override string Description(Mob mob)
    {
        return "Remove target's 50 %DR for 5 turns.";
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
        return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} places the Burden of Sins upon {1}. ", actor.name, target.mob.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        target.mob.mo.BuffDebuff(new Vector2Int(actor.x, actor.y), new Vector2Int(target.mob.x, target.mob.y),
                UIManager.instance.angelDebuffPrefab, UIManager.instance.angelDebuffPrefab,
                () =>
                {
                    target.mob.AddEffect(EffectTypeEnum.effectBurdenOfSins, actor, 5);
                });

        
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

        //Debug.Log(String.Format("Player = ({0},{1}), Pos = {2} ({3},{4}), dist = {5}", player.x, player.y,
        //    (level.mobs[pos.x, pos.y] != null) ? level.mobs[pos.x, pos.y].name : "null", pos.x, pos.y,
        //    Level.GetSimpleDistance(player.x, player.y, pos.x, pos.y)));

        if (level.visible[pos.x, pos.y] &&
            level.mobs[pos.x, pos.y] != null && level.mobs[pos.x, pos.y] != player)
        {
            BoardManager.instance.msgLog.ClearCurMsg();
            TargetStruct target = new TargetStruct(new Vector2Int(pos.x, pos.y), level.mobs[pos.x, pos.y]);
            player.InvokeAbility(ability, target);
            return true;

        }
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        if ((addedAbils.Contains(AbilityTypeEnum.abilJudgement) || mob.abilities.ContainsKey(AbilityTypeEnum.abilJudgement)) && 
            (addedAbils.Contains(AbilityTypeEnum.abilAmbush) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAmbush)) && 
            (addedAbils.Contains(AbilityTypeEnum.abilSweepAttack) || mob.abilities.ContainsKey(AbilityTypeEnum.abilSweepAttack)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilInvisibility) || mob.abilities.ContainsKey(AbilityTypeEnum.abilInvisibility)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilBlindness) || mob.abilities.ContainsKey(AbilityTypeEnum.abilBlindness)))
            return true;
        else return false;
    }
}
