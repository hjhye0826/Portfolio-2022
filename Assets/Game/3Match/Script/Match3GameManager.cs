using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Match3GameManager : MonoBehaviour
{
    public float LimitTime = 100.0f;
    public BlockManager blockMgr;

    [Header("Title")]
    public GameObject TitlePanel;
    public Button StartButton;
    public Button MainSceneButton;

    [Header("Play")]
    public GameObject PlayPanel;

    [Header("BlockColor")]
    public Button BlockColorButton;
    public OptionManager BlockColorPanel;
    public Button CloseButton;

    [Header("End")]
    public GameObject EndPanel;
    public Button ReStartButton;
    public Button ReTitleButton;
    public Button ReMainButton;

    [Header("Text")]
    public Text LimitTimeTxt;
    public Text MatchCntTxt;
    public Text ScoreTxt;
    public Text BestScroeTxt;

    public string LimitTimeMsg = "제한시간 : {0}초";
    public string MatchCntMsg = "매치 수 : {0}";
    public string ScoreMsg = "점수 : {0}";
    public string BestScoreMsg = "최고 점수 : {0}";

    public enum PLAYMODE { TITLE, PLAY, END };
    public static PLAYMODE Playmode = PLAYMODE.TITLE;

    int _matchCnt = 0;
    int _score = 0;
    int _bestScore = 0;
    float _limitTime = 0;

    void Awake()
    {
        TitlePanelInit();
        EndPanelInit();

        BlockColorPanel.gameObject.SetActive(false);
        CloseButton.onClick.AddListener(() => 
        {
            TitlePanel.gameObject.SetActive(true);
            BlockColorPanel.gameObject.SetActive(false);
        });
    }

    public void TitlePanelInit()
    {
        StartButton.onClick.AddListener(OnClickStartButton);
        BlockColorButton.onClick.AddListener(OnClickBlockColorButton);
        MainSceneButton.onClick.AddListener(() => { SceneManager.LoadScene("Main"); });
    }

    public void EndPanelInit()
    {
        ReStartButton.onClick.AddListener(OnClickStartButton);
        ReTitleButton.onClick.AddListener(OnClickReTitleButton);
        ReMainButton.onClick.AddListener(() => { SceneManager.LoadScene("Main"); });
    }

    public void OnClickStartButton()
    {
        GameStart();
        Match3ModeChange(PLAYMODE.PLAY);
    }

    public void OnClickBlockColorButton()
    {
        TitlePanel.gameObject.SetActive(false);
        BlockColorPanel.gameObject.SetActive(true);
        BlockColorPanel.OpenPanel();
    }

    public void OnClickReTitleButton()
    {
        Match3ModeChange(PLAYMODE.TITLE);
    }

    void Update()
    {
        if (Playmode != PLAYMODE.PLAY)
            return;

        UpdatePlayTime();

        if (_limitTime <= 0 && blockMgr.is_able_block_click())
            GameEnd();
    }

    public void GameStart()
    {
        blockMgr.InitBlocks();
        LoadBestScore();

        _matchCnt = 0;
        _score = 0;
        _limitTime = LimitTime + 1;

        InitTextUI();
        EndPanel.SetActive(false);
    }

    public void InitTextUI()
    {
        LimitTimeTxt.text = string.Format(LimitTimeMsg, _limitTime);
        MatchCntTxt.text = string.Format(MatchCntMsg, _matchCnt);
        ScoreTxt.text = string.Format(ScoreMsg, _score);
        BestScroeTxt.text = string.Format(BestScoreMsg, _bestScore);
    }

    public void UpdateMatchCount(int accCnt)
    {
        _matchCnt += accCnt;
        MatchCntTxt.text = string.Format(MatchCntMsg, _matchCnt);
    }

    public void UpdateScore(int accScore)
    {
        _score += accScore;
        ScoreTxt.text = string.Format(ScoreMsg, _score);

        if (_bestScore < _score)
            UpdateBestScore();
    }

    public void UpdateBestScore()
    {
        _bestScore = _score;
        BestScroeTxt.text = string.Format(BestScoreMsg, _bestScore);
    }

    public bool isGamePlay()
    {
        return Playmode == PLAYMODE.PLAY;
    }

    public void LoadBestScore()
    {
        if (PlayerPrefs.HasKey("BestScore"))
            _bestScore = PlayerPrefs.GetInt("BestScore");
    }

    public void SaveBestScore()
    {
        PlayerPrefs.SetInt("BestScore", _bestScore);
    }

    public void Match3ModeChange(PLAYMODE mode)
    {
        Playmode = mode;

        switch (mode)
        {
            case PLAYMODE.TITLE:
                PlayPanel.gameObject.SetActive(false);
                TitlePanel.SetActive(true);
                EndPanel.SetActive(false);
                break;
            case PLAYMODE.PLAY:
                PlayPanel.gameObject.SetActive(true);
                TitlePanel.SetActive(false);
                EndPanel.SetActive(false);
                break;
            case PLAYMODE.END:
                TitlePanel.SetActive(false);
                EndPanel.SetActive(true);
                break;
        }
    }

    private void UpdatePlayTime()
    {
        _limitTime -= Time.deltaTime;

        if (_limitTime < 0)
            _limitTime = 0;

        LimitTimeTxt.text = string.Format(LimitTimeMsg, Mathf.Floor(_limitTime));
    }

    private void GameEnd()
    {
        EndPanel.SetActive(true);
        Playmode = PLAYMODE.END;

        if (_bestScore <= _score)
            SaveBestScore();
    }
}
