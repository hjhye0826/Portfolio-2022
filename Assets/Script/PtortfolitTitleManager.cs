using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PtortfolitTitleManager : MonoBehaviour
{
    public string[] sceneStringList;
    public SceneChangeManager sceneChangeManager; //여기에 이름 쓴 대로 해당하는 panel 클릭 이벤트 함수 추가!

    void Awake()
    {
        foreach(var str in sceneStringList)
        {
            var sceneObj = GameObject.Find($"{str}Panel");
            if (sceneObj == null)
                continue;

            sceneObj.GetComponent<Button>().onClick.AddListener(() => sceneChangeManager.LoadGameScene(str));
        }
        
    }
}
