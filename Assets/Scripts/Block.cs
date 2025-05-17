using UnityEngine;

public class Block : MonoBehaviour
{
    public float durabilitySeconds = 0.5f;

    public bool TryBreak(float breakSeconds)
    {
        if (breakSeconds > durabilitySeconds)
        {

            Break();

            return true;

        }

        return false;

    }

    public void Break()
    {
        Destroy(gameObject);
    }
}
