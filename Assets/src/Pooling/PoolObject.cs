using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public bool state;

    public void setStatus(bool state)
    {
        this.state = state;
        gameObject.SetActive(state);
    }
}
