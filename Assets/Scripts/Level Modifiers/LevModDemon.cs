using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModifierDemon : LevelModifier
{
    List<Nemesis> demons;

    public LevelModifierDemon()
    {
        idType = LevelModifierTypes.LevelModifierEnum.LevModDemon;
        name = "An unnamed demon";
    }

    public override bool CheckRequirements()
    {
        int demonNum = 0;
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.deathStatus != Nemesis.DeathStatusEnum.deceased && nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null && 
                nemesis.activity != NemesisActivityTypes.ActivityEnum.duel)
                demonNum++;
        }
        if (GameManager.instance.levelNum >= 5 && demonNum != 0)
            return true;
        else
            return false;
    }

    public override void Initialize()
    {
        demons = new List<Nemesis>();
        List<Nemesis> availDemons = new List<Nemesis>();

        // collect a list of available demons
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.deathStatus != Nemesis.DeathStatusEnum.deceased && nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null &&
                nemesis.activity != NemesisActivityTypes.ActivityEnum.duel)
            {
                availDemons.Add(nemesis);
            }
        }

        //Debug.Log("availDemons.Count = " + availDemons.Count);

        // pick a random demon, 25% to pick one more random demon, etc.
        bool oneMoreDemon = false;
        do
        {
            if (availDemons.Count > 0)
            {
                int r = Random.Range(0, availDemons.Count);

                Nemesis demon = availDemons[r];

                demons.Add(demon);
                availDemons.Remove(demon);

                if (Random.Range(0, 100) <= 25)
                    oneMoreDemon = true;
                else
                    oneMoreDemon = false;
            }
            else
            {
                oneMoreDemon = false;
            }
        } while (oneMoreDemon);

        List<Nemesis> addDemons = new List<Nemesis>();

        foreach (Nemesis demon in demons)
        {
            foreach (Nemesis nemesis in GameManager.instance.nemeses)
            {
                if (nemesis.superior == demon)
                {
                    addDemons.Add(demon);
                }
            }
        }
        demons.AddRange(addDemons);
        //Debug.Log("demons.Count = " + demons.Count);
    }

    public override string Description()
    {
        string str = "";

        foreach (Nemesis nemesis in demons)
        {
            if (nemesis.personalStatus == Nemesis.PersonalStatusEnum.hidden)
            {
                str += "A demon\nAn unknown demon is present here.\n\n";
            }
            else
            {
                str += System.String.Format("A demon\n{0} is present here.\n\n", nemesis.mob.GetFullName());
            }
        }

        return str;
    }

    public override void Activate(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        Vector2Int loc;
        
        foreach (Nemesis nemesis in demons)
        {
            if (nemesis.superior == null && level.FindFreeSpotInside(out loc))
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

                GameManager.instance.nemesesPresent.Add(nemesis);
            }
        }

        foreach (Nemesis nemesis in demons)
        {
            if (nemesis.superior != null && level.FindFreeSpotNearTarget(new Vector2Int(nemesis.superior.mob.x, nemesis.superior.mob.y), out loc))
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

                GameManager.instance.nemesesPresent.Add(nemesis);
            }
        }
    }
}


