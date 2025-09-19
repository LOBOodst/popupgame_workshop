using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject explainationPanel;
    public void StartButton()
    {
        explainationPanel.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void StartScene()
    {
        SceneManager.LoadScene("Game Scene");
    }
}
