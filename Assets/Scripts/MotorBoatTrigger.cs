using UnityEngine;

public class MotorBoatTrigger : MonoBehaviour
{
    public MotorBoatMover motorBoatMover;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            motorBoatMover.StartMotorBoatMovement();
        }
    }
}