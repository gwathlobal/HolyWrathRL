using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MainScreenStatus
{
    statusMainMenu, statusNormal, statusLookAt, statusMsgWindow, statusCharacterWindow, statusPlayerDied, statusMissionWon, statusGameWon, statusHelpWindow
}

public delegate bool ExecFunc();

public class UIManager : MonoBehaviour {

    public static UIManager instance = null;

    PlayerMob player;

    public Transform canvasTrasform;
    public Camera mapCamera;
    public LeftPanelScript LeftPanel;
    public BottomPanelScript BottomPanel;
    public RightPanelScript RightPanel;

    public MsgDialogScript MsgDialog;
    public CharacterDialogScript OldCharacterDialog;
    public MenuScript MainMenuDialog;
    public YouDiedScript YouDiedDialog;
    public MissionWonScript MissionWonDialog;
    public NewCharacterDialogScript CharacterDialog;
    public HelpDialogScript HelpDialog;

    public MainScreenStatus screenStatus;
    public GameObject selectorPrefab;
    private GameObject selectorGO;
    public Vector2Int selectorPos;
    public GameObject projectilePrefab;
    public GameObject explosionPrefab;
    public GameObject highlightPrefab;
    public GameObject highlightGO;
    public Vector2Int highlightPos;

    public bool anyBtnClicked;
    public ExecFunc execFunc;
    public int curAbility;

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
        //DontDestroyOnLoad(gameObject);

    }

    public void InitializeUI(PlayerMob _player)
    {
        mapCamera = GameObject.Find("Map Camera").GetComponent<Camera>();

        player = _player;

        LeftPanel.player = player;
        LeftPanel.RecreateInterface();

        screenStatus = MainScreenStatus.statusNormal;

        selectorGO = Instantiate(selectorPrefab, new Vector3(player.go.transform.position.x, player.go.transform.position.y, 0f), Quaternion.identity);
        selectorGO.GetComponent<SpriteRenderer>().color = new Color32(255, 216, 0, 255);
        selectorGO.SetActive(false);
        selectorPos = new Vector2Int(player.x, player.y);

        highlightGO = Instantiate(highlightPrefab, new Vector3(player.go.transform.position.x, player.go.transform.position.y, 0f), Quaternion.identity);
        highlightGO.SetActive(true);
        highlightPos = new Vector2Int(player.x, player.y);

        BottomPanel.UpdateInterface();

        //CharacterDialog.InitializeUI(player);
    }
	
    public void SetScreenStatusToNormal()
    {
        screenStatus = MainScreenStatus.statusNormal;
        selectorGO.SetActive(false);
        BottomPanel.status = BottomStatusEnum.statusLog;
        BottomPanel.SetStrings("", "");
        BottomPanel.UpdateInterface();
    }

    public void SetScreenStatusToLook(int dx, int dy, string header, string cmdline)
    {
        screenStatus = MainScreenStatus.statusLookAt;
        selectorGO.transform.position = new Vector3(dx, dy, 0f);
        selectorPos.x = dx;
        selectorPos.y = dy;
        BottomPanel.SetStrings(header, cmdline);
        selectorGO.SetActive(true);
    }

    public void ShowMsgWindow()
    {
        screenStatus = MainScreenStatus.statusMsgWindow;
        MsgDialog.msgTxt.text = BoardManager.instance.msgLog.GetLastMessagesAsText(BoardManager.instance.msgLog.MsgLength());
        MsgDialog.gameObject.SetActive(true);
    }

    public void HideMsgWindow()
    {
        screenStatus = MainScreenStatus.statusNormal;
        MsgDialog.gameObject.SetActive(false);
    }

    public void ShowCharacterWindow(TerrainTypeEnum terrainType, List<Feature> featureList, Mob mob, List<Item> itemList, bool showMobOnly = false)
    {
        screenStatus = MainScreenStatus.statusCharacterWindow;
        CharacterDialog.InitializeUI(terrainType, featureList, mob, itemList, showMobOnly);
        CharacterDialog.gameObject.SetActive(true);
    }

    public void HideCharacterWindow()
    {
        screenStatus = MainScreenStatus.statusNormal;
        CharacterDialog.gameObject.SetActive(false);
        SetScreenStatusToNormal();
    }

    public void ShowMainMenuWindow()
    {
        screenStatus = MainScreenStatus.statusMainMenu;
        MainMenuDialog.gameObject.SetActive(true);
    }

    public void HideMainMenuWindow()
    {
        screenStatus = MainScreenStatus.statusNormal;
        MainMenuDialog.gameObject.SetActive(false);
    }

    public void ShowYouDiedWindow()
    {
        screenStatus = MainScreenStatus.statusPlayerDied;
        YouDiedDialog.gameObject.SetActive(true);
    }

    public void HideYouDiedWindow()
    {
        screenStatus = MainScreenStatus.statusNormal;
        YouDiedDialog.gameObject.SetActive(false);
    }

    public void ShowMissionWonWindow()
    {
        screenStatus = MainScreenStatus.statusMissionWon;
        MissionWonDialog.gameObject.SetActive(true);
    }

    public void HideMissionWonWindow()
    {
        screenStatus = MainScreenStatus.statusNormal;
        MissionWonDialog.gameObject.SetActive(false);
    }

    public void ShowHelpDialogWindow()
    {
        screenStatus = MainScreenStatus.statusHelpWindow;
        HelpDialog.gameObject.SetActive(true);
    }

    public void HideHelpDialogWindow()
    {
        screenStatus = MainScreenStatus.statusNormal;
        HelpDialog.gameObject.SetActive(false);
    }

    public void MoveSelector(int dx, int dy)
    {
        Level level = BoardManager.instance.level;
        Vector2 endPos;

        endPos = new Vector3(selectorPos.x + dx, selectorPos.y + dy, 0);
        if ((endPos.x >= 0) && (endPos.y >= 0) && (endPos.x < level.maxX) && (endPos.y < level.maxY))
        {
            selectorGO.transform.position = endPos;
            selectorPos = new Vector2Int((int)endPos.x, (int)endPos.y);
            if (level.visible[selectorPos.x, selectorPos.y])
            {
                BottomPanel.status = BottomStatusEnum.statusLook;
                BottomPanel.UpdateInterface();
            }
            else
            {
                BottomPanel.status = BottomStatusEnum.statusLog;
                BottomPanel.UpdateInterface();
            }
        }
    }

    public void MoveHighlight(int dx, int dy)
    {
        Level level = BoardManager.instance.level;
        Vector2 endPos;

        endPos = new Vector3(highlightPos.x + dx, highlightPos.y + dy, 0);
        if ((endPos.x >= 0) && (endPos.y >= 0) && (endPos.x < level.maxX) && (endPos.y < level.maxY))
        {
            highlightGO.transform.position = endPos;
            highlightPos = new Vector2Int((int)endPos.x, (int)endPos.y);
        }
    }

    public bool CheckApplicAbility(Ability ability)
    {
        return player.CanInvokeAbility(ability);
    }

    public bool InvokeAbility(Ability ability)
    {
        PlayerMob player = BoardManager.instance.player;

        if (!CheckApplicAbility(ability)) return false;

        anyBtnClicked = true;

        if (!ability.DoesMapCheck(player))
        {
            BoardManager.instance.msgLog.ClearCurMsg();
            TargetStruct target = new TargetStruct(new Vector2Int(player.x, player.y), player);
            player.InvokeAbility(ability, target);
            return true;
        }
        else
        {

            SetScreenStatusToLook(player.x, player.y, "Select the target:", "[Enter] Select [x] Examine tile [Esc] Cancel");
            MoveSelector(0, 0);
            execFunc = () =>
            {
                if (ability.AbilityMapCheck(ability))
                {
                    SetScreenStatusToNormal();
                    execFunc = null;
                    return true;
                }
                return false;
            };

            return false;
        }

    }

    public void InvokeExecFunc()
    {
        if (execFunc != null &&
            execFunc() == true)
        {
            anyBtnClicked = true;
            BoardManager.instance.FinalizePlayerTurn();
        }
    }

    public void SetSelectorPos(int x, int y)
    {
        selectorPos.x = x;
        selectorPos.y = y;
    }

    public void SetHighlightPos(int x, int y)
    {
        highlightPos.x = x;
        highlightPos.y = y;
    }

    public bool MovePlayer(int dx, int dy)
    {
        Debug.Log(System.String.Format("MovePlayer -> ({0}, {1})", dx, dy));
        anyBtnClicked = true;
        BoardManager.instance.prevTime = Time.time;
        BoardManager.instance.msgLog.ClearCurMsg();
        return BoardManager.instance.player.Move(dx, dy);
    }

    public void CreateFloatingText(string str, Vector3 target)
    {

        // Display a floating text
        GameObject UItextGO = new GameObject("Floating Text");
        UItextGO.transform.SetParent(canvasTrasform);
        UItextGO.layer = 5; // UI layer

        RectTransform trans = UItextGO.AddComponent<RectTransform>();
        Vector3 worldPos = mapCamera.WorldToScreenPoint(target);
        trans.anchoredPosition = new Vector3(worldPos.x - Screen.width / 2, worldPos.y - Screen.height / 2, worldPos.z);
        trans.sizeDelta = new Vector2(90, 20);

        Text text = UItextGO.AddComponent<Text>();

        Font font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.font = font;
        text.text = str;
        text.fontStyle = FontStyle.Bold;
        text.supportRichText = true;
        text.fontSize = font.fontSize;
        text.color = new Color(200, 200, 200);
        text.alignment = TextAnchor.MiddleCenter;

        UItextGO.AddComponent<LabelFloatAndDisappear>();
        UItextGO.GetComponent<LabelFloatAndDisappear>().end =
            new Vector3(UItextGO.transform.position.x, UItextGO.transform.position.y + 15, UItextGO.transform.position.z);
    }

    public void ExitToMainMenu()
    {
        Destroy(GameManager.instance.player.go);
        GameManager.instance.player = null;
        GameManager.instance.levelNum = 0;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitToDesktop()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public void GoToDefeatScene()
    {
        SceneManager.LoadScene("DefeatScene");
    }

    public void GoToIntermissionScene()
    {
        SceneManager.LoadScene("IntermissionScene");
    }
}
