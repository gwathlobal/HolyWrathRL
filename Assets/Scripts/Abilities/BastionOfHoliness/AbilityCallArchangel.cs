using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCallArchangel : Ability
{

    public AbilityCallArchangel()
    {
        id = AbilityTypeEnum.abilCallArchangel;
        stdName = "Call Archangel";
        passive = false;
        costType = AbilityCostType.wp;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBastionHoly;
    }

    public override string Description(Mob mob)
    {
        return "Call an Archangel that will fight together with you for 10 turns.";
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
        return 150;
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
        string str = String.Format("{0} calls an Archangel. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Level level = BoardManager.instance.level;
        Mob arch = new Mob(MobTypeEnum.mobArchangel, target.loc.x, target.loc.y);
        arch.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
        BoardManager.instance.mobs.Add(arch.id, arch);
        level.AddMobToLevel(arch, arch.x, arch.y);

        arch.AddEffect(EffectTypeEnum.effectRemoveAfterTime, arch, 10);
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
            TerrainTypes.terrainTypes[level.terrain[pos.x, pos.y]].blocksMovement == false &&
            Level.GetSimpleDistance(player.x, player.y, pos.x, pos.y) <= 1)
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilAbsorbingShield) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAbsorbingShield)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilForceShot) || mob.abilities.ContainsKey(AbilityTypeEnum.abilForceShot)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilReflectiveBlock) || mob.abilities.ContainsKey(AbilityTypeEnum.abilReflectiveBlock)))
            return true;
        else return false;
    }
}
