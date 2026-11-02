using UnityEngine;
using UnityEngine.SceneManagement;

public class stage_1_manager : MonoBehaviour
{
    //This object can manage things like enemy spawning and other stuff the stage needs
    //Stage 1 manager also handles the tutorial screen

    public bool paused = false;
    public GameObject pause_canvas;
    
    public bool tutorial_on = true;
    public GameObject tutorial_canvas;

    public GameObject stage_1_canvas;
    
    void Start()
    {
        pause_canvas = GetComponent<Canvas>().gameObject;
        tutorial_canvas = GetComponent<Canvas>().gameObject;
        stage_1_canvas = GetComponent<Canvas>().gameObject;
    }

    void Update()
    {
        //Check for tutorial ending
        if (Input.GetKeyDown(KeyCode.Z) && tutorial_on)
        {
            tutorial_on = false;
            tutorial_canvas.SetActive(false);
            stage_1_canvas.SetActive(true);
        }
        
        //Check for pausing
        /*if (Input.GetKeyDown(KeyCode.Escape) && !tutorial_on)
        {
            //Pause
            paused = true;
            pause_canvas.SetActive(true);
            stage_1_canvas.SetActive(false);
        }*/
        
        //Check for stage end
        if (Input.GetKeyDown(KeyCode.Return) && !tutorial_on && !paused)
        {
            //Move to next stage
            SceneManager.LoadScene(2);
        }
    }
}
