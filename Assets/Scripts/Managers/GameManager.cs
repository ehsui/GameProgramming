using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리용

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

    // 씬 이동 함수 (포탈 등에서 호출)
    public void LoadNextStage(string sceneName, Vector2 spawnPoint)
    {
        // 1. 씬 로드 (비동기 방식 권장하지만 일단 간단하게)
        SceneManager.LoadScene(sceneName);

        // 2. 플레이어 위치 이동
        // 씬이 로드된 직후에는 바로 이동이 안 될 수 있으므로, 
        // 씬 로드 이벤트(SceneLoaded)를 쓰거나 코루틴을 써야 하지만,
        // 가장 간단한 방법은 플레이어에게 직접 명령하는 것입니다.

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.transform.position = spawnPoint;

            // 혹시 죽거나 이상한 상태일 수 있으니 상태 초기화도 가능
            // PlayerController.Instance.StateMachine.ChangeState(PlayerController.Instance.IdleState);
        }
    }

    // 유니티 자체 이벤트: 씬이 로드될 때마다 호출됨
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
        // 씬이 로드되면 "StartPoint"라는 이름의 오브젝트를 찾아서 거기로 플레이어를 이동시킴
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