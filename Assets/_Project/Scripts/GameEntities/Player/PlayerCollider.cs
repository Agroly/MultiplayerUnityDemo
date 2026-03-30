using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class PlayerCollider : MonoBehaviour
{
    [Inject] PlayerSpawner spawner;
    private void OnCollisionEnter(Collision collision)
    {
        spawner.RespawnPlayerServerRpc();
        
    }
}
