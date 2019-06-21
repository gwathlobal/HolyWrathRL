using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate Feature FeatCheckMerge(Level level, Feature newFeature);
public delegate void FeatMergeFunc(Level level, Feature newFeature, Feature oldFeature);
public delegate void FeatOnTick(Level level, Feature feature);

public enum FeatureTypeEnum
{
    featBloodDrop, featBloodPool, featFire, featHolyRune, featAcidCloud, featArtilleryTarget
};

public class FeatureType {
    public FeatureTypeEnum id;
    public string name = "Feature Template";
    public string descr = "";
    public GameObject prefab;
    public Color color;

    public FeatCheckMerge FeatCheckMerge;
    public FeatMergeFunc FeatMergeFunc;
    public FeatOnTick FeatOnTick;

    public FeatureType(string _name, string _descr, GameObject _prefab, Color _color, FeatCheckMerge _checkMerge = null, FeatMergeFunc _mergeFunc = null, FeatOnTick _tickFunc = null)
    {
        name = _name;
        descr = _descr;
        prefab = _prefab;
        color = _color;
        FeatMergeFunc = _mergeFunc;
        FeatCheckMerge = _checkMerge;
        FeatOnTick = _tickFunc;
    }
}

public class FeatureTypes
{

    public static GameObject featBloodPool;
    public static GameObject featFire;
    public static GameObject featHolyRune;
    public static GameObject featAcidCloud;
    public static GameObject featArtilleryTarget;
    public static Dictionary<FeatureTypeEnum, FeatureType> featureTypes;

    public static void InitializeFeatureTypes()
    {
        featBloodPool = Resources.Load("Prefabs/Features/BloodPool") as GameObject;
        featFire = Resources.Load("Prefabs/Features/Fire") as GameObject;
        featHolyRune = Resources.Load("Prefabs/Features/Holy Rune") as GameObject;
        featAcidCloud = Resources.Load("Prefabs/Features/Acid Cloud") as GameObject;
        featArtilleryTarget = Resources.Load("Prefabs/Features/Target Dot") as GameObject;

        featureTypes = new Dictionary<FeatureTypeEnum, FeatureType>();

        Add(FeatureTypeEnum.featBloodDrop, "Bloodstain", "", null, new Color(255, 0, 0),
            (Level level, Feature newFeature) =>
            {
                foreach (Feature feature in level.features[newFeature.x, newFeature.y])
                {
                    if (feature.idType == FeatureTypeEnum.featBloodDrop || feature.idType == FeatureTypeEnum.featBloodPool)
                        return feature;
                }
                return null;
            },
            (Level level, Feature newFeature, Feature oldFeature) =>
            {
                level.featureList.Remove(newFeature);
                BoardManager.instance.RemoveFeatureFromWorld(newFeature);
            });
        Add(FeatureTypeEnum.featBloodPool, "Bloodstain", "", featBloodPool, featBloodPool.GetComponent<SpriteRenderer>().color,
            (Level level, Feature newFeature) =>
            {
                foreach (Feature feature in level.features[newFeature.x, newFeature.y])
                {
                    if (feature.idType == FeatureTypeEnum.featBloodDrop || feature.idType == FeatureTypeEnum.featBloodPool)
                        return feature;
                }
                return null;
            },
            (Level level, Feature newFeature, Feature oldFeature) =>
            {
                if (oldFeature.idType == FeatureTypeEnum.featBloodPool)
                {
                    level.featureList.Remove(newFeature);
                    BoardManager.instance.RemoveFeatureFromWorld(newFeature);
                }
                else
                {
                    level.RemoveFeatureFromLevel(oldFeature);
                    BoardManager.instance.RemoveFeatureFromWorld(oldFeature);
                    level.AddFeatureToLevel(newFeature, newFeature.x, newFeature.y);
                }
                
            });
        Add(FeatureTypeEnum.featFire, "Fire", "Deals 5 fire damage to the creature standing here. Adds the Burning effect for 5 turns. Can spread to nearby tiles that can burn.", featFire, featFire.GetComponent<SpriteRenderer>().color,
            (Level level, Feature newFeature) =>
            {
                foreach (Feature feature in level.features[newFeature.x, newFeature.y])
                {
                    if (feature.idType == FeatureTypeEnum.featFire)
                        return feature;
                }
                return null;
            },
            (Level level, Feature newFeature, Feature oldFeature) =>
            {
                oldFeature.counter += newFeature.counter;
                level.featureList.Remove(newFeature);
                BoardManager.instance.RemoveFeatureFromWorld(newFeature);

            },
            (Level level, Feature feature) =>
            {
                if (level.mobs[feature.x, feature.y] != null)
                {
                    Mob mob = level.mobs[feature.x, feature.y];
                    int dmg = 0;
                    dmg += Mob.InflictDamage(null, mob,
                        new Dictionary<DmgTypeEnum, int>()
                        {
                            { DmgTypeEnum.Fire, 5 }
                        }, 
                        (int dmg1) =>
                        {
                            string str;
                            if (dmg1 <= 0)
                            {
                                str = String.Format("{0} takes no fire dmg. ",
                                    mob.name);
                            }
                            else
                            {
                                str = String.Format("{0} takes {1} fire dmg. ",
                                    mob.name,
                                    dmg1);
                            }
                            return str;
                        });
                    mob.AddEffect(EffectTypeEnum.effectBurning, null, 5);
                    if (BoardManager.instance.level.visible[mob.x, mob.y])
                    {
                        Vector3 pos = new Vector3(mob.x, mob.y, 0);
                        UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", pos);
                    }
                    BoardManager.instance.CreateBlooddrop(mob.x, mob.y);
                    if (mob.CheckDead())
                    {
                        mob.MakeDead(null, true, true, false);
                    }
                }

                // change terrain tile
                if (TerrainTypes.terrainTypes[level.terrain[feature.x, feature.y]].catchesFire > 0)
                {
                    level.terrain[feature.x, feature.y] = TerrainTypes.terrainTypes[level.terrain[feature.x, feature.y]].burnsToTerrain;
                }

                // spread to neighbouring tiles
                level.CheckSurroundings(feature.x, feature.y, false,
                    (int x, int y) =>
                    {
                        if (UnityEngine.Random.Range(0,100) <= 20 && TerrainTypes.terrainTypes[level.terrain[x, y]].catchesFire > 0)
                        {
                            Feature fire = new Feature(FeatureTypeEnum.featFire, x, y);
                            fire.counter = TerrainTypes.terrainTypes[level.terrain[x, y]].catchesFire;
                            level.terrain[x, y] = TerrainTypes.terrainTypes[level.terrain[x, y]].burnsToTerrain;
                            BoardManager.instance.level.AddFeatureToLevel(fire, fire.x, fire.y);
                        }
                    });

                feature.counter--;
                if (feature.counter <= 0)
                {
                    level.RemoveFeatureFromLevel(feature);
                    //level.featureList.Remove(feature);
                    BoardManager.instance.featuresToRemove.Add(feature);
                    //BoardManager.instance.RemoveFeatureFromWorld(feature);
                }
            });

        Add(FeatureTypeEnum.featHolyRune, "Holy Rune", "Deals 3 holy damage to beasts and demons, gives +3 HP and +3 FP to angels standing here.", featHolyRune, featHolyRune.GetComponent<SpriteRenderer>().color,
            (Level level, Feature newFeature) =>
            {
                foreach (Feature feature in level.features[newFeature.x, newFeature.y])
                {
                    if (feature.idType == FeatureTypeEnum.featHolyRune)
                        return feature;
                }
                return null;
            },
            (Level level, Feature newFeature, Feature oldFeature) =>
            {
                oldFeature.counter = newFeature.counter;
                level.featureList.Remove(newFeature);
                BoardManager.instance.RemoveFeatureFromWorld(newFeature);

            },
            (Level level, Feature feature) =>
            {
                if (level.mobs[feature.x, feature.y] != null && level.mobs[feature.x, feature.y].faction == FactionEnum.factionAngels)
                {
                    Mob mob = level.mobs[feature.x, feature.y];
                    mob.curHP += 3;
                    if (mob.curHP > mob.maxHP)
                        mob.curHP = mob.maxHP;
                    mob.curFP += 3;
                    if (mob.curFP > mob.maxFP)
                        mob.curFP = mob.maxFP;
                }
                if (level.mobs[feature.x, feature.y] != null && 
                    (level.mobs[feature.x, feature.y].faction == FactionEnum.factionDemons || level.mobs[feature.x, feature.y].faction == FactionEnum.factionBeasts))
                {
                    Mob mob = level.mobs[feature.x, feature.y];
                    int dmg = 0;
                    dmg += Mob.InflictDamage(null, mob,
                        new Dictionary<DmgTypeEnum, int>()
                        {
                            { DmgTypeEnum.Holy, 3 }
                        },
                        (int dmg1) =>
                        {
                            string str;
                            if (dmg1 <= 0)
                            {
                                str = String.Format("{0} takes no holy dmg. ",
                                    mob.name);
                            }
                            else
                            {
                                str = String.Format("{0} takes {1} holy dmg. ",
                                    mob.name,
                                    dmg1);
                            }
                            return str;
                        });
                    if (BoardManager.instance.level.visible[mob.x, mob.y])
                        UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", new Vector3(mob.x, mob.y, 0));

                    BoardManager.instance.CreateBlooddrop(mob.x, mob.y);
                    if (mob.CheckDead())
                    {
                        mob.MakeDead(null, true, true, false);
                    }
                }

                feature.counter--;
                if (feature.counter <= 0)
                {
                    level.RemoveFeatureFromLevel(feature);
                    //level.featureList.Remove(feature);
                    BoardManager.instance.featuresToRemove.Add(feature);
                    //BoardManager.instance.RemoveFeatureFromWorld(feature);
                }
            });

        Add(FeatureTypeEnum.featAcidCloud, "Acid cloud", "Deals 10 acid damage to any creature standing here.", featAcidCloud, featAcidCloud.GetComponent<SpriteRenderer>().color,
            (Level level, Feature newFeature) =>
            {
                foreach (Feature feature in level.features[newFeature.x, newFeature.y])
                {
                    if (feature.idType == FeatureTypeEnum.featAcidCloud)
                        return feature;
                }
                return null;
            },
            (Level level, Feature newFeature, Feature oldFeature) =>
            {
                oldFeature.counter += newFeature.counter;
                level.featureList.Remove(newFeature);
                BoardManager.instance.RemoveFeatureFromWorld(newFeature);

            },
            (Level level, Feature feature) =>
            {
                if (level.mobs[feature.x, feature.y] != null)
                {
                    Mob mob = level.mobs[feature.x, feature.y];
                    int dmg = 0;
                    dmg += Mob.InflictDamage(null, mob,
                        new Dictionary<DmgTypeEnum, int>()
                        {
                            { DmgTypeEnum.Acid, 10 }
                        },
                        (int dmg1) =>
                        {
                            string str;
                            if (dmg1 <= 0)
                            {
                                str = String.Format("{0} takes no acid dmg. ",
                                    mob.name);
                            }
                            else
                            {
                                str = String.Format("{0} takes {1} acid dmg. ",
                                    mob.name,
                                    dmg1);
                            }
                            return str;
                        });
                    
                    if (BoardManager.instance.level.visible[mob.x, mob.y])
                    {
                        Vector3 pos = new Vector3(mob.x, mob.y, 0);
                        UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", pos);
                    }
                    BoardManager.instance.CreateBlooddrop(mob.x, mob.y);
                    if (mob.CheckDead())
                    {
                        mob.MakeDead(null, true, true, false);
                    }
                }

                feature.counter--;
                if (UnityEngine.Random.Range(0, 100) < 25) feature.counter--;
                if (feature.counter <= 0)
                {
                    level.RemoveFeatureFromLevel(feature);
                    //level.featureList.Remove(feature);
                    BoardManager.instance.featuresToRemove.Add(feature);
                    //BoardManager.instance.RemoveFeatureFromWorld(feature);
                }
            });

        Add(FeatureTypeEnum.featArtilleryTarget, "Signal flare", "Designates the tile where the artillery strike will land.", featArtilleryTarget, featArtilleryTarget.GetComponent<SpriteRenderer>().color,
            null,
            null,
            (Level level, Feature feature) =>
            {
                feature.counter--;
                if (feature.counter > 0) return;

                string str = String.Format("The artillery bombardment lands. ");
                BoardManager.instance.msgLog.PlayerVisibleMsg(feature.x, feature.y, str);

                if (level.visible[feature.x, feature.y])
                {
                    GameObject shell = GameObject.Instantiate(UIManager.instance.artilleryShellPrefab, new Vector3(feature.x, feature.y, 0), Quaternion.identity);

                    Vector3 offset = new Vector3(0, 15, 0);
                    Vector3 landPos = shell.transform.position;
                    shell.transform.position = landPos + offset;

                    BoardEventController.instance.AddEvent(new BoardEventController.Event(shell,
                        () =>
                        {
                            shell.GetComponent<MovingObject>().moveTime = 0.01f;
                            shell.GetComponent<MovingObject>().inverseMoveTime = 1 / shell.GetComponent<MovingObject>().moveTime;
                            shell.GetComponent<MovingObject>().Move(landPos, level.visible[feature.x, feature.y], null);
                            BoardEventController.instance.RemoveFinishedEvent();
                        }));

                    BoardEventController.instance.AddEvent(new BoardEventController.Event(shell,
                        () =>
                        {
                            GameObject.Destroy(shell);
                            BoardEventController.instance.RemoveFinishedEvent();
                        }));
                }

                List<Mob> affectedMobs = new List<Mob>();
                List<Vector2Int> caughtFire = new List<Vector2Int>();

                LOS_FOV.DrawFOV(feature.x, feature.y, 2,
                    (int dx, int dy, int pdx, int pdy) =>
                    {
                        if (level.mobs[dx, dy] != null && !affectedMobs.Contains(level.mobs[dx, dy]))
                            affectedMobs.Add(level.mobs[dx, dy]);

                        if (TerrainTypes.terrainTypes[level.terrain[dx, dy]].blocksMovement) return false;

                        if (UnityEngine.Random.Range(0, 4) == 0)
                        {
                            if (!caughtFire.Contains(new Vector2Int(dx, dy)))
                                caughtFire.Add(new Vector2Int(dx, dy));
                        }

                        return true;
                    });

                GameObject go = new GameObject("tmp");
                go.transform.SetParent(feature.go.transform.parent);
                go.transform.position = feature.go.transform.position;
                go.AddComponent<MovingObject>();
                go.GetComponent<MovingObject>().Explosion5x5(feature.x, feature.y);

                GameObject go2 = new GameObject("tmp2");
                go.transform.SetParent(feature.go.transform.parent);
                go.transform.position = feature.go.transform.position;
                BoardEventController.instance.AddEvent(new BoardEventController.Event(go2,
                    () =>
                    {
                        foreach (Vector2Int fireLoc in caughtFire)
                        {
                            Feature fire = new Feature(FeatureTypeEnum.featFire, fireLoc.x, fireLoc.y);
                            fire.counter = 3 + TerrainTypes.terrainTypes[level.terrain[fireLoc.x, fireLoc.y]].catchesFire;
                            BoardManager.instance.level.AddFeatureToLevel(fire, fire.x, fire.y);
                        }

                        foreach (Mob mob in affectedMobs)
                        {
                            int dmg = 0;
                            dmg += Mob.InflictDamage(null, mob,
                                new Dictionary<DmgTypeEnum, int>()
                                {
                                    { DmgTypeEnum.Physical, 10 },
                                    { DmgTypeEnum.Fire, 10 }
                                },
                                null);

                            if (BoardManager.instance.level.visible[mob.x, mob.y])
                                UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", new Vector3(mob.x, mob.y, 0));

                            if (mob.CheckDead())
                            {
                                mob.MakeDead(null, true, true, false);
                            }

                        }
                        GameObject.Destroy(go2);
                        BoardEventController.instance.RemoveFinishedEvent();
                    }));

                if (feature.counter <= 0)
                {
                    level.RemoveFeatureFromLevel(feature);
                    BoardManager.instance.featuresToRemove.Add(feature);
                }
            });
    }

    private static void Add(FeatureTypeEnum _id, string _name, string _descr, GameObject _prefab, Color _color, 
        FeatCheckMerge _checkMerge = null, FeatMergeFunc _mergeFunc = null, FeatOnTick _tickFunc = null)
    {
        FeatureType ft = new FeatureType(_name, _descr, _prefab, _color, _checkMerge, _mergeFunc, _tickFunc)
        {
            id = _id
        };
        featureTypes.Add(_id, ft);
    }
}