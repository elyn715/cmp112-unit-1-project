using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemy : MonoBehaviour
{
    public int shot_offset = 5;
    private int offset_counter = 0;
    private int sub_offset_counter = 0;
    public int health = 500;
    [NonSerialized] public EnemyMoveType move_type;
    [NonSerialized] public move_details move_data;
    [NonSerialized] public EnemyShootType shot_type;
    [NonSerialized] public EnemyShootType sub_shot_type;
    [NonSerialized] public shoot_details shot_data;
    public float speed = 5.0f;

    private Vector2 origin_offset;
    private bool is_waiting_to_move = false;
    private bool has_waited = false;
    private float wait_start_time;
    private float para_start_time;
    
    public bullet bullet;

    public bool randomize_rotation = true;
    public bool randomize_colour = true;

    [NonSerialized] public Vector3 shot_target = new Vector2(0.0f, 0.0f);

    private float fixed_z_rotation = 0.0f;

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
            double starting_x = Math.Sqrt(transform.position.y + move_data.quad_stop) / move_data.quad_w;
            Vector3 start = new Vector3((float)starting_x, move_data.quad_stop, 0.0f);
            if (move_data.mirror) transform.position += start;
            else transform.position -= start;
        }
        para_start_time = Time.time;
        
        //FIX THESE FOR SUB TYPE--------------------------------
        if (shot_type == EnemyShootType.VERTICAL) shot_target = new Vector2(transform.position.x, globlal_screen_boundaries.bottom_y_edge);
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
    }

    //move this to fixed
    void Update()
    {
        if ((shot_type == EnemyShootType.SPIN || sub_shot_type == EnemyShootType.SPIN) && GetComponentInChildren<enemy_collide>().is_active)
        {
            transform.eulerAngles += new Vector3(0.0f, 0.0f, shot_data.spin_speed * Time.deltaTime);
            fixed_z_rotation = transform.eulerAngles.z;
        }
    }

    void bounds_check()
    {
        float enemy_offset = 1.0f;
        if (transform.position.x < globlal_screen_boundaries.left_x_edge  - enemy_offset || transform.position.x > globlal_screen_boundaries.right_x_edge  + enemy_offset|
            transform.position.y > globlal_screen_boundaries.top_y_edge + enemy_offset || transform.position.y < globlal_screen_boundaries.bottom_y_edge - enemy_offset)
        {
            Destroy(gameObject);
        }
    }

    private void manage_shooting(EnemyShootType type, bool sub = false)
    {
        int offset_compare = shot_offset;
        if (type == EnemyShootType.MULTIPLE) offset_compare += shot_offset * 3;
        else if (type == EnemyShootType.SPIN) offset_compare -= 5;
        if (offset_compare <= 0) offset_compare = 1;
        //Debug.Log("offset compare is " + offset_compare + ", offset_counter is " + offset_counter);
        if (offset_compare == offset_counter)
        {
            if (type == EnemyShootType.MULTIPLE)
            {
                float difference = 360.0f / shot_data.shots;
                for (int i = 0; i < shot_data.shots; ++i)
                {
                    transform.eulerAngles = new Vector3(0.0f, 0.0f, difference * i);
                    //transform.eulerAngles += new Vector3(0.0f, 0.0f, difference * i);
                    GameObject multiple_bullet = Instantiate(bullet.gameObject, transform.position, transform.rotation);
                    multiple_bullet.GetComponent<bullet>().set_direction(transform.right);
                }
                transform.eulerAngles = new Vector3(0, 0, fixed_z_rotation);
            }
            else
            {
                GameObject new_bullet = Instantiate(bullet.gameObject, transform.position, transform.rotation);
                                
                if (type == EnemyShootType.VERTICAL) shot_target = new Vector3(transform.position.x, globlal_screen_boundaries.bottom_y_edge);
                else if (type == EnemyShootType.SPIN) shot_target = transform.position + transform.right;
                
                Vector3 direction = (shot_target - transform.position).normalized;
                new_bullet.GetComponent<bullet>().set_direction(direction);
            }

            if (sub) sub_offset_counter = 0;
            else offset_counter = 0;
            
        }
        else
        {
            if (sub) ++sub_offset_counter;
            else ++offset_counter;
        }
    }
    
    private void FixedUpdate()
    {
        //if not dead
        if (GetComponentInChildren<enemy_collide>().is_active)
        {
            manage_shooting(shot_type);
            if (sub_shot_type != EnemyShootType.NONE)
            {
                manage_shooting(sub_shot_type, true);
            }
            //if sub type set, do that
            
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

            //Only start checking bounds 2 seconds after spawn
            if (Time.time >= para_start_time + 2)
            {
                bounds_check();
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