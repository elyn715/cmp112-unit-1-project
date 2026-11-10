using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class shot : MonoBehaviour
{
    public bool is_moving = false;
    public bool is_stopping = false;
    public Vector3 last_static_pos;
    public Vector3 last_player_movement;
    private Vector3 initial_offset;
    
    public bullet bullet;
    public bool is_shooting = false;

    public int shot_offset = 100;
    private int offset_counter = 0;
    
    void Start()
    {
        initial_offset = transform.position - transform.parent.position;
        last_static_pos = transform.position;
    }
    
    public void move()
    {
        if (is_stopping)
        {
            //Can move
            transform.position += last_player_movement;
        }
        if(!is_moving)
        {
            transform.position = last_static_pos;
        }
    }

    public void reset_pos()
    {
        Vector3 fixed_pos = transform.parent.position + initial_offset;
        transform.position = fixed_pos;
        
    }

    void Update()
    {
        if (!Keyboard.current.zKey.wasReleasedThisFrame && is_shooting)
        {
            if (shot_offset == offset_counter)
            {
                Vector3 new_pos = new Vector3(transform.position.x - 0.25f, transform.position.y - 0.55f, 0.0f);
                Instantiate(bullet.gameObject, this.transform);
                offset_counter = 0;
            }
            else ++offset_counter;
        }
        else if (Keyboard.current.zKey.wasReleasedThisFrame) is_shooting = false;
    }
}
