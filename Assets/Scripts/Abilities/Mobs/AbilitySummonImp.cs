using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySummonImp : Ability
{

    public AbilitySummonImp()
    {
        id = AbilityTypeEnum.abilSummonImp;
        stdName = "Summon Imp";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Summon a Crimson or a Machine Imp next to you.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        return 10;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return true;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        bool result = false;
        Level level = BoardManager.instance.level;
        level.CheckSurroundings(actor.x, actor.y, false, 
            (int x, int y) =>
            {
                if (TerrainTypes.terrainTypes[level.terrain[x, y]].blocksMovement == false && level.mobs[x, y] == null)
                {
                    result = true;
                }
            });
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && result)
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
        actor.mo.BuffDebuff(new Vector2Int(actor.x, actor.y), new Vector2Int(actor.x, actor.y),
                UIManager.instance.demonBuffPrefab, null,
                () =>
                {
                    int r = UnityEngine.Random.Range(0, 2);
                    Mob mob;
                    if (r == 0)
                    {
                        string str = String.Format("{0} summons a Crimson Imp. ", actor.name);
                        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
                        mob = new Mob(MobTypeEnum.mobCrimsonImp, target.loc.x, target.loc.y);
                    }
                    else
                    {
                        string str = String.Format("{0} summons a Machine Imp. ", actor.name);
                        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
                        mob = new Mob(MobTypeEnum.mobMachineImp, target.loc.x, target.loc.y);
                    }

                    Level level = BoardManager.instance.level;
                    mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                    BoardManager.instance.mobs.Add(mob.id, mob);
                    level.AddMobToLevel(mob, mob.x, mob.y);
                });
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        Vector2Int loc = new Vector2Int(actor.x, actor.y);
        Level level = BoardManager.instance.level;
        level.CheckSurroundings(actor.x, actor.y, false,
            (int x, int y) =>
            {
                if (TerrainTypes.terrainTypes[level.terrain[x, y]].blocksMovement == false && level.mobs[x, y] == null)
                {
                    loc = new Vector2Int(x, y);
                }
            });
        TargetStruct target = new TargetStruct(loc, null);
        actor.InvokeAbility(ability, target);
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
       return false;
    }
}
