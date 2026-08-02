using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumEnemyController : MonoBehaviour
{
    public int Lives = 2;

    [SerializeField] private float m_speed;

    private GameObject m_towerGameObject;
    private Rigidbody2D m_mediumEnemyRb;
    private GameManager m_gameManager;
    private Vector2 m_mediumEnemyDirection;

    // Start is called before the first frame update
    void Start()
    {
        m_towerGameObject = GameObject.Find("Tower");
        m_mediumEnemyRb = GetComponent<Rigidbody2D>();
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Lives <= 0)
        {
            Lives = 2;
            m_gameManager.EnemyDeathCount++;
            m_gameManager.PlayEnemyDeadSound();
            m_gameManager.ReturnMediumEnemyBackIntoPool(this.gameObject);
        }
        else
        {
            UpdateMediumEnemyLocation();
        }
    }
    private void FixedUpdate()
    {
        MoveMediumEnemy();
    }
    public void MoveMediumEnemy()
    {
        if (m_gameManager.IsGameRunning)
        {
            m_mediumEnemyRb.velocity = m_mediumEnemyDirection * m_speed * Time.deltaTime;
        }
        else
        {
            m_mediumEnemyRb.velocity = Vector3.zero;
        }
    }
    private void UpdateMediumEnemyLocation()
    {
        m_mediumEnemyDirection = (m_towerGameObject.transform.position - this.transform.position).normalized;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_gameManager.TowerHealth--;
            m_gameManager.PlayPlayerHitSound();
            m_gameManager.ReturnMediumEnemyBackIntoPool(this.gameObject);
        }
    }
}