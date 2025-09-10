using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Rows in the grid")]
    public RowSpawner[] rowSpawners;   // assign rows (e.g. 3 for 3x4)
    public PrefabSpawner prefabSpawner;
    public Placeholder placeholder;    // assign in inspector

    // per-row lists
    private List<List<GameObject>> gridPrefabs = new List<List<GameObject>>();

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        GameObject[] prefabs = prefabSpawner.GetPrefabs();
        gridPrefabs.Clear();

        for (int row = 0; row < rowSpawners.Length; row++)
        {
            Transform[] positions = rowSpawners[row].GetPositions();
            List<GameObject> rowPrefabs = new List<GameObject>();

            for (int col = 0; col < positions.Length; col++)
            {
                GameObject spawned = SpawnRandomPrefab(prefabs, positions[col].position, positions[col].rotation);
                rowPrefabs.Add(spawned);

                // collider/click setup
                Collider colComp = spawned.GetComponent<Collider>();
                if (colComp != null) colComp.enabled = (col == 0);

                if (col == 0)
                    AddClickHandler(spawned, row);
            }

            gridPrefabs.Add(rowPrefabs);
        }
    }

    GameObject SpawnRandomPrefab(GameObject[] prefabs, Vector3 pos, Quaternion rot)
    {
        if (prefabs == null || prefabs.Length == 0) return null;
        int rnd = Random.Range(0, prefabs.Length);
        GameObject toSpawn = prefabs[rnd];
        return Instantiate(toSpawn, pos, rot);
    }

    void AddClickHandler(GameObject go, int rowIndex)
    {
        if (go.GetComponent<ClickHandler>() == null)
        {
            ClickHandler ch = go.AddComponent<ClickHandler>();
            ch.gridManager = this;
            ch.rowIndex = rowIndex;
        }
    }

    // called from ClickHandler when player clicks the item currently at col1 of the given row
    public void OnPrefabAtCol1Clicked(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= gridPrefabs.Count) return;

        List<GameObject> rowPrefabs = gridPrefabs[rowIndex];
        if (rowPrefabs.Count == 0) return;

        GameObject first = rowPrefabs[0]; // item at col1
        if (first == null)
        {
            rowPrefabs.RemoveAt(0);
            return;
        }

        // PASS TO PLACEHOLDER (Placeholder will instantiate a clone into its slots)
        if (placeholder != null)
        {
            placeholder.PlacePrefab(first);
        }

        // Disable the original in the grid and remove it from active list
        first.SetActive(false);
        rowPrefabs.RemoveAt(0);

        // SHIFT FORWARD: move remaining prefabs to new column positions
        Transform[] positions = rowSpawners[rowIndex].GetPositions();
        for (int col = 0; col < rowPrefabs.Count; col++)
        {
            GameObject item = rowPrefabs[col];
            if (item == null) continue;

            item.transform.position = positions[col].position;
            item.transform.rotation = positions[col].rotation;

            Collider colComp = item.GetComponent<Collider>();
            if (colComp != null)
            {
                if (col == 0) // new col1
                {
                    colComp.enabled = true;
                    if (item.GetComponent<ClickHandler>() == null)
                        AddClickHandler(item, rowIndex);
                }
                else
                {
                    colComp.enabled = false;
                }
            }
        }

        // Refill last column with a new random prefab so row stays full
        GameObject[] pool = prefabSpawner.GetPrefabs();
        GameObject newPrefab = SpawnRandomPrefab(pool, positions[positions.Length - 1].position, positions[positions.Length - 1].rotation);
        if (newPrefab != null)
        {
            // ensure last one collider is OFF
            Collider newCol = newPrefab.GetComponent<Collider>();
            if (newCol != null) newCol.enabled = false;

            rowPrefabs.Add(newPrefab);
        }
    }
}
