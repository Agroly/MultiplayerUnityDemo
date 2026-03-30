using UnityEngine;

public class PropellerRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1000f;

    void FixedUpdate()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}