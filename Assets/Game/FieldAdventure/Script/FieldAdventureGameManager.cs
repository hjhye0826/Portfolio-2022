using UnityEngine;

public class FieldAdventureGameManager : MonoBehaviour
{
    public static float MOVE_AREA_RADIUS = 15.0f;

    public delegate void FieldAdventureStepChange(FieldAdventureSceneControl.STEP step);
    public event FieldAdventureStepChange step_change;

    public FieldAdventureSceneControl.STEP step = FieldAdventureSceneControl.STEP.NONE;
    public FieldAdventureSceneControl.STEP next_step = FieldAdventureSceneControl.STEP.NONE;

    public float step_timer = 0.0f;
    public float clear_time = 0.0f;

    public GUIStyle guistyle;
    public int guifontsize = 64;

    private FieldAdventureGameStatus game_status = null;
    private FieldAdventureRespawnManager respawn_manager = null;

    private GameObject player = null;
    private FieldAdventurePlayerControl player_control = null;

    private float GAME_OVER_TIME = 60.0f;

    void Awake()
    {
        game_status = gameObject.GetComponent<FieldAdventureGameStatus>();
        respawn_manager = GameObject.Find("RespawnManager").GetComponent<FieldAdventureRespawnManager>();
        respawn_manager.enabled = false;

        guistyle.fontSize = guifontsize;

        player = GameObject.Find("Player");

        player_control = player.GetComponent<FieldAdventurePlayerControl>();
        player_control.FieldActionPlayerControlInit();      

        step = FieldAdventureSceneControl.STEP.TITLE;
        next_step = FieldAdventureSceneControl.STEP.TITLE;
    }

    void FieldActionGameStart()
    {
        step = FieldAdventureSceneControl.STEP.PLAY;
        next_step = FieldAdventureSceneControl.STEP.NONE;

        // 플레이어, 우주선 관련 초기화
        player_control.enabled = true;
        player_control.FieldActionPlayerInit();
        game_status.FieldActionGameStatusInit();

        // 필드에 있는 아이템 리스트들 초기화
        var itemroot = GameObject.Find("ItemRoot");
        var cnt = itemroot.transform.childCount;
        for(int i = 0; i < cnt; i++)
        {
            Destroy(itemroot.transform.GetChild(i).gameObject);
        }

        // 매니저 초기화
        respawn_manager.enabled = true;
        respawn_manager.RespawnManagerInit();
    }


    void Update()
    {
        step_timer += Time.deltaTime;
        StepUpdate();
        StepChangeUpdate();
    }

    void StepUpdate()
    {
        if (next_step == FieldAdventureSceneControl.STEP.NONE)
        {
            switch (step)
            {
                case FieldAdventureSceneControl.STEP.PLAY:
                    if (game_status.IsGameClear())
                    {
                        next_step = FieldAdventureSceneControl.STEP.CLEAR;
                        step_change(next_step);
                    }
                    if (game_status.IsGameOver())
                    {
                        next_step = FieldAdventureSceneControl.STEP.GAMEOVER;
                        step_change(next_step);
                    }
                    if (step_timer > GAME_OVER_TIME)
                    {
                        next_step = FieldAdventureSceneControl.STEP.GAMEOVER;
                        step_change(next_step);
                    }
                    break;
                case FieldAdventureSceneControl.STEP.CLEAR:
                case FieldAdventureSceneControl.STEP.GAMEOVER:
                    break;
            }
        }

    }

    void StepChangeUpdate()
    {
        while (next_step != FieldAdventureSceneControl.STEP.NONE)
        {
            step = next_step;
            next_step = FieldAdventureSceneControl.STEP.NONE;

            switch (step)
            {
                case FieldAdventureSceneControl.STEP.TITLE:
                    respawn_manager.enabled = false;
                    break;
                case FieldAdventureSceneControl.STEP.PLAY:
                    // 게임 씬 초기화 내용 추가
                    FieldActionGameStart();
                    break;
                case FieldAdventureSceneControl.STEP.CLEAR:
                    player_control.enabled = false;
                    respawn_manager.enabled = false;

                    clear_time = step_timer;
                    break;
                case FieldAdventureSceneControl.STEP.GAMEOVER:
                    player_control.enabled = false;
                    respawn_manager.enabled = false;
                    break;
            }

            step_timer = 0.0f;
        }
    }

    public void FieldActionGameStepChange(FieldAdventureSceneControl.STEP step)
    {
        next_step = step;
    }

    private void OnGUI()
    {
        var pos_x = Screen.width * 0.04f;
        var pos_y = Screen.height * 0.7f;

        switch (step)
        {
            case FieldAdventureSceneControl.STEP.TITLE:
                break;
            case FieldAdventureSceneControl.STEP.PLAY:
                GUI.color = Color.black;
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), step_timer.ToString("0.00"), guistyle);

                var blast_time = GAME_OVER_TIME - step_timer;
                GUI.Label(new Rect(pos_x, pos_y + 64, 200, 20), blast_time.ToString("0.00"));
                break;
            case FieldAdventureSceneControl.STEP.CLEAR:
                GUI.color = Color.black;
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), $"탈출 {(clear_time).ToString("0.00")}", guistyle);
                pos_y -= 32;

                var ct = (int)clear_time;
                if (ct > 50)
                {
                    GUI.Label(new Rect(pos_x, pos_y, 300, 20), $"아슬아슬탈출! 50초 이내를 목표로 도전하세요!");
                }
                else if (ct > 40)
                {
                    GUI.Label(new Rect(pos_x, pos_y, 300, 20), $"멋져요! 40초 안을 목표로 도전하세요!");
                }
                else if (ct > 30)
                {
                    GUI.Label(new Rect(pos_x, pos_y, 300, 20), $"대단해요! 30초 이내를 목표로 도전하세요!");
                }
                else
                {
                    GUI.Label(new Rect(pos_x, pos_y, 300, 20), $"빨라요~ 게임 마스터!");
                }

                break;
            case FieldAdventureSceneControl.STEP.GAMEOVER:
                GUI.color = Color.black;
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), $"탈출 실패", guistyle);
                break;
        }
    }

}
