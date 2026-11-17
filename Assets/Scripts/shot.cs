using System;
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

    public float shot_start_time;
    public float shot_delay = 0.005f;
    
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

    public void start_shooting()
    {
        is_shooting = true;
        shot_start_time = Time.time;
    }
    
    void Update()
    {
        if (!Keyboard.current.zKey.wasReleasedThisFrame && is_shooting)
        {
            if (shot_start_time + shot_delay <= Time.time)
            {
                Instantiate(bullet.gameObject, transform.position, transform.rotation).GetComponent<bullet>().transform.position += new Vector3(0.0f, 0.2f, 0.0f);
                shot_start_time = Time.time;
            }
        }
        else if (Keyboard.current.zKey.wasReleasedThisFrame) is_shooting = false;
    }
}
