using UnityEngine;
using UnityEngine.SceneManagement;

public class stage_2_manager : MonoBehaviour
{
    public bool paused = false;
    public GameObject pause_canvas;
    
    public GameObject stage_2_canvas;
    
    void Start()
    {
        pause_canvas = GetComponent<Canvas>().gameObject;
        stage_2_canvas = GetComponent<Canvas>().gameObject;
    }
    
    void Update()
    {
        //Check for pausing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Pause
            paused = true;
            pause_canvas.SetActive(true);
            stage_2_canvas.SetActive(false);
        }
        
        //Check for stage end
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Move to last stage
            SceneManager.LoadScene(3);
        }
    }
}
