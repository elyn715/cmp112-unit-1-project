using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class enemy_collide : MonoBehaviour
{
    //Enemy entity general
    [NonSerialized] public bool feedback_running = false;
    [NonSerialized] public bool is_active = true;
    [NonSerialized] public item_details item_data;
    
    //Drop item general
    private SpriteRenderer renderer;
    private ParticleSystem particles;

    void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        particles = GetComponentInParent<ParticleSystem>();
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (is_active)
        {
            if (collision.CompareTag("player_bullet"))
            {
                //Take damage
                GetComponentInParent<enemy>().health -= 10;

                //Add to tracking
                statistics.instance.add_damage(10);
                
                if (GetComponentInParent<enemy>().health <= 0)
                {
                    //Dead
                    //Add to tracker
                    statistics.instance.defeat_enemy();
                    
                    //Drop items
                    particles.Emit(item_data.score_items); //Score
                    if (item_data.drop_bomb) Instantiate(GetComponentInParent<stage_content>().bomb, transform.position, transform.rotation); //Bomb
                    else if (item_data.drop_life) Instantiate(GetComponentInParent<stage_content>().life, transform.position, transform.rotation); //Life
                    
                    //Reset rotation so particles fall downwards
                    transform.parent.eulerAngles = new Vector3(0, 0, 0);
                    
                    //Deactivate hitbox and hide sprite
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
                }
                //Show damage
                else if(!feedback_running) StartCoroutine(GetComponentInParent<enemy>().damage_feedback());
            }
        }
    }
}
