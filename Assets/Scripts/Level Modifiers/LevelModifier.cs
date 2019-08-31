using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class LevelModifierTypes
{
    public enum LevelModifierEnum
    {
        LevModAngel, LevModDemon, LevModLevelCollapse, LevModAcidRain, LevModLevelShrink, LevModDemonDuel
    }

    public static Dictionary<LevelModifierEnum, LevelModifier> levelModifiers;

    static LevelModifierTypes()
    {
        levelModifiers = new Dictionary<LevelModifierEnum, LevelModifier>();

        Add(new LevelModifierAngel());
        Add(new LevelModifierDemon());
        Add(new LevelModifierLevelCollapse());
        Add(new LevelModifierAcidRain());
        Add(new LevelModifierLevelShrink());
        Add(new LevelModifierDemonDuel());
    }

    public static void Add(LevelModifier lm)
    {
        levelModifiers.Add(lm.idType, lm);
    }
}

public class LevelModifier
{
    public LevelModifierTypes.LevelModifierEnum idType;
    public string name;

    public virtual bool CheckRequirements()
    {
        return false;
    }

    public virtual void Initialize()
    {
        return;
    }

    public virtual string Description()
    {
        return "";
    }

    public virtual void Activate(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        return;
    }
}


