using UnityEngine;

public static class TransformExtensions
{
    // 销毁所有子物体
    public static void DestroyAllChildren(this Transform source)
    {
        for (var i = 0; i < source.childCount; i++)
            Object.Destroy(source.GetChild(i).gameObject);
    }

    // 通过名字查找子物体（递归查找） 如果有多个，只返回第一个
    public static Transform FindChildByName(this Transform trans, string goName)
    {
        var child = trans.Find(goName);
        if (child != null)
            return child;

        Transform go = null;
        for (var i = 0; i < trans.childCount; i++)
        {
            child = trans.GetChild(i);
            go = FindChildByName(child, goName);
            if (go != null)
                return go;
        }

        return null;
    }

    // 通过名字查找子物体（递归查找）  where T : UnityEngine.Object 如果有多个，只返回第一个
    public static T FindChildByName<T>(this Transform trans, string goName) where T : Object
    {
        var child = trans.Find(goName);
        if (child != null) return child.GetComponent<T>();

        Transform go = null;
        for (var i = 0; i < trans.childCount; i++)
        {
            child = trans.GetChild(i);
            go = FindChildByName(child, goName);
            if (go != null) return go.GetComponent<T>();
        }

        return null;
    }

    #region 坐标

    public static void SetPositionX(this Transform t, float newX)
    {
        t.position = new Vector3(newX, t.position.y, t.position.z);
    }

    public static void SetPositionY(this Transform t, float newY)
    {
        t.position = new Vector3(t.position.x, newY, t.position.z);
    }

    public static void SetPositionZ(this Transform t, float newZ)
    {
        t.position = new Vector3(t.position.x, t.position.y, newZ);
    }

    public static float GetPositionX(this Transform t)
    {
        return t.position.x;
    }

    public static float GetPositionY(this Transform t)
    {
        return t.position.y;
    }

    public static float GetPositionZ(this Transform t)
    {
        return t.position.z;
    }

    #endregion
}