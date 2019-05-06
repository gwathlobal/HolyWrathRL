using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityShootMachinegun : Ability
{
    public AbilityShootMachinegun()
    {
        id = AbilityTypeEnum.abilShootMachinegun;
        stdName = "Shoot Machinegun";
        passive = false;
        slot = AbilitySlotCategoty.abilRanged;
        category = AbilityPlayerCategory.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Attack the enemy from range 6 times, each dealing 4 physical dmg. The attack is rather inaccurate with the 30% chance to miss the target.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP * 2f;
    }

    public override int Cost(Mob mob)
    {
        return 0;
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
        string str = String.Format("{0} shoots a machinegun. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Color32 color = new Color32(100, 100, 100, 255);
        int disperse;
        int rx, ry;
        Level level = BoardManager.instance.level;

        if (Level.GetSimpleDistance(actor.x, actor.y, target.mob.x, target.mob.y) <= 1)
            disperse = 0;
        else if (Level.GetSimpleDistance(actor.x, actor.y, target.mob.x, target.mob.y) <= 5)
            disperse = 1;
        else
            disperse = 2;

        List<Mob> targetMobs = new List<Mob>();
        List<Vector2Int> targetLocs = new List<Vector2Int>();

        for (int i = 0; i < 6; i++) 
        {
            rx = 0;
            ry = 0;
            if (disperse > 0 && UnityEngine.Random.Range(0, 100) < 30)
            {
                rx = UnityEngine.Random.Range(0, disperse * 2 + 1) - disperse;
                ry = UnityEngine.Random.Range(0, disperse * 2 + 1) - disperse;
            }
            rx = target.mob.x + rx;
            ry = target.mob.y + ry;

            int fx = target.mob.x, fy = target.mob.y;
            bool result = LOS_FOV.DrawLine(actor.x, actor.y, rx, ry,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    fx = x;
                    fy = y;
                    bool blocks = TerrainTypes.terrainTypes[level.terrain[x, y]].blocksProjectiles;
                    if (!blocks) return true;
                    else return false;
                });

            if (level.mobs[fx, fy] == null)
                targetLocs.Add(new Vector2Int(fx, fy));
            else
                targetMobs.Add(level.mobs[fx, fy]);
        }

        foreach (Mob targetMob in targetMobs)
        {
            Ability.ShootProjectile(actor, targetMob, color,
                (Mob attacker, Mob defender) =>
                {
                    string result_str;
                    int dmg = 0;
                    dmg += Mob.InflictDamage(attacker, defender,
                        new Dictionary<DmgTypeEnum, int>()
                        {
                        { DmgTypeEnum.Physical, 4 }
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

        foreach (Vector2Int targetLoc in targetLocs)
        {
            GameObject projectile = GameObject.Instantiate(UIManager.instance.projectilePrefab, new Vector3(actor.x, actor.y, 0), Quaternion.identity);
            projectile.GetComponent<SpriteRenderer>().color = color;
            projectile.GetComponent<MovingObject>().MoveProjectile(targetLoc.x, targetLoc.y, "MISS",
                () =>
                {
                    
                });
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
