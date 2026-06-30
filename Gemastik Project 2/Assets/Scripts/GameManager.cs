using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsGameRunning {  get; private set; }
    public int TowerHealth;

    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private float m_timeRemaining;

    private Queue<GameObject> m_enemyPool = new Queue<GameObject>();
    private float m_xBoundary = 11f;
    private float m_yBoundary = 7f;
    private float m_enemySpawnDelay = 2f;
    private int m_poolSize = 10;
    private int m_minIndex = 1;
    private int m_maxIndex = 5;

    private void Awake()
    {
        for (int i = 0; i < m_poolSize; i++)
        {
            StoreEnemyIntoPool();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        IsGameRunning = true;
        StartCoroutine(SpawnEnemies());
    }
    // Update is called once per frame
    void Update()
    {
        UpdateEnemySpawnDelay();
        if (m_timeRemaining <= 0)
        {
            GameWin();
        }
        if (TowerHealth <= 0)
        {
            GameLose();
        }
        m_timeRemaining -= Time.deltaTime;
    }
    private void GameWin()
    {
        if (IsGameRunning)
        {
            //
            IsGameRunning = false;
        }
    }
    private void GameLose()
    {
        if (IsGameRunning)
        {
            //
            IsGameRunning = false;
        }
    }
    private IEnumerator SpawnEnemies()
    {
        while (IsGameRunning)
        {
            int randomIndex = Random.Range(m_minIndex, m_maxIndex);
            switch (randomIndex)
            {
                case 1:
                    SpawnEnemiesFromTop();
                    break;
                case 2:
                    SpawnEnemiesFromBelow();
                    break;
                case 3:
                    SpawnEnemiesFromRight();
                    break;
                case 4:
                    SpawnEnemiesFromLeft();
                    break;
            }
            yield return new WaitForSeconds(m_enemySpawnDelay);
        }
    }
    private void SpawnEnemiesFromTop()
    {
        float randomXPosition = Random.Range(-m_xBoundary, m_xBoundary);
        Vector2 m_enemySpawnPosition = new Vector2(randomXPosition, m_yBoundary);
        LetOutEnemyFromPool(m_enemySpawnPosition, Quaternion.identity);
    }
    private void SpawnEnemiesFromBelow()
    {
        float randomXPosition = Random.Range(-m_xBoundary, m_xBoundary);
        Vector2 m_enemySpawnPosition = new Vector2(randomXPosition, -m_yBoundary);
        LetOutEnemyFromPool(m_enemySpawnPosition, Quaternion.identity);
    }
    private void SpawnEnemiesFromRight()
    {
        float randomYPosition = Random.Range(-m_yBoundary, m_yBoundary);
        Vector2 m_enemySpawnPosition = new Vector2(m_xBoundary, randomYPosition);
        LetOutEnemyFromPool(m_enemySpawnPosition, Quaternion.identity);
    }
    private void SpawnEnemiesFromLeft()
    {
        float randomYPosition = Random.Range(-m_yBoundary, m_yBoundary);
        Vector2 m_enemySpawnPosition = new Vector2(-m_xBoundary, randomYPosition);
        LetOutEnemyFromPool(m_enemySpawnPosition, Quaternion.identity);
    }
    private void UpdateEnemySpawnDelay()
    {
        if (m_timeRemaining <= 60f)
        {
            m_enemySpawnDelay = 0.25f;
        }
        else if (m_timeRemaining <= 120f)
        {
            m_enemySpawnDelay = 0.5f;
        }
        else if (m_timeRemaining <= 180f)
        {
            m_enemySpawnDelay = 1f;
        }
        else if (m_timeRemaining <= 240f)
        {
            m_enemySpawnDelay = 1.5f;
        }
        else if (m_timeRemaining <= 300f)
        {
            m_enemySpawnDelay = 2f;
        }
    }
    // Enemy pool
    private GameObject StoreEnemyIntoPool()
    {
        GameObject enemy = Instantiate(m_enemyPrefab);
        enemy.SetActive(false);
        m_enemyPool.Enqueue(enemy);
        return enemy;
    }
    private GameObject LetOutEnemyFromPool(Vector2 location, Quaternion rotation)
    {
        GameObject enemy = m_enemyPool.Count > 0 ? m_enemyPool.Dequeue() : StoreEnemyIntoPool();
        enemy.transform.SetPositionAndRotation(location, rotation);
        enemy.SetActive(true);
        return enemy;
    }
    public void ReturnEnemyBackIntoPool(GameObject enemy)
    {
        enemy.SetActive(false);
        m_enemyPool.Enqueue(enemy);
    }
}