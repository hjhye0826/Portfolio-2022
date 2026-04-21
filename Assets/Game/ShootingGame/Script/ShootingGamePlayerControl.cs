using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingGamePlayerControl : MonoBehaviour
{
    public Vector2[] AbleMovePos = new Vector2[2];
    public ShootingGamePlayer player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<ShootingGamePlayer>();
    }

    void Update()
    {
        if (ShootingGameManager.IsPlayAbleCheck() == false)
            return;

        PalyerMoveUpdate();
        PlayerAttackUpdate();
    }

    void PalyerMoveUpdate()
    {
        var playerSpeed = player.PlayerSpeed;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (transform.position.y > AbleMovePos[1].y)
                return;

            transform.Translate(0, playerSpeed, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (transform.position.x < AbleMovePos[0].x)
                return;

            transform.Translate(-playerSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (transform.position.y < AbleMovePos[0].y)
                return;

            transform.Translate(0, -playerSpeed, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (transform.position.x > AbleMovePos[1].x)
                return;

            transform.Translate(playerSpeed, 0, 0);
        }
    }

    void PlayerAttackUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.PlayerAttack();
        }
    }
}
