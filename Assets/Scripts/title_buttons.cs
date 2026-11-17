using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class button_functions : MonoBehaviour
{
    public Canvas tutorial_canvas;

    void Start()
    {
        tutorial_canvas.enabled = false;
    }
    
    public void start_button()
    {
        tutorial_canvas.enabled = true;
    }

    //Dismiss tutorial, load stage 1
    void OnDismiss(InputValue value)
    {
        if (tutorial_canvas.enabled)
        {
            tutorial_canvas.enabled = false;
            SceneManager.LoadScene(1);
        }
    }

    public void quit_button()
    {
        Application.Quit();
    }
}
