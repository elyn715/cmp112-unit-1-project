using System;
using UnityEngine;
using UnityEngine.Events;

public class bullet : MonoBehaviour
{
    public float speed = 5.0f;
    public bool is_enemy_bullet = false;
    private Vector2 direction = Vector2.up;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.y > 4.3f)
        {
            Destroy(gameObject);
        }
    }

    public void set_direction(Vector2 dir)
    {
        direction = dir;
    }
    
    void FixedUpdate()
    {
        Vector3 movement = new Vector2(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime); 
        if (!is_enemy_bullet)
        {
            transform.position += movement;
        }
        else
        {
            rb.AddForce(movement);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "player_bullet" && collision.tag != "enemy_bullet" && !(collision.tag == "enemy" && is_enemy_bullet))
        {
            Destroy(gameObject);
        }
        else if (collision.tag == "player")
        {
            Debug.Log("destroyed bullet");
            //Destroy(gameObject);
        }
    }
}
