using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<GameObject> spawnPoints = new List<GameObject>();
    private static Dictionary<EnemyTypes, List<GameObject>> usingEnemy = new Dictionary<EnemyTypes, List<GameObject>>();
    private static Dictionary<EnemyTypes, List<GameObject>> unusingEnemy = new Dictionary<EnemyTypes, List<GameObject>>();
    WaitForSeconds spawnInterval = new WaitForSeconds(0.3f);
    Coroutine createEnemyCoroutine = null;

    public static int currentEnemyCount = 0;
    public static int wave = 1;
    public static int spawnedEnemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {

        for(int i = 0; i < Enemy.TYPE_COUNT; ++i)
        {
            usingEnemy[(EnemyTypes)i] = new List<GameObject>();
            unusingEnemy[(EnemyTypes)i] = new List<GameObject>();
        }

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

            if(unusingEnemy[(EnemyTypes)type].Count > 0)
            {
                var uEnemy = unusingEnemy[(EnemyTypes)type][0];
                find = true;

                uEnemy.transform.position = spawnPoint.position;
                uEnemy.transform.rotation = spawnPoint.rotation;
                uEnemy.SetActive(true);

                usingEnemy[(EnemyTypes)type].Add(uEnemy);
                unusingEnemy[(EnemyTypes)type].Remove(uEnemy);

                ++spawnedEnemyCount;
                ++currentEnemyCount;
            }

            if (find)
            {
                yield return spawnInterval;
                continue;
            }

            // 타입의 Enemy가 없다면 새로 만들어서 쓴다. (Instantiate)
            var go = Instantiate(enemyPrefabs[type], spawnPoint.position, spawnPoint.rotation);
            if (go != null)
            {
                usingEnemy[(EnemyTypes)type].Add(go);
                ++spawnedEnemyCount;
                ++currentEnemyCount;
            }

            yield return spawnInterval;
        }
    }

    public static void ReturnEnemy(GameObject enemy, EnemyTypes type)
    {
        unusingEnemy[type].Add(enemy);
        usingEnemy[type].Remove(enemy);
        --currentEnemyCount;
    }
}
