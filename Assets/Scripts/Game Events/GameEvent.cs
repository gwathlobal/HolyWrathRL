using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent {

    public GameEventTypes.GameEventEnum idType;
    public string name;

    public virtual string Description()
    {
        return "";
    }

    public virtual bool CheckEvent()
    {
        return false;
    }

    public virtual void Activate()
    {
        return;
    }

    public virtual string LineDescription()
    {
        return "";
    }

    public virtual void Initialize()
    {
        return;
    }
}

public static class GameEventTypes
{
    public enum GameEventEnum
    {
        eventCollapseLevel, eventAcidRain, eventShrinkLevel
    }

    public static Dictionary<GameEventEnum, GameEvent> gameEvents;

    static GameEventTypes()
    {
        gameEvents = new Dictionary<GameEventEnum, GameEvent>();

        Add(new EventLevelCollapse());
        Add(new EventAcidRain());
        Add(new EventLevelShrink());
    }

    public static void Add(GameEvent ge)
    {
        gameEvents.Add(ge.idType, ge);
    }
}