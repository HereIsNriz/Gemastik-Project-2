using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int Lives = 2;

    [SerializeField] private float m_speed;

    private GameObject m_towerGameObject;
    private Rigidbody2D m_enemyRb;
    private GameManager m_gameManager;
    private Vector2 m_enemyDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        m_towerGameObject = GameObject.Find("Tower");
        m_enemyRb = GetComponent<Rigidbody2D>();
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Lives <= 0)
        {
            Lives = 2;
            m_gameManager.ReturnEnemyBackIntoPool(this.gameObject);
        }
        else
        {
            UpdateEnemyLocation();
        }
    }
    private void FixedUpdate()
    {
        MoveEnemy();
    }
    public void MoveEnemy()
    {
        if (m_gameManager.IsGameRunning)
        {
            m_enemyRb.velocity = m_enemyDirection * m_speed * Time.deltaTime;
        }
        else
        {
            m_enemyRb.velocity = Vector3.zero;
        }
    }
    private void UpdateEnemyLocation()
    {
        m_enemyDirection = (m_towerGameObject.transform.position - this.transform.position).normalized;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_gameManager.TowerHealth--;
            m_gameManager.ReturnEnemyBackIntoPool(this.gameObject);
        }
    }
}