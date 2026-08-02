using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongEnemyController : MonoBehaviour
{
    public int Lives = 3;

    [SerializeField] private float m_speed;

    private GameObject m_towerGameObject;
    private Rigidbody2D m_strongEnemyRb;
    private GameManager m_gameManager;
    private Vector2 m_strongEnemyDirection;

    // Start is called before the first frame update
    void Start()
    {
        m_towerGameObject = GameObject.Find("Tower");
        m_strongEnemyRb = GetComponent<Rigidbody2D>();
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Lives <= 0)
        {
            Lives = 3;
            m_gameManager.EnemyDeathCount++;
            m_gameManager.PlayEnemyDeadSound();
            m_gameManager.ReturnStrongEnemyBackIntoPool(this.gameObject);
        }
        else
        {
            UpdateStrongEnemyLocation();
        }
    }
    private void FixedUpdate()
    {
        MoveStrongEnemy();
    }
    public void MoveStrongEnemy()
    {
        if (m_gameManager.IsGameRunning)
        {
            m_strongEnemyRb.velocity = m_strongEnemyDirection * m_speed * Time.deltaTime;
        }
        else
        {
            m_strongEnemyRb.velocity = Vector3.zero;
        }
    }
    private void UpdateStrongEnemyLocation()
    {
        m_strongEnemyDirection = (m_towerGameObject.transform.position - this.transform.position).normalized;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_gameManager.TowerHealth--;
            m_gameManager.PlayPlayerHitSound();
            m_gameManager.ReturnStrongEnemyBackIntoPool(this.gameObject);
        }
    }
}