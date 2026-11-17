using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemy : MonoBehaviour
{
    //Bullet timing
    private float shot_start_time;
    public float shot_delay = 0.05f;
    
    //Movement and shooting details
    [NonSerialized] public EnemyMoveType move_type;
    [NonSerialized] public move_details move_data;
    [NonSerialized] public EnemyShootType shot_type;
    [NonSerialized] public shoot_details shot_data;
    [NonSerialized] public Vector3 shot_target = new Vector2(0.0f, 0.0f);
    [NonSerialized] public bool no_spin = false;
    private float fixed_z_rotation = 0.0f;
    
    //Movement specific flags
    private Vector2 origin_offset;
    private bool is_waiting_to_move = false;
    private bool has_waited = false;
    private float wait_start_time;
    private float para_start_time;
    
    //General details
    public float speed = 5.0f;
    public int health = 500;
    public bullet bullet;
    public bool randomize_rotation = true;
    public bool randomize_colour = true;
    private Color[] colours =
    {
        Color.aquamarine,
        Color.blueViolet,
        Color.cadetBlue,
        Color.coral,
        Color.cornflowerBlue,
        Color.crimson,
        Color.cyan,
        Color.darkMagenta,
        Color.darkRed,
        Color.gold,
        Color.greenYellow,
        Color.mediumSeaGreen,
        Color.khaki,
        Color.lightBlue,
        Color.orangeRed,
        Color.wheat,
        Color.white
    };
    
    void Start()
    {
        //Setup for bullet maths and movement
        origin_offset = -transform.position;
        if (move_type == EnemyMoveType.QUADRATIC)
        {
            //Calculate starting x position from current y and equation width
            double starting_x = Math.Sqrt(transform.position.y + move_data.quad_stop) / move_data.quad_w;
            Vector3 start = new Vector3((float)starting_x, move_data.quad_stop, 0.0f);
            if (move_data.mirror) transform.position += start;
            else transform.position -= start;
        }
        para_start_time = Time.time;
        
        if (shot_type == EnemyShootType.VERTICAL) shot_target = new Vector2(transform.position.x, bounds_check.bottom_y_edge);
        else if(shot_type == EnemyShootType.PLAYER_FIXED) shot_target = GetComponentInParent<stage_content>().player.transform.position;
        
        //Set random rotation
        if (randomize_rotation)
        {
            fixed_z_rotation = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(0, 0, fixed_z_rotation);
        }
        
        //Set random colour
        if (randomize_colour)
        {
            int random_colour = Random.Range(0, colours.Length - 1);
            GetComponentInChildren<SpriteRenderer>().color = colours[random_colour];
        }

        shot_start_time = Time.time;
    }

    void Update()
    {
        //Rotate enemy
        if ((shot_type == EnemyShootType.SPIN) && GetComponentInChildren<enemy_collide>().is_active)
        {
            transform.eulerAngles += new Vector3(0.0f, 0.0f, shot_data.spin_speed * Time.deltaTime);
            fixed_z_rotation = transform.eulerAngles.z;
            if(no_spin) GetComponentInChildren<SpriteRenderer>().transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    private void manage_shooting(EnemyShootType type)
    {
        //Calculate the delay to use - scales to balance out the bullets for multiple and spin
        float delay_to_use = shot_delay;
        if (type == EnemyShootType.MULTIPLE) delay_to_use *= 1.5f;
        else if (type == EnemyShootType.SPIN) delay_to_use /= 2.0f; 
        if (delay_to_use < 0) delay_to_use = 0.0f;
        
        if (shot_start_time + delay_to_use <= Time.time)
        {
            //Shoot
            if (type == EnemyShootType.MULTIPLE)
            {
                float difference = 360.0f / shot_data.shots;
                for (int i = 0; i < shot_data.shots; ++i)
                {
                    transform.eulerAngles = new Vector3(0.0f, 0.0f, difference * i);
                    //transform.eulerAngles += new Vector3(0.0f, 0.0f, difference * i);
                    GameObject multiple_bullet = Instantiate(bullet.gameObject, transform.position, transform.rotation);
                    multiple_bullet.GetComponent<bullet>().set_direction(transform.right);
                    multiple_bullet.GetComponent<bullet>().grow = shot_data.bullet_grow;
                }
                transform.eulerAngles = new Vector3(0, 0, fixed_z_rotation);
            }
            else
            {
                GameObject new_bullet = Instantiate(bullet.gameObject, transform.position, transform.rotation);
                                
                if (type == EnemyShootType.VERTICAL) shot_target = new Vector3(transform.position.x, bounds_check.bottom_y_edge);
                else if (type == EnemyShootType.SPIN) shot_target = transform.position + transform.right;
                
                Vector3 direction = (shot_target - transform.position).normalized;
                new_bullet.GetComponent<bullet>().set_direction(direction);
                new_bullet.GetComponent<bullet>().grow = shot_data.bullet_grow;
            }

            shot_start_time = Time.time;
        }
    }
    
    private void FixedUpdate()
    {
        //if not dead
        if (GetComponentInChildren<enemy_collide>().is_active)
        {
            if (para_start_time + shot_data.start_delay <= Time.time)
            {
                manage_shooting(shot_type);
            }
            
            switch (move_type)
            {
                case EnemyMoveType.PARAMETRIC:
                    //x = scale sin time
                    //y = scale (sin time)(cos time)
                    double testx = move_data.para_scale * Math.Sin(Time.time - para_start_time) * move_data.para_w;
                    double testy = move_data.para_scale * Math.Sin(Time.time - para_start_time) * Math.Cos(Time.time - para_start_time) * move_data.para_h;

                    testx *= speed / 8.0f;
                    testy *= speed / 8.0f;
                    
                    if (move_data.mirror)
                    {
                        transform.position = new Vector3(-(float)testx - origin_offset.x, (float)testy - origin_offset.y, 0.0f);
                    }
                    else
                    {
                        transform.position = new Vector3((float)testx - origin_offset.x, -(float)testy - origin_offset.y, 0.0f);
                    }
                    break;
                
                case EnemyMoveType.QUADRATIC:
                    float x = transform.position.x + origin_offset.x;
                    float new_y = ((x * move_data.quad_w) * (x * move_data.quad_w)) + move_data.quad_stop;
                    transform.position = new Vector2(transform.position.x, new_y);
                    Vector3 next_x = new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
                    if (move_data.mirror) transform.position -= next_x;
                    else transform.position += next_x;
                    break;
                
                case EnemyMoveType.LINEAR_V:
                    Vector3 move = new Vector3(0.0f, speed, 0.0f) * Time.deltaTime;
                    if (!is_waiting_to_move && !has_waited) //moving down
                    {
                        transform.position -= move;
                        if (transform.position.y <= move_data.vertical_stop)
                        {
                            Vector3 set_pos = new Vector3(transform.position.x, move_data.vertical_stop, 0.0f);
                            transform.position = set_pos;
                            is_waiting_to_move = true;
                            wait_start_time = Time.time;
                        }
                    }
                    else if (is_waiting_to_move)
                    {
                        if (wait_start_time + move_data.wait_time <= Time.time)
                        {
                            //move back up
                            has_waited = true;
                            is_waiting_to_move = false;
                        }
                    }
                    else if (!is_waiting_to_move && has_waited) //moving up
                    {
                        transform.position += move;
                    }
                    break;
                
                case EnemyMoveType.LINEAR_H:
                    Vector3 movement = new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
                    if (move_data.mirror) transform.position -= movement;
                    else transform.position += movement;
                    break;
            }

            //Only start checking bounds 5 seconds after spawn
            if (Time.time >= para_start_time + 5)
            {
                if (!bounds_check.check_boundaries(transform.position, 1.0f)) Destroy(gameObject); 
            }
        }
        else
        {
            if(GetComponent<ParticleSystem>().particleCount == 0) Destroy(gameObject);
        }
    }
    
    public IEnumerator damage_feedback()
    {
        GetComponentInChildren<enemy_collide>().feedback_running = true;
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        Color base_color = renderer.color;
        renderer.color = Color.grey;
        yield return new WaitForSeconds(0.005f);
        renderer.color = base_color;
        yield return new WaitForSeconds(0.005f);
        GetComponentInChildren<enemy_collide>().feedback_running = false;
    }
}