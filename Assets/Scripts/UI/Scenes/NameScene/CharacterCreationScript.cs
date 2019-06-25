using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreationScript : MonoBehaviour {

    public Text inputTxt;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        GameManager.instance.player.hasPersonalName = true;
        if (inputTxt.text != "")
        {
            GameManager.instance.player.name = inputTxt.text;
        }
        else
        {
            GameManager.instance.player.name = "Player";
        }
        SceneManager.LoadScene("IntermissionScene");
    }
}
