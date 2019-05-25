using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMob : Mob {

    public Dictionary<AbilityPlayerCategoryEnum, int> categoryBonus;

    public PlayerMob(MobTypeEnum _idType, int _x, int _y) : base(_idType, _x, _y)
    {
        categoryBonus = new Dictionary<AbilityPlayerCategoryEnum, int>();
        foreach (AbilityPlayerCategoryEnum abilityCategory in System.Enum.GetValues(typeof(AbilityPlayerCategoryEnum)))
        {
            categoryBonus[abilityCategory] = 0;
        }
    }

    public override void AiFunction()
    {
        
        //Debug.Log("Player AI Function");
        
        BoardManager.instance.playersTurn = true;
        GetFOV();
        //Debug.Log("Player AI Function: Time Elapsed Since Last Turn = " + (Time.time - BoardManager.instance.prevTime));
    }

    public override void GetFOV()
    {
        Level level = BoardManager.instance.level;
        visibleMobs.Clear();
        visibleItems.Clear();
        visibleFeatures.Clear();

        for (int y = 0; y < level.maxY; y++)
        {
            for (int x = 0; x < level.maxX; x++)
            {
                level.visible[x, y] = false;
                BoardManager.instance.tiles[x, y].GetComponent<SpriteRenderer>().color = TerrainTypes.terrainTypes[level.terrain[x, y]].color;
                BoardManager.instance.fog[x, y].SetActive(true);
            }
        }

        LOS_FOV.DrawFOV(x, y, visionRadius, 
            (int dx,int dy,int pdx,int pdy) => 
            {
                level.visible[dx, dy] = true;
                BoardManager.instance.fog[dx, dy].SetActive(false);
                BoardManager.instance.unexplored[dx, dy].SetActive(false);

                if (level.mobs[dx, dy] != null && !visibleMobs.Contains(level.mobs[dx, dy]))
                {
                    visibleMobs.Add(level.mobs[dx, dy]);
                }

                if (level.items[dx, dy].Count > 0)
                {
                    foreach (Item item in level.items[dx, dy])
                    {
                        if (!visibleItems.Contains(item)) visibleItems.Add(item);
                    }
                }
                if (level.features[dx, dy].Count > 0)
                {
                    foreach (Feature feature in level.features[dx, dy])
                    {
                        if (!visibleFeatures.Contains(feature)) visibleFeatures.Add(feature);
                    }
                }

                if (TerrainTypes.terrainTypes[level.terrain[dx,dy]].blocksVision) return false;
                return true;
            });

        foreach (Feature feature in visibleFeatures)
        {
            if (feature.go == null)
                BoardManager.instance.tiles[feature.x, feature.y].GetComponent<SpriteRenderer>().color = FeatureTypes.featureTypes[feature.idType].color;
        }
        
    }

    public static void QuitGame()
    {
        BoardManager.instance.msgLog.AddMsg("\n\nYou died!");
        BoardManager.instance.msgLog.FinalizeMsg();
        BoardManager.instance.playersTurn = true;
        UIManager.instance.ShowYouDiedWindow();
    }

    /*
    public override string Description()
    {
        string str = "";
        bool noAbilities;
        str += String.Format("{0}\n", name);
        str += String.Format("HP: {0}/{1}\n", curHP, maxHP);
        str += String.Format("FP: {0}/{1}\n", curFP, maxFP);
        str += String.Format("WP: {0}\n", curWP);

        bool hasArmor = false;
        str += "\nDamage Reduction (DR):\n";
        foreach (DmgType dmgType in DmgTypes.dmgTypes.Values)
        {
            if (armorDR[dmgType.dmgType] > 0 || armorPR[dmgType.dmgType] > 0)
            {
                hasArmor = true;
                str += String.Format("   {0}: {1} pts/{2}%\n", dmgType.name.Substring(0, 1).ToUpper() + dmgType.name.Substring(1), armorDR[dmgType.dmgType], armorPR[dmgType.dmgType]);
            }
        }
        if (!hasArmor)
            str += "   None.\n";

        str += "\n";
        str += "-----------------------------------\n";
        str += "CURRENT ABILITIES\n";
        str += "-----------------------------------\n";
        str += "\n";
        noAbilities = true;
        if (GetAbility(meleeAbil) != null && GetAbility(meleeAbil).id != AbilityTypeEnum.abilNone)
        {
            Ability ability = GetAbility(meleeAbil);
            str += ability.GetFullDescription(this);
            str += "\n\n";
            noAbilities = false;
        }
        if (GetAbility(rangedAbil) != null && GetAbility(rangedAbil).id != AbilityTypeEnum.abilNone)
        {
            Ability ability = GetAbility(rangedAbil);
            str += ability.GetFullDescription(this);
            str += "\n\n";
            noAbilities = false;
        }
        foreach (AbilityTypeEnum abilityType in curAbils)
        {
            Ability ability = GetAbility(abilityType);
            if (ability != null && ability.id != AbilityTypeEnum.abilNone)
            {
                str += ability.GetFullDescription(this);
                str += "\n\n";
                noAbilities = false;
            }
        }
        if (GetAbility(dodgeAbil) != null && GetAbility(dodgeAbil).id != AbilityTypeEnum.abilNone)
        {
            Ability ability = GetAbility(dodgeAbil);
            str += ability.GetFullDescription(this);
            str += "\n\n";
            noAbilities = false;
        }
        if (GetAbility(blockAbil) != null && GetAbility(blockAbil).id != AbilityTypeEnum.abilNone)
        {
            Ability ability = GetAbility(blockAbil);
            str += ability.GetFullDescription(this);
            str += "\n\n";
            noAbilities = false;
        }
        if (noAbilities) str += "No abilities.\n\n";

        str += "\n";
        str += "-----------------------------------\n";
        str += "PASSIVE ABILITIES\n";
        str += "-----------------------------------\n";
        str += "\n";
        noAbilities = true;
        foreach (AbilityTypeEnum abilityType in abilities.Keys)
        {
            Ability ability = GetAbility(abilityType);
            if (ability.passive && ability.slot != AbilitySlotEnum.abilMelee)
            {
                str += GetAbility(abilityType).GetFullDescription(this);
                str += "\n\n";
                noAbilities = false;
            }
        }

        if (noAbilities) str += "No abilities.\n\n";

        str += "\n";
        str += "-----------------------------------\n";
        str += "EFFECTS\n";
        str += "-----------------------------------\n";
        str += "\n";
        noAbilities = true;
        foreach (Effect eff in effects.Values)
        {

            str += String.Format("<color=#{2}>{0}{1}</color>\n", EffectTypes.effectTypes[eff.idType].name,
                        (eff.cd == Effect.CD_UNLIMITED) ? "" : String.Format(" ({0} {1} left)", eff.cd, (eff.cd > 1) ? "turns" : "turn"),
                        ColorUtility.ToHtmlStringRGBA(EffectTypes.effectTypes[eff.idType].color));
            str += String.Format("{0}.", EffectTypes.effectTypes[eff.idType].descr);
            str += "\n\n";
            noAbilities = false;
        }

        if (noAbilities) str += "No active effects.\n\n";

        return str;
    }
    */
}