using UnityEngine;
using UnityEngine.InputSystem;

public class HookClickMover : MonoBehaviour
{
    public Camera mainCamera;
    public Transform fishingHook;
    public float targetY = 20.4f;
    public float dipDistance = .5f;
    public float moveSpeed = 5f;

    private bool dipDown = false;
    private bool moveUp = false;
    private float dipTargetY;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("You hit: " + hit.transform.name);

                if (hit.transform == fishingHook)
                {
                    Debug.Log("Fishing hook was clicked!");
                    dipTargetY = fishingHook.position.y - dipDistance;
                    dipDown = true;
                }
            }
        }

        if (dipDown)
        {
            Vector3 pos = fishingHook.position;
            pos.y = Mathf.MoveTowards(pos.y, dipTargetY, moveSpeed * Time.deltaTime);
            fishingHook.position = pos;

            if (Mathf.Approximately(pos.y, dipTargetY))
            {
                dipDown = false;
                moveUp = true;
            }
        }

        if (moveUp)
        {
            Vector3 pos = fishingHook.position;
            pos.y = Mathf.MoveTowards(pos.y, targetY, moveSpeed * Time.deltaTime);
            fishingHook.position = pos;

            if (Mathf.Approximately(pos.y, targetY))
                moveUp = false;
        }
    }
}