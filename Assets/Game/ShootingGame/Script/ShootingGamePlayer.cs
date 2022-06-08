using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingGamePlayer : MonoBehaviour
{
    public Slider PlayerHpBar;
    public Text playerhptext;

    public float PlayerSpeed = 0.1f;
    public float maxHealth = 100.0f;
    float health = 100.0f;

    public GameObject BulletPrefab;
    public float BulletSpeed = 1.0f;
    public int BulletDamage = 10;
    public int OnceBulletCnt = 1;
    public float BulletLifeTime = 3.0f;

    public bool isBlink;

    void Awake()
    {
        this.PlayerHpBar = GameObject.Find("layerHpBarr").GetComponent<Slider>();
        this.playerhptext = GameObject.Find("HpText").GetComponent<Text>();
    }

    public void PlayerInit()
    {
        isBlink = false;

        health = maxHealth;
        PlayerHpBarUpdate(health);

        Color32 color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 255);
    }

    public void PlayerAttack()
    {
        for (int i = 0; i < this.OnceBulletCnt; ++i)
        {
            var bullet = Instantiate(BulletPrefab);
            var bulletPos = transform.position;

            if (1 < this.OnceBulletCnt)
                bulletPos.x += 0.35f * (i - 1);

            bullet.transform.position = bulletPos;
            bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.up * this.BulletSpeed);
            bullet.transform.SetParent(GameObject.Find("BulletList").transform, false);

            Destroy(bullet, this.BulletLifeTime); // n초 뒤 총알 자동 삭제
        }
    }

    void PlayerHpBarUpdate(float health)
    {
        PlayerHpBar.value = health / maxHealth;

        if (health < 0)
            health = 0;

        playerhptext.text = health.ToString();
    }

    void AddSpeed(float add)
    {
        PlayerSpeed += add;
    }

    void AddOnceBulletCnt(int add)
    {
        this.OnceBulletCnt += add;
    }

    bool IsCollision()
    {
        var ret = true;

        if (isBlink == true)
        {
            ret = false;
        }

        return ret;
    }

    public void OnCollisionEnter2D(Collision2D coll)
    {
        if(IsCollision() == false)
        {
            return;
        }

        var obj = coll.gameObject;
        if (obj.layer == LayerMask.NameToLayer("Enemy"))
        {
            var damage = obj.GetComponent<ShootingGameEnemy>().Damage;
            this.TakeDamage(damage);
            Destroy(obj);
        }
    }

    public void TakeDamage(float value)
    {
        health -= value;
        PlayerHpBarUpdate(health);

        if (health <= 0)
            Die();
        else
            BlinkStart();
    }

    void Die()
    {
        GameObject.Find("PlayManager").SendMessage("GameOverEvent");
    }

    public void BlinkStart()
    {
        isBlink = true;
        StartCoroutine(BlinkPlayer());
        StartCoroutine(StopBlink());
    }

    public IEnumerator BlinkPlayer()
    {
        while (isBlink)
        {
            Color32 color = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 10);
            yield return new WaitForSeconds(0.2f);

            if (isBlink == false)
            {
                _StopBlink();
                yield break;
            }

            GetComponent<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 125);
            yield return new WaitForSeconds(0.2f);
        }

        _StopBlink();
        yield break;
    }

    public IEnumerator StopBlink()
    {
        yield return new WaitForSeconds(2.0f);

        _StopBlink();

        isBlink = false;
        yield break;
    }

    public void _StopBlink()
    {
        Color32 color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 255);
    }

}
