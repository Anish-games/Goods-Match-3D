using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeholder : MonoBehaviour
{
    [Header("Slots Settings")]
    public Transform[] slots = new Transform[7];
    public float scaleDownFactor = 0.5f;

    [Header("References")]
    public ScoreSystem scoreSystem; // assign in inspector (optional)

    private int nextIndex = 0;
    private List<GameObject> slotContents = new List<GameObject>();
    private bool isGameOver = false;

    /// <summary>
    /// Places a prefab into the next available slot (called by GridManager).
    /// </summary>
    public void PlacePrefab(GameObject prefab)
    {
        if (isGameOver)
        {
            Debug.Log("[Placeholder] Ignoring PlacePrefab because game is over.");
            return;
        }

        if (prefab == null)
        {
            Debug.LogWarning("[Placeholder] PlacePrefab called with null prefab.");
            return;
        }

        if (nextIndex >= slots.Length)
        {
            Debug.Log("[Placeholder] All slots full — triggering Game Over.");
            TriggerGameOver();
            return;
        }

        Transform target = slots[nextIndex];
        if (target == null)
        {
            Debug.LogWarning("[Placeholder] Slot transform at index " + nextIndex + " is null.");
            return;
        }

        // Instantiate at slot world position and rotation (unparented)
        GameObject placed = Instantiate(prefab, target.position, target.rotation);

        // Remove click handlers and disable colliders/rigidbodies
        foreach (var ch in placed.GetComponentsInChildren<MonoBehaviour>(true))
        {
            // If there is a ClickHandler script, destroy it
            if (ch is ClickHandler) Destroy(ch);
        }
        foreach (var c in placed.GetComponentsInChildren<Collider>(true)) c.enabled = false;
        foreach (var rb in placed.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        // Apply consistent world scale
        placed.transform.localScale = prefab.transform.localScale * scaleDownFactor;

        // Ensure rotation behaviour
        var rot = placed.GetComponent<RotateOnAxis>() ?? placed.AddComponent<RotateOnAxis>();
        rot.axis = Vector3.up;
        rot.speed = 60f;

        // Track in slotContents
        slotContents.Insert(nextIndex, placed);
        nextIndex++;

        // Debug
        Debug.Log("[Placeholder] Placed prefab into slot " + (nextIndex - 1));

        // Check for matches and shift if needed
        CheckForMatches();

        // After check, if still full and not gameover, trigger game over
        if (!isGameOver && nextIndex >= slots.Length)
        {
            Debug.Log("[Placeholder] Slots are full after placement — triggering Game Over.");
            TriggerGameOver();
        }
    }

    /// <summary>
    /// Deletes the last placed prefab (used by delete/undo button).
    /// </summary>
    public void DeleteLastPrefab()
    {
        if (isGameOver)
        {
            Debug.Log("[Placeholder] Cannot delete: game is over.");
            return;
        }

        if (nextIndex <= 0)
        {
            Debug.Log("[Placeholder] Nothing to delete.");
            return;
        }

        int lastIndex = nextIndex - 1;
        if (lastIndex >= 0 && lastIndex < slotContents.Count && slotContents[lastIndex] != null)
        {
            Destroy(slotContents[lastIndex]);
            slotContents.RemoveAt(lastIndex);
            nextIndex = slotContents.Count;
            Debug.Log("[Placeholder] Deleted last prefab from slot " + lastIndex);
        }
    }

    private void CheckForMatches()
    {
        if (slotContents.Count < 3) return;

        Dictionary<string, List<int>> groups = new Dictionary<string, List<int>>();

        // Group objects by prefabName (using PrefabIdentifier)
        for (int i = 0; i < slotContents.Count; i++)
        {
            GameObject obj = slotContents[i];
            if (obj == null) continue;
            PrefabIdentifier id = obj.GetComponent<PrefabIdentifier>();
            if (id == null || string.IsNullOrEmpty(id.prefabName)) continue;

            if (!groups.ContainsKey(id.prefabName))
                groups[id.prefabName] = new List<int>();

            groups[id.prefabName].Add(i);
        }

        bool destroyedAny = false;

        // Destroy first found group with 3+ items (one group per check)
        foreach (var kvp in groups)
        {
            if (kvp.Value.Count >= 3)
            {
                destroyedAny = true;

                foreach (int idx in kvp.Value)
                {
                    if (idx >= 0 && idx < slotContents.Count && slotContents[idx] != null)
                    {
                        Destroy(slotContents[idx]);
                        slotContents[idx] = null;
                    }
                }

                // Remove nulls and recompact list
                slotContents.RemoveAll(item => item == null);
                nextIndex = slotContents.Count;

                // Notify ScoreSystem
                if (scoreSystem != null)
                {
                    scoreSystem.AddScore(1);
                }

                Debug.Log("[Placeholder] Match destroyed for \"" + kvp.Key + "\". New nextIndex = " + nextIndex);
                break; // handle one match at a time
            }
        }

        if (destroyedAny)
        {
            // Reposition remaining objects to slot transforms
            for (int i = 0; i < slotContents.Count; i++)
            {
                if (slotContents[i] != null && i < slots.Length)
                {
                    slotContents[i].transform.position = slots[i].position;
                    slotContents[i].transform.rotation = slots[i].rotation;
                }
            }
        }
    }

    private void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        int finalScore = (scoreSystem != null) ? scoreSystem.currentScore : 0;
        Debug.Log("[Placeholder] Triggering GameOver. Final score = " + finalScore);

        // Use the robust static call (it will find an instance if necessary)
        GameOverManager.ShowGameOverStatic(finalScore);
    }

    public void ResetPlaceholder()
    {
        // Destroy all tracked objects and clear
        for (int i = 0; i < slotContents.Count; i++)
        {
            if (slotContents[i] != null)
                DestroyImmediate(slotContents[i]);
        }
        slotContents.Clear();
        nextIndex = 0;
        isGameOver = false;
    }
}
