using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    public void LoadNextStage(string sceneName, Vector2 spawnPoint)
    {
        
        SceneManager.LoadScene(sceneName);

        
        
        
        

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.transform.position = spawnPoint;

            
            
        }
    }

    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        GameObject startPoint = GameObject.Find("StartPoint");
        if (startPoint != null && PlayerController.Instance != null)
        {
            PlayerController.Instance.transform.position = startPoint.transform.position;
            Debug.Log($"새로운 씬 {scene.name} 로드 완료. 플레이어 위치 이동.");
        }
    }
}
/*
 * 각 씬(Stage 1, Stage 2...)마다 플레이어가 시작할 위치에 빈 오브젝트를 만들고 이름을 **StartPoint**라고 짓습니다.
 */