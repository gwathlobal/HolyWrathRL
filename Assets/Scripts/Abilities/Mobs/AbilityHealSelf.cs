using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHealSelf : Ability
{
    public AbilityHealSelf()
    {
        id = AbilityTypeEnum.abilHealSelf;
        stdName = "Heal Self";
        spd = MobType.NORMAL_AP;
        cost = 30;
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilMobs;
        doesMapCheck = false;
    }

    public override string Description(Mob mob)
    {
        return "Heal self for 40 HP.";
    }

    public override string Name(Mob mob)
    {
        return "Heal Self";
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) &&
                    ((nearestEnemy == null && (double)actor.curHP < actor.maxHP)
                    ||
                    (nearestEnemy != null && (double)actor.curHP / actor.maxHP <= 0.3)))
            return true;
        else return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob actor)
    {
        if (actor.curHP < actor.maxHP) return true;
        else return false;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        int HPhealed;
        if (actor.curHP + 40 > actor.maxHP) HPhealed = actor.maxHP - actor.curHP;
        else HPhealed = 40;

        string str = String.Format("{0} heals itself for {1} HP. ",
            MobTypes.mobTypes[actor.idType].name,
            HPhealed);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.curHP += HPhealed;
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
