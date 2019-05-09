using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityReflectiveBlock : Ability
{
    public AbilityReflectiveBlock()
    {
        id = AbilityTypeEnum.abilReflectiveBlock;
        stdName = "Reflective Block";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBastionHoly;
    }

    public override string Description(Mob mob)
    {
        return "Reflective blocking returns 4 projectiles to the originator. Reflective blocking lasts for 3 turns.";
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
        return 25;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return false;
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
        string str = String.Format("{0} starts to reflect projectiles. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.AddEffect(EffectTypeEnum.effectReflectiveBlocking, actor, 3);
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilAbsorbingShield) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAbsorbingShield)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilForceShot) || mob.abilities.ContainsKey(AbilityTypeEnum.abilForceShot)))
            return true;
        else return false;
    }

}
