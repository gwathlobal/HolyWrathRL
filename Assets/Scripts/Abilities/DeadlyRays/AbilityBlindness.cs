﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBlindness : Ability
{
    public AbilityBlindness()
    {
        id = AbilityTypeEnum.abilBlindness;
        stdName = "Blindness";
        spd = MobType.NORMAL_AP / 2;
        cost = 40;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilDeadlyRays;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Blind a target reducing its vision radius to zero. Blindness lasts for 4 turns or until the target receives any damage.";
    }

    public override string Name(Mob mob)
    {
        return "Blindness";
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
        string str = String.Format("{0} blinds {1}. ", actor.name, target.mob.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        target.mob.AddEffect(EffectTypeEnum.effectBlindness, actor, 4);
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
            (addedAbils.Contains(AbilityTypeEnum.abilInvisibility) || mob.abilities.ContainsKey(AbilityTypeEnum.abilInvisibility)))
            return true;
        else return false;
    }
}
