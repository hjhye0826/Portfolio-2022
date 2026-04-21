using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingGameEnemy1 : ShootingGameEnemy
{
    public ShootingGameEnemy1()
    {
        speed = 0.003f;
    }

    public override void Move()
    {
        transform.position += Vector3.down * speed;
    }

}
