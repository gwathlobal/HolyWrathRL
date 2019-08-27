using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostMissionDialog : MonoBehaviour
{
    public Text descrText;

    void Start()
    {
        string str = "";

        str += "You gained 1 ability point.\n";

        foreach (Nemesis nemesis in GameManager.instance.nemesesPresent)
        {
            if (nemesis.mob.CheckDead() && nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden)
            {
                str += String.Format("{0} the {1} has died!\n", nemesis.mob.name, MobTypes.mobTypes[nemesis.mob.idType].name);
            }
            if (!nemesis.mob.CheckDead() && nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden)
            {
                str += String.Format("{0} the {1} has survived!\n", nemesis.mob.name, MobTypes.mobTypes[nemesis.mob.idType].name);
            }

            
        }

        str += "\n";

        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (!nemesis.mob.CheckDead() && nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden && nemesis.activity != NemesisActivityTypes.ActivityEnum.none)
            {
                str += String.Format("{0}\n", NemesisActivityTypes.nemesisActivities[nemesis.activity].ProcessActivity(nemesis, nemesis.activityTarget));
            }
        }

        descrText.text = str;
    }

    public void GoToIntermissionScene()
    {
        int aliveAngels = 0;
        int aliveDemons = 0;

        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (!nemesis.mob.CheckDead() && nemesis.activity == NemesisActivityTypes.ActivityEnum.none)
            {
                nemesis.AssignRandomActivity();
            }

            if (nemesis.deathStatus == Nemesis.DeathStatusEnum.alive && nemesis.mob.GetAbility(AbilityTypeEnum.abilAngel) != null)
                aliveAngels++;
            if (nemesis.deathStatus == Nemesis.DeathStatusEnum.alive && nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null)
                aliveDemons++;

            
        }

        if (aliveAngels < Nemesis.MAX_ANGEL_NEMESIS)
        {
            for (int i = 0; i < Nemesis.MAX_ANGEL_NEMESIS - aliveAngels; i++)
            {
                if (UnityEngine.Random.Range(0, 100) < 25)
                {
                    Nemesis nemesis = Nemesis.CreateAngelNemesis();

                    GameManager.instance.nemeses.Add(nemesis);
                }
            }
        }

        if (aliveDemons < Nemesis.MAX_DEMON_NEMESIS)
        {
            for (int i = 0; i < Nemesis.MAX_DEMON_NEMESIS - aliveAngels; i++)
            {
                if (UnityEngine.Random.Range(0, 100) < 25)
                {
                    Nemesis nemesis = Nemesis.CreateAngelNemesis();

                    GameManager.instance.nemeses.Add(nemesis);
                }
            }
        }


        UIManager.instance.GoToIntermissionScene();
    }
}
