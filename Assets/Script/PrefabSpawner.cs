using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Assign prefabs here")]
    public GameObject[] prefabs;  // Drag prefabs into inspector

    // Provide prefab list to GridManager
    public GameObject[] GetPrefabs()
    {
        return prefabs;
    }
}
