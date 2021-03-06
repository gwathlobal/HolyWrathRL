﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySweepAttack : Ability
{
    public AbilitySweepAttack()
    {
        id = AbilityTypeEnum.abilSweepAttack;
        stdName = "Sweep Attack";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilDeadlyRays;
    }

    public override string Description(Mob mob)
    {
        return "Deal 10 physical damage to the nearby target and all targets on both sides of it.";
    }

    public override string Name(Mob mob)
    {
        return "Sweep Attack";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        return 35;
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
        string str = String.Format("{0} makes a huge swing. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Level level = BoardManager.instance.level;

        List<Mob> affectedMobs = new List<Mob>();

        // find all mobs around except the one that is targeted
        level.CheckSurroundings(target.mob.x, target.mob.y, true,
            (int x, int y) =>
            {
                if (level.mobs[x, y] != null && Level.GetSimpleDistance(x, y, actor.x, actor.y) <= 1 && level.mobs[x, y] != actor && level.mobs[x, y] != target.mob)
                    affectedMobs.Add(level.mobs[x, y]);
            });

        foreach (Mob mob in affectedMobs)
        {
            int dmg = 10;

            dmg = Mob.InflictDamage(actor, mob,
                new Dictionary<DmgTypeEnum, int>()
                {
                    { DmgTypeEnum.Physical, dmg }
                }, 
                null);

            if (mob.CheckDead())
            {
                mob.MakeDead(actor, true, true, true, "");
            }

            if (level.visible[mob.x, mob.y])
            {
                string str2 = dmg + " <i>DMG</i>";
                UIManager.instance.CreateFloatingText(str2, mob.go.transform.position);
            }
        }

        bool visibleStart = level.visible[actor.x, actor.y];
        bool visibleEnd = level.visible[target.mob.x, target.mob.y];

        Vector3 start = actor.go.transform.position;
        Vector3 end = target.mob.go.transform.position;

        actor.mo.MeleeAttack(start, end, (visibleStart || visibleEnd),
            () =>
            {
                int dmg = 10;

                dmg = Mob.InflictDamage(actor, target.mob,
                    new Dictionary<DmgTypeEnum, int>()
                    {
                        { DmgTypeEnum.Physical, dmg }
                    },
                    null,
                    true);

                if (target.mob.CheckDead())
                {
                    target.mob.MakeDead(actor, true, true, true, "");
                }

                if (visibleEnd)
                {
                    string str2 = dmg + " <i>DMG</i>";
                    UIManager.instance.CreateFloatingText(str2, end);
                }
            });
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        Level level = BoardManager.instance.level;
        PlayerMob player = BoardManager.instance.player;
        Vector2Int pos = UIManager.instance.selectorPos;

        if (level.visible[pos.x, pos.y] &&
            level.mobs[pos.x, pos.y] != null &&
            level.mobs[pos.x, pos.y] != player &&
            Level.GetSimpleDistance(player.x, player.y, pos.x, pos.y) <= 1)
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilJudgement) || mob.abilities.ContainsKey(AbilityTypeEnum.abilJudgement)) && 
            (addedAbils.Contains(AbilityTypeEnum.abilAmbush) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAmbush)))
            return true;
        else return false;
    }
}
