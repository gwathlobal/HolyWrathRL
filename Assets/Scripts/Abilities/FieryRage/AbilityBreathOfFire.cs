using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBreathOfFire : Ability
{
    public AbilityBreathOfFire()
    {
        id = AbilityTypeEnum.abilBreathOfFire;
        stdName = "Breath of Fire";
        spd = MobType.NORMAL_AP;
        cost = 120;
        costType = AbilityCostType.wp;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilFieryRage;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Breath fire in a cone up to 6 tiles away in the direction of the target. All affected enemies will take 25 Fire dmg and get the Burning effect. Burning deals 3 Fire dmg for 5 turns.";
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
        string str = String.Format("{0} breaths fire. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        int w = 3;
        int dist = 6;
        int dx = actor.x + (target.loc.x - actor.x) * dist, dy = actor.y + (target.loc.y - actor.y) * dist;
        LOS_FOV.DrawLine(actor.x, actor.y, dx, dy,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    if (Level.GetSimpleDistance(actor.x, actor.y, x, y) >= dist) 
                    {
                        dx = x;
                        dy = y;
                        return false;
                    }
                    return true;
                });


        //Debug.Log(String.Format("Actor = ({0}, {1}), Target = ({2}, {3})", actor.x, actor.y, dx, dy));
        // coords of the known triangle leg
        float x2x1 = dx - actor.x;
        float y2y1 = dy - actor.y;
        //Debug.Log(String.Format("x2x1 = {0}, y2y1 = {1}", x2x1, y2y1));
        // length of the known triangle leg
        float ab = (float)Math.Sqrt(x2x1 * x2x1 + y2y1 * y2y1);
        //Debug.Log(String.Format("ab = {0}", ab));
        // normalizing vector
        float v1x = (dx - actor.x) / ab;
        float v1y = (dy - actor.y) / ab;
        //Debug.Log(String.Format("v1x = {0}, v1y = {1}", v1x, v1y));
        // rotating 90 degrees
        float v3x;
        float v3y;
        v3x = v1y;
        v3y = v1x * -1;
        //Debug.Log(String.Format("Before: v3x = {0}, v3y = {1}", v3x, v3y));
        v3x *= w;
        v3y *= w;
        //Debug.Log(String.Format("After: v3x = {0}, v3y = {1}", v3x, v3y));

        Vector2Int c1 = new Vector2Int();
        c1.x = dx + (int)v3x;
        c1.y = dy + (int)v3y;
        //Debug.Log(String.Format("c1 = {0}, {1}", c1.x, c1.y));

        // rotating -90 degrees
        v3x = v1y * -1;
        v3y = v1x;
        v3x *= w;
        v3y *= w;

        Vector2Int c2 = new Vector2Int();
        c2.x = dx + (int)v3x;
        c2.y = dy + (int)v3y;
        //Debug.Log(String.Format("c2 = {0}, {1}", c2.x, c2.y));

        List<Vector2Int> dstLine = new List<Vector2Int>();
        LOS_FOV.DrawLine(c1.x, c1.y, c2.x, c2.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    dstLine.Add(new Vector2Int(x, y));
                    return true;
                });

        Level level = BoardManager.instance.level;
        List<Mob> affectedMobs = new List<Mob>();
        foreach (Vector2Int dst in dstLine)
        {
            LOS_FOV.DrawLine(actor.x, actor.y, dst.x, dst.y,
                (int x, int y, int prev_x, int prev_y) =>
                {
                    bool blocks = TerrainTypes.terrainTypes[level.terrain[x, y]].blocksProjectiles;
                    if (blocks) return false;

                    if (level.mobs[x, y] != null && !actor.GetFactionRelation(level.mobs[x, y].faction) && !affectedMobs.Contains(level.mobs[x, y]))
                        affectedMobs.Add(level.mobs[x, y]);

                    if (UnityEngine.Random.Range(0, 4) == 0 && !(actor.x == x && actor.y == y))
                    {
                        bool empty = true;
                        foreach (Feature feature in level.features[x, y])
                        {
                            if (feature.idType == FeatureTypeEnum.featFire)
                            {
                                empty = false;
                                break;
                            }
                        }
                        if (empty)
                        {
                            Feature fire = new Feature(FeatureTypeEnum.featFire, x, y);
                            fire.counter = 3 + TerrainTypes.terrainTypes[level.terrain[x, y]].catchesFire;
                            BoardManager.instance.level.AddFeatureToLevel(fire, fire.x, fire.y);
                        }
                    }

                    return true;
                });
        }

        List<Vector3Int> mobStr = new List<Vector3Int>();

        foreach (Mob mob in affectedMobs)
        {
            int dmg = 0;
            dmg += Mob.InflictDamage(null, mob, 25, DmgTypeEnum.Fire, null);
            mob.AddEffect(EffectTypeEnum.effectBurning, actor, 5);
            
            if (BoardManager.instance.level.visible[mob.x, mob.y])
            {
                mobStr.Add(new Vector3Int(mob.x, mob.y, dmg));
            }
            if (mob.CheckDead())
            {
                mob.MakeDead(actor, true, true, false);
            }
        }
        actor.mo.ExplosionCone(actor.x, actor.y, dstLine, mobStr);

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
            level.mobs[pos.x, pos.y] != player &&
            Level.GetSimpleDistance(player.x, player.y, pos.x, pos.y) <= 6) 
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
            (addedAbils.Contains(AbilityTypeEnum.abilFireAura) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireAura)))
            return true;
        else return false;
    }
}
