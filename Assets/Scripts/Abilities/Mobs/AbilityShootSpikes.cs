using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityShootSpikes : Ability
{
    public AbilityShootSpikes()
    {
        id = AbilityTypeEnum.abilShootSpikes;
        stdName = "Shoot Spikes";
        passive = false;
        slot = AbilitySlotCategoty.abilRanged;
        category = AbilityPlayerCategory.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Attack the enemy from range for 5 physical dmg.";
    }

    public override string Name(Mob mob)
    {
        return "Shoot Spikes";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        return 5;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return true;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && Level.GetSimpleDistance(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y) > 1)
        {
            Level level = BoardManager.instance.level;

            bool result = LOS_FOV.DrawLine(actor.x, actor.y, nearestEnemy.x, nearestEnemy.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    bool blocks = TerrainTypes.terrainTypes[level.terrain[x, y]].blocksProjectiles;
                    if (!blocks) return true;
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
        string str = String.Format("{0} shoots a spike. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Ability.ShootProjectile(actor, target.mob, new Color32(188, 110, 0, 255),
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
            null);
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
                    if (blocks) return true;
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
        return true;
    }
}
