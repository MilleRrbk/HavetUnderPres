using UnityEngine;

public class MotorBoatMover : MonoBehaviour
{
    public Transform boat; // Træk din MotorBoat prefab herind i Inspector
    public Vector3 startPosition = new Vector3(53.4f, 17.51f, 9.2f);
    public Vector3 endPosition = new Vector3(-103.5f, 17.51f, -16.7f);
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