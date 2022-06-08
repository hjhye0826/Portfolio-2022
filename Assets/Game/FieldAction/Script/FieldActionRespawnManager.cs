using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldActionRespawnManager : MonoBehaviour
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
        this.AppleRespawnList = new List<Vector3>();
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("AppleRespawn");
        foreach(GameObject obj in respawns)
        {
            var renderer = obj.GetComponentInChildren<MeshRenderer>();
            if(renderer != null)
            {
                renderer.enabled = false;
            }

            this.AppleRespawnList.Add(obj.transform.position);
        }

        // 철광 리스폰 위치 리스트 초기화
        this.IronRespawnList = new List<Vector3>();
        respawns = GameObject.FindGameObjectsWithTag("IronRespawn");
        foreach (GameObject obj in respawns)
        {
            var renderer = obj.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            this.IronRespawnList.Add(obj.transform.position);
        }

        // 식물 리스폰 위치 리스트 초기화
        this.PlantRespawnList = new List<Vector3>();
        respawns = GameObject.FindGameObjectsWithTag("PlantRespawn");
        foreach (GameObject obj in respawns)
        {
            var renderer = obj.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            this.PlantRespawnList.Add(obj.transform.position);
        }

        RespawnManagerInit();
    }

    public void RespawnManagerInit()
    {
        this.respawn_timer_apple = 0.0f;
        this.respawn_timer_iron = 0.0f;
        this.respawn_timer_plant = 0.0f;

        this.RespawnApple();
        this.RespawnIron();
        this.RespawnPlant();
    }

    void Update()
    {
        this.RespawnUpadate();
    }

    void RespawnUpadate()
    {
        if (!this.is_Respawn)
            return;

        this.respawn_timer_apple += Time.deltaTime;
        this.respawn_timer_iron += Time.deltaTime;
        this.respawn_timer_plant += Time.deltaTime;

        if (respawn_timer_apple > RESPWAN_TIME_APPLE)
        {
            this.respawn_timer_apple = 0.0f;
            this.RespawnApple();
        }


        if (respawn_timer_iron > RESPWAN_TIME_IRON)
        {
            this.respawn_timer_iron = 0.0f;
            this.RespawnIron();
        }


        if (respawn_timer_plant > RESPWAN_TIME_PLANT)
        {
            this.respawn_timer_plant = 0.0f;
            this.RespawnPlant();
        }

    }

    void RespawnApple()
    {
        if (this.AppleRespawnList.Count == 0)
        {
            return;
        }

        var obj = GameObject.Instantiate(this.ApplePrefab) as GameObject;
        var idx = Random.Range(0, this.AppleRespawnList.Count);
        var pos = this.AppleRespawnList[idx];

        // 일부 위치 보정
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);

        obj.transform.position = pos;
        obj.transform.parent = GameObject.Find("ItemRoot").transform;
    }

    void RespawnIron()
    {
        if (this.IronRespawnList.Count == 0)
        {
            return;
        }

        var obj = GameObject.Instantiate(this.IronPrefab) as GameObject;
        var idx = Random.Range(0, this.IronRespawnList.Count);
        var pos = this.IronRespawnList[idx];

        // 일부 위치 보정
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);

        obj.transform.position = pos;
        obj.transform.parent = GameObject.Find("ItemRoot").transform;
    }

    void RespawnPlant()
    {
        if (this.PlantRespawnList.Count == 0)
        {
            return;
        }

        var obj = GameObject.Instantiate(this.PlantPrefab) as GameObject;
        var idx = Random.Range(0, this.PlantRespawnList.Count);
        var pos = this.PlantRespawnList[idx];

        // 일부 위치 보정
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);

        obj.transform.position = pos;
        obj.transform.parent = GameObject.Find("ItemRoot").transform;
    }
}
