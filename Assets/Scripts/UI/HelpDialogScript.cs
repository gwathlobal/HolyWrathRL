using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpDialogScript : MonoBehaviour {

    public Text msgTxt;

    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);
    }

}
