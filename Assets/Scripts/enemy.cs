using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemy : MonoBehaviour
{
    public int health = 500;
    public int shot_offset = 5;
    private int offset_counter = 0;

    public bullet bullet;
    public GameObject player;
    private SpriteRenderer renderer;
    private bool feedback_running = false;
    
    void Start()
    {
        renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        
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
            if (player)
            {
                Vector2 direction = (player.transform.position - transform.position);
                direction.y += 0.5f;
                new_bullet.GetComponent<bullet>().set_direction(direction);
            }
            else
            {
                new_bullet.GetComponent<bullet>().set_direction(new Vector2(0.0f, 10.0f));
            }
            offset_counter = 0;
        }
        else ++offset_counter;
    }
    
    IEnumerator damage_feedback()
    {
        feedback_running = true;
        Color base_color = renderer.color;
        renderer.color = Color.grey;
        yield return new WaitForSeconds(0.005f);
        renderer.color = base_color;
        yield return new WaitForSeconds(0.005f);
        feedback_running = false;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player_bullet")
        {
            health -= 10;
            if (health <= 0)
            {
                Debug.Log("dead");
                Destroy(gameObject);
            }
            else if(!feedback_running) StartCoroutine(damage_feedback());
        }
    }
}
