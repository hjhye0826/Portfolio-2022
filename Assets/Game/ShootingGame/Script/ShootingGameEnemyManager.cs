using System.Collections;
using UnityEngine;

public class ShootingGameEnemyManager : MonoBehaviour
{
    public GameObject[] EnemyPrefabs = { };
    public Vector2[] PosRange = new Vector2[2];

    void Start()
    {

    }

    void Update()
    {

    }

    public IEnumerator SpawnRandom()
    {
        while (true)
        {
            // random로 spawn
            var pos = new Vector2(Random.Range(PosRange[0].x, PosRange[1].x), 
                                                    Random.Range(PosRange[0].y, PosRange[1].y));
            SpawnEnemy(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)], pos);

            // n초간 제어권을 넘겨 준 후 다시 함수 내용 실행
            yield return new WaitForSeconds(2);
        }
    }

    public void SpawnEnemy(GameObject prefab, Vector3 _pos)
    {
        var enemy = Instantiate(prefab);
        enemy.transform.position = _pos;

        enemy.transform.SetParent(GameObject.Find("EnemyList").transform, false);
    }
}
