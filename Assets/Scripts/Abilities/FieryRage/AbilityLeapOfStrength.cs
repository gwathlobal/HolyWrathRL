using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLeapOfStrength : Ability
{
    public AbilityLeapOfStrength()
    {
        id = AbilityTypeEnum.abilLeapOfStrength;
        stdName = "Leap of Strength";
        spd = MobType.NORMAL_AP;
        cost = 90;
        costType = AbilityCostType.wp;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilFieryRage;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Leap to a unoccupied cell up to 6 tiles away. Upon landing you deal 35 Fire dmg to all enemies around you and applies the Burning effect to them. Burning deals 3 Fire dmg for 5 turns.";
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
        if (mob.GetEffect(EffectTypeEnum.effectImmobilize) != null) return false;
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} makes a Leap of Strength. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.SetPosition(target.loc.x, target.loc.y);
        actor.mo.Move(actor.x, actor.y);

        List<Mob> affectedMobs = new List<Mob>();

        Level level = BoardManager.instance.level;
        level.CheckSurroundings(actor.x, actor.y, false,
            (int x, int y) =>
            {
                if (level.mobs[x, y] != null && !actor.GetFactionRelation(level.mobs[x, y].faction))
                {
                    affectedMobs.Add(level.mobs[x, y]);
                }

            });

        actor.mo.Explosion3x3(actor.x, actor.y);

        foreach (Mob mob in affectedMobs)
        {
            int dmg = 0;
            dmg += Mob.InflictDamage(actor, mob, 35, DmgTypeEnum.Fire, (int dmg1) =>
            {
                string str1;
                if (dmg1 <= 0)
                {
                    str1 = String.Format("{0} takes no fire dmg. ",
                        mob.name);
                }
                else
                {
                    str1 = String.Format("{0} takes {1} fire dmg. ",
                        mob.name,
                        dmg1);
                }
                return str1;
            });
            mob.AddEffect(EffectTypeEnum.effectBurning, actor, 5);

            if (BoardManager.instance.level.visible[mob.x, mob.y])
                UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", new Vector3(mob.x, mob.y, 0));

            if (mob.CheckDead())
            {
                mob.MakeDead(actor, true, true, false);
            }

            if (actor.GetAbility(AbilityTypeEnum.abilDivineVengeance) != null)
                actor.GetAbility(AbilityTypeEnum.abilDivineVengeance).AbilityInvoke(actor, new TargetStruct(new Vector2Int(actor.x, actor.y), actor));
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
            level.mobs[pos.x, pos.y] == null &&
            Level.GetSimpleDistance(player.x, player.y, pos.x, pos.y) <= 6)
        {
            bool fresult = LOS_FOV.DrawLine(player.x, player.y, pos.x, pos.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    AttemptMoveResult result = player.CanMoveToPos(x, y);
                    if (result.result == AttemptMoveResultEnum.moveClear) return true;
                    else if (result.result == AttemptMoveResultEnum.moveBlockedbyMob) return true;
                    else return false;
                });

            if (fresult)
            {
                BoardManager.instance.msgLog.ClearCurMsg();
                TargetStruct target = new TargetStruct(new Vector2Int(pos.x, pos.y), null);
                player.InvokeAbility(ability, target);
                return true;
            }
            else return false;

        }
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        if ((addedAbils.Contains(AbilityTypeEnum.abilFireFists) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireFists)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFlamingArrow) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFlamingArrow)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFireAura) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireAura)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilBreathOfFire) || mob.abilities.ContainsKey(AbilityTypeEnum.abilBreathOfFire)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilIncineration) || mob.abilities.ContainsKey(AbilityTypeEnum.abilIncineration)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilWarmingLight) || mob.abilities.ContainsKey(AbilityTypeEnum.abilWarmingLight)))
            return true;
        else return false;
    }
}
