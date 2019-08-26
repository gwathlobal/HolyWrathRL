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
                str += String.Format("{0} the {1} died!\n", nemesis.mob.name, MobTypes.mobTypes[nemesis.mob.idType].name);
            }
            if (!nemesis.mob.CheckDead() && nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden)
            {
                str += String.Format("{0} the {1} survived and gained a level up!\n", nemesis.mob.name, MobTypes.mobTypes[nemesis.mob.idType].name);
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
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (!nemesis.mob.CheckDead() && nemesis.activity == NemesisActivityTypes.ActivityEnum.none)
            {
                nemesis.AssignRandomActivity();
            }
        }
        

        UIManager.instance.GoToIntermissionScene();
    }
}
