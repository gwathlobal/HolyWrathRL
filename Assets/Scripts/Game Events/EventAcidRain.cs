using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAcidRain : GameEvent
{
    public EventAcidRain()
    {
        idType = GameEventTypes.GameEventEnum.eventAcidRain;
        name = "Acid rain";
    }

    public override void Activate()
    {
        bool oneMoreTime;
        Level level = BoardManager.instance.level;

        do
        {
            oneMoreTime = false;

            int r = Random.Range(0, 100);
            if (r <= 50)
            {
                int tx, ty;

                tx = Random.Range(1, level.maxX - 1);
                ty = Random.Range(1, level.maxY - 1);

                if (!TerrainTypes.terrainTypes[level.terrain[tx,ty]].blocksMovement)
                {
                    if (level.visible[tx, ty])
                    {
                        GameObject drop = GameObject.Instantiate(UIManager.instance.acidDropPrefab, new Vector3(tx, ty, 0), Quaternion.identity);

                        Vector3 offset = new Vector3(0, 15, 0);
                        Vector3 landPos = drop.transform.position;
                        drop.transform.position = landPos + offset;

                        BoardEventController.instance.AddEvent(new BoardEventController.Event(drop,
                            () =>
                            {
                                drop.GetComponent<MovingObject>().moveTime = 0.015f;
                                drop.GetComponent<MovingObject>().inverseMoveTime = 1 / drop.GetComponent<MovingObject>().moveTime;
                                drop.GetComponent<MovingObject>().Move(landPos, level.visible[tx, ty], null);
                                BoardEventController.instance.RemoveFinishedEvent();
                            }));

                        BoardEventController.instance.AddEvent(new BoardEventController.Event(drop,
                            () =>
                            {
                                GameObject.Destroy(drop);
                                BoardEventController.instance.RemoveFinishedEvent();
                            }));
                    }

                    BoardEventController.instance.AddEvent(new BoardEventController.Event(BoardManager.instance.player.go, 
                        () =>
                        {
                            level.CheckSurroundings(tx, ty, true,
                                (int x, int y) =>
                                {
                                    if (!TerrainTypes.terrainTypes[BoardManager.instance.level.terrain[x, y]].blocksMovement)
                                    {
                                        Feature cloud = new Feature(FeatureTypeEnum.featAcidCloud, x, y);
                                        cloud.counter = 5;
                                        BoardManager.instance.level.AddFeatureToLevel(cloud, cloud.x, cloud.y);
                                    }
                                });
                            BoardEventController.instance.RemoveFinishedEvent();
                        }));
                }

                oneMoreTime = true;
            }
        } while (oneMoreTime);

        

        
    }

    public override bool CheckEvent()
    {
        if (BoardManager.instance.turnNum % 2 == 0)
            return true;
        else
            return false;
    }

    public override string Description()
    {
        string str = System.String.Format("Acid frequently drops from the sky here.");
        return str;
    }

    public override void Initialize()
    {
        return;
    }

    public override string LineDescription()
    {
        string str = System.String.Format("Acid rain");
        return str;
    }
}