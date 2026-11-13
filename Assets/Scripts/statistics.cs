using UnityEngine;

public class statistics : MonoBehaviour
{
    public player player;
    
    //Add references to text elements here

    private int lives_lost = 0;
    private int damage_dealt = 0;
    private int bombs_used = 0;
    private int enemies_defeated = 0;
    private int total_score = 0;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void reset()
    {
        lives_lost = 0;
        damage_dealt = 0;
        bombs_used = 0;
        enemies_defeated = 0;
        total_score = 0;
    }

    public void show_details()
    {
        Debug.Log("player lost " + lives_lost + " lives");
        Debug.Log("player dealt " + damage_dealt + " to enemies");
        Debug.Log("player used " + bombs_used + " bombs");
        Debug.Log("player defeated " + enemies_defeated + " enemies");
        Debug.Log("total score is " + total_score);
    }

    public void use_bomb()
    {
        ++bombs_used;
    }

    public void lose_life()
    {
        ++lives_lost;
    }

    public void add_damage(int damage)
    {
        damage_dealt += damage;
    }

    public void defeat_enemy()
    {
        ++enemies_defeated;
    }

    public void add_score(int score)
    {
        total_score += score;
    }
}
