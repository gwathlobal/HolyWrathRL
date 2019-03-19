using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelScript : MonoBehaviour {

    public Image EffectScrollPanel;
    private List<GameObject> effectPanels;

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

            ep.GetComponent<Text>().text = EffectTypes.effectTypes[effectType].name;
            if (player.effects[effectType].cd != Effect.CD_UNLIMITED)
                ep.GetComponent<Text>().text += " (" + player.effects[effectType].cd + ")";
            ep.GetComponent<Text>().color = EffectTypes.effectTypes[effectType].color;

            effectPanels.Add(ep);
            i++;
        }
        
        
    }
}
