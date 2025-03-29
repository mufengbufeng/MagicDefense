using UnityEngine;

public static class UnityUtil
{
    public static Vector3 zero = new Vector3(0, 0, 0);
    public static Vector3 one = new Vector3(1, 1, 1);
    public static void SetActivityScale(this GameObject go, bool active)
    {
        if (go == null)
        {
            return;
        }
        go.GetComponent<Transform>().localScale = active ? one : zero;
    }

    public static void SetActive(this GameObject go, bool active)
    {
        if (go == null)
        {
            return;
        }

        go.SetActive(active);
    }
}
