using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldActionGameStatus : MonoBehaviour
{
    // 수리 값
    public static float GAIN_REPAIRMENT_IRON = 0.30f;
    public static float GAIN_REPAIRMENT_PLANT = 0.10f;

    // 체력 소모 값
    public static float CONSUME_SATIETY_APPLE = 0.1f;
    public static float CONSUME_SATIETY_IRON = 0.15f;
    public static float CONSUME_SATIETY_PLANT = 0.1f;
    public static float CONSUME_SATIETY_ALWAYS = 0.03f;

    // 체력 회복
    public static float REGAIN_SATIETY_APPLE = 0.7f;
    public static float REGAIN_SATIETY_PLANT = 0.2f;

    public float repairment = 0.0f;
    public float satiety = 1.0f;

    public GUIStyle guistyle;
    public int guifontsize = 24;

    void Start()
    {
        this.guistyle.fontSize = guifontsize;
    }

    public void FieldActionGameStatusInit()
    {
        this.repairment = 0.0f;
        this.satiety = 1.0f;
    }

    private void OnGUI()
    {
        if(FieldActionSceneControl.GameMode < FieldActionSceneControl.STEP.PLAY)
        {
            return;
        }

        var x = Screen.width * 0.04f;
        var y = 20.0f;

        GUI.Label(new Rect(x, y, 200.0f, 20.0f), $"체력 : {(this.satiety * 100.0f).ToString("000")}", guistyle);

        x += 200.0f;
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), $"로켓 : {(this.repairment * 100.0f).ToString("000")}", guistyle);
    }

    public void AlwaysSatiety()
    {
        this.satiety = Mathf.Clamp01(this.satiety - CONSUME_SATIETY_ALWAYS * Time.deltaTime);
    }

    public void AddRepairment(float add)
    {
        this.repairment = Mathf.Clamp01(this.repairment + add);
    }

    public void AddSatiety(float add)
    {
        this.satiety = Mathf.Clamp01(this.satiety + add);
    }

    public bool IsGameClear()
    {
        var is_clear = false;
        if (this.repairment >= 1.0f)
        {
            is_clear = true;
        }

        return is_clear;
    }

    public bool IsGameOver()
    {
        var is_over = false;
        if(this.satiety <= 0.0f)
        {
            is_over = true;
        }

        return is_over;
    }

}
