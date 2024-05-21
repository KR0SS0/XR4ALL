using UnityEngine;

public class DroneController : MonoBehaviour
{
    public enum RequiredSwingDirection { Any, Up, Down, Left, Right }
    public RequiredSwingDirection requiredDirection = RequiredSwingDirection.Any;
    public float requiredSpeed = 1.0f;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip destroyClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LightsaberController lightsaber))
        {
            Debug.Log("Drone found saber in trigger");
            Vector3 swingDirection = lightsaber.GetSwingDirection();
            float swingSpeed = lightsaber.GetVelocity().magnitude;

            if (IsValidSwing(swingDirection, swingSpeed))
            {
                DestroyDrone();
            }
        }
    }

    private bool IsValidSwing(Vector3 direction, float speed)
    {
        Debug.Log("Speed: " + speed);

        if (speed < requiredSpeed)
        {
            Debug.Log("Not enough speed in swing");
            return false;
        }

        switch (requiredDirection)
        {
            case RequiredSwingDirection.Any:
            return true;
            case RequiredSwingDirection.Up:
            return Vector3.Dot(direction, Vector3.up) > 0.7f;
            case RequiredSwingDirection.Down:
            return Vector3.Dot(direction, Vector3.down) > 0.7f;
            case RequiredSwingDirection.Left:
            return Vector3.Dot(direction, Vector3.left) > 0.7f;
            case RequiredSwingDirection.Right:
            return Vector3.Dot(direction, Vector3.right) > 0.7f;
            default:
            return false;
        }
    }

    private void DestroyDrone()
    {
        Debug.Log("Destroy drone");
        source.PlayOneShot(destroyClip);
        //Destroy(gameObject);
    }
}
