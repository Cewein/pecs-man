using ECS;
using UnityEngine;

public class ECSUpdaterScript : MonoBehaviour
{
    private void Awake()
    {
        ECSSystem.ScanForEnabledSystems();
    }
    
    private void Start()
    {
        ECSSystem.StartAll();
    }

    private void Update()
    {
        ECSSystem.UpdateAll();
    }

    private void LateUpdate()
    {
        EntityActionBuffer.Instance.FlushBufferedActions();
    }
}
