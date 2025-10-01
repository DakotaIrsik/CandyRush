using UnityEngine;
using UnityEngine.EventSystems;

namespace OctoberStudio.UI
{
    /// <summary>
    /// Ensures there is always exactly one EventSystem in the scene.
    /// This component should be placed on EventSystem GameObjects to prevent duplicates.
    /// </summary>
    public class EventSystemManager : MonoBehaviour
    {
        private EventSystem thisEventSystem;

        private void Awake()
        {
            thisEventSystem = GetComponent<EventSystem>();

            // Find all EventSystems in the scene
            EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

            if (eventSystems.Length > 1)
            {
                Debug.Log($"[EventSystemManager] Found {eventSystems.Length} EventSystems. Ensuring only one remains active.");

                // Keep the first one found and disable the others
                bool isFirstEventSystem = true;
                foreach (EventSystem eventSystem in eventSystems)
                {
                    if (isFirstEventSystem)
                    {
                        // Keep the first one active
                        isFirstEventSystem = false;
                        Debug.Log($"[EventSystemManager] Keeping EventSystem active: {eventSystem.gameObject.name}");
                    }
                    else
                    {
                        // Disable duplicate EventSystems
                        Debug.Log($"[EventSystemManager] Disabling duplicate EventSystem: {eventSystem.gameObject.name}");
                        eventSystem.enabled = false;

                        // Optionally destroy the GameObject if it only contains EventSystem
                        var components = eventSystem.GetComponents<Component>();
                        if (components.Length <= 3) // Transform, EventSystem, and this manager
                        {
                            Destroy(eventSystem.gameObject);
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"[EventSystemManager] Single EventSystem found: {thisEventSystem.gameObject.name}");
            }
        }

        private void Start()
        {
            // Double-check in Start() in case other objects were instantiated
            EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
            if (eventSystems.Length > 1)
            {
                Debug.LogWarning($"[EventSystemManager] Still {eventSystems.Length} EventSystems after Awake. This may cause UI input issues.");
            }
        }
    }
}