using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class shot : MonoBehaviour
{
    public bool is_moving = false;
    public bool is_stopping = false;
    public Vector3 last_static_pos;
    public Vector3 last_player_movement;
    public Vector3 initial_offset;

    void Start()
    {
        initial_offset = transform.position;
        last_static_pos = transform.position;
    }
    
    public void move()
    {
        Debug.Log(transform.position);
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
}
