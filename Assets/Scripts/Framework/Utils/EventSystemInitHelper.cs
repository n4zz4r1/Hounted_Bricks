using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Utils {
public class EventSystemInitHelper : MonoBehaviour {
    private void Awake() {
        // Find all EventSystem components in the scene
        var eventSystems = FindObjectsOfType<EventSystem>();

        switch (eventSystems.Length) {
            case 0: {
                // No Event System found, so add one
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>(); // Or add any other input module you need
                break;
            }
            case > 1: {
                // More than one Event System found, keep the first one, remove the rest
                for (var i = 1; i < eventSystems.Length; i++) Destroy(eventSystems[i].gameObject);

                break;
            }
        }
        // If there's exactly one, do nothing
    }
}
}