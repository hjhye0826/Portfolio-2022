using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ColorDataManager : MonoBehaviour
{
    public int maxCnt = 6;
    public Color[] ColorList = new Color[6];
    string FileName = "/userdata.dat";

    public Color GetColor(int index)
    {
        return ColorList[index];
    }

    public void InitBlockColor()
    {
        // 일단 초기 설정 적용
        ColorList[0] = new Color(1.0f, 0.5f, 0.5f);
        ColorList[1] = Color.blue;
        ColorList[2] = Color.yellow;
        ColorList[3] = Color.green;
        ColorList[4] = Color.magenta;
        ColorList[5] = new Color(1.0f, 0.46f, 0.0f);
    }

    public void LoadBlockColor()
    {
        InitBlockColor();

        string path = Application.persistentDataPath + FileName;
        if (File.Exists(path))
        {
            try
            {
                FileStream file = new FileStream(path, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                CColorListData ColorListData = (CColorListData)bf.Deserialize(file);

                for (int i = 0; i < maxCnt; ++i)
                {
                    string[] strlist = ColorListData._colorStrList[i].Split(',');
                    ColorList[i].r = float.Parse(strlist[0]);
                    ColorList[i].g = float.Parse(strlist[1]);
                    ColorList[i].b = float.Parse(strlist[2]);
                }
                file.Close();
            }
            catch (FileNotFoundException e)
            {
                Debug.Log(e.Message);
            }

        }
    }

    public void SaveBlockColor()
    {
        CColorListData ColorListData = new CColorListData();
        for (int i = 0; i < maxCnt; ++i)
        {
            Color _color = ColorList[i];
            ColorListData._colorStrList[i] = 
                string.Format("{0}, {1}, {2}", _color.r, _color.g, _color.b);
        }

        try
        {
            string path = Application.persistentDataPath + FileName;
            FileStream file = new FileStream(path, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, ColorListData);
            file.Close();
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e.Message);
        }
    }

}

[System.Serializable]
public class CColorListData
{
    public string[] _colorStrList = new string[6];
}