using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldAdventureRespawnManager : MonoBehaviour
{
    [Header("DebugOption")]
    public bool is_Respawn = true;

    [Header("Item")]
    public GameObject ApplePrefab = null;
    public List<Vector3> AppleRespawnList = null;

    public GameObject IronPrefab = null;
    public List<Vector3> IronRespawnList = null;

    public GameObject PlantPrefab = null;
    public List<Vector3> PlantRespawnList = null;

    [Header("member")]
    public float step_timer = 0.0f;

    public static float RESPWAN_TIME_APPLE = 20.0f;
    public static float RESPWAN_TIME_IRON = 12.0f;
    public static float RESPWAN_TIME_PLANT = 6.0f;

    private float respawn_timer_apple = 0.0f;
    private float respawn_timer_iron = 0.0f;
    private float respawn_timer_plant = 0.0f;

    void Start()
    {
        // 사과 리스폰 위치 리스트 초기화
        AppleRespawnList = new List<Vector3>();
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("AppleRespawn");
        foreach(GameObject obj in respawns)
        {
            var renderer = obj.GetComponentInChildren<MeshRenderer>();
            if(renderer != null)
            {
                renderer.enabled = false;
            }

            AppleRespawnList.Add(obj.transform.position);
        }

        // 철광 리스폰 위치 리스트 초기화
        IronRespawnList = new List<Vector3>();
        respawns = GameObject.FindGameObjectsWithTag("IronRespawn");
        foreach (GameObject obj in respawns)
        {
            var renderer = obj.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            IronRespawnList.Add(obj.transform.position);
        }

        // 식물 리스폰 위치 리스트 초기화
        PlantRespawnList = new List<Vector3>();
        respawns = GameObject.FindGameObjectsWithTag("PlantRespawn");
        foreach (GameObject obj in respawns)
        {
            var renderer = obj.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            PlantRespawnList.Add(obj.transform.position);
        }

        RespawnManagerInit();
    }

    public void RespawnManagerInit()
    {
        respawn_timer_apple = 0.0f;
        respawn_timer_iron = 0.0f;
        respawn_timer_plant = 0.0f;

        RespawnApple();
        RespawnIron();
        RespawnPlant();
    }

    void Update()
    {
        RespawnUpadate();
    }

    void RespawnUpadate()
    {
        if (!is_Respawn)
            return;

        respawn_timer_apple += Time.deltaTime;
        respawn_timer_iron += Time.deltaTime;
        respawn_timer_plant += Time.deltaTime;

        if (respawn_timer_apple > RESPWAN_TIME_APPLE)
        {
            respawn_timer_apple = 0.0f;
            RespawnApple();
        }


        if (respawn_timer_iron > RESPWAN_TIME_IRON)
        {
            respawn_timer_iron = 0.0f;
            RespawnIron();
        }


        if (respawn_timer_plant > RESPWAN_TIME_PLANT)
        {
            respawn_timer_plant = 0.0f;
            RespawnPlant();
        }

    }

    void RespawnApple()
    {
        if (AppleRespawnList.Count == 0)
        {
            return;
        }

        var obj = GameObject.Instantiate(ApplePrefab) as GameObject;
        var idx = Random.Range(0, AppleRespawnList.Count);
        var pos = AppleRespawnList[idx];

        // 일부 위치 보정
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);

        obj.transform.position = pos;
        obj.transform.parent = GameObject.Find("ItemRoot").transform;
    }

    void RespawnIron()
    {
        if (IronRespawnList.Count == 0)
        {
            return;
        }

        var obj = GameObject.Instantiate(IronPrefab) as GameObject;
        var idx = Random.Range(0, IronRespawnList.Count);
        var pos = IronRespawnList[idx];

        // 일부 위치 보정
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);

        obj.transform.position = pos;
        obj.transform.parent = GameObject.Find("ItemRoot").transform;
    }

    void RespawnPlant()
    {
        if (PlantRespawnList.Count == 0)
        {
            return;
        }

        var obj = GameObject.Instantiate(PlantPrefab) as GameObject;
        var idx = Random.Range(0, PlantRespawnList.Count);
        var pos = PlantRespawnList[idx];

        // 일부 위치 보정
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);

        obj.transform.position = pos;
        obj.transform.parent = GameObject.Find("ItemRoot").transform;
    }
}
