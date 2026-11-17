using System;
using UnityEngine;

public class item : MonoBehaviour
{
    void FixedUpdate()
    {
        //Screen bounds check
        if (!bounds_check.check_boundaries(transform.position, 0.2f)) Destroy(gameObject); 
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Hit player, so destroy
        if (collision.CompareTag("player")) Destroy(gameObject);
    }
}
