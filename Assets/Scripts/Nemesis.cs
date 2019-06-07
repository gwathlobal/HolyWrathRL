using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Nemesis {

    public enum PersonalStatusEnum
    {
        hidden, revealedAbils, revealedName
    }

    public enum DeathStatusEnum
    {
        alive, deceased
    }

    public static List<string> demonNames;
    public static List<string> angelNames;

    public Mob mob;
    public PersonalStatusEnum personalStatus;
    public DeathStatusEnum deathStatus;

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

        str = String.Format("{0} the {1}\n\n", mob.name, MobTypes.mobTypes[mob.idType].name);

        if (deathStatus == DeathStatusEnum.deceased)
        {
            str += "Deceased.\n\n";
        }

        if (personalStatus == PersonalStatusEnum.revealedAbils)
        {
            str += "Abilities\n\n";
            str += "   Active:\n";
            foreach (AbilityTypeEnum abil in mob.abilities.Keys)
            {
                Ability ability = mob.GetAbility(abil);
                if (ability.id != AbilityTypeEnum.abilNone &&
                    (!ability.passive ||
                    (ability.passive && ability.slot == AbilitySlotEnum.abilMelee)))
                {
                    str += "      " + ability.stdName;
                    str += " (" + ability.Description(mob) + ")\n";
                }
            }
            str += "\n   Passive:\n";
            foreach (AbilityTypeEnum abil in mob.abilities.Keys)
            {
                Ability ability = mob.GetAbility(abil);
                if (ability.passive && ability.slot != AbilitySlotEnum.abilMelee)
                {
                    str += "      " + ability.stdName;
                    str += " (" + ability.Description(mob) + ")\n";
                }
            }
        }
        else if (personalStatus == PersonalStatusEnum.revealedName)
        {
            str += "You know nothing else about this character.\n";
        }

        return str;
    }
    
}
