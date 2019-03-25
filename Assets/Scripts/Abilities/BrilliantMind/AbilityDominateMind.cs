using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDominateMind : Ability
{
    public AbilityDominateMind()
    {
        id = AbilityTypeEnum.abilDominateMind;
        stdName = "Dominate Mind";
        spd = MobType.NORMAL_AP;
        cost = 120;
        costType = AbilityCostType.wp;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBrilliantMind;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Dominate the mind of an enemy to make it fight for you for 10 turns.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
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
        string str = String.Format("{0} dominates the mind of {1}. ", actor.name, target.mob.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        target.mob.AddEffect(EffectTypeEnum.effectDominateMind, actor, 10);
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilMindBurn) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMindBurn)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFear) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFear)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilMeditate) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMeditate)))
            return true;
        else return false;
    }
}
