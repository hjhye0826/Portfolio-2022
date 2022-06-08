using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldActionCaneraControl : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    void Start()
    {
        this.offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        this.transform.position = player.transform.position + offset;
    }
}
