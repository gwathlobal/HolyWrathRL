using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMob : Mob {

    public PlayerMob(MobTypeEnum _idType, int _x, int _y) : base(_idType, _x, _y)
    {
        
    }

    public override void AiFunction()
    {
        
        //Debug.Log("Player AI Function");
        
        BoardManager.instance.playersTurn = true;
        GetFOV();
        //Debug.Log("Player AI Function: Time Elapsed Since Last Turn = " + (Time.time - BoardManager.instance.prevTime));
    }

    public override void GetFOV()
    {
        Level level = BoardManager.instance.level;
        visibleMobs.Clear();
        visibleItems.Clear();
        visibleFeatures.Clear();

        for (int y = 0; y < level.maxY; y++)
        {
            for (int x = 0; x < level.maxX; x++)
            {
                level.visible[x, y] = false;
                BoardManager.instance.tiles[x, y].GetComponent<SpriteRenderer>().color = TerrainTypes.terrainTypes[level.terrain[x, y]].color;
                BoardManager.instance.fog[x, y].SetActive(true);
            }
        }
        /*
        foreach (Mob mob in BoardManager.instance.mobs.Values)
        {
            mob.go.GetComponent<Renderer>().enabled = false;
        }
        foreach (Feature feature in BoardManager.instance.features.Values)
        {
            if (feature.go != null)
                feature.go.GetComponent<Renderer>().enabled = false;
        }
        foreach (Item item in BoardManager.instance.items.Values)
        {
            item.go.GetComponent<Renderer>().enabled = false;
        }
        */

        LOS_FOV.DrawFOV(x, y, visionRadius, 
            (int dx,int dy,int pdx,int pdy) => 
            {
                level.visible[dx, dy] = true;
                //BoardManager.instance.tiles[dx, dy].GetComponent<SpriteRenderer>().color = TerrainTypes.terrainTypes[level.terrain[dx, dy]].color;
                BoardManager.instance.fog[dx, dy].SetActive(false);

                if (level.mobs[dx, dy] != null && !visibleMobs.Contains(level.mobs[dx, dy]))
                {
                    visibleMobs.Add(level.mobs[dx, dy]);
                }

                if (level.items[dx, dy].Count > 0)
                {
                    foreach (Item item in level.items[dx, dy])
                    {
                        if (!visibleItems.Contains(item)) visibleItems.Add(item);
                    }
                }
                if (level.features[dx, dy].Count > 0)
                {
                    foreach (Feature feature in level.features[dx, dy])
                    {
                        if (!visibleFeatures.Contains(feature)) visibleFeatures.Add(feature);
                    }
                }

                if (TerrainTypes.terrainTypes[level.terrain[dx,dy]].blocksVision) return false;
                return true;
            });

        /*
        foreach (Mob mob in visibleMobs)
        {
            mob.go.GetComponent<Renderer>().enabled = true;
        }
        foreach (Item item in visibleItems)
        {
            item.go.GetComponent<Renderer>().enabled = true;
        }
        */
        foreach (Feature feature in visibleFeatures)
        {
            if (feature.go == null)
                BoardManager.instance.tiles[feature.x, feature.y].GetComponent<SpriteRenderer>().color = FeatureTypes.featureTypes[feature.idType].color;
        }
        
    }

    public static void QuitGame()
    {
        BoardManager.instance.msgLog.AddMsg("\n\nYou died!");
        BoardManager.instance.msgLog.FinalizeMsg();
        BoardManager.instance.playersTurn = true;
        UIManager.instance.ShowYouDiedWindow();
    }
}