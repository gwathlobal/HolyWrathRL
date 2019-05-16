using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCannibalize : Ability
{
    public AbilityCannibalize()
    {
        id = AbilityTypeEnum.abilCannibalize;
        stdName = "Cannibalize";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Eat the corpse that you are standing on. Eating the corpse gives 40 HP and 40 FP. Incompleted bodies (like severed heads) give less.";
    }

    public override string Name(Mob mob)
    {
        return "Cannibalize";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
    }

    public override int Cost(Mob mob)
    {
        return 35;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return false;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && (actor.curHP < actor.maxHP || actor.curFP < actor.maxFP)) return true;
        else return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        Level level = BoardManager.instance.level;
        foreach (Item item in level.items[mob.x, mob.y])
            if (item.corpsePwr > 0) return true;
        return false;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} eats a corpse and looks healthier and more vigorous. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        Item corpse = null;
        Level level = BoardManager.instance.level;
        foreach (Item item in level.items[actor.x, actor.y])
        {
            if (item.corpsePwr > 0)
            {
                corpse = item;

                break;
            }
        }

        actor.curFP += 30 * corpse.corpsePwr;
        actor.curHP += 30 * corpse.corpsePwr;
        if (actor.curFP > actor.maxFP) actor.curFP = actor.maxFP;
        if (actor.curHP > actor.maxHP) actor.curHP = actor.maxHP;

        BoardManager.instance.RemoveItemFromWorld(corpse);
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        TargetStruct target = new TargetStruct(new Vector2Int(actor.x, actor.y), actor);
        actor.InvokeAbility(ability, target);
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        return true;
    }
}
