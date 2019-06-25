using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLevelCollapse : GameEvent
{
    public int turnsLeft = 0;

    public EventLevelCollapse()
    {
        idType = GameEventTypes.GameEventEnum.eventCollapseLevel;
        name = "Collapsing dimension";
    }

    public override void Activate()
    {
        turnsLeft--;

        // kill everybody on the level when the time runs out
        if (turnsLeft <= 0)
        {
            string str = System.String.Format("The pocket dimension is collapsing around you! ");
            BoardManager.instance.msgLog.PlayerVisibleMsg(BoardManager.instance.player.x, BoardManager.instance.player.y, str);
            BoardManager.instance.msgLog.FinalizeMsg();

            foreach (Mob mob in BoardManager.instance.mobs.Values)
            {
                if (!mob.CheckDead())
                {
                    mob.curHP = 0;
                    mob.MakeDead(null, false, true, false, "Obliterated by a collapsed dimension.");
                }
            }
        }
    }

    public override bool CheckEvent()
    {
        return true;
    }

    public override string Description()
    {
        string str = System.String.Format("The level will collapse in {0} {1}", turnsLeft, (turnsLeft > 1) ? "turns" : "turn");
        return str;
    }

    public override void Initialize()
    {
        turnsLeft = 30;
    }

    public override string LineDescription()
    {
        string str = System.String.Format("Turns left before collapse: {0}", turnsLeft);
        return str;
    }
}