using UnityEngine;

public class RowSpawner : MonoBehaviour
{
    public Transform position1;
    public Transform position2;
    public Transform position3;
    public Transform position4;

    public Transform[] GetPositions()
    {
        return new Transform[] { position1, position2, position3, position4 };
    }
}
