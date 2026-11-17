using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class win_screen_manager : MonoBehaviour
{
    void Start()
    {
        //Show final details through text
        statistics.instance.update_results();
    }
    
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            //Reset stats
            statistics.instance.reset();
            
            //Move to title
            SceneManager.LoadScene(0);
        }
    }
}
