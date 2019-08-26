using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NemActLeaveSubordinate : NemesisActivity
{
    public NemActLeaveSubordinate()
    {
        idType = NemesisActivityTypes.ActivityEnum.leaveSubordinate;
    }

    public override bool CheckRequirements(Nemesis nemesis)
    {
        if (nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null && nemesis.superior != null && nemesis.superior.level < nemesis.level)
            return true;
        else
            return false;
    }

    public override string DescrLineShort(Nemesis target)
    {
        return System.String.Format("Is leaving the host of {0}.", target.mob.GetFullName());
    }

    public override Nemesis FindTarget(Nemesis nemesis)
    {
        return nemesis.superior;
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
            // mob is no longer subordinate to target 
            nemesis.IncreaseLevel(1);
            str += System.String.Format("{0} is no longer subordinate to {1}.", nemesis.mob.GetFullName(), target.mob.GetFullName());
        }
        else
        {
            // mob failed to leave the target
            nemesis.superior = null;
            target.IncreaseLevel(1);
            str += System.String.Format("{0} failed to leave the host of {1}.", nemesis.mob.GetFullName(), target.mob.GetFullName());
        }
        nemesis.activity = NemesisActivityTypes.ActivityEnum.none;
        nemesis.activityTarget = null;
        return str;
    }
}
