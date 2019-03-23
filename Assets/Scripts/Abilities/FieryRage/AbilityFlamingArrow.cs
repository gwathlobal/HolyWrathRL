using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFlamingArrow : Ability
{
    public AbilityFlamingArrow()
    {
        id = AbilityTypeEnum.abilFlamingArrow;
        stdName = "Flaming Arrow";
        spd = MobType.NORMAL_AP;
        cost = 10;
        passive = false;
        slot = AbilitySlotCategoty.abilRanged;
        category = AbilityPlayerCategory.abilFieryRage;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Attack the enemy from range for 5 Fire dmg. Places a Burning effect on the target that deals 3 Fire dmg for 5 turns.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && Level.GetDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) > 2)
        {
            Level level = BoardManager.instance.level;

            bool result = LOS_FOV.DrawLine(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    bool blocks = TerrainTypes.terrainTypes[level.terrain[x, y]].blocksProjectiles;
                    if (blocks) return true;
                    else return false;
                });
            if (result) return true;
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
        string str = String.Format("{0} shoots a flaming arrow. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);


        int dmg = 0;
        dmg += Mob.InflictDamage(actor, target.mob, 5, DmgTypeEnum.Fire, null);
        target.mob.AddEffect(EffectTypeEnum.effectBurning, actor, 5);

        GameObject projectile = GameObject.Instantiate(UIManager.instance.projectilePrefab, new Vector3(actor.x, actor.y, 0), Quaternion.identity);
        projectile.GetComponent<SpriteRenderer>().color = new Color32(255, 168, 0, 255);
        projectile.GetComponent<MovingObject>().MoveProjectile(target.mob.x, target.mob.y, dmg + " <i>DMG</i>",
            () =>
            {
                BoardManager.instance.CreateBlooddrop(target.mob.x, target.mob.y);
            });

        if (target.mob.CheckDead())
        {
            target.mob.MakeDead(actor, true, true, false);
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
            level.mobs[pos.x, pos.y] != player)
        {
            bool result = LOS_FOV.DrawLine(player.x, player.y, pos.x, pos.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    bool blocks = TerrainTypes.terrainTypes[level.terrain[x, y]].blocksProjectiles;
                    if (!blocks) return true;
                    else return false;
                });

            if (result)
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilFireFists) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireFists)))
            return true;
        else return false;
    }
}
