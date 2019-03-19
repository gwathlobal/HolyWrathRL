using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum NemesisStatusEnum
{
    hidden, revealedName, revealedAbils
}

public class Nemesis {

    public static List<string> names;

    public Mob mob;
    public NemesisStatusEnum status;

    public static void InitializeNames()
    {
        names = new List<string>()
        { "Amon", "Abaddon", "Agares", "Haborym", "Alastor", "Allocer", "Amaymon", "Amdusias", "Andras", "Amdusias", "Andromalius", "Anzu", "Asmodeus", "Astaroth",
            "Bael", "Balam", "Barbatos", "Bathin", "Beelzebub", "Behemoth", "Beleth", "Belial", "Belthegor", "Berith", "Bifrons", "Botis", "Buer", "Cacus", "Cerberus",
            "Mastema", "Melchiresus", "Moloch", "Onoskelis", "Shedim", "Xaphan", "Ornias", "Mammon", "Lix Tetrax", "Nybbas", "Focalor", "Furfur", "Gaap", "Geryon",
            "Haures", "Ipos", "Jezebeth", "Kasdeya", "Kobal", "Malphas", "Melchom", "Mullin", "Naberius", "Nergal", "Nicor", "Nysrogh", "Oriax", "Paymon", "Philatnus",
            "Pruflas", "Raum", "Rimmon", "Ronove", "Ronwe", "Shax", "Shalbriri", "Sonellion", "Stolas", "Succorbenoth", "Thamuz", "Ukobach", "Uphir", "Uvall", "Valafar",
            "Vepar", "Verdelet", "Verin", "Xaphan", "Zagan", "Zepar", "Ioz", "Zohadam", "Hardaz" };
    }

    public string GetNemesisDescription()
    {
        string str = "";
        str = String.Format("{0} the {1}\n\nAbilities\n", mob.name, MobTypes.mobTypes[mob.idType].name);
        foreach (AbilityTypeEnum abil in mob.abilities.Keys)
        {
            if (abil != AbilityTypeEnum.abilNone)
            {
                Ability ability = AbilityTypes.abilTypes[abil];
                str += "   " + ability.stdName;
                str += " (" + ability.Description(mob) + ")\n";
            }
        }
           
        return str;
    }
    
}
