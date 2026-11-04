using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    private float move_x;
    private float move_y;
    public float speed;
    private bool is_moving = false;
    private bool movement_start_pending = false;
    public float LEFT_X_EDGE = -6.3f;
    public float RIGHT_X_EDGE = 2.0f;
    public float TOP_Y_EDGE = 4.3f;
    public float BOTTOM_Y_EDGE = -4.3f;
    private SpriteRenderer[] sprites;
    private shot[] shots;
    
    public UnityEvent pause_event;
    public UnityEvent tutorial_event;
    public bool tutorial_on = true;
    
    private Vector2 start_pos;

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

    void OnShoot(InputValue movementvalue)
    {
        Debug.Log("shooting");
        if (tutorial_on)
        {
            tutorial_event.Invoke();
            tutorial_on = false;
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.enabled = true;
            }
            transform.position = start_pos;
        }
    }

    void OnPause(InputValue movementvalue)
    {
        Debug.Log("pausing");
        pause_event.Invoke();
    }

    void OnBomb(InputValue movementvalue)
    {
        Debug.Log("using bomb");
    }

    IEnumerator wait_to_move_shot()
    {
        yield return new WaitForSeconds(0.02f);
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
        yield return new WaitForSeconds(0.02f);
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
    }
}
