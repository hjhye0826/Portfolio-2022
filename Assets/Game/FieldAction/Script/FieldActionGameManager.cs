using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldActionGameManager : MonoBehaviour
{
    public static float MOVE_AREA_RADIUS = 15.0f;

    public delegate void FieldActionStepChange(FieldActionSceneControl.STEP step);
    public event FieldActionStepChange step_change;

    public FieldActionSceneControl.STEP step = FieldActionSceneControl.STEP.NONE;
    public FieldActionSceneControl.STEP next_step = FieldActionSceneControl.STEP.NONE;

    public float step_timer = 0.0f;
    public float clear_time = 0.0f;

    public GUIStyle guistyle;
    public int guifontsize = 64;

    private FieldActionGameStatus game_status = null;
    private FieldActionRespawnManager respawn_manager = null;

    private GameObject player = null;
    private FieldActionPlayerControl player_control = null;

    private float GAME_OVER_TIME = 60.0f;

    void Start()
    {
        this.game_status = this.gameObject.GetComponent<FieldActionGameStatus>();
        this.respawn_manager = GameObject.Find("RespawnManager").GetComponent<FieldActionRespawnManager>();
        this.respawn_manager.enabled = false;

        this.guistyle.fontSize = guifontsize;

        this.player = GameObject.Find("Player");

        this.player_control = this.player.GetComponent<FieldActionPlayerControl>();
        this.player_control.FieldActionPlayerControlInit();      

        this.step = FieldActionSceneControl.STEP.TITLE;
        this.next_step = FieldActionSceneControl.STEP.TITLE;
    }

    void FieldActionGameStart()
    {
        this.step = FieldActionSceneControl.STEP.PLAY;
        this.next_step = FieldActionSceneControl.STEP.NONE;

        // 플레이어, 우주선 관련 초기화
        this.player_control.enabled = true;
        this.player_control.FieldActionPlayerInit();
        this.game_status.FieldActionGameStatusInit();

        // 필드에 있는 아이템 리스트들 초기화
        var itemroot = GameObject.Find("ItemRoot");
        var cnt = itemroot.transform.childCount;
        for(int i = 0; i < cnt; i++)
        {
            Destroy(itemroot.transform.GetChild(i).gameObject);
        }

        // 매니저 초기화
        this.respawn_manager.enabled = true;
        this.respawn_manager.RespawnManagerInit();
    }


    void Update()
    {
        this.step_timer += Time.deltaTime;
        this.StepUpdate();
        this.StepChangeUpdate();
    }

    void StepUpdate()
    {
        if (this.next_step == FieldActionSceneControl.STEP.NONE)
        {
            switch (this.step)
            {
                case FieldActionSceneControl.STEP.PLAY:
                    if (this.game_status.IsGameClear())
                    {
                        this.next_step = FieldActionSceneControl.STEP.CLEAR;
                        step_change(this.next_step);
                    }
                    if (this.game_status.IsGameOver())
                    {
                        this.next_step = FieldActionSceneControl.STEP.GAMEOVER;
                        step_change(this.next_step);
                    }
                    if (this.step_timer > GAME_OVER_TIME)
                    {
                        this.next_step = FieldActionSceneControl.STEP.GAMEOVER;
                        step_change(this.next_step);
                    }
                    break;
                case FieldActionSceneControl.STEP.CLEAR:
                case FieldActionSceneControl.STEP.GAMEOVER:
                    break;
            }
        }

    }

    void StepChangeUpdate()
    {
        while (this.next_step != FieldActionSceneControl.STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = FieldActionSceneControl.STEP.NONE;

            switch (step)
            {
                case FieldActionSceneControl.STEP.TITLE:
                    this.respawn_manager.enabled = false;
                    break;
                case FieldActionSceneControl.STEP.PLAY:
                    // 게임 씬 초기화 내용 추가
                    this.FieldActionGameStart();
                    break;
                case FieldActionSceneControl.STEP.CLEAR:
                    this.player_control.enabled = false;
                    this.respawn_manager.enabled = false;

                    this.clear_time = this.step_timer;
                    break;
                case FieldActionSceneControl.STEP.GAMEOVER:
                    this.player_control.enabled = false;
                    this.respawn_manager.enabled = false;
                    break;
            }

            this.step_timer = 0.0f;
        }
    }

    public void FieldActionGameStepChange(FieldActionSceneControl.STEP step)
    {
        this.next_step = step;
    }

    private void OnGUI()
    {
        var pos_x = Screen.width * 0.04f;
        var pos_y = Screen.height * 0.7f;

        switch (this.step)
        {
            case FieldActionSceneControl.STEP.TITLE:
                break;
            case FieldActionSceneControl.STEP.PLAY:
                GUI.color = Color.black;
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), this.step_timer.ToString("0.00"), guistyle);

                var blast_time = GAME_OVER_TIME - this.step_timer;
                GUI.Label(new Rect(pos_x, pos_y + 64, 200, 20), blast_time.ToString("0.00"));
                break;
            case FieldActionSceneControl.STEP.CLEAR:
                GUI.color = Color.black;
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), $"탈출 {(this.clear_time).ToString("0.00")}", guistyle);
                pos_y -= 32;

                var ct = (int)this.clear_time;
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
            case FieldActionSceneControl.STEP.GAMEOVER:
                GUI.color = Color.black;
                GUI.Label(new Rect(pos_x, pos_y, 200, 20), $"탈출 실패", guistyle);
                break;
        }
    }

}
