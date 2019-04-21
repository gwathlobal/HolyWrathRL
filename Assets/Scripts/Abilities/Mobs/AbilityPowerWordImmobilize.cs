using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPowerWordImmobilize : Ability
{
    public AbilityPowerWordImmobilize()
    {
        id = AbilityTypeEnum.abilPowerWordImmobilize;
        stdName = "Power Word: Immobilize";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Invoke a power word to immobilize the target. Immobilization lasts for 4 turns.";
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
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && nearestEnemy.GetEffect(EffectTypeEnum.effectImmobilize) == null &&
            nearestEnemy.GetEffect(EffectTypeEnum.effectImmobilizeImmunity) == null && 
            Level.GetSimpleDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) > 1)
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
        string str = String.Format("{0} invokes Power Word: Immobilize on {1}. ", actor.name, target.mob.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        if (actor.GetEffect(EffectTypeEnum.effectImmobilizeImmunity) == null)
            target.mob.AddEffect(EffectTypeEnum.effectImmobilize, actor, 4);
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
       return false;
    }
}
