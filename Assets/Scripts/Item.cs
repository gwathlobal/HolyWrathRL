using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

    public ItemTypeEnum idType;
    public int id;
    public int x = 0;
    public int y = 0;
    public string name
    {
        get
        {
            return ItemTypes.itemTypes[idType].name;
        }
    }
    public string descr
    {
        get
        {
            return ItemTypes.itemTypes[idType].descr;
        }
    }

    public GameObject go;
    public MovingObject mo;

    public int corpsePwr
    {
        get { return ItemTypes.itemTypes[idType].corpsePwr; }
    }

    public Item(ItemTypeEnum _idType, int _x, int _y)
    {
        idType = _idType;
        x = _x;
        y = _y;
        go = GameObject.Instantiate(ItemTypes.itemTypes[idType].prefab, new Vector3(x, y, 0f), Quaternion.identity);
        id = BoardManager.instance.FindFreeID(BoardManager.instance.items);
        BoardManager.instance.items.Add(this.id, this);

        mo = go.GetComponent<MovingObject>();
    }

    public string Description()
    {
        string str;

        str = descr;

        return str;
    }
}
