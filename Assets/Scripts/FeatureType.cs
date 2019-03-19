using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate Feature FeatCheckMerge(Level level, Feature newFeature);
public delegate void FeatMergeFunc(Level level, Feature newFeature, Feature oldFeature);

public enum FeatureTypeEnum
{
    featBloodDrop, featBloodPool
};

public class FeatureType {
    public FeatureTypeEnum id;
    public string name = "Feature Template";
    public GameObject prefab;
    public Color color;

    public FeatCheckMerge FeatCheckMerge;
    public FeatMergeFunc FeatMergeFunc;

    public FeatureType(string _name, GameObject _prefab, Color _color, FeatCheckMerge _checkMerge = null, FeatMergeFunc _mergeFunc = null)
    {
        name = _name;
        prefab = _prefab;
        color = _color;
        FeatMergeFunc = _mergeFunc;
        FeatCheckMerge = _checkMerge;
    }
}

public class FeatureTypes
{

    public static GameObject featBloodPool;
    public static Dictionary<FeatureTypeEnum, FeatureType> featureTypes;

    public static void InitializeFeatureTypes()
    {
        featBloodPool = Resources.Load("Prefabs/Features/BloodPool") as GameObject;

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

    }

    private static void Add(FeatureTypeEnum _id, string _name, GameObject _prefab, Color _color, FeatCheckMerge _checkMerge = null, FeatMergeFunc _mergeFunc = null)
    {
        FeatureType ft = new FeatureType(_name, _prefab, _color, _checkMerge, _mergeFunc)
        {
            id = _id
        };
        featureTypes.Add(_id, ft);
    }
}