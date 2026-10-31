using UnityEngine;

public class test_script : MonoBehaviour
{
    //This object can manage things like enemy spawning and other stuff the stage needs
    //Stage 1 manager also handles the tutorial screen
    private bool tutorial_on = true;
    public GameObject tutorial_canvas;

    public GameObject stage_1_canvas;
    
    void Start()
    {
        tutorial_canvas = GetComponent<Canvas>().gameObject;
        
        stage_1_canvas = GetComponent<Canvas>().gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && tutorial_on)
        {
            tutorial_on = false;
            tutorial_canvas.SetActive(false);
            stage_1_canvas.SetActive(true);
        }
        
        //Check for stage end
    }
}
