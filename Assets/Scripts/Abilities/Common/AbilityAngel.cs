using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAngel : Ability
{

    public AbilityAngel()
    {
        id = AbilityTypeEnum.abilAngel;
        stdName = "Angel";
        passive = true;
        slot = AbilitySlotEnum.abilNone;
        category = AbilityPlayerCategoryEnum.abilCommon;
    }

    public override string Description(Mob mob)
    {
        return "This one belongs to the Thirdborn, the soulful and spiritual beings.";
    }

    public override string Name(Mob mob)
    {
        return stdName;
    }

    public override float Spd(Mob mob)
    {
        return 0;
    }

    public override int Cost(Mob mob)
    {
        return 0;
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
        if (actor.idType == MobTypeEnum.mobAngel)
        {
            List<AbilityTypeEnum> abilities = new List<AbilityTypeEnum>();

            int r = Random.Range(0, 4);

            switch (r)
            {
                // Deadly Rays
                case 1:
                    abilities.Add(AbilityTypeEnum.abilJudgement);
                    abilities.Add(AbilityTypeEnum.abilAmbush);
                    break;
                // Brilliant Mind
                case 2:
                    abilities.Add(AbilityTypeEnum.abilMindBurn);
                    abilities.Add(AbilityTypeEnum.abilFear);
                    break;
                // Holy Bastion
                case 3:
                    abilities.Add(AbilityTypeEnum.abilAbsorbingShield);
                    abilities.Add(AbilityTypeEnum.abilForceShot);
                    break;
                // Fiery Rage
                default:
                    abilities.Add(AbilityTypeEnum.abilFireFists);
                    abilities.Add(AbilityTypeEnum.abilFlamingArrow);
                    break;
            }

            foreach (AbilityTypeEnum abilType in abilities)
            {
                if (!actor.abilities.ContainsKey(abilType))
                    actor.abilities.Add(abilType, true);

                if (AbilityTypes.abilTypes[abilType].slot == AbilitySlotEnum.abilMelee)
                {
                    actor.abilities.Remove(actor.meleeAbil);
                    actor.meleeAbil = abilType;
                }
                if (AbilityTypes.abilTypes[abilType].slot == AbilitySlotEnum.abilRanged)
                {
                    actor.abilities.Remove(actor.rangedAbil);
                    actor.rangedAbil = abilType;
                }
                if (AbilityTypes.abilTypes[abilType].slot == AbilitySlotEnum.abilDodge)
                {
                    actor.abilities.Remove(actor.dodgeAbil);
                    actor.dodgeAbil = abilType;
                }
                if (AbilityTypes.abilTypes[abilType].slot == AbilitySlotEnum.abilBlock)
                {
                    actor.abilities.Remove(actor.blockAbil);
                    actor.blockAbil = abilType;
                }
            }
        }
    }

    public override void AbilityInvokeAI(Ability ability, Mob actor, Mob nearestEnemy, Mob nearestAlly)
    {

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
