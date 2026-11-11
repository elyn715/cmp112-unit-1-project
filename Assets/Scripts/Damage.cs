using System;
using System.Timers;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Damage : MonoBehaviour
{
    public int lives = 3;
    public int respawnTimer = 2500;
    //public Sprite sprite;
    private SpriteRenderer m_spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_spriteRenderer = GetComponentInParent<SpriteRenderer>();
        //m_spriteRenderer.sprite = sprite;
    }
    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        lives -= 1;
        collision2D.collider.enabled = false;
        if (lives >= 0){
            m_spriteRenderer.color = Color.crimson;
            string output = $"Damage Taken";
            Debug.Log(output);
            Thread.Sleep(respawnTimer);
            timerEnded();
            m_spriteRenderer.color = Color.white;
            collision2D.collider.enabled = true;
        }
    }
    void timerEnded()
    {
        transform.parent.position = new Vector2(-0.0f, -2.0f); 
    }
}
