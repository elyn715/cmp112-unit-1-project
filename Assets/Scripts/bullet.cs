using System;
using UnityEngine;
using UnityEngine.Events;

public class bullet : MonoBehaviour
{
    //General
    public float speed = 5.0f;
    public bool is_enemy_bullet = false;
    private Vector2 direction = Vector2.up;
    public bool grow = false;
    
    //Used to set bullet direction
    public void set_direction(Vector2 dir)
    {
        direction = dir;
    }
    
    void FixedUpdate()
    {
        //Move bullet
        Vector3 movement = new Vector2(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime); 
        transform.position += movement;
        
        //Scale if needed
        if (grow) transform.localScale += new Vector3(speed / 100.0f, speed / 100.0f, 0.0f);

        //Screen bounds check
        if (!bounds_check.check_boundaries(transform.position, 0.6f)) Destroy(gameObject); 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Covers player bullet hitting enemies, enemy bullet hitting player
        if (!collision.CompareTag("player_bullet") && !collision.CompareTag("enemy_bullet") && !(collision.CompareTag("enemy") && is_enemy_bullet))
        {
            //Destroy delayed if bomb
            if(collision.CompareTag("bomb")) Destroy(gameObject, 0.005f);
            else Destroy(gameObject);
        }
    }
}