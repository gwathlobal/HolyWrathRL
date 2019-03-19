using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMindBurn : Ability
{
    public AbilityMindBurn()
    {
        id = AbilityTypeEnum.abilMindBurn;
        stdName = "Mind Burn";
        spd = MobType.NORMAL_AP;
        cost = 40;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBrilliantMind;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Burn a single enemy's mind inflicting 30 mind dmg.";
    }

    public override string Name(Mob mob)
    {
        return "Mind Burn";
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        throw new System.NotImplementedException();
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} invokes mind burn. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Mob.InflictDamage(actor, target.mob, 30, DmgTypeEnum.Mind);
        if (target.mob.CheckDead())
        {
            target.mob.MakeDead(actor, true, true, false);
        }
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        return;
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        Level level = BoardManager.instance.level;
        PlayerMob player = BoardManager.instance.player;
        Vector2Int pos = UIManager.instance.selectorPos;

        if (level.visible[pos.x, pos.y] &&
            level.mobs[pos.x, pos.y] != null &&
            level.mobs[pos.x, pos.y] != player)
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
        return true;
    }
}
