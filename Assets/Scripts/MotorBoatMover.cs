using UnityEngine;

public class MotorBoatMover : MonoBehaviour
{
    public Transform boat; // Træk din MotorBoat prefab herind i Inspector
    public Vector3 startPosition = new Vector3(49f, 17.98f, -25.6f);
    public Vector3 endPosition = new Vector3(-142f, 17.98f, -25.6f);
    public float moveDuration = 15f; // Hvor lang tid det tager at sejle
    private bool isMoving = false;
    private float moveTimer = 0f;

    private void Start()
    {
        boat.position = startPosition;
    }

    private void Update()
    {
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);
            boat.position = Vector3.Lerp(startPosition, endPosition, t);
        }
    }

    public void StartMotorBoatMovement()
    {
        isMoving = true;
        moveTimer = 0f;
    }
}