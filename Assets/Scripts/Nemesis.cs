using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum NemesisStatusEnum
{
    hidden, revealedName, revealedAbils
}

public class Nemesis {

    public static List<string> demonNames;
    public static List<string> angelNames;

    public Mob mob;
    public NemesisStatusEnum status;

    public static void InitializeNames()
    {
        demonNames = new List<string>()
        { "Amon", "Abaddon", "Agares", "Haborym", "Alastor", "Allocer", "Amaymon", "Amdusias", "Andras", "Amdusias", "Andromalius", "Anzu", "Asmodeus", "Astaroth",
            "Bael", "Balam", "Barbatos", "Bathin", "Beelzebub", "Behemoth", "Beleth", "Belial", "Belthegor", "Berith", "Bifrons", "Botis", "Buer", "Cacus", "Cerberus",
            "Mastema", "Melchiresus", "Moloch", "Onoskelis", "Shedim", "Xaphan", "Ornias", "Mammon", "Lix Tetrax", "Nybbas", "Focalor", "Furfur", "Gaap", "Geryon",
            "Haures", "Ipos", "Jezebeth", "Kasdeya", "Kobal", "Malphas", "Melchom", "Mullin", "Naberius", "Nergal", "Nicor", "Nysrogh", "Oriax", "Paymon", "Philatnus",
            "Pruflas", "Raum", "Rimmon", "Ronove", "Ronwe", "Shax", "Shalbriri", "Sonellion", "Stolas", "Succorbenoth", "Thamuz", "Ukobach", "Uphir", "Uvall", "Valafar",
            "Vepar", "Verdelet", "Verin", "Xaphan", "Zagan", "Zepar", "Ioz", "Zohadam", "Hardaz" };

        angelNames = new List<string>()
        { "Barachiel", "Jegudiel", "Muriel", "Pahaliah", "Selaphiel", "Zachariel", "Adriel", "Ambriel", "Camael", "Cassiel", "Daniel", "Eremiel", "Hadraniel", "Haniel",
            "Hesediel", "Jehoel", "Jerahmeel", "Jophiel", "Kushiel", "Leliel", "Metatron", "Nanael", "Nithael", "Netzach", "Ophaniel", "Puriel", "Qaphsiel", "Raziel",
            "Remiel", "Rikbiel", "Sachiel", "Samael", "Sandalphon", "Seraphiel", "Shamsiel", "Tzaphqiel", "Uriel", "Uzziel", "Vehuel", "Zophiel", "Azazel", "Azrael",
            "Sariel", "Gabriel", "Raphael", "Michael" };
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
