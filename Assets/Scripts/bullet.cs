using System;
using UnityEngine;
using UnityEngine.Events;

public class bullet : MonoBehaviour
{
    public float speed = 5.0f;
    public bool is_enemy_bullet = false;
    private Vector2 direction = Vector2.up;
    
    void Update()
    {
        float bullet_offset = 0.6f;
        if (transform.position.x < globlal_screen_boundaries.left_x_edge - bullet_offset || transform.position.x > globlal_screen_boundaries.right_x_edge + bullet_offset ||
            transform.position.y > globlal_screen_boundaries.top_y_edge  + bullet_offset || transform.position.y < globlal_screen_boundaries.bottom_y_edge - bullet_offset)
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
