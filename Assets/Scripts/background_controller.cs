using System;
using System.Linq;
using UnityEngine;

public class background_controller : MonoBehaviour
{
    //this file is for stage 1 only
    private Transform water;
    private Vector3 initial_water;
    
    private Transform[] clouds = new Transform[8];
    private bool[] move_cloud_down = new bool[8];
    private Vector3 bottom_cloud;
    
    public float water_speed = 0.5f;
    public float cloud_speed = 0.5f;
    
    void Start()
    {
        water = transform.Find("water");
        initial_water = water.position;
        for (int i = 0; i < transform.Find("clouds").transform.childCount; i++)
        {
            clouds[i] = transform.Find("clouds").transform.GetChild(i);
            move_cloud_down[i] = false;
            if(i <= 2) move_cloud_down[i] = true; //improve this
        }
        bottom_cloud = clouds.Last().transform.position;
    }
    
    void Update()
    {
        //move water
        water.position -= new Vector3(0.0f, water_speed * Time.deltaTime, 0.0f);
        if (water.position.y < -2.7)
        {
            water.position = initial_water;
        }
        
        //move_clouds
        for (int i = 0; i < clouds.Length; i++)
        {
            if (!move_cloud_down[i])
            {
                clouds[i].transform.position += new Vector3(0.0f, cloud_speed * Time.deltaTime, -0.01f);
                
                if (clouds[i].transform.position.y > 6.2f)
                {
                    //set that cloud to move down to initial
                    move_cloud_down[i] = true;
                }
            }
            else
            {
                clouds[i].transform.position = new Vector3(0.0f, clouds[i].transform.position.y - (cloud_speed *  Time.deltaTime), bottom_cloud.z + ((i / 2.0f) * 1.0001f));
                if (clouds[i].position.y <= bottom_cloud.y)
                {
                    move_cloud_down[i] = false;
                    clouds[i].transform.position = new Vector3(0.0f, bottom_cloud.y, bottom_cloud.z);
                }
            }
        }
    }
}
