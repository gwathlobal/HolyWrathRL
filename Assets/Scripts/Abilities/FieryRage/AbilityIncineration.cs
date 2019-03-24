using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIncineration : Ability
{
    public AbilityIncineration()
    {
        id = AbilityTypeEnum.abilIncineration;
        stdName = "Incineration";
        spd = MobType.NORMAL_AP;
        cost = 40;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilFieryRage;
        doesMapCheck = true;
    }

    public override string Description(Mob mob)
    {
        return "Ignite a 3x3 area around the target cell to deal 15 Fire dmg to enemies there and apply the Burning effect. Burning deals 3 Fire dmg for 5 turns.";
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
        string str = String.Format("{0} invokes incineration. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        List<Mob> affectedMobs = new List<Mob>();
        Level level = BoardManager.instance.level;
        level.CheckSurroundings(target.loc.x, target.loc.y, true,
            (int x, int y) =>
            {
                if (level.mobs[x, y] != null && !actor.GetFactionRelation(level.mobs[x, y].faction)) 
                {
                    affectedMobs.Add(level.mobs[x, y]);
                }

                if (UnityEngine.Random.Range(0, 4) == 0)
                {
                    Feature fire = new Feature(FeatureTypeEnum.featFire, x, y);
                    fire.counter = 3 + TerrainTypes.terrainTypes[level.terrain[x, y]].catchesFire;
                    BoardManager.instance.level.AddFeatureToLevel(fire, fire.x, fire.y);
                }
            });

        actor.mo.Explosion3x3(target.loc.x, target.loc.y);

        foreach (Mob mob in affectedMobs)
        {
            int dmg = 0;
            dmg += Mob.InflictDamage(actor, mob, 15, DmgTypeEnum.Fire, (int dmg1) =>
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

        if (level.visible[pos.x, pos.y])
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilFireFists) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireFists)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFlamingArrow) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFlamingArrow)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFireAura) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireAura)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilBreathOfFire) || mob.abilities.ContainsKey(AbilityTypeEnum.abilBreathOfFire)))
            return true;
        else return false;
    }
}
