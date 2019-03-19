using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType
{
    public ItemTypeEnum id;
    public string name = "Item Template";
    public GameObject prefab;
    public int corpsePwr;
}

public enum ItemTypeEnum
{
    itemCorpseFull, itemCorpseArm, itemCorpseLeg, itemCorpseHead, itemCorpseMutilated, itemCorpseUpper, itemCorpseLower
};

public class ItemTypes
{

    public static GameObject itemCorpse;
    public static Dictionary<ItemTypeEnum, ItemType> itemTypes;

    public static void InitializeItemTypes()
    {
        itemCorpse = Resources.Load("Prefabs/Items/Corpse") as GameObject;

        itemTypes = new Dictionary<ItemTypeEnum, ItemType>();

        Add(ItemTypeEnum.itemCorpseFull, "Corpse", itemCorpse, 4);
        Add(ItemTypeEnum.itemCorpseArm, "Severed arm", itemCorpse, 1);
        Add(ItemTypeEnum.itemCorpseLeg, "Severed leg", itemCorpse, 1);
        Add(ItemTypeEnum.itemCorpseHead, "Severed head", itemCorpse, 1);
        Add(ItemTypeEnum.itemCorpseMutilated, "Mutilated body", itemCorpse, 3);
        Add(ItemTypeEnum.itemCorpseUpper, "Upper body", itemCorpse, 2);
        Add(ItemTypeEnum.itemCorpseLower, "Lower body", itemCorpse, 2);
    }

    private static void Add(ItemTypeEnum _id, string _name, GameObject _prefab, int _corpsePwr)
    {
        ItemType it = new ItemType()
        {
            id = _id,
            name = _name,
            prefab = _prefab,
            corpsePwr = _corpsePwr
        };
        itemTypes.Add(_id, it);
    }
}