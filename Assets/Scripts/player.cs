using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Threading;

public class player : MonoBehaviour
{
    private float move_x;
    private float move_y;
    public float speed;
    private bool is_moving = false;
    private bool movement_start_pending = false;
    public float shot_move_delay = 0.02f;
    public float LEFT_X_EDGE = -6.3f;
    public float RIGHT_X_EDGE = 2.0f;
    public float TOP_Y_EDGE = 4.3f;
    public float BOTTOM_Y_EDGE = -4.3f;
    private SpriteRenderer[] sprites;
    private shot[] shots;
    
    public UnityEvent pause_event;
    public UnityEvent tutorial_event;
    public bool tutorial_on = true;
    
    public Vector2 start_pos;
    
    public int lives = 3;
    public int respawnTimer = 2500;
    private bool damage_wait_pending = false;
    private Collider2D last_collided_object;

    void Start()
    {
        sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        shots = gameObject.GetComponentsInChildren<shot>();
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.enabled = false;
            }
        }
        else
        {
            tutorial_on = false;
        }
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
        Vector2 movement_vector = movementvalue.Get<Vector2>();
        move_x = movement_vector.x;
        move_y = movement_vector.y;
    }

    void OnShoot(InputValue value)
    {
        if (tutorial_on)
        {
            tutorial_event.Invoke();
            tutorial_on = false;
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.enabled = true;
            }
            transform.position = start_pos;
            foreach (shot shot in shots)
            {
                shot.reset_pos();
            }
        }
        else
        {
            foreach (shot shot in shots)
            {
                shot.is_shooting = true;
            }
        }
    }

    void OnPause(InputValue value)
    {
        pause_event.Invoke();
    }

    void OnBomb(InputValue value)
    {
        Debug.Log("using bomb");
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
        if (tutorial_on || damage_wait_pending)
        {
            move_x = 0;
            move_y = 0;
        }
        Vector3 movement = new Vector3(move_x * speed * Time.deltaTime, move_y * speed * Time.deltaTime, 0.0f);
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
        
        //Check against boundaries
        Vector3 boundary_fixed_pos = transform.position;
        if(transform.position.x < LEFT_X_EDGE) boundary_fixed_pos.x = LEFT_X_EDGE;
        else if(transform.position.x > RIGHT_X_EDGE) boundary_fixed_pos.x = RIGHT_X_EDGE;
        if(transform.position.y > TOP_Y_EDGE) boundary_fixed_pos.y = TOP_Y_EDGE;
        else if(transform.position.y < BOTTOM_Y_EDGE) boundary_fixed_pos.y = BOTTOM_Y_EDGE;
        transform.position = boundary_fixed_pos;
        
        //Damage display delay
        if (damage_wait_pending)
        {
            Thread.Sleep(respawnTimer);
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.enabled = true;
            }
            timerEnded();
            Debug.Log("in player fixed update");
            if (last_collided_object)
            {
                last_collided_object.enabled = true;
            }
            damage_wait_pending = false;
            StartCoroutine(respawn_cooldown());
        }
    }

    private bool is_cooldown = false;
    public int damage_cooldown = 1;

    IEnumerator respawn_cooldown()
    {
        is_cooldown = true;
        yield return new WaitForSeconds(damage_cooldown);
        is_cooldown = false;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_cooldown)
        {
            if (collision.tag != "enemy_bullet")
            {
                last_collided_object = collision;
            }
            
            lives -= 1;
            collision.enabled = false;
            if (lives >= 0){
                foreach (SpriteRenderer sprite in sprites)
                {
                    sprite.enabled = false;
                }
                damage_wait_pending = true;
                string output = $"Damage Taken";
                Debug.Log(output);
            }
            else Destroy(gameObject);
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
}
