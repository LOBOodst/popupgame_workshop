using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene("Main Menu");
        }
        if(Input.GetKeyUp(KeyCode.A))
        {
            Application.Quit();
        }
    }
}
