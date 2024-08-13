using Photon.Pun;
using UnityEngine;

public class PlayerPrefabManager : MonoBehaviourPunCallbacks
{
    // Define the components or scripts you want to deactivate if the object is not yours
    public MonoBehaviour[] componentsToDeactivate;

    void Start()
    {
        // Check if this prefab belongs to the local player
        if (!photonView.IsMine)
        {
            // If it doesn't belong to the local player, deactivate the specified components
            DeactivateComponents();
        }
    }

    void DeactivateComponents()
    {
        foreach (MonoBehaviour component in componentsToDeactivate)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }
    }
}
