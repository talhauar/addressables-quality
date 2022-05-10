using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QualitySetting", menuName = "ScriptableObjects/QualitySetting", order = 1)]
public class QualitySetting : ScriptableObject
{
    public string name;
    public List<Quality> qualities = new List<Quality>();
}
