using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NemActAssassinate : NemesisActivity
{
    public NemActAssassinate()
    {
        idType = NemesisActivityTypes.ActivityEnum.assassinate;
    }

    public override bool CheckRequirements(Nemesis nemesis)
    {
        if (nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null)
            return true;
        else
            return false;
    }

    public override string DescrLineShort(Nemesis target)
    {
        return System.String.Format("Is assassinating {0}.", target.mob.GetFullName());
    }

    public override Nemesis FindTarget(Nemesis nemesis)
    {
        List<Nemesis> mobs = new List<Nemesis>();
        Nemesis target = null;

        // find an ally of higher level
        foreach (Nemesis ally in GameManager.instance.nemeses)
        {
            if (ally != nemesis && ally.level > nemesis.level && ally.mob.GetFactionRelation(nemesis.mob.faction) && nemesis.superior != ally)
            {
                mobs.Add(ally);
            }
        }

        if (mobs.Count > 0)
            target = mobs[Random.Range(0, mobs.Count)];

        return target;
    }
    public override void AssignActivity(Nemesis nemesis, Nemesis target)
    {
        nemesis.activity = idType;
        nemesis.activityTarget = target;
    }


    public override string ProcessActivity(Nemesis nemesis, Nemesis target)
    {
        string str = "";
        int r = Random.Range(0, nemesis.level + target.level);
        if (r < nemesis.level)
        {
            // target is killed
            target.mob.curHP = 0;
            target.mob.alreadyDied = true;
            target.deathStatus = Nemesis.DeathStatusEnum.deceased;
            nemesis.IncreaseLevel(1);
            str += System.String.Format("{0} assassinates {1}.", nemesis.mob.GetFullName(), target.mob.GetFullName());
        }
        else
        {
            // attacker is killed
            nemesis.mob.curHP = 0;
            nemesis.mob.alreadyDied = true;
            nemesis.deathStatus = Nemesis.DeathStatusEnum.deceased;
            target.IncreaseLevel(1);
            str += System.String.Format("{0} is killed while trying to assassinate {1}.", nemesis.mob.GetFullName(), target.mob.GetFullName());
        }
        nemesis.activity = NemesisActivityTypes.ActivityEnum.none;
        nemesis.activityTarget = null;
        return str;
    }
}
