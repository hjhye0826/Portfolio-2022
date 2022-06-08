using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootingGameEnemy : MonoBehaviour
{
    protected float LifeTime = 10.0f;
    protected float health = 10.0f;
    protected float speed = 0.001f;
    protected float damage = 10.0f;
    protected int score = 1;

    GameObject PlayerObj;

    public float Health
    {
        get{ return health; }
    }

    public float Damage
    {
        get { return damage; }
    }

    public abstract void Move();

    void Start()
    {
        DestroyAfterTime();
        PlayerObj = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (ShootingGameManager.IsPlayAbleCheck() == false)
            return;

        Move();
    }

    public void OnCollisionEnter2D(Collision2D coll)
    {
        var obj = coll.gameObject;
        if (obj.CompareTag("Bullet"))
        {
            var bulletDamage = PlayerObj.GetComponent<ShootingGamePlayer>().BulletDamage;
            this.TakeDamage(bulletDamage);
            Destroy(obj);
        }
    }

    void DestroyAfterTime()
    {
        StartCoroutine(DestroyThis());
    }

    void Die()
    {
        GameObject.Find("PlayManager").SendMessage("AddScore", score);
        Destroy(this.gameObject);
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(LifeTime);

        Destroy(this.gameObject);
        yield break;
    }

    void TakeDamage(int value)
    {
        health -= value;

        if (health <= 0)
            Die();

    }
}
