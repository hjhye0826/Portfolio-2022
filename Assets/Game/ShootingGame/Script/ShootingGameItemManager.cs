using System.Collections;
using UnityEngine;

public class ShootingGameItemManager : MonoBehaviour
{
    public GameObject[] ItemPrefabs = { };
    public Vector2[] PosList = { };

    public IEnumerator SpawnRandom()
    {
        while (true)
        {
            // random로 spawn
            var prefab = ItemPrefabs[Random.Range(0, ItemPrefabs.Length)];
            var pos = PosList[Random.Range(0, PosList.Length)];
            SpwanItem(prefab, pos);

            // n초간 제어권을 넘겨 준 후 다시 함수 내용 실행
            yield return new WaitForSeconds(5);
        }
    }

    public void SpwanItem(GameObject itemPrefab, Vector2 pos)
    {
        var item = Instantiate(itemPrefab);
        item.transform.position = pos;
        item.AddComponent<ShootingGameItem>();

        item.transform.SetParent(GameObject.Find("ItemList").transform, false);
    }
}
