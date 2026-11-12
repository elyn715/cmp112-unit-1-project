using System;
using UnityEngine;
using System.Collections;

public class enemy_collide : MonoBehaviour
{
    private bool feedback_running = false;
    private SpriteRenderer renderer;
    private ParticleSystem particles;
    public int health = 500;

    [NonSerialized] public bool is_active = true;

    void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        particles = GetComponentInParent<ParticleSystem>();
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
        if (is_active)
        {
            if (collision.tag == "player_bullet")
            {
                health -= 10;
                Debug.Log("health is now " + health);
                if (health <= 0)
                {
                    //particles.Play();
                    particles.Emit(5);
                    transform.parent.eulerAngles = new Vector3(0, 0, 0);
                    
                    is_active = false;
                    renderer.enabled = false;
                    GetComponent<CircleCollider2D>().enabled = false;
                    Debug.Log("destroying enemy");
                }
                else if(!feedback_running)
                {
                    StartCoroutine(damage_feedback());
                }
            }
        }
    }
}
