using UnityEngine;

public class tutorial_manager : MonoBehaviour
{
    public bool tutorial_on = true;
    public GameObject tutorial_canvas;
    private stage_content stage;

    void Start()
    {
        stage = GetComponentInChildren<stage_content>();
    }
    
    public void close_tutorial()
    {
        if (tutorial_on)
        {
            //Close tutorial
            tutorial_on = false;
            tutorial_canvas.SetActive(false);
            stage_manager parent = GetComponentInParent<stage_manager>();
            parent.stage_canvas.SetActive(true);

            stage.stage_running = true;
            stage.start_time = Time.time;
        }
    }
}
