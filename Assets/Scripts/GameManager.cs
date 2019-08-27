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
    public string msgLog;

    public List<Nemesis> nemeses;
    public List<Nemesis> nemesesPresent; 

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
        nemesesPresent = new List<Nemesis>();

    }

    public void SetUpNemeses()
    {
        nemeses = new List<Nemesis>();

        // add angels as nemeses
        for (int i = 0; i < Nemesis.MAX_ANGEL_NEMESIS; i++)
        {
            Nemesis nemesis = Nemesis.CreateAngelNemesis();

            nemeses.Add(nemesis);
        }

        // add demons as nemeses
        for (int i = 0; i < Nemesis.MAX_DEMON_NEMESIS; i++)
        {

            Nemesis nemesis = Nemesis.CreateDemonNemesis();

            nemeses.Add(nemesis);
        }
    }
}
