using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraOwner : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _camera; 

    public override void OnNetworkSpawn()
    {
        bool isMine = IsOwner;

        if (_camera != null)
            _camera.enabled = isMine;
    }
}
