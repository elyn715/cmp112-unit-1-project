using System.Collections.Generic;
using Unity.U2D.Physics;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

public enum EnemyMoveType {LINEAR_H, LINEAR_V, QUADRATIC, PARAMETRIC}
public enum EnemyShootType {VERTICAL, PLAYER_FIXED, PLAYER_FOLLOW, SPIN, MULTIPLE}

[System.Serializable] public struct move_details
{
    public bool mirror;

    [Header("Parametric")]
    public float para_w;
    public float para_h;
    public float para_scale;
    
    [Header("Quadratic")]
    public float quad_w;

    public float quad_stop;
    
    [Header("Linear vertical")]
    public float vertical_stop;
    public float wait_time;
    
    //parametric
    public move_details(bool flip, float parascale, float paraw, float parah) : this()
    {
        mirror = flip;
        para_scale = parascale;
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
}
[System.Serializable] public struct shoot_details
{
    [Header("Spin")]
    public float spin_speed;

    [Header("Multiple")]
    public int shots;

    private bullet bullet;

    public shoot_details(bullet b, float speed = 0.0f, int shotnum = 1)
    {
        bullet = b;
        spin_speed = speed;
        shots = shotnum;
    }
}
[System.Serializable] public struct spawn_event
{
    public Vector3 spawn_pos;
    public enemy spawn_enemy;
    public int health;
    public float timestamp;
    private bool has_spawned;
    public int shot_rate;
    public EnemyShootType shot_type;
    public shoot_details shot_data;
    public EnemyMoveType move_type;
    public move_details move_data;

    public bool get_state(){return has_spawned;}
    public void set_spawned(){has_spawned = true;}
    public spawn_event(Vector3 pos, enemy e, int h, float time, int shotrate, EnemyShootType shot, bullet bullet, float spinspeed, int shotnum, EnemyMoveType move,
        bool flip,  float paras, float paraw, float parah,
        float quadw, float quadstop,
        float vstop,  float vwait)
    {
        spawn_pos = pos;
        spawn_enemy = e;
        health = h;
        timestamp = time;
        has_spawned = false;
        move_type = move;
        shot_type = shot;
        shot_rate = shotrate;

        switch (shot)
        {
            case EnemyShootType.VERTICAL: case EnemyShootType.PLAYER_FIXED: case EnemyShootType.PLAYER_FOLLOW:
                shot_data = new shoot_details(bullet);
                break;
            case EnemyShootType.SPIN:
                shot_data = new shoot_details(bullet, spinspeed);
                break;
            case EnemyShootType.MULTIPLE:
                shot_data = new shoot_details(bullet, 0.0f, shotnum);
                break;
            default:
                shot_data = new shoot_details();
                break;
        }
        
        switch (move)
        {
            case EnemyMoveType.PARAMETRIC:
                move_data = new move_details(flip, paras, paraw, parah);
                break;
            case EnemyMoveType.QUADRATIC:
                move_data = new move_details(quadw, quadstop);
                break;
            case EnemyMoveType.LINEAR_V:
                move_data = new move_details(vstop, vwait, false);
                break;
            default:
                move_data = new move_details();
                move_data.mirror = flip;
                break;
        }
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
                    new_enemy.move_type = spawn.move_type;
                    new_enemy.move_data = spawn.move_data;
                    new_enemy.shot_type = spawn.shot_type;
                    new_enemy.shot_data = spawn.shot_data;
                    new_enemy.shot_offset = spawn.shot_rate;
                    new_enemy.health = spawn.health;
                    if(spawn.shot_type == EnemyShootType.PLAYER_FIXED) new_enemy.shot_target = player.transform.position;
                }
                ++index;
            }
            
            enemy[] enemies = GetComponentsInChildren<enemy>();
            foreach (enemy e in enemies)
            {
                if(player && e.shot_type == EnemyShootType.PLAYER_FOLLOW) e.shot_target = player.transform.position;
            }
        }
    }
}

