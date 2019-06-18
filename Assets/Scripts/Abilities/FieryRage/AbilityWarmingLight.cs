using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityWarmingLight : Ability
{
    public AbilityWarmingLight()
    {
        id = AbilityTypeEnum.abilWarmingLight;
        stdName = "Warming Light";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilFieryRage;
    }

    public override string Description(Mob mob)
    {
        return "Give yourself Minor Regeneration for 10 turns. Minor Regeneration increases HP regen by 3.";
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

    public override bool AbilityCheckApplic(Ability ability, Mob actor)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} invokes warming light and gains Minor Regeneration. ",
            MobTypes.mobTypes[actor.idType].name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.mo.BuffDebuff(new Vector2Int(actor.x, actor.y), new Vector2Int(actor.x, actor.y),
                UIManager.instance.angelBuffPrefab, UIManager.instance.angelBuffPrefab,
                () =>
                {
                    actor.AddEffect(EffectTypeEnum.effectMinorRegeneration, actor, 10);
                });
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
        if ((addedAbils.Contains(AbilityTypeEnum.abilFireFists) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireFists)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFlamingArrow) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFlamingArrow)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilFireAura) || mob.abilities.ContainsKey(AbilityTypeEnum.abilFireAura)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilBreathOfFire) || mob.abilities.ContainsKey(AbilityTypeEnum.abilBreathOfFire)) &&
            (addedAbils.Contains(AbilityTypeEnum.abilIncineration) || mob.abilities.ContainsKey(AbilityTypeEnum.abilIncineration)))
            return true;
        else return false;
    }
}
