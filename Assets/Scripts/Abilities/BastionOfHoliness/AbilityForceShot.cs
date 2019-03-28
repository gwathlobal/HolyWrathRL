using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityForceShot : Ability
{
    public AbilityForceShot()
    {
        id = AbilityTypeEnum.abilForceShot;
        stdName = "Force Arrow";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBastionHoly;
    }

    public override string Description(Mob mob)
    {
        return "Shoot a force bolt to push away an enemy from you and deal 5 Physical dmg to it.";
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
        return 25;
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
        string str = String.Format("{0} shoots a force bolt. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        int dmg = 0;
        dmg += Mob.InflictDamage(actor, target.mob, 5, DmgTypeEnum.Physical, null);
        
        GameObject projectile = GameObject.Instantiate(UIManager.instance.projectilePrefab, new Vector3(actor.x, actor.y, 0), Quaternion.identity);
        projectile.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        projectile.GetComponent<MovingObject>().MoveProjectile(target.mob.x, target.mob.y, dmg + " <i>DMG</i>",
            () =>
            {
                BoardManager.instance.CreateBlooddrop(target.mob.x, target.mob.y);
            });

        int dx, dy;
        double a = Math.Atan2(actor.y - target.mob.y, actor.x - target.mob.x) * (180 / Math.PI);
        if (a > 22.5 && a <= 67.5)
        {
            dx = -1; dy = -1;
        }
        else if (a > 67.5 && a <= 112.5)
        {
            dx = 0; dy = -1;
        }
        else if (a > 112.5 && a <= 157.5)
        {
            dx = 1; dy = -1;
        }
        else if (a < -22.5 && a >= -67.5)
        {
            dx = -1; dy = 1;
        }
        else if (a < -67.5 && a >= -112.5)
        {
            dx = 0; dy = 1;
        }
        else if (a < -112.5 && a >= -157.5)
        {
            dx = 1; dy = 1;
        }
        else if (a > 157.5 || a < -157.5)
        {
            dx = 1; dy = 0;
        }
        else
        {
           dx = -1; dy = 0;
        }

        dx = target.mob.x + dx;
        dy = target.mob.y + dy;
        Level level = BoardManager.instance.level;
        if (level.mobs[dx, dy] == null)
        {
            target.mob.SetPosition(dx, dy);
            target.mob.mo.Move(target.mob.x, target.mob.y);
        }
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilAbsorbingShield) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAbsorbingShield)))
            return true;
        else return false;
    }
}
