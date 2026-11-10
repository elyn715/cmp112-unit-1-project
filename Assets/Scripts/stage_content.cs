using System.Collections.Generic;
using Unity.Tutorials.Editor;
using Unity.U2D.Physics;
using UnityEngine;
using UnityEditor;

public enum EnemyMoveType {PARAMETRIC, QUADRATIC, LINEAR_V, LINEAR_H, CIRCLE}

[System.Serializable] public struct move_details
{
    public bool mirror;
    
    [Header("Parametric")]
    public float para_w;
    public float para_h;

    [Header("Quadratic")]
    public float quad_w;

    public float quad_stop;
    
    [Header("Linear vertical")]
    public float vertical_stop;
    public float wait_time;
    
    [Header("Circular")]
    public Vector2 circle_start;
    public int rotations;
    
    //parametric
    public move_details(bool flip, float paraw, float parah) : this()
    {
        mirror = flip;
        para_w = paraw;
        para_h = parah;
    }

    //quadratic and linear v - had to use the same constructor as the signature was the same
    public move_details(float widthorstop, float stoporwait, bool usequad = true) : this()
    {
        if (usequad)
        {
            quad_w = widthorstop;
            quad_stop = stoporwait;
        }
        else
        {
            vertical_stop = widthorstop;
            wait_time = stoporwait;
        }
    }
    
    //circular
    public move_details(Vector2 stop, int rotate) : this()
    {
        circle_start = stop;
        rotations = rotate;
    }
}

[System.Serializable] public struct spawn_event
{
    public Vector3 spawn_pos;
    public enemy spawn_enemy; //Will be used later when there are more enemy types
    public float timestamp;
    public bool shoot_player;
    private bool has_spawned;
    public EnemyMoveType move_type;
    public move_details move_data;

    public bool get_state(){return has_spawned;}
    public void set_spawned(){has_spawned = true;}
    public spawn_event(Vector3 pos, enemy e, float time, bool targetplayer, EnemyMoveType move,
        bool flip,  float paraw, float parah,
        float quadw, float quadstop,
        float vstop,  float vwait,
        Vector2 circlestop, int rotate)
    {
        spawn_pos = pos;
        spawn_enemy = e;
        timestamp = time;
        shoot_player = targetplayer;
        has_spawned = false;
        move_type = move;
        switch (move)
        {
            case EnemyMoveType.PARAMETRIC:
                move_data = new move_details(flip, paraw, parah);
                break;
            case EnemyMoveType.QUADRATIC:
                move_data = new move_details(quadw, quadstop);
                break;
            case EnemyMoveType.LINEAR_V:
                move_data = new move_details(vstop, vwait, false);
                break;
            case EnemyMoveType.CIRCLE:
                move_data = new move_details(circlestop, rotate);
                break;
            default:
                move_data = new move_details();
                move_data.mirror = flip;
                break;
        }
        move_data = new move_details(flip, paraw, parah);
    }
}

public class stage_content : MonoBehaviour
{
    public enemy enemy;
    public float start_time;
    [SerializeField]public List<spawn_event> waves = new List<spawn_event>();
    public bool stage_running = false;
    public player player;
    
    private List<bool> spawn_state = new List<bool>();
    
    void Start()
    {
        if (!GetComponentInParent<stage_manager>().has_tutorial)
        {
            stage_running = true;
            start_time = Time.time;
        }
        
        foreach(spawn_event e in waves)
        {
            spawn_state.Add(e.get_state());
        }
    }
    
    void FixedUpdate()
    {
        if (stage_running)
        {
            int index = 0;
            foreach (spawn_event spawn in waves)
            {
                if (!spawn_state[index] && start_time + spawn.timestamp <= Time.time)
                {
                    spawn_state[index] = true;
                    enemy new_enemy = Instantiate(enemy.gameObject, this.transform).GetComponent<enemy>();
                    new_enemy.transform.position += spawn.spawn_pos;
                    new_enemy.shoot_at_player = spawn.shoot_player;
                    new_enemy.shot_direction = player.transform.position;
                    new_enemy.move_type = spawn.move_type;
                    new_enemy.move_data = spawn.move_data;
                }
                ++index;
            }
            
            enemy[] enemies = GetComponentsInChildren<enemy>();
            foreach (enemy e in enemies)
            {
                if(e.shoot_at_player) e.shot_direction = player.transform.position;
            }
        }
    }
}

