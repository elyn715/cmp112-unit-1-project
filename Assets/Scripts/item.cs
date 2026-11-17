using System;
using UnityEngine;

public class item : MonoBehaviour
{
    void FixedUpdate()
    {
        if (!bounds_check.check_boundaries(transform.position, 0.2f)) Destroy(gameObject); 
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player") Destroy(gameObject);
    }
}
