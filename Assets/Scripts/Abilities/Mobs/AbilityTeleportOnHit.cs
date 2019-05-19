using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTeleportOnHit : Ability
{
    public AbilityTeleportOnHit()
    {
        id = AbilityTypeEnum.abilTeleportOnHit;
        stdName = "Escape Teleport";
        passive = true;
        slot = AbilitySlotEnum.abilNone;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Whenever the character is hit, it teleport to a random nearby location.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return 0;
    }

    public override int Cost(Mob mob)
    {
        return 0;
    }

    public override bool DoesMapCheck(Mob mob)
    {
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
        string str = String.Format("{0} disappears in thin air. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        // make 200 attempts to find a free spot within 5 tile distance from the actor's position
        int tx = actor.x;
        int ty = actor.y;
        for (int i = 0; i < 200; i++)
        {
            tx = actor.x + UnityEngine.Random.Range(0, 11) - 5;
            ty = actor.y + UnityEngine.Random.Range(0, 11) - 5;

            if (actor.CanMoveToPos(tx, ty).result == AttemptMoveResultEnum.moveClear)
            {
                break;
            }
        }

        Level level = BoardManager.instance.level;
        bool visibleStart = false;
        bool visibleEnd = false;

        if (level.visible[actor.x, actor.y])
            visibleStart = true;
        if (level.visible[tx, ty])
            visibleEnd = true;

        actor.mo.Teleport(visibleStart, visibleEnd,
            () => 
            {
                actor.SetPosition(tx, ty);
                actor.go.transform.position = new Vector2(tx, ty);
            });

        str = String.Format("{0} appears out of nowhere. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(tx, ty, str);
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        return;
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        return true;
    }
}
