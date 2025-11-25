using UnityEngine.SceneManagement;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OnOptionsButton()
    {
        // 옵션 패널 활성화
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
