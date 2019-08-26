using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NemesisActivityDuel : NemesisActivity
{
    public NemesisActivityDuel()
    {
        idType = NemesisActivityTypes.ActivityEnum.duel;
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
        return System.String.Format("Is dueling {0}.", target.mob.GetFullName());
    }

    public override Nemesis FindTarget(Nemesis nemesis)
    {
        List<Nemesis> mobs = new List<Nemesis>();
        Nemesis target = null;

        // find an ally of similar level
        foreach (Nemesis ally in GameManager.instance.nemeses)
        {
            if (ally != nemesis && System.Math.Abs(ally.level - nemesis.level) <= 2 && ally.mob.GetFactionRelation(nemesis.mob.faction))
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
        target.activity = idType;
        target.activityTarget = nemesis;
    }

    public override string ProcessActivity(Nemesis nemesis, Nemesis target)
    {
        string str = "";
        int r = Random.Range(0, nemesis.level + target.level);
        if (r < nemesis.level)
        {
            // target is defeated
            nemesis.IncreaseLevel(1);
            str += System.String.Format("{0} defeated {1} in a duel.", nemesis.mob.GetFullName(), target.mob.GetFullName());
        }
        else
        {
            // attacker is defeated
            target.IncreaseLevel(1);
            str += System.String.Format("{0} failed to defeat {1} in a duel.", nemesis.mob.GetFullName(), target.mob.GetFullName());
        }
        nemesis.activity = NemesisActivityTypes.ActivityEnum.none;
        nemesis.activityTarget = null;
        target.activity = NemesisActivityTypes.ActivityEnum.none;
        target.activityTarget = null;
        return str;
    }
}
