using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingGamePlayManager : MonoBehaviour
{
    public ShootingGameManager GameManager;
    public ShootingGameEnemyManager EnemyManager;
    public ShootingGameItemManager ItemManager;

    public Text ScoreText;
    public Text BestScoreText;

    public int score;
    public int bestScore;

    public GameObject PlayerPrefab;
    public GameObject player;
    public Vector3 playerStartPos = new Vector3(0.0f, -3.0f, 0.0f);


    void Start()
    {
        LoadUserData();
    }

    public void NewGameStart()
    {
        PlayerCreate();
        player.transform.position = playerStartPos;
        player.GetComponent<ShootingGamePlayer>().PlayerInit();

        var enemyList = GameObject.Find("EnemyList").transform;
        for(var i = 0; i< enemyList.childCount; i++)
        {
            Destroy(enemyList.GetChild(i).gameObject);
        }

        var itemList = GameObject.Find("ItemList").transform;
        for (var i = 0; i < itemList.childCount; i++)
        {
            Destroy(itemList.GetChild(i).gameObject);
        }

        var bulletList = GameObject.Find("BulletList").transform;
        for (var i = 0; i < bulletList.childCount; i++)
        {
            Destroy(bulletList.GetChild(i).gameObject);
        }

        StartCoroutine(EnemyManager.SpawnRandom());
        StartCoroutine(ItemManager.SpawnRandom());

        InitScore();
    }

    void PlayerCreate()
    {
        if (player != null)
            return;

        player = Instantiate(PlayerPrefab);
        player.transform.SetParent(GameObject.Find("GameObjectList").transform, false);
    }

    public void InitScore()
    {
        score = 0;
        ScoreText.text = string.Format("Score : {0}", score);
    }


    public void AddScore(int value)
    {
        score += value;
        ScoreText.text = string.Format("Score : {0}", score);

        if (bestScore < score)
        {
            UpdateBestScore();
            SaveUserData();
        }
    }

    void UpdateBestScore()
    {
        bestScore = score;
        BestScoreText.text = string.Format("Best Score : {0}", bestScore);

    }

    void SaveUserData()
    {
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();
    }

    void LoadUserData()
    {
        if (PlayerPrefs.HasKey("BestScore"))
            bestScore = PlayerPrefs.GetInt("BestScore");

        BestScoreText.text = "Best Score : " + bestScore.ToString();
    }

    public void GameOverEvent()
    {
        GameManager.ShootingGameModeChange(ShootingGameManager.PLAYMODE.END);
    }
}
