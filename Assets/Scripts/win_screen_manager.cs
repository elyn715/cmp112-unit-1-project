using UnityEngine;
using UnityEngine.SceneManagement;

public class win_screen_manager : MonoBehaviour
{
    void Update()
    {
        //Check for stage end
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Move to next stage
            SceneManager.LoadScene(0);
        }
    }
}
