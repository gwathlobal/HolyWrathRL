using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFear : Ability
{
    public AbilityFear()
    {
        id = AbilityTypeEnum.abilFear;
        stdName = "Fear";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilBrilliantMind;
    }

    public override string Description(Mob mob)
    {
        return "Fear the target making it flee from any enemy. Fear lasts for 4 turns.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP / 2;
    }

    public override int Cost(Mob mob)
    {
        return 40;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return true;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && ((float)actor.curHP / actor.maxHP <= 0.5))
            return true;
        else
            return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} instills fear into {1}. ", actor.name, target.mob.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        target.mob.mo.BuffDebuff(new Vector2Int(actor.x, actor.y), new Vector2Int(target.mob.x, target.mob.y),
                UIManager.instance.demonDebuffPrefab, UIManager.instance.demonDebuffPrefab,
                () =>
                {
                    target.mob.AddEffect(EffectTypeEnum.effectFear, actor, 4);
                });   
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
            level.mobs[pos.x, pos.y] != null && level.mobs[pos.x, pos.y] != player)
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
        
        if ((addedAbils.Contains(AbilityTypeEnum.abilMindBurn) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMindBurn)))
            return true;
        else return false;
    }
}
