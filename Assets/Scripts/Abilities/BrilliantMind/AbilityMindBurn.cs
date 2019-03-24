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
        cost = 30;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBrilliantMind;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Burn a single enemy's mind inflicting 20 Mind dmg. Mindless creatures take only half.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
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
        string str = String.Format("{0} invokes mind burn. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        int dmg = 0;
        dmg += Mob.InflictDamage(actor, target.mob, 20, DmgTypeEnum.Mind, (int dmg1) =>
        {
            string str1;
            if (dmg1 <= 0)
            {
                str1 = String.Format("{0} takes no mind dmg. ",
                    target.mob.name);
            }
            else
            {
                str1 = String.Format("{0} takes {1} mind dmg. ",
                    target.mob.name,
                    dmg1);
            }
            return str1;
        });

        if (BoardManager.instance.level.visible[target.mob.x, target.mob.y])
            UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", new Vector3(target.mob.x, target.mob.y, 0));

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
