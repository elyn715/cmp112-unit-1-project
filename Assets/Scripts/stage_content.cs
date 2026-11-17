using System;
using System.Collections;
using System.Collections.Generic;
using Unity.U2D.Physics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

public enum EnemyMoveType {LINEAR_H, LINEAR_V, QUADRATIC, PARAMETRIC}
public enum EnemyShootType {NONE, VERTICAL, PLAYER_FIXED, PLAYER_FOLLOW, SPIN, MULTIPLE}

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
    public bool bullet_grow;
    public float start_delay;
    
    [Header("Spin")]
    public float spin_speed;

    [Header("Multiple")]
    public int shots;

    private bullet bullet;

    public shoot_details(bullet b, bool grow, float delay = 0.0f, float speed = 0.0f, int shotnum = 1)
    {
        bullet = b;
        spin_speed = speed;
        shots = shotnum;
        bullet_grow = grow;
        start_delay = delay;
    }
}

[System.Serializable] public struct item_details
{
    public int score_items;
    
    [Header("Only pick one")]
    public bool drop_bomb;
    public bool drop_life;

    public item_details(int items, bool bomb, bool life)
    {
        score_items = items;
        drop_bomb = bomb;
        drop_life = life;
    }
}
[System.Serializable] public struct spawn_event
{
    public string event_name;
    
    public Vector3 spawn_pos;
    public enemy spawn_enemy;
    
    public int enemies_to_spawn;
    public float delay_between_spawn;
    
    public float timestamp;

    public bool prevent_spinning;
    public bool is_final;

    public float speed_mod;
    
    public item_details item_data;
    public EnemyShootType shot_type;
    public EnemyShootType sub_shot_type;
    public shoot_details shot_data;
    public EnemyMoveType move_type;
    public move_details move_data;
    
    private bool has_spawned;

    public bool get_state(){return has_spawned;}
    public spawn_event(string name, Vector3 pos, enemy e, int num, float interval, int items, bool bomb, bool life, float time, bool spin, EnemyShootType shot, EnemyShootType subshot, bullet bullet, float spinspeed, int shotnum, EnemyMoveType move,
        bool flip,  float paras, float paraw, float parah,
        float quadw, float quadstop,
        float vstop,  float vwait, float smod, bool final, bool grow, float shotdelay)
    {
        event_name = name;
        
        spawn_pos = pos;
        spawn_enemy = e;

        enemies_to_spawn = num;
        delay_between_spawn = interval;
        
        timestamp = time;

        prevent_spinning = spin;
        is_final = final;

        if (smod > 0.0f) speed_mod = smod;
        else speed_mod = 1.0f;
        
        move_type = move;
        shot_type = shot;
        sub_shot_type = subshot;
        
        has_spawned = false;

        item_data = new item_details(items, bomb, life);
        
        switch (shot)
        {
            case EnemyShootType.VERTICAL: case EnemyShootType.PLAYER_FIXED: case EnemyShootType.PLAYER_FOLLOW:
                shot_data = new shoot_details(bullet, grow, shotdelay);
                break;
            case EnemyShootType.SPIN:
                shot_data = new shoot_details(bullet, grow, shotdelay, spinspeed);
                break;
            case EnemyShootType.MULTIPLE:
                shot_data = new shoot_details(bullet, grow, shotdelay, 0.0f, shotnum);
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
    [NonSerialized] public float start_time;
    [SerializeField] public List<spawn_event> waves = new List<spawn_event>();
    public player player;
    
    private List<bool> spawn_state = new List<bool>();
    
    public GameObject bomb;
    public GameObject life;
    
    private List<int> num_spawned_from_event = new List<int>();
    private List<bool> has_active_timer = new List<bool>();

    private bool final_enemy_set = false;
    private enemy final_enemy;
    
    void Start()
    {
        foreach(spawn_event e in waves)
        {
            spawn_state.Add(e.get_state());
            num_spawned_from_event.Add(0);
            has_active_timer.Add(false);
        }

        start_time = Time.time;
    }

    IEnumerator start_spawn_timer(float delay, int index)
    {
        has_active_timer[index] = true;
        yield return new WaitForSeconds(delay);
        has_active_timer[index] = false;
    }
    
    void FixedUpdate()
    {
        //for each active spawn event, if there is no active timer, spawn an enemy and increment count
        //if count = spawn details count, deavtivate the event
        
        //from when the last event in the list has started, check for no enemies on screen
        //when last enemy is dead, go to next level

        for (int i = 0; i < waves.Count; ++i)
        {
            if (!spawn_state[i] && start_time + waves[i].timestamp <= Time.time)
            {
                spawn_state[i] = true;
            }
            
            //If the event has been set off and there are still enemies to spawn
            if (spawn_state[i] && waves[i].enemies_to_spawn != num_spawned_from_event[i])
            {
                if (num_spawned_from_event[i] == 0) Debug.Log("starting event " + i);
                //Check for active timer
                if (!has_active_timer[i])
                {
                    //spawn enemy, start timer, increment count
                    Debug.Log("spawning enemy");
                    enemy new_enemy = Instantiate(waves[i].spawn_enemy, transform).GetComponent<enemy>();
                    new_enemy.transform.position += waves[i].spawn_pos;
                    new_enemy.move_type = waves[i].move_type;
                    new_enemy.move_data = waves[i].move_data;
                    new_enemy.shot_type = waves[i].shot_type;
                    new_enemy.sub_shot_type = waves[i].sub_shot_type;
                    new_enemy.shot_data = waves[i].shot_data;
                    new_enemy.no_spin = waves[i].prevent_spinning;
                    if(waves[i].speed_mod > 0.0f) new_enemy.speed += waves[i].speed_mod;
                    new_enemy.GetComponentInChildren<enemy_collide>().item_data = waves[i].item_data;
                    if (waves[i].shot_type == EnemyShootType.PLAYER_FIXED)
                        new_enemy.shot_target = player.transform.position;

                    StartCoroutine(start_spawn_timer(waves[i].delay_between_spawn, i));
                    num_spawned_from_event[i] += 1;

                    if (waves[i].is_final)
                    {
                        final_enemy_set = true;
                        final_enemy = new_enemy;
                    }
                }
            }
        }
        
        //Update position of player follow enemies
        enemy[] enemies = GetComponentsInChildren<enemy>();
        foreach (enemy e in enemies)
        {
            if(player && (e.shot_type == EnemyShootType.PLAYER_FOLLOW || e.sub_shot_type == EnemyShootType.PLAYER_FOLLOW)) e.shot_target = player.transform.position;
        }

        //check for stage end - boss defeat
        if (final_enemy_set && !final_enemy) StartCoroutine(stage_transition());
    }
    
    IEnumerator stage_transition()
    {
        Debug.Log("STAGE TRANSITION");
        yield return new WaitForSeconds(2);
        GetComponentInParent<stage_manager>().next_stage();
    }
}

