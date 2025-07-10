using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Big, Small }

    [Header("Chase Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float returnDistance = 8f;

    [Header("Stats")]
    [SerializeField] private EnemyType enemyType = EnemyType.Big;
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float attackPower = 15f;
    [SerializeField] private float damageCooldown = 1f;

    [Header("References")]
    [SerializeField] private GameObject smallEnemyPrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private ItemData[] itemDropPool;

    private float currentHp;
    private float lastDamageTime;
    private bool isChasing = false;
    private Vector3 startPosition;
    private Transform player;

    private void OnEnable() => PlayerStat.OnDeath += OnPlayerDeath;
    private void OnDisable() => PlayerStat.OnDeath -= OnPlayerDeath;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPosition = transform.position;
        currentHp = maxHp;
    }

    private void Update()
    {
        if (player == null || player.GetComponent<PlayerStat>().GetCurrentHP() <= 0)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isChasing = distanceToPlayer < chaseDistance || (isChasing && distanceToPlayer < returnDistance);

        if (isChasing)
            MoveTo(player.position);
        else if (enemyType == EnemyType.Big && !IsAtPosition(startPosition))
            MoveTo(startPosition);
    }

    private void MoveTo(Vector3 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private bool IsAtPosition(Vector2 target, float tolerance = 0.05f)
    {
        return Vector2.Distance(transform.position, target) < tolerance;
    }

    public void TakeDamage(float amount)
    {
        float dmg = Random.Range(amount * 0.8f, amount * 1.2f);

        currentHp -= dmg;
        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        animator.SetTrigger("isDeath");
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        if (enemyType == EnemyType.Big)
            SpawnSmallSlimes();

        DropRandomItem();
        Destroy(gameObject);
    }

    private void SpawnSmallSlimes()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = transform.position + (Vector3)(Random.insideUnitCircle * 1f);
            Instantiate(smallEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void DropRandomItem()
    {
        int amount = Random.Range(0, 3);
        if (amount > 0 && itemDropPool.Length > 0)
        {
            int index = Random.Range(0, itemDropPool.Length);
            Inventory.Instance.AddItem(itemDropPool[index], amount);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            TryDamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            TryDamagePlayer(collision.collider);
    }

    private void TryDamagePlayer(Collider2D playerCollider)
    {
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        Debug.Log("Enemy hit player");
        playerCollider.GetComponent<PlayerStat>()?.TakeDamage(attackPower);
        lastDamageTime = Time.time;
    }

    private void OnPlayerDeath()
    {
        isChasing = false;
        currentHp = maxHp;
        transform.position = startPosition;
    }
}
