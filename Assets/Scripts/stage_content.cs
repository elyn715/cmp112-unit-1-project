using System.Collections.Generic;
using Unity.Tutorials.Editor;
using UnityEngine;

struct spawn_event
{
    public Vector3 spawn_pos;
    public enemy spawn_enemy; //Will be used later when there are more enemy types
    public float timestamp;
    public bool shoot_player;

    public spawn_event(Vector3 pos, enemy e, float time, bool targetplayer)
    {
        spawn_pos = pos;
        spawn_enemy = e;
        timestamp = time;
        shoot_player = targetplayer;
    }
}

public class stage_content : MonoBehaviour
{
    public enemy enemy;
    public float start_time;
    List<spawn_event> waves = new List<spawn_event>();
    public bool stage_running = false;
    public player player;
    private bool enemy_spawned = false;
    
    void Start()
    {
        start_time = Time.time;
        spawn_event new_wave = new spawn_event(new Vector3(0.0f, 0.0f, 0.0f), enemy, 2.0f, true);
        waves.Add(new_wave);
    }

    
    
    void FixedUpdate()
    {
        if (stage_running)
        {
            if (!enemy_spawned && start_time + waves[0].timestamp <= Time.time)
            {
                enemy_spawned = true;
                enemy new_enemy = Instantiate(enemy.gameObject, this.transform).GetComponent<enemy>();
                Vector3 new_pos = new_enemy.transform.position + waves[0].spawn_pos;
                new_enemy.transform.position = new_pos;

                if (waves[0].shoot_player)
                {
                    enemy.shoot_at_player = true;
                }
                enemy.shot_direction = player.transform.position;
            }
            
            enemy[] enemies = GetComponentsInChildren<enemy>();
            foreach (enemy e in enemies)
            {
                if(e.shoot_at_player) e.shot_direction = player.transform.position;
            }
        }
    }
}

