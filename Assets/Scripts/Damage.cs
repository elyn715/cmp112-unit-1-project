using System.Timers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Damage : MonoBehaviour
{
    public int lives = 3;
    public double respawnTimer = 25f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        lives -= 1;
        if (lives >= 0){
            collision2D.otherCollider.enabled = false;
            string output = $"Damage Taken";
            Debug.Log(output);
            double timeLeft = respawnTimer;
            while (timeLeft>0){
            }
            timerEnded();
            collision2D.collider.enabled = true;
        }
    }
    void timerEnded()
    {
        transform.position = new Vector2(0,-4f); 
    }
}
