using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Service that manages EventSystem lifecycle to ensure only one exists.
    /// Can be used as singleton or static utility.
    /// </summary>
    public class EventSystemService
    {
        private EventSystem currentEventSystem;
        private GameObject eventSystemGameObject;

        public EventSystem Current => currentEventSystem;

        public EventSystemService()
        {
            // Create our managed EventSystem since scenes no longer contain them
            CreateEventSystem();
        }


        private void CreateEventSystem()
        {
            eventSystemGameObject = new GameObject("EventSystem");
            currentEventSystem = eventSystemGameObject.AddComponent<EventSystem>();
            eventSystemGameObject.AddComponent<InputSystemUIInputModule>();

            Object.DontDestroyOnLoad(eventSystemGameObject);
        }


        /// <summary>
        /// Static utility method to ensure single EventSystem without DI.
        /// </summary>
        public static void EnsureSingleEventSystemStatic()
        {
            EventSystem[] existingEventSystems = Object.FindObjectsOfType<EventSystem>();

            if (existingEventSystems.Length <= 1)
            {
                return; // No issue
            }

            Debug.Log($"[EventSystemService] Found {existingEventSystems.Length} EventSystems. Removing duplicates.");

            // Keep the first one and remove others
            for (int i = 1; i < existingEventSystems.Length; i++)
            {
                var duplicateEventSystem = existingEventSystems[i];
                Debug.Log($"[EventSystemService] Disabling duplicate EventSystem: {duplicateEventSystem.gameObject.name}");
                duplicateEventSystem.enabled = false;

                // Optionally destroy if it only contains EventSystem components
                var components = duplicateEventSystem.GetComponents<Component>();
                if (components.Length <= 3) // Transform, EventSystem, InputModule
                {
                    Object.Destroy(duplicateEventSystem.gameObject);
                }
            }
        }
    }
}