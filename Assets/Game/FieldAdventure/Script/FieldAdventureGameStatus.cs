using UnityEngine;

public class FieldAdventureGameStatus : MonoBehaviour
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
        guistyle.fontSize = guifontsize;
    }

    public void FieldActionGameStatusInit()
    {
        repairment = 0.0f;
        satiety = 1.0f;
    }

    private void OnGUI()
    {
        if(FieldAdventureSceneControl.GameMode < FieldAdventureSceneControl.STEP.PLAY)
        {
            return;
        }

        var x = Screen.width * 0.04f;
        var y = 20.0f;

        GUI.Label(new Rect(x, y, 200.0f, 20.0f), $"체력 : {(satiety * 100.0f).ToString("000")}", guistyle);

        x += 200.0f;
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), $"로켓 : {(repairment * 100.0f).ToString("000")}", guistyle);
    }

    public void AlwaysSatiety()
    {
        satiety = Mathf.Clamp01(satiety - CONSUME_SATIETY_ALWAYS * Time.deltaTime);
    }

    public void AddRepairment(float add)
    {
        repairment = Mathf.Clamp01(repairment + add);
    }

    public void AddSatiety(float add)
    {
        satiety = Mathf.Clamp01(satiety + add);
    }

    public bool IsGameClear()
    {
        var is_clear = false;
        if (repairment >= 1.0f)
        {
            is_clear = true;
        }

        return is_clear;
    }

    public bool IsGameOver()
    {
        var is_over = false;
        if(satiety <= 0.0f)
        {
            is_over = true;
        }

        return is_over;
    }

}
