using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    private float move_x;
    private float move_y;
    public float speed;
    public float LEFT_X_EDGE = -6.3f;
    public float RIGHT_X_EDGE = 2.0f;
    public float TOP_Y_EDGE = 4.3f;
    public float BOTTOM_Y_EDGE = -4.3f;

    public UnityEvent pause_event;
    public UnityEvent tutorial_event;
    public bool tutorial_on = true;
    
    private Vector2 start_pos;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        start_pos = transform.position;
    }
    
    void OnMove(InputValue movementvalue)
    {
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
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
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

    void FixedUpdate()
    {
        Vector3 movement = new Vector2(move_x * speed * Time.deltaTime, move_y * speed * Time.deltaTime);
        transform.position += movement;
        
        //Check against boundaries
        Vector3 boundary_fixed_pos = transform.position;
        if(transform.position.x < LEFT_X_EDGE) boundary_fixed_pos.x = LEFT_X_EDGE;
        else if(transform.position.x > RIGHT_X_EDGE) boundary_fixed_pos.x = RIGHT_X_EDGE;
        if(transform.position.y > TOP_Y_EDGE) boundary_fixed_pos.y = TOP_Y_EDGE;
        else if(transform.position.y < BOTTOM_Y_EDGE) boundary_fixed_pos.y = BOTTOM_Y_EDGE;
        transform.position = boundary_fixed_pos;
    }
}
