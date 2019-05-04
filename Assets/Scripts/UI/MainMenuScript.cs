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

        GameManager.instance.levelNum = 0;
        GameManager.instance.player = new PlayerMob(MobTypeEnum.mobPCAngel, 1, 1);
        SceneManager.LoadScene("IntermissionScene");
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
        GameManager.instance.levelLayout = LevelLayoutEnum.levelTest;
        GameManager.instance.monsterLayout = MonsterLayoutEnum.levelTest;
        GameManager.instance.objectiveLayout = ObjectiveLayoutEnum.levelTest;
        GameManager.instance.levelNum = 0;
        SceneManager.LoadScene("LevelScene");
    }
}
