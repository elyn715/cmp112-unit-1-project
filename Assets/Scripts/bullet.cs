using System;
using UnityEngine;
using UnityEngine.Events;

public class bullet : MonoBehaviour
{
    public float speed = 5.0f;
    public bool is_enemy_bullet = false;
    private Vector2 direction = Vector2.up;
    private Rigidbody2D rb;
    
    public float LEFT_X_EDGE = -6.3f;
    public float RIGHT_X_EDGE = 2.0f;
    public float TOP_Y_EDGE = 4.3f;
    public float BOTTOM_Y_EDGE = -4.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.x < LEFT_X_EDGE || transform.position.x > RIGHT_X_EDGE ||
            transform.position.y > TOP_Y_EDGE || transform.position.y < BOTTOM_Y_EDGE)
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
        transform.position += movement;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "player_bullet" && collision.tag != "enemy_bullet" && !(collision.tag == "enemy" && is_enemy_bullet))
        {
            Destroy(gameObject);
        }
    }
}
