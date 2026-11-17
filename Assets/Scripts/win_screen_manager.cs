using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class win_screen_manager : MonoBehaviour
{
    void Start()
    {
        //Show final details through text
        statistics.instance.show_details();
    }
    
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            //Move to title
            SceneManager.LoadScene(0);
            
            //Reset stats
            statistics.instance.reset();
        }
    }
}
