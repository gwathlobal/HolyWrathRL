using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySpearOfLight : Ability
{

    public AbilitySpearOfLight()
    {
        id = AbilityTypeEnum.abilSpearOfLight;
        stdName = "Spear of Light";
        passive = false;
        costType = AbilityCostType.wp;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBastionHoly;
    }

    public override string Description(Mob mob)
    {
        return "Summon a Spear of Light to land at a designated free space for 10 turns. Standing near the Spear of Light grants you increased DDR by 2 and the Spear burns all enemies next to it with a Fire Aura.";
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
        return 100;
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
        string str = String.Format("{0} summons a Spear of Light. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Level level = BoardManager.instance.level;
        Mob spear = new Mob(MobTypeEnum.mobSpearOfLight, target.loc.x, target.loc.y);
        spear.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
        BoardManager.instance.mobs.Add(spear.id, spear);
        level.AddMobToLevel(spear, spear.x, spear.y);

        actor.mo.Explosion3x3(spear.x, spear.y);

        level.CheckSurroundings(spear.x, spear.y, false,
            (int x, int y) =>
            {
                if (level.mobs[x, y] != null && !actor.GetFactionRelation(level.mobs[x, y].faction))
                {
                    Mob mob = level.mobs[x, y];

                    int dx, dy;
                    double a = Math.Atan2(spear.y - mob.y, spear.x - mob.x) * (180 / Math.PI);
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

                    dx = mob.x + dx;
                    dy = mob.y + dy;
                    if (level.mobs[dx, dy] == null)
                    {
                        mob.SetPosition(dx, dy);
                        mob.mo.Move(mob.x, mob.y);
                    }
                }
            });

        spear.AddEffect(EffectTypeEnum.effectAuraMinorProtection, spear, Effect.CD_UNLIMITED);
        spear.AddEffect(EffectTypeEnum.effectRemoveAfterTime, spear, 10);
        spear.AddEffect(EffectTypeEnum.effectFireAura, spear, Effect.CD_UNLIMITED);
        spear.AddEffect(EffectTypeEnum.effectImmobilize, spear, Effect.CD_UNLIMITED);
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
            level.mobs[pos.x, pos.y] == null &&
            TerrainTypes.terrainTypes[level.terrain[pos.x, pos.y]].blocksMovement == false)
        {
            BoardManager.instance.msgLog.ClearCurMsg();
            TargetStruct target = new TargetStruct(new Vector2Int(pos.x, pos.y), null);
            player.InvokeAbility(ability, target);
            return true;
        }
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        if ((addedAbils.Contains(AbilityTypeEnum.abilAbsorbingShield) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAbsorbingShield)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilForceShot) || mob.abilities.ContainsKey(AbilityTypeEnum.abilForceShot)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilReflectiveBlock) || mob.abilities.ContainsKey(AbilityTypeEnum.abilReflectiveBlock)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilCallArchangel) || mob.abilities.ContainsKey(AbilityTypeEnum.abilCallArchangel)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilHolyRune) || mob.abilities.ContainsKey(AbilityTypeEnum.abilHolyRune)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilPurgeRitual) || mob.abilities.ContainsKey(AbilityTypeEnum.abilPurgeRitual)))
            return true;
        else return false;
    }
}
