using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsTxtUI : MonoBehaviour {

    private Text txt;

	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        txt.text = String.Format("HP: {0}/{1} + {2}\nFP: {3}/{4} + {5}\nWP: {6}\n{7}AP: {8}", 
            BoardManager.instance.player.curHP, BoardManager.instance.player.maxHP, BoardManager.instance.player.regenHP,
            BoardManager.instance.player.curFP, BoardManager.instance.player.maxFP, BoardManager.instance.player.regenFP,
            BoardManager.instance.player.curWP,
            (BoardManager.instance.player.curSH != 0) ? "Shield: " + BoardManager.instance.player.curSH + "\n" : "",
            BoardManager.instance.player.curAP);
	}
}
