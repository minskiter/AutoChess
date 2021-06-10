using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class LayerTool
{
    public static void ChangeLayer(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        var children = obj.GetComponentsInChildren<Transform>(true);
        foreach (var trans in children)
        {
            trans.gameObject.layer = layer;
        }
    }
}
