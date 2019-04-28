using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTrapMind : Ability
{
    public AbilityTrapMind()
    {
        id = AbilityTypeEnum.abilTrapMind;
        stdName = "Trap Mind";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBrilliantMind;
    }

    public override string Description(Mob mob)
    {
        return "Trap the mind of an enemy to immobilize it. Immobilization lasts for 5 turns.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP / 2;
    }

    public override int Cost(Mob mob)
    {
        return 25;
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
        string str = String.Format("{0} immobilizes {1}. ", actor.name, target.mob.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        if (actor.GetEffect(EffectTypeEnum.effectImmobilizeImmunity) == null)
            target.mob.AddEffect(EffectTypeEnum.effectImmobilize, actor, 5);
        else
        {
            str = String.Format("{0} is unaffected. ", target.mob.name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
        }
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
            (addedAbils.Contains(AbilityTypeEnum.abilMeditate) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMeditate)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilDominateMind) || mob.abilities.ContainsKey(AbilityTypeEnum.abilDominateMind)))
            return true;
        else return false;
    }
}
