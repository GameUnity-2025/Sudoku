using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Auto-setup helper: ensures an UndoManager exists at runtime and wires Undo buttons automatically.
public class AutoSetup : MonoBehaviour
{
    IEnumerator Start()
    {
        // Wait one frame so other Awake() methods (InputButton) run first
        yield return null;

        // Ensure UndoManager exists
        if (UndoManager.instance == null)
        {
            GameObject mgr = new GameObject("Managers");
            mgr.AddComponent<UndoManager>();
            Debug.Log("AutoSetup: Created Managers GameObject with UndoManager.");
        }

    // Try to find and wire Undo buttons by name heuristics
    // Use the no-argument FindObjectsOfType for compatibility with older Unity versions.
    Button[] buttons = FindObjectsOfType<Button>();
        foreach (var b in buttons)
        {
            if (b == null || b.gameObject == null) continue;
            string nm = b.gameObject.name.ToLower();
            bool matchedName = nm.Contains("undo") || nm.Contains("back") || nm.Contains("quay") || nm == "button";

            bool hasPersistentCalls = false;
            try
            {
                // Use reflection-free test: check persistent call count via serialized data is not available here,
                // so we'll approximate by checking the onClick.GetPersistentEventCount() when available.
                hasPersistentCalls = b.onClick.GetPersistentEventCount() > 0;
            }
            catch { hasPersistentCalls = false; }

            if (matchedName || !hasPersistentCalls)
            {
                if (b.GetComponent<UndoButtonBinder>() == null)
                {
                    b.onClick.AddListener(() => { if (InputButton.instance != null) InputButton.instance.ClickedUndo(); });
                    b.gameObject.AddComponent<UndoButtonBinder>();
                    Debug.Log($"AutoSetup: Wired undo listener to button '{b.gameObject.name}' (matchedName={matchedName}, hasPersistentCalls={hasPersistentCalls}).");
                }
            }
        }
    }
}
