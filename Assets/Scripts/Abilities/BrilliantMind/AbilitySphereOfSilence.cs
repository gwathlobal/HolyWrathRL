using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySphereOfSilence : Ability
{
    public AbilitySphereOfSilence()
    {
        id = AbilityTypeEnum.abilSphereOfSilence;
        stdName = "Sphere of Silence";
        costType = AbilityCostType.wp;
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilBrilliantMind;
    }

    public override string Description(Mob mob)
    {
        return "Silences all visible enemies around you for 8 turns. Silence prevents non-physical abilities to be invoked.";
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
        return 90;
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
        string str = String.Format("{0} invokes Sphere of Silence. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
        foreach (Mob mob in actor.visibleMobs)
        {
            if (mob != actor && !actor.GetFactionRelation(mob.faction))
            {
                str = String.Format("{0} is affected. ", mob.name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(mob.x, mob.y, str);
                mob.AddEffect(EffectTypeEnum.effectSilence, actor, 8);
            }
        }  
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilMindBurn) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMindBurn)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFear) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFear)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilMeditate) || mob.abilities.ContainsKey(AbilityTypeEnum.abilMeditate)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilDominateMind) || mob.abilities.ContainsKey(AbilityTypeEnum.abilDominateMind)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilTrapMind) || mob.abilities.ContainsKey(AbilityTypeEnum.abilTrapMind)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilSplitSoul) || mob.abilities.ContainsKey(AbilityTypeEnum.abilSplitSoul)))
            return true;
        else return false;
    }
}
