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

        Ability.ShootProjectile(actor, target.mob, new Color32(255, 255, 255, 255),
            (Mob attacker, Mob defender) =>
            {
                string result_str;
                int dmg = 0;
                dmg += Mob.InflictDamage(attacker, defender, 
                    new Dictionary<DmgTypeEnum, int>()
                    {
                        { DmgTypeEnum.Physical, 5 }
                    },
                    (int dmg1) =>
                    {
                        string str1;
                        if (dmg1 <= 0)
                        {
                            str1 = String.Format("{0} takes no physical dmg. ",
                                defender.name);
                        }
                        else
                        {
                            str1 = String.Format("{0} takes {1} physical dmg. ",
                                defender.name,
                                dmg1);
                        }
                        return str1;
                    });
                result_str = dmg + " <i>DMG</i>";
                return result_str;
            },
            (Mob attacker, Mob defender) =>
            {
                int dx, dy;
                double a = Math.Atan2(attacker.y - defender.y, attacker.x - defender.x) * (180 / Math.PI);
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

                dx = defender.x + dx;
                dy = defender.y + dy;
                Level level = BoardManager.instance.level;
                if (level.mobs[dx, dy] == null)
                {
                    defender.SetPosition(dx, dy);
                    defender.mo.Move(defender.x, defender.y);
                }
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
