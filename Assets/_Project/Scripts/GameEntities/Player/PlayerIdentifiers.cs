 using Unity.Collections;
using Unity.Netcode;
using VContainer;

public class PlayerIdentifiers : NetworkBehaviour
{
    [Inject] private SessionManager sessionManager;

    public readonly NetworkVariable<ulong> networkPlayerId = new NetworkVariable<ulong>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public readonly NetworkVariable<FixedString64Bytes> sessionPlayerId = new NetworkVariable<FixedString64Bytes>("", 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            networkPlayerId.Value = OwnerClientId;

            if (sessionManager.session != null)
            {
                sessionPlayerId.Value = sessionManager.session.CurrentPlayer.Id;
            }
        }
    }
}