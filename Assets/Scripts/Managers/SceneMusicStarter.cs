using UnityEngine;

public class SceneMusicStarter : MonoBehaviour
{
    public AudioClip sceneBGM; // 이 씬에서 재생할 음악 파일

    void Start()
    {
        // 씬이 시작될 때 사운드 매니저에게 BGM 재생 요청
        // SoundManager가 아직 생성 안 된 경우를 대비해 예외처리
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(sceneBGM);
        }
        else
        {
            Debug.LogWarning("SoundManager가 씬에 없습니다. Title 씬부터 시작해주세요.");
        }
    }
}