using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class win_screen_manager : MonoBehaviour
{
    void Start()
    {
        FindAnyObjectByType<statistics>().show_details();
    }
    
    void Update()
    {
        //Check for stage end
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            //Move to next stage
            SceneManager.LoadScene(0);
            
            //Reset stats
            FindAnyObjectByType<statistics>().reset();
        }
    }
}
