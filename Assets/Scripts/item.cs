using System;
using UnityEngine;

public class item : MonoBehaviour
{
    void FixedUpdate()
    {
        float item_offset = 0.2f;
        if (transform.position.x < globlal_screen_boundaries.left_x_edge - item_offset || transform.position.x > globlal_screen_boundaries.right_x_edge + item_offset ||
            transform.position.y > globlal_screen_boundaries.top_y_edge  + item_offset || transform.position.y < globlal_screen_boundaries.bottom_y_edge - item_offset)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player") Destroy(gameObject);
    }
}
