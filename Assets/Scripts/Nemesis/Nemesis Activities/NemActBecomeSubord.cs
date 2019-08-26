using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NemActBecomeSubordinate : NemesisActivity
{
    public NemActBecomeSubordinate()
    {
        idType = NemesisActivityTypes.ActivityEnum.becomeSubordinate;
    }

    public override bool CheckRequirements(Nemesis nemesis)
    {
        if (nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null && nemesis.superior == null)
            return true;
        else
            return false;
    }

    public override string DescrLineShort(Nemesis target)
    {
        return System.String.Format("Is pledging allegiance to {0}.", target.mob.GetFullName());
    }

    public override Nemesis FindTarget(Nemesis nemesis)
    {
        List<Nemesis> mobs = new List<Nemesis>();
        Nemesis target = null;

        /// find an ally of higher level
        foreach (Nemesis ally in GameManager.instance.nemeses)
        {
            if (ally != nemesis && ally.level > nemesis.level && ally.mob.GetFactionRelation(nemesis.mob.faction))
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
        int r = UnityEngine.Random.Range(0, nemesis.level + target.level);
        if (r < nemesis.level)
        {
            // mob became subordinate to target 
            nemesis.superior = target;
            nemesis.IncreaseLevel(1);
            target.IncreaseLevel(1);
            str += System.String.Format("{0} becomes subordinate to {1}.", nemesis.mob.GetFullName(), target.mob.GetFullName());
        }
        else
        {
            // mob failed to become subordinate to target
            str += System.String.Format("{0} is rejected as a subordinate to {1}.", nemesis.mob.GetFullName(), target.mob.GetFullName());
        }
        nemesis.activity = NemesisActivityTypes.ActivityEnum.none;
        nemesis.activityTarget = null;
        return str;
    }
}
