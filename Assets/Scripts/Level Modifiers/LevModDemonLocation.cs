using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModifierDemonLocation : LevelModifier
{
    List<Nemesis> demons;

    public LevelModifierDemonLocation()
    {
        idType = LevelModifierTypes.LevelModifierEnum.LevModDemonLocation;
        name = "An unnamed demon";
    }

    public override bool CheckRequirements()
    {
        return false;
    }

    public void SetInitDemon(Nemesis demon)
    {
        demons = new List<Nemesis>();
        demons.Add(demon);
    }

    public override void Initialize()
    {

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


