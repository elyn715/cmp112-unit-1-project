using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;

//This is a singleton
public class statistics : MonoBehaviour
{
    public static statistics instance;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            current_player_lives = get_player().lives;
            current_player_bombs = get_player().bombs;
        }
        else
        {
            //Move stored data to this instance
            copy_data(instance);
            
            int current_scene = SceneManager.GetActiveScene().buildIndex;
            if (current_scene != 0 && current_scene != 4)
            {
                //Update data
                get_player().lives = current_player_lives;
                get_player().bombs = current_player_bombs;
                
                update_score();
                update_lives();
                update_bombs();
                update_stage();
            }
            
            Destroy(gameObject);
        }
    }
    
    private int lives_lost = 0;
    private int damage_dealt = 0;
    private int bombs_used = 0;
    private int enemies_defeated = 0;
    private int total_score = 0;

    [NonSerialized] public int current_player_lives;
    [NonSerialized] public int current_player_bombs;
    
    //Get a text field
    private TMP_Text get_field(string name)
    {
        foreach (TMP_Text text in FindObjectsByType<TMP_Text>())
        {
            if (name == text.name)
            {
                return text;
            }
        }
        return null;
    }
    
    private player get_player()
    {
        return FindAnyObjectByType<player>();
    }

    //Copy data from old instance to this
    private void copy_data(statistics newobject)
    {
        lives_lost = newobject.lives_lost;
        damage_dealt = newobject.damage_dealt;
        bombs_used = newobject.bombs_used;
        enemies_defeated = newobject.enemies_defeated;
        total_score = newobject.total_score;
        current_player_lives = newobject.current_player_lives;
        current_player_bombs = newobject.current_player_bombs;
    }
    
    public void reset()
    {
        lives_lost = 0;
        damage_dealt = 0;
        bombs_used = 0;
        enemies_defeated = 0;
        total_score = 0;
        if(get_player()) get_player().lives = 3;
        if(get_player()) get_player().bombs = 3;
        current_player_lives = 3;
        current_player_bombs = 3;
    }

    public void use_bomb()
    {
        ++bombs_used;
        update_bombs();
    }
    public void update_bombs()
    {
        get_field("bombs_text").text = "bombs\n   " + get_player().bombs;
        current_player_bombs = get_player().bombs;
    }

    public void lose_life()
    {
        ++lives_lost;
        update_lives();
    }
    public void update_lives()
    {
        get_field("lives_text").text = "lives\n   " + get_player().lives;
        current_player_lives = get_player().lives;
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
        update_score();
    }

    public void reset_score()
    {
        total_score = 0;
        update_score();
    }
    private void update_score()
    {
        get_field("score_text").text = "score\n   " + total_score;
    }

    private void update_stage()
    {
        get_field("stage_text").text = "stage " + SceneManager.GetActiveScene().buildIndex;
    }

    public void update_results()
    {
        get_field("lives_num_text").text = lives_lost.ToString();
        get_field("damage_num_text").text = damage_dealt.ToString();
        get_field("bombs_num_text").text = bombs_used.ToString();
        get_field("enemy_num_text").text = enemies_defeated.ToString();
        get_field("score_num_text").text = total_score.ToString();
    }
}
