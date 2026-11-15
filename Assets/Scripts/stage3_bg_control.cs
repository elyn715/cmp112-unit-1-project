using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class stage3_bg_control : MonoBehaviour
{
    private Transform[] edges = new Transform[5];
    private Vector3 initial_pos = new Vector3();
    private float initial_scale;
    private Color initial_colour =  new Color(122, 158, 166, 255);
    public float speed = 1.0f;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "5")
            {
                initial_pos = transform.GetChild(i).position;
                initial_scale = transform.GetChild(i).localScale.x;
                initial_colour = transform.GetChild(i).GetComponentInChildren<SpriteRenderer>().color;
            }
            
            if (transform.GetChild(i).name != "static")
            {
                edges[i] = transform.GetChild(i);
            }
        }
    }

    private int front_index = 0;

    void FixedUpdate()
    {
        for (int i = 0; i < edges.Length; i++)
        {
            edges[i].position += new Vector3(0.0f, 0.0f, -0.01f);
            edges[i].localScale += new Vector3(speed * (edges[i].localScale.x * 0.8f) * Time.fixedDeltaTime, 0.0f, 0.0f);
            foreach (SpriteRenderer sprite in edges[i].GetComponentsInChildren<SpriteRenderer>())
            {
                float increase = 0.003f;
                sprite.color = new Color(sprite.color.r + increase, sprite.color.g + increase, sprite.color.b + increase, 1.0f);
            }

            if (edges[i].localScale.x > 7.5f)
            {
                front_index++;
                if(front_index >= edges.Length) front_index = 0;
                edges[i].position = initial_pos;
                edges[i].localScale = new Vector3(initial_scale, 1.0f, 1.0f);
                foreach (SpriteRenderer sprite in edges[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.color = initial_colour;
                }
            }
        }
    }
}
