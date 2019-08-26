using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    public Button TestGameBtn;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        TestGameBtn.gameObject.SetActive(true);
#else
		TestGameBtn.gameObject.SetActive(false);
#endif
    }

    public void PlayGame()
    {
        Initialize();

        GameManager.instance.levelNum = 0;
        GameManager.instance.player = new PlayerMob(MobTypeEnum.mobPCAngel, 1, 1);
        SceneManager.LoadScene("NameScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public void TestGame()
    {
        Initialize();

        GameManager.instance.levelLayout = LevelLayoutEnum.levelTest;
        GameManager.instance.monsterLayout = MonsterLayoutEnum.levelTest;
        GameManager.instance.objectiveLayout = ObjectiveLayoutEnum.levelTest;
        GameManager.instance.levelNum = 0;

        /*
        List<LevelModifier> levelModifiers = new List<LevelModifier>();

        System.Type t = LevelModifierTypes.levelModifiers[LevelModifierTypes.LevelModifierEnum.LevModLevelShrink].GetType();
        LevelModifier lm = (LevelModifier)System.Activator.CreateInstance(t);
        lm.Initialize();
        levelModifiers.Add(lm);

        GameManager.instance.levelModifiers = levelModifiers;
        */

        SceneManager.LoadScene("LevelScene");
    }

    private void Initialize()
    {
        NemesisActivityTypes.InitNemesisActivityTypes();
        Nemesis.InitializeNames();
        DmgTypes.InitializeDmgTypes();
        AIs.InitializeAIPackages();
        Factions.InitializeFactions();
        LevelLayouts.InitializeLayouts();
        MonsterLayouts.InitializeLayouts();
        ObjectiveLayouts.InitializeLayouts();
        FinalObjectives.InitializeObjectives();

        TerrainTypes.InitializeTerrainTypes();
        MobTypes.InitializeMobTypes();
        ItemTypes.InitializeItemTypes();
        FeatureTypes.InitializeFeatureTypes();
        AbilityTypes.InitializeAbilTypes();
        EffectTypes.InitializeEffects();
        BuildingLayouts.InitializeLayouts();

        GameManager.instance.SetUpNemeses();
    }
}
