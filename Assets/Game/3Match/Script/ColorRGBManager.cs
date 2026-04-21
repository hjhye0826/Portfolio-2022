using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorRGBManager : MonoBehaviour
{
    public Slider slider;
    public InputField edit;

    public OptionManager optionMgr;
    
    public void UpdateOptionColorSlider()
    {
        float value = slider.value;
        edit.text = value.ToString();

        optionMgr.UpdateColorOption();
    }

    public void UpdateOptionColorEdit()
    {
        float value = float.Parse(edit.text);
        slider.value = value;

        optionMgr.UpdateColorOption();
    }
}
