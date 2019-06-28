using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModifierAcidRain : LevelModifier
{

    public LevelModifierAcidRain()
    {
        idType = LevelModifierTypes.LevelModifierEnum.LevModAcidRain;
        name = "Acid rain";
    }

    public override bool CheckRequirements()
    {
        if (GameManager.instance.levelNum >= 2)
            return true;
        else
            return false;
    }

    public override void Initialize()
    {
        return;
    }

    public override string Description()
    {
        string str = "";

        str += System.String.Format("{0}\nAcid frequently drops from the sky here.\n\n", name);

        return str;
    }

    public override void Activate(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        EventAcidRain gameEvent = new EventAcidRain();

        level.gameEvents.Add(gameEvent);
    }
}


