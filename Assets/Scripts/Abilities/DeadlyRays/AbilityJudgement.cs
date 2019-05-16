using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityJudgement : Ability
{
    public AbilityJudgement()
    {
        id = AbilityTypeEnum.abilJudgement;
        stdName = "Judgement";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilDeadlyRays;
    }

    public override string Description(Mob mob)
    {
        return "Deal 10 physical damage. If the target's HP are yellow, deal twice as much damage. If the target's HP are red, deal three times as much damage.";
    }

    public override string Name(Mob mob)
    {
        return "Judgement";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        return 20;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return true;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && Level.GetSimpleDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) <= 1)
        {
            if ((float)nearestEnemy.curHP / nearestEnemy.maxHP <= 0.5) return true;
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
        string str = String.Format("{0} judges {1}. ", actor.name, target.mob.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        int dmg;
        if ((float)target.mob.curHP / target.mob.maxHP > 0.5) dmg = 10;
        else if ((float)target.mob.curHP / target.mob.maxHP > 0.25) dmg = 20;
        else dmg = 30;

        dmg = Mob.InflictDamage(actor, target.mob,
            new Dictionary<DmgTypeEnum, int>()
            {
                { DmgTypeEnum.Physical, dmg }
            }, 
            null);

        actor.mo.MeleeAttack(target.mob.x - actor.x, target.mob.y - actor.y, dmg + " <i>DMG</i>",
            () =>
            {
                BoardManager.instance.CreateBlooddrop(target.mob.x, target.mob.y);
            });
        if (target.mob.CheckDead())
        {
            target.mob.MakeDead(actor, true, true, true);
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
        return true;
    }
}
