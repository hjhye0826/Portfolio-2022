using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingGameItemPowerUp : ShootingGameItem
{
    public int countAcc = 2;

    public ShootingGameItemPowerUp()
    {
        resetTime = 5.0f;
    }

    public override void ApplyItem()
    {
        GameObject.FindWithTag("Player").SendMessage("AddOnceBulletCnt", countAcc);
        StartCoroutine(ResetItemEvent());
    }

    public IEnumerator ResetItemEvent()
    {
        yield return new WaitForSeconds(resetTime);

        GameObject.FindWithTag("Player").SendMessage("AddOnceBulletCnt", -countAcc);
        DestroyThis();
    }
}
