using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    [SerializeField] bool LockOnStart = false;

    bool IsUiOpen = true;
    bool IsManuallyUnlocked = false;

    void Start()
    {
        ApplyCursorState(LockOnStart);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F2))
        {
            IsManuallyUnlocked = true;
            ApplyCursorState(false);
            return;
        }

        if (!Application.isFocused) return;

        if (!IsUiOpen && IsManuallyUnlocked && Input.GetMouseButtonDown(0))
        {
            IsManuallyUnlocked = false;
            ApplyCursorState(true);
        }
    }

    void OnApplicationFocus(bool HasFocus)
    {
        if (!HasFocus)
        {
            IsManuallyUnlocked = true;
            ApplyCursorState(false);
            return;
        }

        ApplyCursorState(GetShouldLock());
    }

    public void SetUiOpen(bool UiOpen)
    {
        IsUiOpen = UiOpen;

        if (UiOpen)
        {
            IsManuallyUnlocked = false;
            ApplyCursorState(false);
            return;
        }

        IsManuallyUnlocked = false;

        if (Application.isFocused)
            ApplyCursorState(true);
        else
            ApplyCursorState(false);
    }

    bool GetShouldLock()
    {
        if (IsUiOpen) return false;
        if (IsManuallyUnlocked) return false;
        return true;
    }

    void ApplyCursorState(bool Locked)
    {
        Cursor.lockState = Locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !Locked;
    }
}
