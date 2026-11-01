using UnityEngine;
using UnityEngine.SceneManagement;

public class stage_3_manager : MonoBehaviour
{
    public bool paused = false;
    public GameObject pause_canvas;
    
    public GameObject stage_3_canvas;
    
    void Start()
    {
        pause_canvas = GetComponent<Canvas>().gameObject;
        stage_3_canvas = GetComponent<Canvas>().gameObject;
    }
    
    void Update()
    {
        //Check for pausing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Pause
            pause_canvas.SetActive(true);
            stage_3_canvas.SetActive(false);
        }
        
        //Check for stage end
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Move to next stage
            SceneManager.LoadScene(4);
        }
    }
}
