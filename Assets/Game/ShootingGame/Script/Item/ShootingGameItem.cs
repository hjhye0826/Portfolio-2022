using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootingGameItem : MonoBehaviour
{
    protected float LifeTime = 5.0f;
    protected float resetTime = 0.0f;

    bool isEventStart = false;

    public abstract void ApplyItem();

    void Start()
    {
        DestroyAfterTime();
    }

    public void OnCollisionEnter2D(Collision2D coll)
    {
        if (isEventStart == true)
            return;

        var obj = coll.gameObject;
        if (obj.layer == LayerMask.NameToLayer("Player"))
        {
            isEventStart = true;
            Transparency();
            ApplyItem();
        }
    }

    void Transparency()
    {
        Color color = gameObject.GetComponent<SpriteRenderer>().color;
        color.a = 0.0f;
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    void DestroyAfterTime()
    {
        StartCoroutine(DestroyThisCheck());
    }

    protected IEnumerator DestroyThisCheck()
    {
        yield return new WaitForSeconds(LifeTime);

        if (isEventStart == true)
            yield break;

        Destroy(this.gameObject);
    }

    protected void DestroyThis()
    {
        Destroy(this.gameObject);
    }
}
