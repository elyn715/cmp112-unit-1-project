using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.TextCore.LowLevel;

public class player : MonoBehaviour
{
    private float move_x;
    private float move_y;
    public float speed;
    private bool is_moving = false;
    private bool movement_start_pending = false;
    public float shot_move_delay = 0.02f;
    private SpriteRenderer[] sprites;
    private shot[] shots;

    [NonSerialized] public int score = 0;
    
    public UnityEvent pause_event;
    public UnityEvent restart_event;
    
    [NonSerialized] public Vector2 start_pos;
    
    public int lives = 3;
    public int bombs = 3;
    public int respawnTimer = 2500;
    private bool damage_wait_pending = false;
    private Collider2D last_collided_object;
    
    private bool is_cooldown = false;
    public float damage_cooldown = 1;

    private bool showing_hitbox = false;
    private SpriteRenderer hitbox_sprite;
    
    void Start()
    {
        sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        
        hitbox_sprite = transform.Find("hitbox").GetComponent<SpriteRenderer>();
        hitbox_sprite.enabled = false;
        
        shots = gameObject.GetComponentsInChildren<shot>();
        
        start_pos = transform.position;
        
        if(!hitbox_sprite) Debug.Log("hitbox not found");
    }
    
    void OnMove(InputValue movementvalue)
    {
        if (!is_moving)
        {
            is_moving = true;
            //Set off shot movement delay
            movement_start_pending = true;
        }
        Vector2 movement_vector = movementvalue.Get<Vector2>();
        move_x = movement_vector.x;
        move_y = movement_vector.y;
    }

    void OnShoot(InputValue value)
    {
        foreach (shot shot in shots)
        {
            shot.is_shooting = true;
        }
    }

    void OnShowHitbox(InputValue value)
    {
        showing_hitbox = true;
        hitbox_sprite.enabled = true;
    }
    
    void OnPause(InputValue value)
    {
        pause_event.Invoke();
    }

    void OnBomb(InputValue value)
    {
        if (bombs > 0)
        {
            Debug.Log("using bomb");
            statistics.instance.use_bomb();
        }
    }

    IEnumerator wait_to_move_shot()
    {
        yield return new WaitForSeconds(shot_move_delay);
        foreach (shot shot in shots)
        {
            shot.is_moving = true;
        }
    }

    IEnumerator wait_to_stop_shot()
    {
        foreach (shot shot in shots)
        {
            shot.is_stopping = true;
        }
        yield return new WaitForSeconds(shot_move_delay);
        //Check if the player had started moving again before the delay ended
        foreach (shot shot in shots)
        {
            shot.reset_pos();
            shot.is_stopping = false;
        }
        
        if (!is_moving)
        {
            foreach (shot shot in shots)
            {
                shot.is_moving = false;
                shot.last_static_pos = shot.transform.position;
            }
        }
    }
    
    void FixedUpdate()
    {
        if (damage_wait_pending)
        {
            move_x = 0;
            move_y = 0;
        }

        float speed_to_use = speed;
        if (showing_hitbox) speed_to_use *= 0.4f;
        Vector3 movement = new Vector3(move_x * speed_to_use * Time.deltaTime, move_y * speed_to_use * Time.deltaTime, 0.0f);
        transform.position += movement;

        //Save the movement for the shot
        if (movement != Vector3.zero)
        {
            foreach (shot shot in shots)
            {
                shot.last_player_movement = movement;
            }
        }
        
        //Player stopped
        if (is_moving && movement == Vector3.zero)
        {
            is_moving = false;
            StartCoroutine(wait_to_stop_shot());
        }

        if (movement_start_pending)
        {
            StartCoroutine(wait_to_move_shot());
            movement_start_pending = false;
        }

        foreach (shot shot in shots)
        {
            shot.move();
        }
        
        //Screen boundary check
        transform.position = bounds_check.fix_to_boundaries(transform.position);
        
        //Damage display delay
        if (damage_wait_pending)
        {
            Thread.Sleep(respawnTimer);
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.enabled = true;
            }
            timerEnded();
            if (last_collided_object)
            {
                last_collided_object.enabled = true;
            }
            damage_wait_pending = false;
            StartCoroutine(respawn_cooldown());
        }
        
        //Hitbox showing
        if (showing_hitbox && !Keyboard.current.ctrlKey.isPressed)
        {
            showing_hitbox = false;
            hitbox_sprite.enabled = false;
        }
    }
    
    IEnumerator respawn_cooldown()
    {
        is_cooldown = true;
        yield return new WaitForSeconds(damage_cooldown);
        is_cooldown = false;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "bomb")
        {
            ++bombs;
            Debug.Log("player gained bomb, bombs is now " + bombs);
        }
        else if (collision.tag == "life")
        {
            ++lives;
            Debug.Log("player gained life, lives is now " + lives);
        }
        else if (!is_cooldown)
        {
            Debug.Log("player collision");
            if (collision.tag != "enemy_bullet")
            {
                last_collided_object = collision;
            }

            statistics.instance.lose_life();
            
            lives -= 1;
            collision.enabled = false;
            if (lives >= 0){
                foreach (SpriteRenderer sprite in sprites)
                {
                    if(sprite.name != "hitbox") sprite.enabled = false;
                }
                damage_wait_pending = true;
                string output = $"Damage Taken";
                Debug.Log(output);
            }
            else
            {
                restart_event.Invoke();
            }
        }
    }
    void timerEnded()
    {
        transform.position = start_pos;
        foreach(shot shot in shots)
        {
            shot.reset_pos();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        score += 1000;
        statistics.instance.add_score(1000);
    }
}