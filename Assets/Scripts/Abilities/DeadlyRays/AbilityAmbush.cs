﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAmbush : Ability
{
    public AbilityAmbush()
    {
        id = AbilityTypeEnum.abilAmbush;
        stdName = "Ambush";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilDeadlyRays;
    }

    public override string Description(Mob mob)
    {
        return "Charge to the enemy up to 4 tiles away, dealing 25 physical dmg. You cannot charge to enemies next to you.";
    }

    public override string Name(Mob mob)
    {
        return "Ambush";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        return 30;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return true;
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
        if (mob.GetEffect(EffectTypeEnum.effectImmobilize) != null) return false;
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        
        string str = String.Format("{0} ambushes {1}. ", actor.name, target.mob.name);
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
            Level level = BoardManager.instance.level;
            bool visibleStart = level.visible[actor.x, actor.y];
            bool visibleMiddle = level.visible[fx, fy];
            bool visibleEnd = level.visible[target.mob.x, target.mob.y];

            Vector2 movPos = new Vector2(fx, fy);
            Vector2 attPos = target.mob.go.transform.position;

            BoardEventController.instance.AddEvent(new BoardEventController.Event(actor.go,
                () =>
                {
                    actor.mo.Move(movPos, (visibleStart || visibleMiddle),
                        () =>
                        {
                            actor.SetPosition(fx, fy);
                        });
                    BoardEventController.instance.RemoveFinishedEvent();
                }));

            BoardEventController.instance.AddEvent(new BoardEventController.Event(actor.go,
                () =>
                {
                    actor.mo.MeleeAttack(movPos, attPos, (visibleMiddle || visibleEnd),
                        () =>
                        {
                            int dmg = 25;

                            dmg = Mob.InflictDamage(actor, target.mob,
                            new Dictionary<DmgTypeEnum, int>()
                            {
                                { DmgTypeEnum.Physical, dmg }
                            },
                            null,
                            true);

                            if (target.mob.CheckDead())
                            {
                                target.mob.MakeDead(actor, true, true, false, "");
                            }

                            if (visibleEnd)
                            {
                                string str2 = dmg + " <i>DMG</i>";
                                UIManager.instance.CreateFloatingText(str2, attPos);
                            }
                        });
                    BoardEventController.instance.RemoveFinishedEvent();
                }));
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
        if (addedAbils.Contains(AbilityTypeEnum.abilJudgement) || mob.abilities.ContainsKey(AbilityTypeEnum.abilJudgement)) return true;
        else return false;
    }
}
