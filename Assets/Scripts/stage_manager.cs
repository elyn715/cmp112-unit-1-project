using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class stage_manager : MonoBehaviour
{
    public bool has_tutorial = false;
    public bool paused = false;
    public GameObject pause_canvas;
    
    public GameObject stage_canvas;
    private stage_content stage;

    void Start()
    {
        stage = GetComponentInChildren<stage_content>();
        /*if(!has_tutorial)
        {
            Debug.Log("starting stage");
            stage.stage_running = true;
            stage.start_time = Time.time;
        }
        else
        {
            stage.stage_running = false;
        }*/
    }
    
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame && !paused)
        {
            //Move to next stage
            int next_scene = SceneManager.GetActiveScene().buildIndex + 1;
            if (next_scene >= SceneManager.sceneCountInBuildSettings)
            {
                next_scene = 0;
            }
            
            SceneManager.LoadScene(next_scene);
        }
    }

    public void toggle_pause()
    {
        bool can_pause = true;
        if (has_tutorial)
        {
            tutorial_manager tutorial = GetComponentInChildren<tutorial_manager>();
            if (tutorial.tutorial_on)
            {
                can_pause = false;
            }
        }

        if (can_pause)
        {
            paused = !paused;
            pause_canvas.SetActive(paused);
            stage_canvas.SetActive(!paused);
        }
        //PAUSE SCENE CONTENT
    }
}
