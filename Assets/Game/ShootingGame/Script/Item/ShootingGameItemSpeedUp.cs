using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingGameItemSpeedUp : ShootingGameItem
{
    public float speedAcc = 0.01f;

    public ShootingGameItemSpeedUp()
    {
        resetTime = 3.0f;
    }

    public override void ApplyItem()
    {
        GameObject.FindWithTag("Player").SendMessage("AddSpeed", speedAcc);
        StartCoroutine(ResetItemEvent());
    }

    public IEnumerator ResetItemEvent()
    {
        yield return new WaitForSeconds(resetTime);

        GameObject.FindWithTag("Player").SendMessage("AddSpeed", -speedAcc);
        DestroyThis();
    }
}
