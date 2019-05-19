using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageLog  {

    private List<string> curMsg;
    private List<string> msg;
    private List<string> tmpMsg;
    private bool hasMsgThisTurn;
    private bool playerStartedTurn;

    public MessageLog()
    {
        msg = new List<string>();
        tmpMsg = new List<string>();
        curMsg = new List<string>();
    }

    public void AddMsg(string str)
    {
        if (playerStartedTurn)
        {
            curMsg.Clear();
            playerStartedTurn = false;
        }
        tmpMsg.Add(str);
        hasMsgThisTurn = true;
    }

    public void FinalizeMsg()
    {
        if (hasMsgThisTurn)
        {
            string result = "";
            foreach (string str in tmpMsg)
            {
                result = result + str;
            }
            result = result + "\n";
            tmpMsg.Clear();
            msg.Add(result);
            curMsg.Add(result);
            hasMsgThisTurn = false;
        }
        UIManager.instance.BottomPanel.UpdateInterface();
    }

    public string GetMsg(int i)
    {
        return msg[i];
    }

    public int MsgLength()
    {
        return msg.Count;
    }

    public string GetLastMessagesAsText(int n)
    {
        string result = "";

        if (n > msg.Count) n = msg.Count;

        for (int i = msg.Count - 1; i >= msg.Count - n; i--)
        {
            result = result + msg[i];
        }
        return result;
    }

    public string GetCurMessages()
    {
        string result = "";

        for (int i = 0; i < curMsg.Count; i++)
        {
            result = result + curMsg[i];
        }
        return result;
    }

    public bool HasMessageThisTurn()
    {
        return hasMsgThisTurn;
    }

    public void SetHasMessageThisTurn(bool hasMsg)
    {
        hasMsgThisTurn = hasMsg;
    }

    public void SetPlayerStartedTurn(bool started)
    {
        playerStartedTurn = started;
    }

    public void ClearCurMsg()
    {
        curMsg.Clear();
    }

    public void PlayerVisibleMsg(int x, int y, string str)
    {
        if (BoardManager.instance.level.visible[x, y])
            AddMsg(str);
    }

}
