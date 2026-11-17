using System;
using UnityEngine;

public class bomb : MonoBehaviour
{
    //Timings
    public float effect_time = 2;
    public float speed;
    private float start_time;
    
    void Start()
    {
        start_time = Time.time;
    }
    
    void FixedUpdate()
    {
        //Increase scale
        transform.localScale += new Vector3(speed, speed, 0.0f);
        
        if (start_time + effect_time <= Time.time)
        {
            //Fade out
            GetComponentInChildren<SpriteRenderer>().color -= new Color(0, 0, 0, 0.09f);
            if(GetComponentInChildren<SpriteRenderer>().color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
