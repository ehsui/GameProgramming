using UnityEngine.SceneManagement;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("Story");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
