using UnityEngine;
using UnityEngine.SceneManagement;

public class button_functions : MonoBehaviour
{
    public void start_button()
    {
        SceneManager.LoadScene(1);
    }

    public void quit_button()
    {
        Application.Quit();
    }
}
