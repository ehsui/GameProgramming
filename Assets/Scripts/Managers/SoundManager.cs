using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // 어디서든 접근 가능하게 함

    private AudioSource bgmPlayer; // BGM 재생용 오디오 소스

    void Awake()
    {
        // 싱글톤 패턴: 게임 내에 단 하나만 존재하게 함
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀔 때 파괴되지 않음
            
            // 오디오 소스 컴포넌트 추가 또는 가져오기
            bgmPlayer = GetComponent<AudioSource>();
            if (bgmPlayer == null) bgmPlayer = gameObject.AddComponent<AudioSource>();
            
            bgmPlayer.loop = true; // BGM은 반복 재생
            bgmPlayer.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    // 외부에서 BGM 재생을 요청하는 함수
    public void PlayBGM(AudioClip clip)
    {
        // 1. 클립이 없으면 재생 중지
        if (clip == null)
        {
            bgmPlayer.Stop();
            return;
        }

        // 2. 이미 같은 음악이 재생 중이라면 처음부터 다시 틀지 않고 유지
        if (bgmPlayer.clip == clip && bgmPlayer.isPlaying)
            return;

        // 3. 다른 음악이라면 교체 후 재생
        bgmPlayer.Stop();
        bgmPlayer.clip = clip;
        bgmPlayer.Play();
    }
}