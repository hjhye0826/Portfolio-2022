using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FieldActionSceneControl : MonoBehaviour
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
    public GameObject StartButton = null;
    public GameObject InfoPanel = null;
    public GameObject InfoButton = null;
    public GameObject MainSceneButton = null;

    [Header("End")]
    public GameObject EndPanel = null;
    public GameObject ReStartButton = null;
    public GameObject ReTitleButton = null;
    public GameObject ReMainButton = null;

    [Header("Manager")]
    public GameObject RespawnMgr = null;
    public GameObject GameMgr = null;

    void Start()
    {
        this.TitlePanelInit();
        this.EndPanelInit();

        var player_control = FindObjectOfType<FieldActionGameManager>();
        player_control.step_change += FieldActionModeChange;

        this.FieldActionModeChange(STEP.TITLE);

        if (isStartRightAway)
        {
            this.OnClickStartButton();
        }
    }

    public void TitlePanelInit()
    {
        this.StartButton.GetComponent<Button>().onClick.AddListener(OnClickStartButton);
        this.InfoButton.GetComponent<Button>().onClick.AddListener(OnClickInfoButton);
        this.MainSceneButton.GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("Main"); });

        this.InfoPanel.SetActive(false);
    }

    public void EndPanelInit()
    {
        this.ReStartButton.GetComponent<Button>().onClick.AddListener(OnClickStartButton);
        this.ReTitleButton.GetComponent<Button>().onClick.AddListener(OnClickReTitleButton);
        this.ReMainButton.GetComponent<Button>().onClick.AddListener(()=> { SceneManager.LoadScene("Main"); });
    }


    public void OnClickStartButton()
    {
        this.FieldActionModeChange(STEP.PLAY);
        GameMgr.GetComponent<FieldActionGameManager>().FieldActionGameStepChange(STEP.PLAY);
    }

    public void OnClickInfoButton()
    {
        this.InfoPanel.SetActive(true);
    }

    public void OnClickReTitleButton()
    {
        this.FieldActionModeChange(STEP.TITLE);
        GameMgr.GetComponent<FieldActionGameManager>().FieldActionGameStepChange(STEP.TITLE);
    }

    public void FieldActionModeChange(STEP mode)
    {
        GameMode = mode;

        switch (mode)
        {
            case STEP.TITLE:
                this.TitlePanel.SetActive(true);
                this.EndPanel.SetActive(false);
                break;
            case STEP.PLAY:
                this.TitlePanel.SetActive(false);
                this.EndPanel.SetActive(false);
                break;
            case STEP.CLEAR:
            case STEP.GAMEOVER:
                this.TitlePanel.SetActive(false);
                this.EndPanel.SetActive(true);
                break;
        }
    }
}
