using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using TMPro;
public class SpawnEnemies : NetworkBehaviour
{
    [SerializeField] private List<GameObject> spawnpoints = new List<GameObject>();
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private List<int> fibonacci = new List<int>();
    //[SerializeField] private GameObject boss;
    int indexSpawn, indexEnemies, fibonacciIndex, a = 0, b = 1;
    public static float enemiesKilled;
    static int level;
    bool startLevel;
    public static int spawnCount;
    public static bool bossExist;
    Coroutine coroutine;
    bool spawnEnemy, startCoroutine;
    float timer;
    public static float timerBoss;
    void Start()
    {
            Fibonacci();
            coroutine = StartCoroutine(StartSpawn(fibonacci[level], 0.19f));
        
    }

    void Update()
    {
        if (IsServer)
        {
            //ChooseSpawnPoints();
            ChangeLevel();
            if (spawnEnemy)
            {
                Spawn();
            }
            if (startLevel)
            {
                EffectAnimation();
            }
        }
        
    }

    void Fibonacci()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i <= 1)
            {
                fibonacciIndex = i;
            }
            else
            {
                fibonacciIndex = a + b;
                a = b;
                b = fibonacciIndex;
            }
            fibonacci.Add(fibonacciIndex);
        }
    }

    void ChangeLevel()
    {
        if (enemiesKilled == fibonacci[level])
        {
            level += 1;
            enemiesKilled = 0;
            startLevel = true;
            spawnCount = 0;
        }
    }
    void EffectAnimation()
    {
        /*if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }*/
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            startLevel = false;

            /*if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }*/
            timer = 0;
        }
    }
    private IEnumerator StartSpawn(int effectAmount, float timeBetweenEffects)
    {
        while (!startCoroutine)
        {
            if (startLevel)
            {
                for (spawnCount = 0; spawnCount < fibonacci[level]; spawnCount++)
                {
                    Debug.Log(fibonacci[level]);
                    Debug.Log(spawnCount);
                    indexSpawn = Random.Range(0, spawnpoints.Count);
                    indexEnemies = Random.Range(0, enemies.Count);
                    spawnEnemy = true;
                    yield return new WaitForSeconds(timeBetweenEffects);
                }
            }
            yield return new WaitForSeconds(timeBetweenEffects);
        }
    }
    void Spawn()
    {
        spawnEnemy = false;
        GameObject obj = Instantiate(enemies[indexEnemies], spawnpoints[indexSpawn].transform.position, spawnpoints[indexSpawn].transform.rotation);
        obj.GetComponent<Enemies>().Init(OwnerClientId);
        obj.GetComponent<NetworkObject>().Spawn();
    }
}
