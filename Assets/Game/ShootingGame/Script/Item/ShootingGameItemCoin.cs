using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingGameItemCoin : ShootingGameItem
{
    public int score = 1;

    public override void ApplyItem()
    {
        GameObject.Find("PlayManager").SendMessage("AddScore", score);
        DestroyThis();
    }
}
