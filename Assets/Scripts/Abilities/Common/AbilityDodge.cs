using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDodge : Ability
{
    public AbilityDodge()
    {
        id = AbilityTypeEnum.abilDodge;
        stdName = "Dodge";
        passive = false;
        slot = AbilitySlotEnum.abilDodge;
        category = AbilityPlayerCategoryEnum.abilCommon;
    }

    public override string Description(Mob mob)
    {
        return "Instantly move 3 tiles away. If you are immobilized, remove immobilization and make you immune to immobilization for 5 turns.";
    }

    public override string Name(Mob mob)
    {
        return "Dodge";
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
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && (double)actor.curHP / actor.maxHP <= 0.3 &&
                    Level.GetDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) < 2)
        {
            List<Vector2Int> availCells = new List<Vector2Int>();
            BoardManager.instance.level.CheckSurroundings(actor.x, actor.y, false,
                (int dx, int dy) =>
                {
                    AttemptMoveResult result = actor.CanMoveToPos(dx, dy);
                    if (result.result == AttemptMoveResultEnum.moveClear || result.result == AttemptMoveResultEnum.moveBlockedbyMob)
                        availCells.Add(new Vector2Int(dx, dy));
                });
            if (availCells.Count > 0)
                return true;
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
        string str = String.Format("{0} makes a roll. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        if (actor.GetEffect(EffectTypeEnum.effectImmobilize) != null)
        {
            actor.RemoveEffect(EffectTypeEnum.effectImmobilize);
            actor.AddEffect(EffectTypeEnum.effectImmobilizeImmunity, actor, 5);
        }

        int dx = target.loc.x - actor.x;
        int dy = target.loc.y - actor.y;

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int target1 = new Vector2Int(target.loc.x, target.loc.y); ;

        if (Math.Abs(dx) < 3 || Math.Abs(dy) < 3)
        {
            target1 = new Vector2Int(target.loc.x + dx * 3, target.loc.y + dy * 3);

        }

        LOS_FOV.DrawLine(actor.x, actor.y, target1.x, target1.y,
            (int x, int y, int prev_x, int prev_y) =>
            {
                path.Add(new Vector2Int(x, y));
                return true;
            });

        path.RemoveAt(0);

        int i = 0;
        do
        {
            AttemptMoveResult moveResult = actor.CanMoveToPos(path[i].x, path[i].y);
            if (moveResult.result == AttemptMoveResultEnum.moveOutOfBounds || moveResult.result == AttemptMoveResultEnum.moveBlockedByTerrain)
                break;
            i++;
        } while (i < 3);
        i--;
        if (i >= 0)
        {
            int n = 0;
            for (n = i; n >= 0; n--)
            {
                AttemptMoveResult moveResult = actor.CanMoveToPos(path[n].x, path[n].y);
                if (moveResult.result == AttemptMoveResultEnum.moveClear)
                    break;
            }

            actor.SetPosition(path[n].x, path[n].y);
            actor.mo.Move(actor.x, actor.y);
        }
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        List<Vector2Int> availCells = new List<Vector2Int>();
        BoardManager.instance.level.CheckSurroundings(actor.x, actor.y, false,
            (int dx, int dy) =>
            {
                AttemptMoveResult result = actor.CanMoveToPos(dx, dy);
                if (result.result == AttemptMoveResultEnum.moveClear || result.result == AttemptMoveResultEnum.moveBlockedbyMob)
                    availCells.Add(new Vector2Int(dx, dy));
            });
        Vector2Int farCell = availCells[0]; ;
        foreach (Vector2Int cell in availCells)
        {
            if (Level.GetDistance(cell.x, cell.y, nearestEnemy.x, nearestEnemy.y) > Level.GetDistance(farCell.x, farCell.y, nearestEnemy.x, nearestEnemy.y))
                farCell = cell;
        }

        TargetStruct target = new TargetStruct(farCell, null);
        actor.InvokeAbility(ability, target);
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        Level level = BoardManager.instance.level;
        PlayerMob player = BoardManager.instance.player;
        Vector2Int pos = UIManager.instance.selectorPos;

        if (level.visible[pos.x, pos.y] &&
            player.CanMoveToPos(pos.x, pos.y).result == AttemptMoveResultEnum.moveClear)
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
