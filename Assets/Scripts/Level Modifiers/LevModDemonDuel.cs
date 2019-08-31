using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModifierDemonDuel : LevelModifier
{
    List<Nemesis> demons;

    public LevelModifierDemonDuel()
    {
        idType = LevelModifierTypes.LevelModifierEnum.LevModDemonDuel;
        name = "A demon duel";
    }

    public override bool CheckRequirements()
    {
        int demonNum = 0;
        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.deathStatus != Nemesis.DeathStatusEnum.deceased && nemesis.mob.GetAbility(AbilityTypeEnum.abilDemon) != null && 
                nemesis.activity == NemesisActivityTypes.ActivityEnum.duel)
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
                nemesis.activity == NemesisActivityTypes.ActivityEnum.duel)
            {
                availDemons.Add(nemesis);
            }
        }

        // pick a demon and its duel partner
        int r = Random.Range(0, availDemons.Count);

        Nemesis demon = availDemons[r];
        demons.Add(demon);
        demons.Add(demon.activityTarget);

        //Debug.Log("demons.Count = " + demons.Count);
    }

    public override string Description()
    {
        string str = "";

        string demon1 = "";
        string demon2 = "";

        if (demons[0].personalStatus == Nemesis.PersonalStatusEnum.hidden)
            demon1 = "an unknown demon";
        else
            demon1 = demons[0].mob.GetFullName();

        if (demons[1].personalStatus == Nemesis.PersonalStatusEnum.hidden)
            demon2 = "an unknown demon";
        else
            demon2 = demons[0].mob.GetFullName();

        str += string.Format("A demonic duel\n{0} is duelling with {1} here.\n\n", char.ToUpper(demon1[0]) + demon1.Substring(1), demon2);

        return str;
    }

    public override void Activate(Level level, LevelGeneratorResult levelGeneratorResult)
    {
        Vector2Int loc;
        int i = 0;
        
        foreach (Nemesis nemesis in demons)
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

                if (i == 0) mob.faction = FactionEnum.factionDemonDuelist1;
                else mob.faction = FactionEnum.factionDemonDuelist2;

                BoardManager.instance.mobs.Add(mob.id, mob);
                level.AddMobToLevel(mob, loc.x, loc.y);

                GameManager.instance.nemesesPresent.Add(nemesis);

                i++;
            }
        }

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

        foreach (Nemesis nemesis in addDemons)
        {
            if (level.FindFreeSpotNearTarget(new Vector2Int(nemesis.superior.mob.x, nemesis.superior.mob.y), out loc))
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

                mob.faction = nemesis.superior.mob.faction;

                BoardManager.instance.mobs.Add(mob.id, mob);
                level.AddMobToLevel(mob, loc.x, loc.y);

                GameManager.instance.nemesesPresent.Add(nemesis);
            }
        }
    }
}


