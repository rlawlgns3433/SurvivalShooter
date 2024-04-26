using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<GameObject> spawnPoints = new List<GameObject>();
    public static List<GameObject> usingEnemy = new List<GameObject>();
    public static List<GameObject> unusingEnemy = new List<GameObject>();
    WaitForSeconds spawnInterval = new WaitForSeconds(1f);
    Coroutine createEnemyCoroutine = null;

    public static int currentEnemyCount = 0;
    public static int wave = 1;
    public static int spawnedEnemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        createEnemyCoroutine = StartCoroutine(CoCreateEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGameover) return;

        if (currentEnemyCount <= 0)
        {
            if (createEnemyCoroutine != null)
            {
                spawnedEnemyCount = 0;
                StopCoroutine(createEnemyCoroutine);
                SpawnWave();
            }
        }
    }

    private void SpawnWave()
    {
        ++wave;
        createEnemyCoroutine = StartCoroutine(CoCreateEnemy());
    }

    IEnumerator CoCreateEnemy()
    {
        while (spawnedEnemyCount < wave * 20)
        {
            if (GameManager.instance.isGameover)
            {
                StopCoroutine(createEnemyCoroutine);
            }

            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
            int type = Random.Range(0, Enemy.TYPE_COUNT);
            bool find = false;
            // 타입의 Enemy가 있다면 가져다 쓴다. (SetActive(true);)
            foreach (var uEnemy in unusingEnemy)
            {
                EnemyTypes eType = uEnemy.GetComponent<Enemy>().type;
                if ((int)eType == type)
                {
                    find = true;

                    uEnemy.transform.position = spawnPoint.position;
                    uEnemy.transform.rotation = spawnPoint.rotation;

                    uEnemy.SetActive(true);

                    usingEnemy.Add(uEnemy);
                    unusingEnemy.Remove(uEnemy);

                    ++spawnedEnemyCount;
                    ++currentEnemyCount;
                    break;
                }
            }
            // 타입의 Enemy가 없다면 새로 만들어서 쓴다. (Instantiate)
            if (find)
            {
                yield return spawnInterval;
                continue;
            }

            var go = Instantiate(enemyPrefabs[type], spawnPoint.position, spawnPoint.rotation);
            Enemy enemy = go.GetComponent<Enemy>();
            enemy.type = (EnemyTypes)type;

            if (go != null)
            {
                usingEnemy.Add(go);
                ++spawnedEnemyCount;
                ++currentEnemyCount;
            }

            yield return spawnInterval;
        }
    }
}
