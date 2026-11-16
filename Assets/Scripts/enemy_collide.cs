using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class enemy_collide : MonoBehaviour
{
    [NonSerialized] public bool feedback_running = false;
    private SpriteRenderer renderer;
    private ParticleSystem particles;
    [NonSerialized] public item_details item_data;
    [NonSerialized] public bool is_active = true;

    void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        particles = GetComponentInParent<ParticleSystem>();
    }

    /*IEnumerator feedback_delay(float delay)
    {
        feedback_running = true;
        yield return new WaitForSeconds(delay);
        feedback_running = false;
    }*/
    
    /*IEnumerator damage_feedback()
    {
        feedback_running = true;
        Color base_color = renderer.color;
        renderer.color = Color.grey;
        yield return new WaitForSeconds(0.005f);
        renderer.color = base_color;
        yield return new WaitForSeconds(0.005f);
        feedback_running = false;
    }*/

    /*void FixedUpdate()
    {
        if (feedback_running)
        {
            
        }
    }*/
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (is_active)
        {
            if (collision.tag == "player_bullet")
            {
                GetComponentInParent<enemy>().health -= 10;

                statistics.instance.add_damage(10);
                
                if (GetComponentInParent<enemy>().health <= 0)
                {
                    statistics.instance.defeat_enemy();
                    
                    particles.Emit(item_data.score_items);
                    if (item_data.drop_bomb)
                    {
                        Instantiate(GetComponentInParent<stage_content>().bomb, transform.position, transform.rotation);
                    }
                    else if (item_data.drop_life)
                    {
                        Instantiate(GetComponentInParent<stage_content>().life, transform.position, transform.rotation);
                    }
                    transform.parent.eulerAngles = new Vector3(0, 0, 0);
                    
                    is_active = false;
                    renderer.enabled = false;
                    if(GetComponent<CircleCollider2D>()) GetComponent<CircleCollider2D>().enabled = false;
                    //Extra colliders, for bosses
                    if (GetComponent<BoxCollider2D>())
                    {
                        foreach (BoxCollider2D box in GetComponents<BoxCollider2D>())
                        {
                            box.enabled = false;
                        }
                    }
                    
                    Debug.Log("destroying enemy");
                }
                else if(!feedback_running)
                {
                    StartCoroutine(GetComponentInParent<enemy>().damage_feedback());
                }
            }
        }
    }
}
