using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public LevelLayoutEnum levelLayout;
    public MonsterLayoutEnum monsterLayout;
    public ObjectiveLayoutEnum objectiveLayout;
    public List<LevelModifier> levelModifiers;

    public int levelNum;
    public PlayerMob player;

    public List<Nemesis> nemeses;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        AbilityCategoryTypes.InitializeAbilityCategories();

    }

    public void SetUpNemeses()
    {

        nemeses = new List<Nemesis>();

        for (int i = 0; i < 10; i++)
        {
            Nemesis nemesis = new Nemesis()
            {
                mob = new Mob(MobTypeEnum.mobAngel, 1, 1),
                personalStatus = Nemesis.PersonalStatusEnum.hidden,
                deathStatus = Nemesis.DeathStatusEnum.alive
            };
            GameObject.Destroy(nemesis.mob.go);

            nemeses.Add(nemesis);
        }

    }
}
