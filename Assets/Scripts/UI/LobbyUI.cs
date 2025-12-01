using UnityEngine.SceneManagement;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("Story");
    }

    public void OnOptionsButton()
    {
        // �샃�뀡 �뙣�꼸 �솢�꽦�솕
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
