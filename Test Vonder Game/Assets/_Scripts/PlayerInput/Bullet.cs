using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public int damage = 5;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 direction)
    {
        Vector2 curvedDirection = new Vector2(direction.x, 0.2f).normalized;

        rb.AddForce(curvedDirection * speed, ForceMode2D.Impulse);
        Destroy(gameObject, lifeTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Hit Something: " + collision.name + ", Tag: " + collision.tag);

        if (collision.CompareTag("Enemy"))
        {
            float dmg = Random.Range(damage * 0.8f, damage * 1.2f);

            collision.GetComponent<Enemy>().TakeDamage(dmg);
            Destroy(gameObject);
        }
    }

}
