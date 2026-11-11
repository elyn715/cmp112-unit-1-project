using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemy : MonoBehaviour
{
    public int health = 500;
    public int shot_offset = 5;
    private int offset_counter = 0;
    public EnemyMoveType move_type;
    public move_details move_data;
    public EnemyShootType shot_type;
    public shoot_details shot_data;
    public float speed = 5.0f;

    private Vector2 origin_offset;
    private bool is_waiting_to_move = false;
    private bool has_waited = false;
    private float wait_start_time;
    private float para_start_time;
    
    public bullet bullet;
    private SpriteRenderer renderer;
    private bool feedback_running = false;

    public Vector3 shot_target = new Vector2(0.0f, 0.0f);

    private float fixed_z_rotation = 0.0f;
    
    void Start()
    {
        renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        
        //Setup for bullet maths and movement
        origin_offset = -transform.position;
        if (move_type == EnemyMoveType.QUADRATIC)
        {
            double starting_x = Math.Sqrt(transform.position.y) / move_data.quad_w;
            Vector3 start = new Vector3((float)starting_x, move_data.quad_stop, 0.0f);
            if (move_data.mirror) transform.position += start;
            else transform.position -= start;
        }
        para_start_time = Time.time;
        if (shot_type == EnemyShootType.VERTICAL) shot_target = new Vector2(transform.position.x, globlal_screen_boundaries.bottom_y_edge);
        
        //Set random rotation
        fixed_z_rotation = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, 0, fixed_z_rotation);
        
        //Set random colour
        Color random_colour = new Color(Random.Range(0F, 1F), Random.Range(0F, 1F), Random.Range(0F, 1F));
        GetComponentInChildren<SpriteRenderer>().color = random_colour;
    }

    void Update()
    {
        if (shot_type == EnemyShootType.SPIN) transform.eulerAngles += new Vector3(0.0f, 0.0f, shot_data.spin_speed * Time.deltaTime);
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
    
    private void FixedUpdate()
    {
        if (shot_offset == offset_counter)
        {
            if (shot_type == EnemyShootType.MULTIPLE)
            {
                float difference = 360.0f / shot_data.shots;
                for (int i = 0; i < shot_data.shots; ++i)
                {
                    transform.eulerAngles = new Vector3(0.0f, 0.0f, difference * i);
                    GameObject multiple_bullet = Instantiate(bullet.gameObject, transform.position, transform.rotation);
                    multiple_bullet.GetComponent<bullet>().set_direction(transform.right);
                }
                transform.eulerAngles = new Vector3(0, 0, fixed_z_rotation);
            }
            else
            {
                GameObject new_bullet = Instantiate(bullet.gameObject, transform.position, transform.rotation);
                                
                if (shot_type == EnemyShootType.VERTICAL) shot_target = new Vector3(transform.position.x, globlal_screen_boundaries.bottom_y_edge);
                else if (shot_type == EnemyShootType.SPIN) shot_target = transform.position + transform.right;
                
                Vector3 direction = (shot_target - transform.position).normalized;
                new_bullet.GetComponent<bullet>().set_direction(direction);
            }
            offset_counter = 0;
        }
        else ++offset_counter;
        
        switch (move_type)
        {
            case EnemyMoveType.PARAMETRIC: // the enemy will only be visible once here, just delete after it goes offscreen
                //x = scale sin time
                //y = scale (sin time)(cos time)
                double testx = move_data.para_scale * Math.Sin(Time.time - para_start_time) * move_data.para_w;
                double testy = move_data.para_scale * Math.Sin(Time.time - para_start_time) * Math.Cos(Time.time - para_start_time) * move_data.para_h;
                if (move_data.mirror)
                {
                    transform.position = new Vector3(-(float)testx - origin_offset.x, (float)testy - origin_offset.y, 0.0f);
                }
                else
                {
                    transform.position = new Vector3((float)testx - origin_offset.x, -(float)testy - origin_offset.y, 0.0f);
                }
                break;
            
            case EnemyMoveType.QUADRATIC: //When using quadratic movement, place the enemy in line with the origin of the curve
                float x = transform.position.x + origin_offset.x;
                float new_y = ((x * move_data.quad_w) * (x * move_data.quad_w)) - move_data.quad_stop;
                transform.position = new Vector2(transform.position.x, new_y);
                Vector3 next_x = new Vector3(speed, 0.0f, 0.0f) * Time.deltaTime;
                if (move_data.mirror) transform.position -= next_x;
                else transform.position += next_x;
                break;
            
            case EnemyMoveType.LINEAR_V: //delete when offscreen
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
            
            case EnemyMoveType.LINEAR_H: //delete offscreen
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
    
    IEnumerator damage_feedback()
    {
        feedback_running = true;
        Color base_color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
        renderer.color = Color.grey;
        yield return new WaitForSeconds(0.005f);
        renderer.color = base_color;
        yield return new WaitForSeconds(0.005f);
        feedback_running = false;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player_bullet")
        {
            health -= 10;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
            else if(!feedback_running)
            {
                StartCoroutine(damage_feedback());
            }
        }
    }
}
