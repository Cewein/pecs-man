using ECS;
using UnityEngine;

public class ECSUpdaterScript : MonoBehaviour
{
    private void LateUpdate()
    {
        EntityActionBuffer.Instance.FlushBufferedActions();
    }
}
