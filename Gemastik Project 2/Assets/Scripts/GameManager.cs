using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool IsGameRunning {  get; private set; }
    public int TowerHealth;

    [SerializeField] private TextMeshProUGUI m_countDownText;
    [SerializeField] private Slider m_towerHealthBar;
    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private GameObject m_levelSelectionPanel;
    [SerializeField] private GameObject m_levelNotCompletedWarning;
    [SerializeField] private GameObject m_gameLosePanel;
    [SerializeField] private GameObject m_gameWinPanel;
    [SerializeField] private List<EnemyController> m_selectedEnemies = new List<EnemyController>();
    [SerializeField] private float m_timeRemaining;
    [SerializeField] private float m_maxWidth = 300f;
    [SerializeField] private float m_maxHeight = 150f;
    [SerializeField] private int m_selectedLevel;
    [SerializeField] private int m_level1Status;
    [SerializeField] private int m_level2Status;
    [SerializeField] private int m_level3Status;
    [SerializeField] private int m_level4Status;
    [SerializeField] private int m_level5Status;

    private Queue<GameObject> m_enemyPool = new Queue<GameObject>();
    private Vector2 m_dragStartPosition;
    private Vector2 m_dragEndPosition;
    private bool m_isDragging;
    private float m_xBoundary = 11f;
    private float m_yBoundary = 7f;
    private float m_enemySpawnDelay = 2f;
    private float m_warningDelay = 2f;
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
        IsGameRunning = false;
        m_levelSelectionPanel.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        UpdateTowerHealth();
        UpdateEnemySpawnDelay();
        GetPlayerDragPosition();
        if (m_timeRemaining <= 0 && TowerHealth > 0)
        {
            GameWin();
        }
        if (TowerHealth <= 0)
        {
            GameLose();
        }
        if (IsGameRunning)
        {
            UpdateCountDown();
        }
    }
    private void GameWin()
    {
        if (IsGameRunning)
        {
            // SFX and stop the music
            switch (m_selectedLevel)
            {
                case 1:
                    m_level1Status = 1;
                    PlayerPrefs.SetInt("Level1Status", m_level1Status);
                    break;
                case 2:
                    m_level2Status = 1;
                    PlayerPrefs.SetInt("Level2Status", m_level2Status);
                    break;
                case 3:
                    m_level3Status = 1;
                    PlayerPrefs.SetInt("Level3Status", m_level3Status);
                    break;
                case 4:
                    m_level4Status = 1;
                    PlayerPrefs.SetInt("Level4Status", m_level4Status);
                    break;
                case 5:
                    m_level5Status = 1;
                    PlayerPrefs.SetInt("Level5Status", m_level5Status);
                    break;
            }
            m_countDownText.gameObject.SetActive(false);
            m_towerHealthBar.gameObject.SetActive(false);
            m_gameWinPanel.gameObject.SetActive(true);
            IsGameRunning = false;
        }
    }
    private void GameLose()
    {
        if (IsGameRunning)
        {
            // SFX and stop the music
            m_countDownText.gameObject.SetActive(false);
            m_towerHealthBar.gameObject.SetActive(false);
            m_gameLosePanel.gameObject.SetActive(true);
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
    public void ClickLevel1Button()
    {
        m_selectedLevel = 1;
        TowerHealth = 20;
        m_timeRemaining = 60f;
        m_towerHealthBar.maxValue = TowerHealth;
        m_countDownText.gameObject.SetActive(true);
        m_towerHealthBar.gameObject.SetActive(true);
        m_levelSelectionPanel.gameObject.SetActive(false);
        IsGameRunning = true;
        StartCoroutine(SpawnEnemies());
    }
    public void ClickLevel2Button()
    {
        if (PlayerPrefs.GetInt("Level1Status") == 1)
        {
            m_selectedLevel = 2;
            TowerHealth = 30;
            m_timeRemaining = 120f;
            m_towerHealthBar.maxValue = TowerHealth;
            m_countDownText.gameObject.SetActive(true);
            m_towerHealthBar.gameObject.SetActive(true);
            m_levelSelectionPanel.gameObject.SetActive(false);
            IsGameRunning = true;
            StartCoroutine(SpawnEnemies());
        }
        else
        {
            m_levelNotCompletedWarning.gameObject.SetActive(true);
            StartCoroutine(RemoveWarning());
        }
    }
    public void ClickLevel3Button()
    {
        if (PlayerPrefs.GetInt("Level2Status") == 1)
        {
            m_selectedLevel = 3;
            TowerHealth = 50;
            m_timeRemaining = 180f;
            m_towerHealthBar.maxValue = TowerHealth;
            m_countDownText.gameObject.SetActive(true);
            m_towerHealthBar.gameObject.SetActive(true);
            m_levelSelectionPanel.gameObject.SetActive(false);
            IsGameRunning = true;
            StartCoroutine(SpawnEnemies());
        }
        else
        {
            m_levelNotCompletedWarning.gameObject.SetActive(true);
            StartCoroutine(RemoveWarning());
        }
    }
    public void ClickLevel4Button()
    {
        if (PlayerPrefs.GetInt("Level3Status") == 1)
        {
            m_selectedLevel = 4;
            TowerHealth = 60;
            m_timeRemaining = 240f;
            m_towerHealthBar.maxValue = TowerHealth;
            m_countDownText.gameObject.SetActive(true);
            m_towerHealthBar.gameObject.SetActive(true);
            m_levelSelectionPanel.gameObject.SetActive(false);
            IsGameRunning = true;
            StartCoroutine(SpawnEnemies());
        }
        else
        {
            m_levelNotCompletedWarning.gameObject.SetActive(true);
            StartCoroutine(RemoveWarning());
        }
    }
    public void ClickLevel5Button()
    {
        if (PlayerPrefs.GetInt("Level4Status") == 1)
        {
            m_selectedLevel = 5;
            TowerHealth = 80;
            m_timeRemaining = 300f;
            m_towerHealthBar.maxValue = TowerHealth;
            m_countDownText.gameObject.SetActive(true);
            m_towerHealthBar.gameObject.SetActive(true);
            m_levelSelectionPanel.gameObject.SetActive(false);
            IsGameRunning = true;
            StartCoroutine(SpawnEnemies());
        }
        else
        {
            m_levelNotCompletedWarning.gameObject.SetActive(true);
            StartCoroutine(RemoveWarning());
        }
    }
    private IEnumerator RemoveWarning()
    {
        yield return new WaitForSeconds(m_warningDelay);
        m_levelNotCompletedWarning.gameObject.SetActive(false);
    }
    private void UpdateEnemySpawnDelay()
    {
        switch (m_selectedLevel)
        {
            case 1:
                if (m_timeRemaining <= 60f)
                {
                    m_enemySpawnDelay = 2f;
                }
                break;
            case 2:
                if (m_timeRemaining <= 60f)
                {
                    m_enemySpawnDelay = 1.75f;
                }
                else if (m_timeRemaining <= 120f)
                {
                    m_enemySpawnDelay = 2f;
                }
                break;
            case 3:
                if (m_timeRemaining <= 60f)
                {
                    m_enemySpawnDelay = 1.5f;
                }
                else if (m_timeRemaining <= 120f)
                {
                    m_enemySpawnDelay = 1.75f;
                }
                else if (m_timeRemaining <= 180f)
                {
                    m_enemySpawnDelay = 2f;
                }
                break;
            case 4:
                if (m_timeRemaining <= 60f)
                {
                    m_enemySpawnDelay = 1.25f;
                }
                else if (m_timeRemaining <= 120f)
                {
                    m_enemySpawnDelay = 1.5f;
                }
                else if (m_timeRemaining <= 180f)
                {
                    m_enemySpawnDelay = 1.75f;
                }
                else if (m_timeRemaining <= 240f)
                {
                    m_enemySpawnDelay = 2f;
                }
                break;
            case 5:
                if (m_timeRemaining <= 60f)
                {
                    m_enemySpawnDelay = 1f;
                }
                else if (m_timeRemaining <= 120f)
                {
                    m_enemySpawnDelay = 1.25f;
                }
                else if (m_timeRemaining <= 180f)
                {
                    m_enemySpawnDelay = 1.5f;
                }
                else if (m_timeRemaining <= 240f)
                {
                    m_enemySpawnDelay = 1.75f;
                }
                else if (m_timeRemaining <= 300f)
                {
                    m_enemySpawnDelay = 2f;
                }
                break;
        }
    }
    private void UpdateCountDown()
    {
        m_timeRemaining -= Time.deltaTime;
        int second = Mathf.FloorToInt(m_timeRemaining % 60);
        int minute = Mathf.FloorToInt(m_timeRemaining / 60);
        m_countDownText.text = string.Format("{0:00}:{1:00}", minute, second);

    }
    private void UpdateTowerHealth()
    {
        m_towerHealthBar.value = TowerHealth;
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