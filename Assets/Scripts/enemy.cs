using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemy : MonoBehaviour
{
    public int health = 500;
    public int shot_offset = 5;
    private int offset_counter = 0;

    public bullet bullet;
    public GameObject player;
    
    void Start()
    {
        float starting_rotation = Random.Range(0, 360);
        transform.Rotate(0, 0, starting_rotation);
        
        Color random_colour = new Color(Random.Range(0F, 1F), Random.Range(0F, 1F), Random.Range(0F, 1F));
        GetComponentInChildren<SpriteRenderer>().color = random_colour;
    }

    private void FixedUpdate()
    {
        if (shot_offset == offset_counter)
        {
            GameObject new_bullet = Instantiate(bullet.gameObject, transform.position, transform.rotation);
            new_bullet.GetComponent<bullet>().set_direction(player.transform.position -  transform.position);
            offset_counter = 0;
        }
        else ++offset_counter;
    }
}
