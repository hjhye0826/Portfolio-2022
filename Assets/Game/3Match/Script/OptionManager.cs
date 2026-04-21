using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public Button SaveBoutton;
    public Button ResetButton;

    public int maxCnt = 6;
    public Image[]  ImageList;
    public Toggle[] ToggleList;
    public GameObject[] RGBList;
    public ColorDataManager colorMgr;

    private int curIndex;

    void Awake()
    {
        ResetBlockColor();

        SaveBoutton.onClick.AddListener(() => { colorMgr.SaveBlockColor(); });
        ResetButton.onClick.AddListener(() => 
        { 
            colorMgr.InitBlockColor();
            UpdateBlockColor();
        });
    }

    public void OpenPanel()
    {
        InitOptionControl();

        colorMgr.LoadBlockColor();
        UpdateBlockColor();
    }

    public void InitOptionControl()
    {
        ToggleGroup toggleGroup = ToggleList[0].group;
        toggleGroup.SetAllTogglesOff();

        GameObject RGroupBox = RGBList[0];
        Slider RSlider = RGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        RSlider.value = 0;

        GameObject GGroupBox = RGBList[1];
        Slider GSlider = GGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        GSlider.value = 0;

        GameObject BGroupBox = RGBList[2];
        Slider BSlider = BGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        BSlider.value = 0;
    }
  
    public void UpdateBlockColor()
    {
        for (int i = 0; i < 6; i++)
        {
            if (ImageList[i])
                ImageList[i].GetComponent<Image>().color = colorMgr.ColorList[i];
        }
    }

    public void UpdateColorOption(int _index)
    {
        curIndex = _index;
        Color color = colorMgr.ColorList[_index];

        Vector3 color255;
        color255.x = Mathf.Round(color.r * 255.0f);
        color255.y = Mathf.Round(color.g * 255.0f);
        color255.z = Mathf.Round(color.b * 255.0f);

        GameObject RGroupBox = RGBList[0];
        Slider RSlider = RGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        RSlider.value = color255.x;

        GameObject GGroupBox = RGBList[1];
        Slider GSlider = GGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        GSlider.value = color255.y;

        GameObject BGroupBox = RGBList[2];
        Slider BSlider = BGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        BSlider.value = color255.z;
    }

    public void UpdateColorOption()
    {
        Color color = new Color();

        GameObject RGroupBox = RGBList[0];
        Slider RSlider = RGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        color.r = RSlider.value / 255.0f;

        GameObject GGroupBox = RGBList[1];
        Slider GSlider = GGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        color.g = GSlider.value / 255.0f;

        GameObject BGroupBox = RGBList[2];
        Slider BSlider = BGroupBox.transform.GetChild(1).gameObject.GetComponent<Slider>();
        color.b = BSlider.value / 255.0f;

        color.a = 1.0f;

        colorMgr.ColorList[curIndex] = color;
        ImageList[curIndex].GetComponent<Image>().color = color;
    }

    public void ResetBlockColor()
    {
        InitOptionControl();
        colorMgr.InitBlockColor();
        UpdateBlockColor();
    }
}