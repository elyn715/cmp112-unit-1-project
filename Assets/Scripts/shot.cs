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
    private Vector3 initial_offset;
    
    public bullet bullet;
    public bool is_shooting = false;

    void Start()
    {
        initial_offset = transform.position - transform.parent.position;
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

    void Update()
    {
        //Will manage this with the new input system in next commit
        if (is_shooting && Input.GetKey(KeyCode.Z))
        {
            Vector3 new_pos = new Vector3(transform.position.x - 0.25f, transform.position.y - 0.55f, 0.0f);
            Instantiate(bullet.gameObject, new_pos, transform.rotation);
        }
        else if (Input.GetKeyUp(KeyCode.Z)) is_shooting = false;
    }
}
