using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModifierLevelShrink : LevelModifier
{

    int turnsLeft;

    public LevelModifierLevelShrink()
    {
        idType = LevelModifierTypes.LevelModifierEnum.LevModLevelShrink;
        name = "Shrinking dimension";
    }

    public override bool CheckRequirements()
    {
        if (GameManager.instance.levelNum >= 4)
            return true;
        else
            return false;
    }

    public override void Initialize()
    {
        turnsLeft = 80;
    }

    public override string Description()
    {
        string str = "";

        str += System.String.Format("{0}\nThis pocket dimension will be gradually shrinking until it completely collapses in {1} turns killing everybody inside. Complete your objective before the counter expires.\n\n", name, turnsLeft);

        return str;
    }

    public override void Activate(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        EventLevelShrink gameEvent = new EventLevelShrink();
        gameEvent.initTurns = turnsLeft;
        gameEvent.turnsLeft = turnsLeft;
        gameEvent.level = level;

        gameEvent.Initialize();

        level.gameEvents.Add(gameEvent);
    }
}


