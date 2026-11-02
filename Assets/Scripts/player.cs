using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class player : MonoBehaviour
{
    private float move_x;
    private float move_y;
    public float speed;

    public UnityEvent pause_event;
    public UnityEvent tutorial_event;
    private bool tutorial_on = true;
    
    private Vector2 start_pos;

    void Start()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
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
    }
}
