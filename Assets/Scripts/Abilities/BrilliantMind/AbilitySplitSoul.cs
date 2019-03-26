using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySplitSoul : Ability
{

    public AbilitySplitSoul()
    {
        id = AbilityTypeEnum.abilSplitSoul;
        stdName = "Split Soul";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBrilliantMind;
    }

    public override string Description(Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectSplitSoulSource) == null)
            return "Split your soul to anchor it in place for 10 turns. While the anchor holds, you can reunite with your soul teleporting back to it.";
        else
            return "Teleport back to your anchored soul shard.";
    }

    public override string Name(Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectSplitSoulSource) == null)
            return stdName;
        else
            return "Return to Soul";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectSplitSoulSource) == null)
            return 60;
        else
            return 0;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        if (mob.GetEffect(EffectTypeEnum.effectSplitSoulSource) == null)
            return true;
        else
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
        if (actor.GetEffect(EffectTypeEnum.effectSplitSoulSource) == null)
        {
            string str = String.Format("{0} splits its soul. ", actor.name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

            Level level = BoardManager.instance.level;
            Mob soul = new Mob(MobTypeEnum.mobSplitSoul, target.loc.x, target.loc.y);
            soul.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
            BoardManager.instance.mobs.Add(soul.id, soul);
            level.AddMobToLevel(soul, soul.x, soul.y);

            actor.AddEffect(EffectTypeEnum.effectSplitSoulSource, soul, 10);
            soul.AddEffect(EffectTypeEnum.effectSplitSoulTarget, actor, 10);
            soul.AddEffect(EffectTypeEnum.effectImmobilize, actor, Effect.CD_UNLIMITED);
        }
        else
        {
            Effect effect = actor.GetEffect(EffectTypeEnum.effectSplitSoulSource);
            Mob soul = effect.actor;
            int dx = soul.x;
            int dy = soul.y;
            soul.RemoveEffect(EffectTypeEnum.effectSplitSoulTarget);

            actor.SetPosition(dx, dy);
            actor.go.transform.position = new Vector3(dx, dy, 0);
            actor.RemoveEffect(EffectTypeEnum.effectSplitSoulSource);
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

        if (level.visible[pos.x, pos.y] &&
            level.mobs[pos.x, pos.y] == null &&
            TerrainTypes.terrainTypes[level.terrain[pos.x,pos.y]].blocksMovement == false)
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilMindBurn) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMindBurn)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFear) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFear)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilMeditate) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMeditate)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilDominateMind) || mob.abilities.ContainsKey(AbilityTypeEnum.abilDominateMind)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilTrapMind) || mob.abilities.ContainsKey(AbilityTypeEnum.abilTrapMind)))
            return true;
        else return false;
    }
}
