using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterDialogScript : MonoBehaviour
{

    public Mob mob;

    public Text DescrText;

    // Use this for initialization
    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);
    }

    public void InitializeUI(Mob _mob)
    {
        mob = _mob;

        DescrText.text = mob.Description();
    }
}
