using UnityEngine;

public class HookClickMover : MonoBehaviour
{
    public Transform fishingHook;     // Hele krogen inkl. line og alt
    public float targetY = 20.4f;
    public float dipDistance = 0.5f;
    public float moveSpeed = 5f;

    private bool dipDown = false;
    private bool moveUp = false;
    private float dipTargetY;

    public void TriggerHookDip()
    {
        if (!dipDown && !moveUp)
        {
            Debug.Log("Hook triggered via XR!");
            dipTargetY = fishingHook.position.y - dipDistance;
            dipDown = true;
        }
    }

    void Update()
    {
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
            {
                moveUp = false;

                // 🔽 Deaktiver hele fiskekrogen
                fishingHook.gameObject.SetActive(false);
                Debug.Log("Fishing hook deactivated");
            }
        }
    }
}