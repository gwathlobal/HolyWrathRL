using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAbsorbingShield : Ability
{
    public AbilityAbsorbingShield()
    {
        id = AbilityTypeEnum.abilAbsorbingShield;
        stdName = "Absorbing Shield";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilBastionHoly;
    }

    public override string Description(Mob mob)
    {
        return "Create a shield on you that absorbs 30 dmg for 8 turns.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP / 2;
    }

    public override int Cost(Mob mob)
    {
        return 40;
    }

    public override bool DoesMapCheck(Mob mob)
    {
        return false;
    }

    public override bool AbilityCheckAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {
        if (actor.CanInvokeAbility(ability) && nearestEnemy != null && actor.GetEffect(EffectTypeEnum.effectAbsorbingShield) == null)
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
        string str = String.Format("{0} invokes an absorbing shield. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.mo.BuffDebuff(new Vector2Int(actor.x, actor.y), new Vector2Int(actor.x, actor.y),
                null, UIManager.instance.absorbShieldPrefab,
                () =>
                {
                    actor.AddEffect(EffectTypeEnum.effectAbsorbingShield, actor, 8);
                    actor.CalculateShieldValue();
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
        return true;
    }
}
