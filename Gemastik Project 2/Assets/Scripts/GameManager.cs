using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsGameRunning {  get; private set; }
    public int TowerHealth;

    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private List<EnemyController> m_selectedEnemies = new List<EnemyController>();
    [SerializeField] private float m_timeRemaining;
    [SerializeField] private float m_maxWidth = 300f;
    [SerializeField] private float m_maxHeight = 150f;

    private Queue<GameObject> m_enemyPool = new Queue<GameObject>();
    private Vector2 m_dragStartPosition;
    private Vector2 m_dragEndPosition;
    private bool m_isDragging;
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
        GetPlayerDragPosition();
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
    private void GetPlayerDragPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_dragStartPosition = Input.mousePosition;
            m_isDragging = true;
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - m_dragStartPosition;
            delta.x = Mathf.Clamp(delta.x, -m_maxWidth, m_maxWidth);
            delta.y = Mathf.Clamp(delta.y, -m_maxHeight, m_maxHeight);
            m_dragEndPosition = m_dragStartPosition + delta;
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_isDragging = false;
            SelectEnemiesInRectangle();
        }
    }
    private void SelectEnemiesInRectangle()
    {
        m_selectedEnemies.Clear();
        Rect selectionRect = GetScreenRect(m_dragStartPosition, m_dragEndPosition);
        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in allEnemies)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(enemy.transform.position);
            if (selectionRect.Contains(screenPosition))
            {
                m_selectedEnemies.Add(enemy);
                enemy.Lives--;
            }
        }
    }
    private void OnGUI()
    {
        if (!m_isDragging)
        {
            return;
        }
        Vector2 start = m_dragStartPosition;
        Vector2 end = m_dragEndPosition;
        start.y = Screen.height - start.y;
        end.y = Screen.height - end.y;
        Rect rect = GetScreenRect(start, end);
        GUI.color = new Color(0, 1, 0, 0.25f);
        GUI.Box(rect, "");
    }
    private Rect GetScreenRect(Vector2 p1, Vector2 p2)
    {
        float x = Mathf.Min(p1.x, p2.x);
        float y = Mathf.Min(p1.y, p2.y);
        float width = Mathf.Abs(p1.x - p2.x);
        float height = Mathf.Abs(p1.y - p2.y);
        return new Rect(x, y, width, height);
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