using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingGameEnemy2 : ShootingGameEnemy
{
    public ShootingGameEnemy2()
    {
        health = 20.0f;
        damage = 15.0f;
        speed = 0.008f;
        score = 2;
    }

    public override void Move()
    {
        transform.position += Vector3.down * speed;
    }

}
