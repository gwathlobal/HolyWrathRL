using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySweepAttack : Ability
{
    public AbilitySweepAttack()
    {
        id = AbilityTypeEnum.abilSweepAttack;
        stdName = "Sweep Attack";
        spd = MobType.NORMAL_AP;
        cost = 35;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilDeadlyRays;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Deal 10 physical damage to the nearby target and all targets on both sides of it.";
    }

    public override string Name(Mob mob)
    {
        return "Sweep Attack";
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

        int dx = target.mob.x - actor.x;
        int dy = target.mob.y - actor.y;

        List<Mob> affectedMobs = new List<Mob>();

        level.CheckSurroundings(target.mob.x, target.mob.y, true,
            (int x, int y) =>
            {
                if (level.mobs[x, y] != null && Level.GetSimpleDistance(x, y, actor.x, actor.y) <= 1 && level.mobs[x, y] != actor)
                    affectedMobs.Add(level.mobs[x, y]);
            });

        foreach (Mob mob in affectedMobs)
        {
            int dmg = 10;

            dmg = Mob.InflictDamage(actor, mob, dmg, DmgTypeEnum.Physical);

            actor.mo.MeleeAttack(mob.x - actor.x, mob.y - actor.y, dmg + " <i>DMG</i>",
                () =>
                {
                    BoardManager.instance.CreateBlooddrop(mob.x, mob.y);
                });
            if (mob.CheckDead())
            {
                mob.MakeDead(actor, true, true, true);
            }
        }
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
