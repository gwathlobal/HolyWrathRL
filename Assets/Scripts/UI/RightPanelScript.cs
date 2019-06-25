using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelScript : MonoBehaviour {

    public Image EffectScrollPanel;
    private List<GameObject> effectPanels;
    public Text statusText;

    public GameObject effectPanelPrefab;

	// Use this for initialization
	void Start () {
        effectPanels = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowEffects()
    {
        PlayerMob player = BoardManager.instance.player;

        foreach (GameObject go in effectPanels)
            Destroy(go);
        effectPanels.Clear();

        int i = 0;
        foreach (EffectTypeEnum effectType in player.effects.Keys)
        {

            GameObject ep = Instantiate(effectPanelPrefab);
            ep.transform.SetParent(EffectScrollPanel.transform, false);
            ep.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0 + i * -20);

            EffectScrollPanel.rectTransform.sizeDelta = new Vector2(173, i * 20);

            ep.GetComponent<Text>().text = player.effects[effectType].GetEffectLine();
            ep.GetComponent<Text>().color = EffectTypes.effectTypes[effectType].color;

            ep.GetComponent<HintPanelScript>().SetPanelName("Hint Panel " + EffectTypes.effectTypes[effectType].name);
            ep.GetComponent<HintPanelScript>().hintStr = player.effects[effectType].GetEffectFullLine() + "\n" + EffectTypes.effectTypes[effectType].descr + ".";

            effectPanels.Add(ep);
            i++;
        }
    }

    public void ShowStatus()
    {
        string str = "";
        foreach (GameEvent gameEvent in BoardManager.instance.level.gameEvents)
        {
            if (gameEvent.Description() != "")
                str += System.String.Format("{0}\n", gameEvent.LineDescription());
        }
        statusText.text = System.String.Format("{0}Turns: {1}", str, BoardManager.instance.turnNum);
    }
}
