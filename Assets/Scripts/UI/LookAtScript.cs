using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtScript : MonoBehaviour {

    void OnMouseOver()
    {
        if ((int)transform.position.x == UIManager.instance.selectorPos.x &&
            (int)transform.position.y == UIManager.instance.selectorPos.y)
            return;

        UIManager.instance.selectorPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int pos = UIManager.instance.selectorPos;

        if (UIManager.instance.screenStatus == MainScreenStatus.statusNormal)
        {
            Level level = BoardManager.instance.level;

            if (level.visible[pos.x, pos.y])
            {
                UIManager.instance.BottomPanel.status = BottomStatusEnum.statusLook;
                UIManager.instance.BottomPanel.SetStrings("", "");
                UIManager.instance.BottomPanel.UpdateInterface();
            }
        }
        else if (UIManager.instance.screenStatus == MainScreenStatus.statusLookAt)
        {
            UIManager.instance.SetSelectorPos(pos.x, pos.y);
            UIManager.instance.MoveSelector(0, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Right mouse clicked");
            Level level = BoardManager.instance.level;

            if (level.visible[pos.x, pos.y] && level.mobs[pos.x, pos.y] != null)
            {
                UIManager.instance.ShowCharacterWindow(level.mobs[pos.x, pos.y]);
            }
        }
    }

    void OnMouseExit()
    {
        if (UIManager.instance.screenStatus == MainScreenStatus.statusNormal)
        {
            UIManager.instance.BottomPanel.status = BottomStatusEnum.statusLog;
            UIManager.instance.BottomPanel.UpdateInterface();
        }
        else if (UIManager.instance.screenStatus == MainScreenStatus.statusLookAt)
        {

        }
    }

    private void OnMouseDown()
    {
        if (UIManager.instance.screenStatus == MainScreenStatus.statusNormal)
        {
            Vector2Int pos = UIManager.instance.selectorPos;
            Level level = BoardManager.instance.level;
            PlayerMob player = BoardManager.instance.player;

            if (level.visible[pos.x, pos.y] && level.AreCellsConnected(player.x, player.y, pos.x, pos.y))
            {
                player.path = Astar.FindPath(player.x, player.y, pos.x, pos.y,
                    delegate (int dx, int dy, int prevx, int prevy)
                    {
                        AttemptMoveResult attemptMoveResult = player.CanMoveToPos(dx, dy);

                        if (attemptMoveResult.result == AttemptMoveResultEnum.moveClear || attemptMoveResult.result == AttemptMoveResultEnum.moveBlockedbyMob)
                            return true;
                        return false;
                    },
                    delegate (int dx, int dy)
                    {
                        return 10;
                    });

                if (player.path.Count > 0)
                {
                    if (UIManager.instance.MovePlayer(player.path[0].x - player.x, player.path[0].y - player.y))
                    {
                        BoardManager.instance.FinalizePlayerTurn();
                    }
                    player.path.Clear();
                }
            }
        }
        else
        {
            UIManager.instance.InvokeExecFunc();
        }
    }
}
