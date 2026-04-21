using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingGameManager : MonoBehaviour
{
    public bool isStartRightAway = false;
    [Header("Title")]
    public GameObject TitlePanel;
    public Button StartButton;
    public Button InfoButton;
    public GameObject InfoPanel;

    [Header("Game")]
    public GameObject GamePanel;
    public GameObject GameObjectList;

    [Header("End")]
    public GameObject EndPanel;
    public Button ReStartButton;
    public Button ReTitleButton;

    [Header("Manager")]
    public GameObject PlayManager;

    public enum PLAYMODE { TITLE, PLAY, END };
    public static PLAYMODE playmode = PLAYMODE.TITLE;

    void Start()
    {
        TitlePanelInit();
        EndPanelInit();

        ShootingGameModeInit();

        if(isStartRightAway)
        {
            OnClickStartButton();
        }
    }

    public void TitlePanelInit()
    {
        StartButton.onClick.AddListener(OnClickStartButton);
        InfoButton.onClick.AddListener(OnClickInfoButton);
    }

    public void EndPanelInit()
    {
        ReStartButton.onClick.AddListener(OnClickStartButton);
        ReTitleButton.onClick.AddListener(OnClickReTitleButton);
    }

    public void ShootingGameModeInit()
    {
        ShootingGameModeChange(PLAYMODE.TITLE);
    }

    public void ShootingGameModeChange(PLAYMODE mode)
    {
        playmode = mode;
        switch (mode)
        {
            case PLAYMODE.TITLE:
                TitlePanel.SetActive(true);
                GamePanel.SetActive(false);
                EndPanel.SetActive(false);

                PlayManager.SetActive(false);

                GameObjectList.SetActive(false);
                break;
            case PLAYMODE.PLAY:
                Time.timeScale = 1.0f;

                TitlePanel.SetActive(false);
                GamePanel.SetActive(true);
                EndPanel.SetActive(false);

                PlayManager.SetActive(true);

                GameObjectList.SetActive(true);

                PlayManager.GetComponent<ShootingGamePlayManager>().NewGameStart();
                break;
            case PLAYMODE.END:
                Time.timeScale = 0.0f;

                TitlePanel.SetActive(false);
                GamePanel.SetActive(true);
                EndPanel.SetActive(true);

                PlayManager.SetActive(false);

                GameObjectList.SetActive(true);

                break;
        }
    }


    public void OnClickStartButton()
    {
        ShootingGameModeChange(PLAYMODE.PLAY);
    }

    public void OnClickInfoButton()
    {
        InfoPanel.SetActive(true);
    }
    
    public void OnClickReTitleButton()
    {
        ShootingGameModeChange(PLAYMODE.TITLE);
    }


    public static bool IsPlayAbleCheck()
    {
        if (playmode != ShootingGameManager.PLAYMODE.PLAY)
            return false;

        return true;
    }
}
