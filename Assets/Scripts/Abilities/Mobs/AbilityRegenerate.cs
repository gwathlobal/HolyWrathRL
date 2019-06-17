using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRegenerate : Ability
{
    public AbilityRegenerate()
    {
        id = AbilityTypeEnum.abilRegenerate;
        stdName = "Regenerate";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Increase your HP regen by 4 for 10 turns.";
    }

    public override string Name(Mob mob)
    {
        return "Regenerate";
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP;
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
        if (actor.CanInvokeAbility(ability) && ((double)actor.curHP / actor.maxHP <= 0.5) && actor.GetEffect(EffectTypeEnum.effectRegenerate) == null)
            return true;
        else return false;
    }

    public override bool AbilityCheckApplic(Ability ability, Mob mob)
    {
        return true;
    }

    public override void AbilityInvoke(Mob actor, TargetStruct target)
    {
        string str = String.Format("{0} starts to regenerate faster. ", actor.name);
        BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

        actor.mo.BuffDebuff(new Vector2Int(actor.x, actor.y), new Vector2Int(actor.x, actor.y),
                UIManager.instance.demonBuffPrefab, UIManager.instance.demonBuffPrefab,
                () =>
                {
                    actor.AddEffect(EffectTypeEnum.effectRegenerate, actor, 10);
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
