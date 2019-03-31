using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum BottomStatusEnum
{
    statusLog, statusLook
}

public class BottomPanelScript : MonoBehaviour {

    public BottomStatusEnum status;

    public Image lookPanel;
    public Text lookTxt;
    public Text cmdLine;

    public Image logPanel;
    public Text logTxt;

    private string headerStr;
    private string cmdlineStr;

    public void UpdateInterface()
    {
        Vector2Int pos = UIManager.instance.selectorPos;

        if (status == BottomStatusEnum.statusLook)
        {
            lookPanel.gameObject.SetActive(true);
            logPanel.gameObject.SetActive(false);

            Level level = BoardManager.instance.level;
            
            string _headerStr = "";
            if (headerStr != "") _headerStr = headerStr + "\n\n";
            string featuresTxt = "";
            foreach (Feature feature in level.features[pos.x,pos.y])
            {
                featuresTxt += ", " + FeatureTypes.featureTypes[feature.idType].name + " [" + feature.counter + "]";
            }
            string mobTxt = "";
            if (level.mobs[pos.x, pos.y] != null && 
                  (BoardManager.instance.player.GetFactionRelation(level.mobs[pos.x, pos.y].faction) ||
                  ((!BoardManager.instance.player.GetFactionRelation(level.mobs[pos.x, pos.y].faction) && 
                    level.mobs[pos.x, pos.y].GetEffect(EffectTypeEnum.effectInvisibility) == null))))
            {
                Mob mob = level.mobs[pos.x, pos.y];
                string color;
                if (mob.curHP == mob.maxHP) color = "white";
                else if ((float)mob.curHP / mob.maxHP > 0.5) color = "green";
                else if ((float)mob.curHP / mob.maxHP > 0.25) color = "yellow";
                else color = "red";

                string effects = "";
                bool hasAnyEffects = false;
                bool firstEffect = true;
                foreach (Effect eff in mob.effects.Values)
                {
                    if (!hasAnyEffects)
                    {
                        hasAnyEffects = true;
                        effects = " (";
                    }
                    if (!firstEffect) effects += ", ";
                    if (firstEffect) firstEffect = false;
                    effects += String.Format("<color=#{2}>{0}{1}</color>", EffectTypes.effectTypes[eff.idType].name,
                        (eff.cd == Effect.CD_UNLIMITED) ? "" : String.Format(" [{0}]", eff.cd), 
                        ColorUtility.ToHtmlStringRGBA(EffectTypes.effectTypes[eff.idType].color));
                }
                if (hasAnyEffects)
                {
                    effects += ")";
                }
                mobTxt = String.Format("{0} (<color={3}>{1}</color>/{2}){4}\n\n",
                    mob.name,
                    mob.curHP,
                    mob.maxHP,
                    color,
                    effects);
            }
            string itemsTxt = "";
            foreach (Item item in level.items[pos.x, pos.y])
            {
                itemsTxt += ItemTypes.itemTypes[item.idType].name + "\n";
            }
            string coordTxt = "";
            #if UNITY_EDITOR
            coordTxt = String.Format(" [{0},{1}] ", pos.x, pos.y);
            #endif
            lookTxt.text = String.Format("{0}{1}{2}{3}\n\n{4}{5}",
                _headerStr,
                coordTxt,
                TerrainTypes.terrainTypes[level.terrain[pos.x, pos.y]].name,
                featuresTxt,
                mobTxt,
                itemsTxt);
            cmdLine.text = cmdlineStr;
        }
        else
        {
            logPanel.gameObject.SetActive(true);
            lookPanel.gameObject.SetActive(false);

            logTxt.text = BoardManager.instance.msgLog.GetCurMessages();
        }
    }

    public void SetStrings(string header, string cmdline)
    {
        headerStr = header;
        cmdlineStr = cmdline;
    }
}
