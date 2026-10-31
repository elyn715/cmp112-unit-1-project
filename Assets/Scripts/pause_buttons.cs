using UnityEngine;
using UnityEngine.SceneManagement;

public class pause_buttons : MonoBehaviour
{
    public GameObject stage_manager;

    public void resume_button()
    {
        int current_stage_index = SceneManager.GetActiveScene().buildIndex;
        switch (current_stage_index)
        {
            case 1:
                stage_manager.GetComponent<stage_1_manager>().paused = false;
                stage_manager.GetComponent<stage_1_manager>().pause_canvas.SetActive(false);
                stage_manager.GetComponent<stage_1_manager>().stage_1_canvas.SetActive(true);
                break;
            
            case 2:
                stage_manager.GetComponent<stage_2_manager>().paused = false;
                stage_manager.GetComponent<stage_2_manager>().pause_canvas.SetActive(false);
                stage_manager.GetComponent<stage_2_manager>().stage_2_canvas.SetActive(true);
                break;
            
            case 3:
                stage_manager.GetComponent<stage_3_manager>().paused = false;
                stage_manager.GetComponent<stage_3_manager>().pause_canvas.SetActive(false);
                stage_manager.GetComponent<stage_3_manager>().stage_3_canvas.SetActive(true);
                break;
        }
            
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
