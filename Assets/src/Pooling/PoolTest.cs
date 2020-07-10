using UnityEngine;

public class PoolTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
            PoolManager.Instance().createPools();
    }
}
