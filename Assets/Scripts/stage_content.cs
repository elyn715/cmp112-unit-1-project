using System.Collections.Generic;
using Unity.Tutorials.Editor;
using UnityEngine;
using UnityEditor;

[System.Serializable] public struct spawn_event
{
    public Vector3 spawn_pos;
    public enemy spawn_enemy; //Will be used later when there are more enemy types
    public float timestamp;
    public bool shoot_player;
    private bool has_spawned;

    public bool get_state(){return has_spawned;}
    public void set_spawned(){has_spawned = true;}
    public spawn_event(Vector3 pos, enemy e, float time, bool targetplayer)
    {
        spawn_pos = pos;
        spawn_enemy = e;
        timestamp = time;
        shoot_player = targetplayer;
        has_spawned = false;
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
                    new_enemy.transform.parent = this.transform;
                    Vector3 new_pos = new_enemy.transform.position + spawn.spawn_pos;
                    new_enemy.transform.position = new_pos;
                    new_enemy.shoot_at_player = spawn.shoot_player;
                    new_enemy.shot_direction = player.transform.position;
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

