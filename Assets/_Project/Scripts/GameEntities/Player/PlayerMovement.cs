using Unity.Netcode;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Rigidbody))]
public sealed class PlayerMovement : NetworkBehaviour
{
    private IPlayerInput _input;
    private CountDownController _countDownController;
    private Rigidbody _rb;
    private bool IsEnabled = false;

    [SerializeField] private float _forwardSpeed = 30f;
    [SerializeField] private float _pitchSpeed = 60f;
    [SerializeField] private float _yawSpeed = 60f;
    [SerializeField] private float _rollAmount = 30f;
    [SerializeField] private float _rotateSmooth = 8f;

    [Inject]
    private void Construct(IPlayerInput input, CountDownController countDownController)
    {
        _input = input;
        _countDownController = countDownController;
        _countDownController.GameStarted += EnableControls;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _countDownController.GameStarted -= EnableControls;
    }
    private void EnableControls() => IsEnabled = true;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _rb.isKinematic = !IsOwner;
    }

    private void FixedUpdate()
    {
        if (!IsOwner || !IsEnabled) return;
        if (_input == null)
        {
            Debug.Log("Инъекция ввода не прошла");
            return;

        }


        float pitchInput = _input.Pitch;
        float yawInput = _input.Roll;

        float pitchDelta = -pitchInput * _pitchSpeed * Time.fixedDeltaTime;
        float yawDelta = yawInput * _yawSpeed * Time.fixedDeltaTime;

        transform.Rotate(0f, yawDelta, 0f, Space.World);
        transform.Rotate(pitchDelta, 0f, 0f, Space.Self);

        float targetRoll = -yawInput * _rollAmount;
        var euler = transform.localEulerAngles;
        float currentRoll = (euler.z > 180f) ? euler.z - 360f : euler.z;
        float newRoll = Mathf.Lerp(currentRoll, targetRoll, _rotateSmooth * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(euler.x, euler.y, newRoll);

        _rb.linearVelocity = transform.forward * _forwardSpeed;
    }
}
