using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiPackageEnum
{
    aiFindRandomLocation, aiMeleeEnemy, aiUseAbility, aiFindCorpse
}

public delegate bool OnCheckAI(Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies);
public delegate void OnInvokeAI(Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies);

public class AIPackage  {
    public AiPackageEnum idType;
    public int priority;
    public OnCheckAI OnCheckAI;
    public OnInvokeAI OnInvokeAI;
}

public static class AIs
{
    public static Dictionary<AiPackageEnum, AIPackage> aiPackages;

    public static void InitializeAIPackages()
    {
        aiPackages = new Dictionary<AiPackageEnum, AIPackage>();

        Add(AiPackageEnum.aiFindRandomLocation, 3,
            (Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies) =>
            {
                if (nearestEnemy == null) return true;
                else return false;
            },
            (Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies) =>
            {
                int rx;
                int ry;
                bool result = false;
                bool calcPath = false;
                Level level = BoardManager.instance.level;

                if (actor.pathDst.x == actor.x && actor.pathDst.y == actor.y)
                {
                    do
                    {
                        rx = actor.x + Random.Range(0, 20) - 10;
                        ry = actor.y + Random.Range(0, 20) - 10;
                        if (rx >= 0 && ry >= 0 && rx < level.maxX && ry < level.maxY && level.AreCellsConnected(rx, ry, actor.x, actor.y))
                            result = true;
                        else result = false;
                    } while (result == false);
                    actor.SetPathDst(new Vector2Int(rx, ry));
                }

                if (actor.pathDst.x == actor.x && actor.pathDst.y == actor.y)
                {
                    actor.MakeRandomMove();
                    return;
                }

                actor.PlotPathToDst(actor.pathDst);

                result = actor.MoveAlongPath();
                if (!result)
                {
                    actor.MakeRandomMove();
                    actor.pathDst = new Vector2Int(actor.x, actor.y);
                }
            });

        Add(AiPackageEnum.aiMeleeEnemy, 5,
            (Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies) =>
            {
                Level level = BoardManager.instance.level;
                if (nearestEnemy != null &&
                    level.AreCellsConnected(nearestEnemy.x, nearestEnemy.y, actor.x, actor.y))
                    return true;
                else return false;
            },
            (Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies) =>
            {
                bool result = false;

                if (actor.pathDst.x != nearestEnemy.x || actor.pathDst.y != nearestEnemy.y)
                    actor.SetPathDst(new Vector2Int(nearestEnemy.x, nearestEnemy.y));

                if (actor.pathDst.x == actor.x && actor.pathDst.y == actor.y)
                {
                    actor.MakeRandomMove();
                    return;
                }

                actor.PlotPathToDst(actor.pathDst);

                result = actor.MoveAlongPath();
                if (!result)
                {
                    actor.MakeRandomMove();
                    actor.pathDst = new Vector2Int(actor.x, actor.y);
                }
            });

        Add(AiPackageEnum.aiUseAbility, 8,
            (Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies) =>
            {
                Ability ability;
                foreach (AbilityTypeEnum abilityType in actor.abilities.Keys)
                {
                    ability = actor.GetAbility(abilityType);
                    if (ability.id != AbilityTypeEnum.abilNone && !ability.passive && ability.AbilityCheckAI(ability, actor, nearestEnemy, nearestAlly))
                        return true;
                }
                return false;
            },
            (Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies) =>
            {
                List<Ability> abilities = new List<Ability>();
                Ability ability;
                foreach (AbilityTypeEnum abilityType in actor.abilities.Keys)
                {
                    ability = actor.GetAbility(abilityType);
                    if (ability.AbilityCheckAI(ability, actor, nearestEnemy, nearestAlly))
                        abilities.Add(ability);
                }

                int r = Random.Range(0, abilities.Count - 1);
                abilities[r].AbilityInvokeAI(abilities[r], actor, nearestEnemy, nearestAlly);
            });

        Add(AiPackageEnum.aiFindCorpse, 7,
            (Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies) =>
            {
                bool visibleCorpse = false;
                foreach (Item item in actor.visibleItems)
                    if (item.corpsePwr > 0) visibleCorpse = true;

                if (visibleCorpse == true &&
                    (actor.curHP < actor.maxHP || actor.curFP < actor.maxFP))
                    return true;
                else return false;
            },
            (Mob actor, Mob nearestEnemy, Mob nearestAlly, List<Mob> enemies, List<Mob> allies) =>
            {

                Item corpse = null;
                foreach (Item item in actor.visibleItems)
                {
                    if (item.corpsePwr > 0)
                    {
                        if (corpse == null) corpse = item;
                        if (Level.GetDistance(actor.x, actor.y, item.x, item.y) <
                            Level.GetDistance(actor.x, actor.y, corpse.x, corpse.y))
                            corpse = item;
                    }
                }

                actor.SetPathDst(new Vector2Int(corpse.x, corpse.y));

                if (actor.pathDst.x == actor.x && actor.pathDst.y == actor.y)
                {
                    actor.Move(0, 0);
                    return;
                }

                if (actor.path.Count == 0)
                    actor.PlotPathToDst(actor.pathDst);

                bool result = false;
                result = actor.MoveAlongPath();
                if (!result)
                {
                    actor.MakeRandomMove();
                    actor.pathDst = new Vector2Int(actor.x, actor.y);
                }
            });
    }

    public static void Add(AiPackageEnum _idType, int _priority, OnCheckAI _OnCheckAI, OnInvokeAI _OnInvokeAI)
    {
        AIPackage ai = new AIPackage
        {
            idType = _idType,
            priority = _priority,
            OnCheckAI = _OnCheckAI,
            OnInvokeAI = _OnInvokeAI
        };

        aiPackages.Add(_idType, ai);
    }
}
