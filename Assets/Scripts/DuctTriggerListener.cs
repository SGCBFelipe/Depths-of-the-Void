using UnityEngine;

public enum DuctTriggerType { Entrance, Exit }

public class DuctTriggerListener : MonoBehaviour
{
    [SerializeField] private DuctSystem ductParent;
    [SerializeField] private DuctTriggerType triggerType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerDuctHandler>(out var player))
        {
            if (triggerType == DuctTriggerType.Entrance)
            {
                ductParent.OnPlayerEnterEntrance(player);
            }
            else if (triggerType == DuctTriggerType.Exit)
            {
                ductParent.TriggerExit(player);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerDuctHandler>(out var player))
        {
            if (triggerType == DuctTriggerType.Entrance)
            {
                ductParent.OnPlayerExitEntrance();
            }
        }
    }
}