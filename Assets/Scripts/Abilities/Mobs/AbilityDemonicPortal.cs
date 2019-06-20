using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDemonicPortal : Ability
{

    public AbilityDemonicPortal()
    {
        id = AbilityTypeEnum.abilDemonicPortal;
        stdName = "Demonic Portal";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Summon a Demonic Portal next to you. The portal is able to periodically summon imps. You cannot summon a second portal while the first one is present.";
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
        return 60;
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
        if (mob.GetEffect(EffectTypeEnum.effectPortalSummoned) == null)
            return true;
        else
            return false;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} summons a Demonic Portal. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.mo.BuffDebuff(new Vector2Int(actor.x, actor.y), new Vector2Int(actor.x, actor.y),
                UIManager.instance.demonBuffPrefab, null,
                () =>
                {
                    Level level = BoardManager.instance.level;
                    Mob portal = new Mob(MobTypeEnum.mobDemonicPortal, target.loc.x, target.loc.y);
                    portal.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);
                    BoardManager.instance.mobs.Add(portal.id, portal);
                    level.AddMobToLevel(portal, portal.x, portal.y);

                    portal.AddEffect(EffectTypeEnum.effectImmobilize, portal, Effect.CD_UNLIMITED);
                    actor.AddEffect(EffectTypeEnum.effectPortalSummoned, actor, Effect.CD_UNLIMITED);
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
