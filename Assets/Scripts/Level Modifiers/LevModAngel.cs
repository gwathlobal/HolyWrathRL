using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModifierAngel : LevelModifier
{
    List<Nemesis> angels;

    public LevelModifierAngel()
    {
        idType = LevelModifierTypes.LevelModifierEnum.LevModAngel;
        name = "An unnamed angel";
    }

    public override bool CheckRequirements()
    {
        int angelNum = 0;
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.deathStatus != Nemesis.DeathStatusEnum.deceased && nemesis.mob.GetAbility(AbilityTypeEnum.abilAngel) != null)
                angelNum++;
        }
        if (angelNum != 0)
            return true;
        else
            return false;
    }

    public override void Initialize()
    {
        angels = new List<Nemesis>();
        List<Nemesis> availAngels = new List<Nemesis>();

        // collect a list of available angels
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.deathStatus != Nemesis.DeathStatusEnum.deceased && nemesis.mob.GetAbility(AbilityTypeEnum.abilAngel) != null)
            {
                availAngels.Add(nemesis);
            }
        }

        //Debug.Log("availAngels.Count = " + availAngels.Count);

        // pick a random angel, 25% to pick one more random angel, etc.
        bool oneMoreAngel = false;
        do
        {
            if (availAngels.Count > 0)
            {
                int r = Random.Range(0, availAngels.Count);

                angels.Add(availAngels[r]);
                availAngels.RemoveAt(r);

                if (Random.Range(0, 100) <= 25)
                    oneMoreAngel = true;
                else
                    oneMoreAngel = false;
            }
            else
            {
                oneMoreAngel = false;
            }
        } while (oneMoreAngel);

        //Debug.Log("angels.Count = " + angels.Count);
    }

    public override string Description()
    {
        string str = "";

        foreach (Nemesis nemesis in angels)
        {
            if (nemesis.personalStatus == Nemesis.PersonalStatusEnum.hidden)
            {
                str += "An angel\nAn unknown angel is present here.\n\n";
            }
            else
            {
                str += System.String.Format("An angel\n{0} {1} is present here.\n\n", nemesis.mob.typeName, nemesis.mob.GetFullName());
            }
        }

        return str;
    }

    public override void Activate(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        Vector2Int loc;
        
        foreach (Nemesis nemesis in angels)
        {
            if (level.FindFreeSpotInside(out loc))
            {
                Mob mob = nemesis.mob;

                mob.id = BoardManager.instance.FindFreeID(BoardManager.instance.mobs);

                mob.go = GameObject.Instantiate(MobTypes.mobTypes[mob.idType].prefab, new Vector3(loc.x, loc.y, 0f), Quaternion.identity);
                mob.mo = mob.go.GetComponent<MovingObject>();

                mob.curHP = mob.maxHP;
                mob.curFP = mob.maxFP;
                mob.curWP = 0;
                mob.curAP = MobType.NORMAL_AP;

                mob.effects = new Dictionary<EffectTypeEnum, Effect>();

                BoardManager.instance.mobs.Add(mob.id, mob);
                level.AddMobToLevel(mob, loc.x, loc.y);

                BoardManager.instance.nemesesPresent.Add(nemesis);
            }
        }
    }
}


