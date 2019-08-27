using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NemActHuntDemons : NemesisActivity
{
    public NemActHuntDemons()
    {
        idType = NemesisActivityTypes.ActivityEnum.huntDemons;
    }

    public override bool CheckRequirements(Nemesis nemesis)
    {
        if (nemesis.mob.GetAbility(AbilityTypeEnum.abilAngel) != null)
            return true;
        else
            return false;
    }

    public override string DescrLineShort(Nemesis target)
    {
        return System.String.Format("Is hunting demons.");
    }

    public override Nemesis FindTarget(Nemesis nemesis)
    {
        return nemesis;
    }

    public override void AssignActivity(Nemesis nemesis, Nemesis target)
    {
        nemesis.activity = idType;
        nemesis.activityTarget = target;
    }


    public override string ProcessActivity(Nemesis nemesis, Nemesis target)
    {
        string str = "";

        nemesis.IncreaseLevel(1);
        str += System.String.Format("{0} has hunted demons successfully.", nemesis.mob.GetFullName());

        nemesis.activity = NemesisActivityTypes.ActivityEnum.none;
        nemesis.activityTarget = null;
        return str;
    }
}
