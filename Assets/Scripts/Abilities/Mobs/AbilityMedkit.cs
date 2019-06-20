using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMedkit : Ability
{
    public AbilityMedkit()
    {
        id = AbilityTypeEnum.abilMedkit;
        stdName = "Medkit";
        passive = false;
        slot = AbilitySlotEnum.abilNormal;
        category = AbilityPlayerCategoryEnum.abilMobs;
    }

    public override string Description(Mob mob)
    {
        return "Heal self for 20 HP.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return MobType.NORMAL_AP * 2;
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
        if (actor.CanInvokeAbility(ability) &&
                    ((nearestEnemy == null && (double)actor.curHP < actor.maxHP)
                    ||
                    (nearestEnemy != null && (double)actor.curHP / actor.maxHP <= 0.4)))
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
        actor.mo.BuffDebuff(new Vector2Int(actor.x, actor.y), new Vector2Int(actor.x, actor.y),
                null, UIManager.instance.medkitPrefab,
                () =>
                {
                    int HPhealed;
                    if (actor.curHP + 20 > actor.maxHP) HPhealed = actor.maxHP - actor.curHP;
                    else HPhealed = 20;

                    string str = String.Format("{0} uses a medkit to heal himself for {1} HP. ",
                        MobTypes.mobTypes[actor.idType].name,
                        HPhealed);
                    BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);

                    actor.curHP += HPhealed;
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
