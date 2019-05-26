using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature  {

    public FeatureTypeEnum idType;
    public int id;
    public int x = 0;
    public int y = 0;
    public int counter = 0;

    public string name
    {
        get
        {
            return FeatureTypes.featureTypes[idType].name;
        }
    }

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

    public string Description()
    {
        return FeatureTypes.featureTypes[idType].descr;
    }

    public string GetEffectLine()
    {
        string str = "";

        str = System.String.Format("{0}{1}", name, (counter > 0) ? System.String.Format(" ({0})", counter) : "");

        return str;
    }

    public string GetEffectFullLine()
    {
        string str = "";

        str = name;
        if (counter > 0)
            str += System.String.Format(" ({0} {1} left)", counter, (counter > 1) ? "turns" : "turn");

        return str;
    }
}
