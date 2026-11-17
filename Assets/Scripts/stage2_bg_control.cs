using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class stage2_bg_control : MonoBehaviour
{
    //Layers
    private Transform floor;
    private Vector3 initial_floor;
    public float floor_speed = 0.1f;

    private Transform third;
    private Vector3 initial_third;
    public float third_speed = 0.2f;

    private Transform second;
    private Vector3 initial_second;
    public float second_speed = 0.3f;

    private Transform top;
    private Vector3 initial_top;
    public float top_speed = 0.4f;

    private Transform shadows;
    private Vector3 initial_shadows;
    public float shadow_speed = 0.5f;

    private float reset_y = -10.0f;

    void Start()
    {
        floor = transform.Find("floor");
        initial_floor = floor.position;
        
        third = transform.Find("third");
        initial_third = third.position;
        
        second = transform.Find("second");
        initial_second = second.position;
        
        top =  transform.Find("top");
        initial_top = top.position;
        
        shadows = transform.Find("shadows");
        initial_shadows = shadows.position;
    }

    void Update()
    {
        //Move and reset if needed
        floor.position -= new Vector3(0.0f, floor_speed * Time.deltaTime, 0.0f);
        if (floor.position.y <= reset_y) floor.position = initial_floor;
        
        third.position -= new Vector3(0.0f, third_speed * Time.deltaTime, 0.0f);
        if (third.position.y <= reset_y) third.position = initial_third;
        
        second.position -= new Vector3(0.0f, second_speed * Time.deltaTime, 0.0f);
        if (second.position.y <= reset_y) second.position = initial_second;
        
        top.position -= new Vector3(0.0f, top_speed * Time.deltaTime, 0.0f);
        if(top.position.y <= reset_y) top.position = initial_top;
        
        shadows.position -= new Vector3(0.0f, shadow_speed * Time.deltaTime, 0.0f);
        if (shadows.position.y <= reset_y) shadows.position = initial_shadows;
    }
}
