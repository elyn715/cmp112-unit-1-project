using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class pause_buttons : MonoBehaviour
{
    public UnityEvent pause_event;

    public void resume_button()
    {
        pause_event.Invoke();
    }
    
    public void restart_button()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void quit_to_title_button()
    {
        SceneManager.LoadScene(0);
    }
}
