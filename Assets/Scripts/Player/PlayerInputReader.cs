using UnityEngine;
using System;

public class PlayerInputReader : MonoBehaviour
{
    // --- [이벤트 정의] (이전과 동일) ---
    // 다른 스크립트들은 이 이벤트만 구독하면 됩니다.
    public event Action<Vector2> OnMoveEvent;
    public event Action OnJumpEvent;
    public event Action OnAttackEvent;
    public event Action OnSkillEvent;
    public event Action OnInteractEvent;
    public event Action OnPotionEvent;
    public event Action OnMapEvent;
    public event Action OnGuardEvent;

    // 무기 교체 (0~2: 주무기, 0~2: 보조무기)
    public event Action<int> OnMainWeaponSwitchEvent;
    public event Action<int> OnSubWeaponSwitchEvent;

    void Update()
    {
        HandleMoveInput();
        HandleActionInput();
        HandleWeaponInput();
    }

    // 1. 이동 입력 처리
    private void HandleMoveInput()
    {
        // GetAxisRaw는 -1, 0, 1 등 즉각적인 값을 반환해 2D 플랫포머에 적합합니다.
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical"); // 사다리 등을 위해 일단 받아둡니다.

        // 매 프레임 이동 벡터를 보냅니다.
        OnMoveEvent?.Invoke(new Vector2(x, y));
    }

    // 2. 행동 입력 처리
    private void HandleActionInput()
    {
        if (Input.GetButtonDown("Jump")) // Unity Input Manager의 "Jump" (Space)
            OnJumpEvent?.Invoke();

        if (Input.GetMouseButtonDown(0)) // 좌클릭
            OnAttackEvent?.Invoke();

        if (Input.GetMouseButtonDown(1)) // 우클릭
            OnGuardEvent?.Invoke();

        if (Input.GetKeyDown(KeyCode.E))
            OnSkillEvent?.Invoke();

        if (Input.GetKeyDown(KeyCode.F))
            OnInteractEvent?.Invoke();

        if (Input.GetKeyDown(KeyCode.R))
            OnPotionEvent?.Invoke();

        if (Input.GetKeyDown(KeyCode.M))
            OnMapEvent?.Invoke();
    }

    // 3. 무기 교체 입력 처리
    private void HandleWeaponInput()
    {
        // 주 무기 (1, 2, 3 키)
        if (Input.GetKeyDown(KeyCode.Alpha1)) OnMainWeaponSwitchEvent?.Invoke(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) OnMainWeaponSwitchEvent?.Invoke(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) OnMainWeaponSwitchEvent?.Invoke(2);

        // 보조 무기 (4, 5, 6 키)
        if (Input.GetKeyDown(KeyCode.Alpha4)) OnSubWeaponSwitchEvent?.Invoke(0);
        if (Input.GetKeyDown(KeyCode.Alpha5)) OnSubWeaponSwitchEvent?.Invoke(1);
        if (Input.GetKeyDown(KeyCode.Alpha6)) OnSubWeaponSwitchEvent?.Invoke(2);
    }
}