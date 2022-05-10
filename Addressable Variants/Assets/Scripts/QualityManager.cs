using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "QualityManager", menuName = "ScriptableObjects/QualityManager", order = 2)]
public class QualityManager : ScriptableSingleton<QualityManager>
{
    [SerializeField] public QualityType CurrentQuality = QualityType.Low;
    [SerializeField] private List<QualitySetting> qualitySettings = new List<QualitySetting>();

    /*public GameObject GetPrefabByName(string characterName, QualityType type = QualityType.Low)
    {
       foreach (var quality in qualitySettings)
       {
            if (quality.name.Equals(characterName))
            {
                foreach (var models in quality.qualities)
                {
                    if (models.qualityType == type) return models.prefab;
                }
            }
        }
        return null;
    }

    public void SetModel(string modelName,List<GameObject> renderers)
    {
        QualitySetting tempQuality = null;
        foreach (var vQualitySetting in qualitySettings)
        {
            if (vQualitySetting.name == modelName) tempQuality = vQualitySetting;
        }
        if(tempQuality == null) return;
        foreach (GameObject meshRenderer in renderers)
        {
            tempQuality.Renderers.Add(meshRenderer);
        }
    }*/
}
