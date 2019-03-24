using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate Feature FeatCheckMerge(Level level, Feature newFeature);
public delegate void FeatMergeFunc(Level level, Feature newFeature, Feature oldFeature);
public delegate void FeatOnTick(Level level, Feature Feature);

public enum FeatureTypeEnum
{
    featBloodDrop, featBloodPool, featFire
};

public class FeatureType {
    public FeatureTypeEnum id;
    public string name = "Feature Template";
    public GameObject prefab;
    public Color color;

    public FeatCheckMerge FeatCheckMerge;
    public FeatMergeFunc FeatMergeFunc;
    public FeatOnTick FeatOnTick;

    public FeatureType(string _name, GameObject _prefab, Color _color, FeatCheckMerge _checkMerge = null, FeatMergeFunc _mergeFunc = null, FeatOnTick _tickFunc = null)
    {
        name = _name;
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
    public static Dictionary<FeatureTypeEnum, FeatureType> featureTypes;

    public static void InitializeFeatureTypes()
    {
        featBloodPool = Resources.Load("Prefabs/Features/BloodPool") as GameObject;
        featFire = Resources.Load("Prefabs/Features/Fire") as GameObject;

        featureTypes = new Dictionary<FeatureTypeEnum, FeatureType>();

        Add(FeatureTypeEnum.featBloodDrop, "Bloodstain", null, new Color(255, 0, 0),
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
        Add(FeatureTypeEnum.featBloodPool, "Bloodstain", featBloodPool, featBloodPool.GetComponent<SpriteRenderer>().color,
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
        Add(FeatureTypeEnum.featFire, "Fire", featFire, featFire.GetComponent<SpriteRenderer>().color,
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
                    dmg += Mob.InflictDamage(null, mob, 5, DmgTypeEnum.Fire,
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

                feature.counter--;
                if (feature.counter <= 0)
                {
                    level.RemoveFeatureFromLevel(feature);
                    //level.featureList.Remove(feature);
                    BoardManager.instance.featuresToRemove.Add(feature);
                    //BoardManager.instance.RemoveFeatureFromWorld(feature);
                }
            });

    }

    private static void Add(FeatureTypeEnum _id, string _name, GameObject _prefab, Color _color, 
        FeatCheckMerge _checkMerge = null, FeatMergeFunc _mergeFunc = null, FeatOnTick _tickFunc = null)
    {
        FeatureType ft = new FeatureType(_name, _prefab, _color, _checkMerge, _mergeFunc, _tickFunc)
        {
            id = _id
        };
        featureTypes.Add(_id, ft);
    }
}