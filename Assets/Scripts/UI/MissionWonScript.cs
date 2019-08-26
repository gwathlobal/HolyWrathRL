using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWonScript : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);
    }

    public void GoToPostMissionScene()
    {
        UIManager.instance.GoToPostMissionScene();
    }
}
