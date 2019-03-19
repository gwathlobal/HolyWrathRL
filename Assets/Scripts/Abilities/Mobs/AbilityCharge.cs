using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCharge : Ability
{
    public AbilityCharge()
    {
        id = AbilityTypeEnum.abilCharge;
        stdName = "Charge";
        spd = MobType.NORMAL_AP;
        cost = 35;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilMobs;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Charge to the enemy up to 4 tiles away, dealing 10 physical dmg for each tile you moved. You cannot charge to enemies next to you.";
    }

    public override string Name(Mob mob)
    {
        return "Charge";
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && Level.GetSimpleDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) < 5 &&
                    Level.GetSimpleDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) >= 2)
        {
            bool fresult = LOS_FOV.DrawLine(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    AttemptMoveResult result = actor.CanMoveToPos(x, y);
                    if (result.result == AttemptMoveResultEnum.moveClear) return true;
                    else if (result.result == AttemptMoveResultEnum.moveBlockedbyMob && result.mob == nearestEnemy) return true;
                    else return false;
                });

            if (fresult) return true;
            else return false;
        }
        else return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} charges. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
        int fx = actor.x, fy = actor.y;

        LOS_FOV.DrawLine(actor.x, actor.y, target.mob.x, target.mob.y,
            (int x, int y, int prev_x, int prev_y) =>
            {
                if (actor.CanMoveToPos(x, y).result == AttemptMoveResultEnum.moveClear)
                {
                    fx = x; fy = y;
                    return true;
                }
                else return false;
            });

        if (!(fx == actor.x && fy == actor.y))
        {
            int dmg = (int)(Level.GetDistance(actor.x, actor.y, fx, fy));
            dmg = dmg * 10;

            actor.SetPosition(fx, fy);
            actor.mo.Move(actor.x, actor.y);

            Mob.InflictDamage(actor, target.mob, dmg, DmgTypeEnum.Physical);
            actor.mo.MeleeAttack(target.mob.x - actor.x, target.mob.y - actor.y, dmg + " <i>DMG</i>",
            () =>
            {
                BoardManager.instance.CreateBlooddrop(target.mob.x, target.mob.y);
            });
            if (target.mob.CheckDead())
            {
                target.mob.MakeDead(actor, true, true, false);
            }
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

        Debug.Log(String.Format("Player = ({0},{1}), Pos = {2} ({3},{4}), dist = {5}", player.x, player.y,
            (level.mobs[pos.x, pos.y] != null) ? level.mobs[pos.x, pos.y].name : "null", pos.x, pos.y,
            Level.GetSimpleDistance(player.x, player.y, pos.x, pos.y)));

        if (level.visible[pos.x, pos.y] &&
            level.mobs[pos.x, pos.y] != null && level.mobs[pos.x, pos.y] != player &&
            Level.GetSimpleDistance(player.x, player.y, pos.x, pos.y) < 5 &&
            Level.GetSimpleDistance(player.x, player.y, pos.x, pos.y) >= 2)
        {
            bool fresult = LOS_FOV.DrawLine(player.x, player.y, pos.x, pos.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    AttemptMoveResult result = player.CanMoveToPos(x, y);
                    if (result.result == AttemptMoveResultEnum.moveClear) return true;
                    else if (result.result == AttemptMoveResultEnum.moveBlockedbyMob && result.mob == level.mobs[pos.x, pos.y]) return true;
                    else return false;
                });

            if (fresult)
            {
                BoardManager.instance.msgLog.ClearCurMsg();
                TargetStruct target = new TargetStruct(new Vector2Int(pos.x, pos.y), level.mobs[pos.x, pos.y]);
                player.InvokeAbility(ability, target);
                return true;
            }
            else return false;

        }
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        return true;
    }
}
