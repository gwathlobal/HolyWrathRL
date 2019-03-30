using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPurgeRitual : Ability
{
    public AbilityPurgeRitual()
    {
        id = AbilityTypeEnum.abilPurgeRitual;
        stdName = "Purging Ritual";
        passive = false;
        slot = AbilitySlotCategoty.abilNormal;
        category = AbilityPlayerCategory.abilBastionHoly;
    }

    public override string Description(Mob mob)
    {
        return "Remove all negative effects from yourself. Gain 10 HP for each purged effect.";
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
        return 30;
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
        string str = String.Format("{0} recites a purging incantation. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        List<Effect> effectsToRemove = new List<Effect>();
        foreach (Effect effect in actor.effects.Values)
        {
            if (EffectTypes.effectTypes[effect.idType].canBePurged)
            {
                effectsToRemove.Add(effect);
            }
        }

        actor.curHP += 10 * effectsToRemove.Count;
        if (actor.curHP > actor.maxHP)
            actor.curHP = actor.maxHP;

        for (int i = effectsToRemove.Count - 1; i >= 0; i--)
        {
            actor.RemoveEffect(effectsToRemove[i].idType);
        }
        effectsToRemove.Clear();
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        TargetStruct target = new TargetStruct(new Vector2Int(nearestEnemy.x, nearestEnemy.y), nearestEnemy);
        actor.InvokeAbility(ability, target);
    }

    public override bool AbilityMapCheck(Ability ability)
    {
        return false;
    }

    public override bool CheckRequirements(Mob mob, List<AbilityTypeEnum> addedAbils)
    {
        if ((addedAbils.Contains(AbilityTypeEnum.abilAbsorbingShield) || mob.abilities.ContainsKey(AbilityTypeEnum.abilAbsorbingShield)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilForceShot) || mob.abilities.ContainsKey(AbilityTypeEnum.abilForceShot)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilReflectiveBlock) || mob.abilities.ContainsKey(AbilityTypeEnum.abilReflectiveBlock)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilCallArchangel) || mob.abilities.ContainsKey(AbilityTypeEnum.abilCallArchangel)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilHolyRune) || mob.abilities.ContainsKey(AbilityTypeEnum.abilHolyRune)))
            return true;
        else return false;
    }
}
