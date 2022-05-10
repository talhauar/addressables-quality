using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"Model: {gameObject.name} -- Current Quality: {QualityManager.Instance.CurrentQuality}");
    }
}
