using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public GridManager gridManager;
    public int rowIndex; // which row this belongs to

    void OnMouseDown()
    {
        if (gridManager != null)
        {
            gridManager.OnPrefabAtCol1Clicked(rowIndex);
        }
    }
}
