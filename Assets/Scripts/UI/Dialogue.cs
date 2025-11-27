using UnityEngine;

[System.Serializable] // 인스펙터에서 리스트로 보기 위해 필수!
public class Dialogue
{
    [Tooltip("캐릭터 이름")]
    public string speakerName; 
    
    [Tooltip("대사 내용")]
    [TextArea(3, 5)] // 인스펙터에서 글상자를 크게 보여줌
    public string sentence;

    [Tooltip("캐릭터 스프라이트 (없으면 비워도 됨)")]
    public Sprite portrait; 
}