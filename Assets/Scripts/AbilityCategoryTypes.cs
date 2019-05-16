using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityPlayerCategoryEnum
{
    abilMobs, abilCommon, abilDeadlyRays, abilFieryRage, abilBrilliantMind, abilBastionHoly
}

public class AbilityPlayerCategory
{
    public AbilityPlayerCategoryEnum id;
    public string name;
    public string descr;
}

public class AbilityCategoryTypes {

    public static Dictionary<AbilityPlayerCategoryEnum, AbilityPlayerCategory> categories;

    public static void InitializeAbilityCategories()
    {
        categories = new Dictionary<AbilityPlayerCategoryEnum, AbilityPlayerCategory>();

        Add(AbilityPlayerCategoryEnum.abilCommon, "Common Abilities", "Common Abilities.\n\nThe most common abilities available to you.");
        Add(AbilityPlayerCategoryEnum.abilDeadlyRays, "Deadly Rays", "Deadly Rays.\n\nThis skill tree is dedicated to quickly dive in, kill the enemy and withdraw.");
        Add(AbilityPlayerCategoryEnum.abilFieryRage, "Fiery Rage", "Fiery Rage.\n\nThis skill tree is focused on raw fire damage, be it direct or over time.");
        Add(AbilityPlayerCategoryEnum.abilBrilliantMind, "Brilliant Mind", "Brilliant Mind.\n\nUse this skill tree to manipulate the minds of your enemies and control the flow of battle.");
        Add(AbilityPlayerCategoryEnum.abilBastionHoly, "Bastion of Holiness", "Bastion of Holiness.\n\nWith this tree, specialize in shielding yourself against the enemies during the battle.");
    }

    public static void Add(AbilityPlayerCategoryEnum _id, string _name, string _descr)
    {
        AbilityPlayerCategory ac = new AbilityPlayerCategory()
        {
            id = _id,
            name = _name,
            descr = _descr
        };
        categories.Add(_id, ac);
    }

}
