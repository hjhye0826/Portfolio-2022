using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FieldAdventureSceneControl : MonoBehaviour
{
    public bool isStartRightAway = false;

    public enum STEP
    {
        NONE = -1,
        TITLE = 0,
        PLAY,
        CLEAR,
        GAMEOVER,
        NUM,
    };


    public static STEP GameMode = STEP.TITLE;

    [Header("Title")]
    public GameObject TitlePanel = null;
    public Button StartButton = null;
    public GameObject InfoPanel = null;
    public Button InfoButton = null;
    public Button MainSceneButton = null;

    [Header("End")]
    public GameObject EndPanel = null;
    public Button ReStartButton = null;
    public Button ReTitleButton = null;
    public Button ReMainButton = null;

    [Header("Manager")]
    public GameObject RespawnMgr = null;
    public GameObject GameMgr = null;

    void Awake()
    {
        TitlePanelInit();
        EndPanelInit();

        var player_control = FindObjectOfType<FieldAdventureGameManager>();
        player_control.step_change += FieldActionModeChange;

        FieldActionModeChange(STEP.TITLE);

        if (isStartRightAway)
        {
            OnClickStartButton();
        }
    }

    public void TitlePanelInit()
    {
        StartButton.onClick.AddListener(OnClickStartButton);
        InfoButton.onClick.AddListener(OnClickInfoButton);
        MainSceneButton.onClick.AddListener(() => { SceneManager.LoadScene("Main"); });

        InfoPanel.SetActive(false);
    }

    public void EndPanelInit()
    {
        ReStartButton.onClick.AddListener(OnClickStartButton);
        ReTitleButton.onClick.AddListener(OnClickReTitleButton);
        ReMainButton.onClick.AddListener(()=> { SceneManager.LoadScene("Main"); });
    }


    public void OnClickStartButton()
    {
        FieldActionModeChange(STEP.PLAY);
        GameMgr.GetComponent<FieldAdventureGameManager>().FieldActionGameStepChange(STEP.PLAY);
    }

    public void OnClickInfoButton()
    {
        InfoPanel.SetActive(true);
    }

    public void OnClickReTitleButton()
    {
        FieldActionModeChange(STEP.TITLE);
        GameMgr.GetComponent<FieldAdventureGameManager>().FieldActionGameStepChange(STEP.TITLE);
    }

    public void FieldActionModeChange(STEP mode)
    {
        GameMode = mode;

        switch (mode)
        {
            case STEP.TITLE:
                TitlePanel.SetActive(true);
                EndPanel.SetActive(false);
                break;
            case STEP.PLAY:
                TitlePanel.SetActive(false);
                EndPanel.SetActive(false);
                break;
            case STEP.CLEAR:
            case STEP.GAMEOVER:
                TitlePanel.SetActive(false);
                EndPanel.SetActive(true);
                break;
        }
    }
}
