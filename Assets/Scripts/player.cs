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
    //General
    private float move_x;
    private float move_y;
    public float speed;
    private bool is_moving = false;
    [NonSerialized] public int score = 0;
    [NonSerialized] public Vector2 start_pos;
    
    
    private bool movement_start_pending = false;
    public float shot_move_delay = 0.02f;
    
    private SpriteRenderer[] sprites;
    private shot[] shots;
    
    //Events
    public UnityEvent pause_event;
    public UnityEvent restart_event;
    
    //Items and damage
    public int lives = 3;
    public int bombs = 3;
    public int respawnTimer = 2500;
    private bool damage_wait_pending = false;
    
    private bool is_cooldown = false;
    public float damage_cooldown = 1;

    private bool showing_hitbox = false;
    private SpriteRenderer hitbox_sprite;
    public bomb bomb;
    
    void Start()
    {
        //Get sprites and shots
        sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        hitbox_sprite = transform.Find("hitbox").GetComponent<SpriteRenderer>();
        hitbox_sprite.enabled = false;
        shots = gameObject.GetComponentsInChildren<shot>();
        
        start_pos = transform.position;
    }
    
    void OnMove(InputValue movementvalue)
    {
        if (!is_moving)
        {
            is_moving = true;
            //Set off shot movement delay
            movement_start_pending = true;
        }
        
        //Set velocity
        Vector2 movement_vector = movementvalue.Get<Vector2>();
        move_x = movement_vector.x;
        move_y = movement_vector.y;
    }

    void OnShoot(InputValue value)
    {
        foreach (shot shot in shots)
        {
            //Shoot
            shot.is_shooting = true;
        }
    }

    void OnShowHitbox(InputValue value)
    {
        //Show hitbox sprite
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
            --bombs;
            Instantiate(bomb.gameObject, transform);
            
            //Add to tracking
            statistics.instance.use_bomb();
        }
    }

    IEnumerator wait_to_move_shot()
    {
        //Wait a small amount of time before moving with the player
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
        
        //This is needed to keep following the player if they started moving again before the delay ended
        foreach (shot shot in shots)
        {
            shot.reset_pos();
            shot.is_stopping = false;
        }
        
        //Position reset
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
            //Stop player from moving
            move_x = 0;
            move_y = 0;
        }

        //Calculate speed - moves slower if showing hitbox
        float speed_to_use = speed;
        if (showing_hitbox) speed_to_use *= 0.4f;
        
        //Move
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
        
        //Player stopped moving
        if (is_moving && movement == Vector3.zero)
        {
            is_moving = false;
            StartCoroutine(wait_to_stop_shot());
        }

        //Player started moving
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
            //Pause and hide player
            Thread.Sleep(respawnTimer);
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.enabled = true;
            }
            
            //Reset to start position
            timerEnded();
            
            //Start invincibillity cooldown
            damage_wait_pending = false;
            StartCoroutine(respawn_cooldown());
        }
        
        if (showing_hitbox && !Keyboard.current.ctrlKey.isPressed)
        {
            //Hide hitbox
            showing_hitbox = false;
            hitbox_sprite.enabled = false;
        }
    }
    
    IEnumerator respawn_cooldown()
    {
        //Stops damage for the cooldown
        is_cooldown = true;
        yield return new WaitForSeconds(damage_cooldown);
        is_cooldown = false;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bomb_item"))
        {
            ++bombs;
            Debug.Log("player gained bomb, bombs is now " + bombs);
        }
        else if (collision.CompareTag("life"))
        {
            ++lives;
            Debug.Log("player gained life, lives is now " + lives);
        }
        else if (!is_cooldown)
        {
            //Hit
            //Log in tracker
            statistics.instance.lose_life();
            
            lives -= 1;
            if (lives >= 0){
                //Hide player
                foreach (SpriteRenderer sprite in sprites)
                {
                    if(sprite.name != "hitbox") sprite.enabled = false;
                }
                damage_wait_pending = true;
            }
            else
            {
                //Restart level
                restart_event.Invoke();
            }
        }
    }
    
    //Resets position
    void timerEnded()
    {
        transform.position = start_pos;
        foreach(shot shot in shots)
        {
            shot.reset_pos();
        }
    }

    //Increases score
    void OnParticleCollision(GameObject other)
    {
        score += 1000;
        statistics.instance.add_score(1000);
    }
}