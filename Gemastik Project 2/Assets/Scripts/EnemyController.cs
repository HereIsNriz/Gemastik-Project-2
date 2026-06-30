using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private int m_lives;

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
        if (m_lives <= 0)
        {
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
        
    }
}