using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nemesis {

    public static int MAX_ANGEL_NEMESIS = 10;
    public static int MAX_DEMON_NEMESIS = 20;

    public enum PersonalStatusEnum
    {
        hidden, revealedAbils, revealedName
    }

    public enum DeathStatusEnum
    {
        alive, deceased
    }

    public Nemesis superior;
    public Nemesis activityTarget;
    public NemesisActivityTypes.ActivityEnum activity;

    public static List<string> demonNames;
    public static List<string> angelNames;

    public Mob mob;
    public PersonalStatusEnum personalStatus;
    public DeathStatusEnum deathStatus;
    public bool revealedLocation;

    public int level = 0;

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

    public static Nemesis CreateDemonNemesis()
    {
        int r = UnityEngine.Random.Range(0, 2);
        MobTypeEnum mobType;
        switch (r)
        {
            case 1:
                mobType = MobTypeEnum.mobArchdemon;
                break;
            default:
                mobType = MobTypeEnum.mobArchdevil;
                break;
        }

        Nemesis nemesis = new Nemesis()
        {
            mob = new Mob(mobType, 1, 1),
            personalStatus = Nemesis.PersonalStatusEnum.hidden,
            deathStatus = Nemesis.DeathStatusEnum.alive,
            revealedLocation = false
        };
        GameObject.Destroy(nemesis.mob.go);

        nemesis.activity = NemesisActivityTypes.ActivityEnum.none;
        nemesis.AssignRandomActivity();

        return nemesis;
    }

    public static Nemesis CreateAngelNemesis()
    {
        Nemesis nemesis = new Nemesis()
        {
            mob = new Mob(MobTypeEnum.mobAngel, 1, 1),
            personalStatus = Nemesis.PersonalStatusEnum.revealedAbils,
            deathStatus = Nemesis.DeathStatusEnum.alive
        };
        GameObject.Destroy(nemesis.mob.go);

        nemesis.activity = NemesisActivityTypes.ActivityEnum.none;
        nemesis.AssignRandomActivity();

        return nemesis;
    }

    public string GetNemesisDescription()
    {
        string str = "";

        str = String.Format("{0}\nLevel: {1}\n\n", mob.GetFullName(), level);

        if (deathStatus == DeathStatusEnum.deceased)
        {
            str += String.Format("{0}\n\n", mob.killedBy);
        }
        else
        {
            if (revealedLocation)
            {
                str += String.Format("You know the location of this creature.\n\n");
            }

            if (superior != null)
            {
                str += String.Format("Is subordinate to {0} the {1}.\n\n", superior.mob.name, MobTypes.mobTypes[superior.mob.idType].name);
            }

            if (activity != NemesisActivityTypes.ActivityEnum.none)
                str += String.Format("{0}\n\n", NemesisActivityTypes.nemesisActivities[activity].DescrLineShort(activityTarget)); 
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

    public void AssignRandomActivity()
    {
        if (activity == NemesisActivityTypes.ActivityEnum.none)
        {
            if (UnityEngine.Random.Range(0, 100) < 50)
            {

                List<NemesisActivityTypes.ActivityEnum> availActivities = new List<NemesisActivityTypes.ActivityEnum>();
                foreach (NemesisActivity curActivity in NemesisActivityTypes.nemesisActivities.Values)
                {
                    if (curActivity.CheckRequirements(this))
                    {
                        availActivities.Add(curActivity.idType);
                    }
                }
                if (availActivities.Count > 0)
                {
                    activity = availActivities[UnityEngine.Random.Range(0, availActivities.Count)];
                    activityTarget = NemesisActivityTypes.nemesisActivities[activity].FindTarget(this);
                    if (activityTarget == null)
                        activity = NemesisActivityTypes.ActivityEnum.none;
                    else
                        NemesisActivityTypes.nemesisActivities[activity].AssignActivity(this, activityTarget);
                    
                }
            }
        }
    }

    public string ProcessActivity()
    {
        string str = "";
        if (activity != NemesisActivityTypes.ActivityEnum.none)
        {
            str = NemesisActivityTypes.nemesisActivities[activity].ProcessActivity(this, activityTarget);
        }

        return str;
    }

    public void IncreaseLevel(int step)
    {
        level += step;
        mob.maxHP = MobTypes.mobTypes[mob.idType].maxHP + level * 5;
        mob.maxFP = MobTypes.mobTypes[mob.idType].maxFP + level * 5;

        if (mob.idType == MobTypeEnum.mobAngel && !mob.CheckDead())
        {
            //MobTypes.UpgradeAngel(mob);
        }
    }
}
