using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature  {

    public FeatureTypeEnum idType;
    public int id;
    public int x = 0;
    public int y = 0;

    public GameObject go;

    public Feature(FeatureTypeEnum _idType, int _x, int _y)
    {
        idType = _idType;
        x = _x;
        y = _y;
        if (FeatureTypes.featureTypes[idType].prefab != null)
        {
            go = GameObject.Instantiate(FeatureTypes.featureTypes[idType].prefab, new Vector3(x, y, 0f), Quaternion.identity);
        }
        id = BoardManager.instance.FindFreeID(BoardManager.instance.features);
        BoardManager.instance.features.Add(this.id, this);
    }

}
