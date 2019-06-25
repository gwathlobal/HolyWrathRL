using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModifierLevelCollapse : LevelModifier
{

    int turnsLeft;

    public LevelModifierLevelCollapse()
    {
        idType = LevelModifierTypes.LevelModifierEnum.LevModLevelCollapse;
        name = "Collapsing dimension";
    }

    public override bool CheckRequirements()
    {
        if (GameManager.instance.levelNum >= 6)
            return true;
        else
            return false;
    }

    public override void Initialize()
    {
        turnsLeft = 100;
    }

    public override string Description()
    {
        string str = "";

        str += System.String.Format("{0}\nThis pocket dimension is unstable and will collapse in {1} turns killing everybody inside. Complete your objective before the counter expires.\n\n", name, turnsLeft);

        return str;
    }

    public override void Activate(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        EventLevelCollapse gameEvent = new EventLevelCollapse();
        gameEvent.turnsLeft = turnsLeft;

        level.gameEvents.Add(gameEvent);
    }
}


