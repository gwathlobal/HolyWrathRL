using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    void GetInputNormal()
    {
        PlayerMob player = BoardManager.instance.player;
        bool turnEnded = false;

        /*
        int[] values = (int[])System.Enum.GetValues(typeof(KeyCode));
        for (int i = 0; i < values.Length; i++)
        {
            if (Input.GetKey((KeyCode)values[i]))
                Debug.Log("Key Pressed = " + ((KeyCode)values[i]).ToString());
        }
        */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.ShowMainMenuWindow();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            turnEnded = UIManager.instance.MovePlayer(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            turnEnded = UIManager.instance.MovePlayer(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            turnEnded = UIManager.instance.MovePlayer(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            turnEnded = UIManager.instance.MovePlayer(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.End))
        {
            turnEnded = UIManager.instance.MovePlayer(-1, -1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.PageDown))
        {
            turnEnded = UIManager.instance.MovePlayer(1, -1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Home))
        {
            turnEnded = UIManager.instance.MovePlayer(-1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.PageUp))
        {
            turnEnded = UIManager.instance.MovePlayer(1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Period))
        {
            turnEnded = UIManager.instance.MovePlayer(0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            UIManager.instance.SetScreenStatusToLook(player.x, player.y, "", "[x] Examine tile [Esc] Cancel");
            UIManager.instance.MoveSelector(0, 0);

        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            UIManager.instance.ShowMsgWindow();
        }
        else if (Input.GetKeyDown(KeyCode.Slash) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            UIManager.instance.ShowHelpDialogWindow();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            UIManager.instance.ShowCharacterWindow(BoardManager.instance.level.terrain[player.x, player.y], 
                BoardManager.instance.level.features[player.x, player.y], 
                player,
                BoardManager.instance.level.items[player.x, player.y],
                true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[0]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[1]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[2]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[3]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[4]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[5]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[6]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[7]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[8]));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.curAbils[9]));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.dodgeAbil));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.blockAbil));
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            turnEnded = UIManager.instance.InvokeAbility(player.GetAbility(player.rangedAbil));
        }
        

        if (turnEnded)
        {
            BoardManager.instance.FinalizePlayerTurn();
        }
    }

    void GetInputLookAt()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.SetScreenStatusToNormal();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            UIManager.instance.InvokeExecFunc();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            UIManager.instance.MoveSelector(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            UIManager.instance.MoveSelector(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            UIManager.instance.MoveSelector(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            UIManager.instance.MoveSelector(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.End))
        {
            UIManager.instance.MoveSelector(-1, -1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.PageDown))
        {
            UIManager.instance.MoveSelector(1, -1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Home))
        {
            UIManager.instance.MoveSelector(-1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.PageUp))
        {
            UIManager.instance.MoveSelector(1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Level level = BoardManager.instance.level;
            Vector2Int pos = UIManager.instance.selectorPos;
            if (level.visible[pos.x, pos.y])
            {
                UIManager.instance.ShowCharacterWindow(level.terrain[pos.x, pos.y], level.features[pos.x, pos.y], level.mobs[pos.x, pos.y], level.items[pos.x, pos.y]);
            }
        }
    }

    void GetInputMsgWindow()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.HideMsgWindow();
        }
    }

    void GetInputAbilityWindow()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.HideCharacterWindow();
        }
    }

    void GetInputMenuWindow()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.HideMainMenuWindow();
        }
    }

    void GetInputPlayerDied()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            UIManager.instance.GoToDefeatScene();
        }
    }

    void GetInputMissionWon()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            UIManager.instance.GoToIntermissionScene();
        }
    }

    void GetInputHelpDialog()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.HideHelpDialogWindow();
        }
    }

    void GetPlayerInput()
    {
        if (!BoardManager.instance.playersTurn) return;

        UIManager.instance.anyBtnClicked = false;

        switch (UIManager.instance.screenStatus)
        {
            case MainScreenStatus.statusNormal:
                GetInputNormal();
                break;
            case MainScreenStatus.statusLookAt:
                GetInputLookAt();
                break;
            case MainScreenStatus.statusMsgWindow:
                GetInputMsgWindow();
                break;
            case MainScreenStatus.statusCharacterWindow:
                GetInputAbilityWindow();
                break;
            case MainScreenStatus.statusMainMenu:
                GetInputMenuWindow();
                break;
            case MainScreenStatus.statusPlayerDied:
                GetInputPlayerDied();
                break;
            case MainScreenStatus.statusMissionWon:
                GetInputMissionWon();
                break;
            case MainScreenStatus.statusHelpWindow:
                GetInputHelpDialog();
                break;
        }

        if (UIManager.instance.anyBtnClicked)
            UIManager.instance.LeftPanel.UpdateLeftPanel();
    }

    // Update is called once per frame
    void Update()
    {

        GetPlayerInput();

    }
}
