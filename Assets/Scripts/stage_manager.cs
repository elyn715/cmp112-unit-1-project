using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class stage_manager : MonoBehaviour
{
    public bool paused = false;
    public Canvas pause_canvas;

    void Start()
    {
        pause_canvas.enabled = false;
    }
    
    public void next_stage()
    {
        //Move to the next scene
        int next_scene = SceneManager.GetActiveScene().buildIndex + 1;
        if (next_scene >= SceneManager.sceneCountInBuildSettings)
        {
            next_scene = 0;
        }
        
        SceneManager.LoadScene(next_scene);
    }
    
    //Unpause level
    public void resume_button()
    {
        toggle_pause();
    }
    
    //Restart level
    public void restart_button()
    {
        if(paused) toggle_pause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Go to title screen
    public void quit_to_title_button()
    {
        toggle_pause();
        statistics.instance.reset();
        SceneManager.LoadScene(0);
    }

    //Pause/unpause
    public void toggle_pause()
    {
        paused = !paused;
        pause_canvas.enabled = paused;
        if(!paused) Time.timeScale = 1;
        else Time.timeScale = 0;
    }
}
