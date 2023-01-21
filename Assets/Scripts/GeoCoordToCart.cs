using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GeoCoordToCart : MonoBehaviour
{
    public static GeoCoordToCart Instance { get; private set; }

    [SerializeField] private float _spherreScale = 10;
    [SerializeField] private GameObject _spherePrefab;    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Vector3 origin = transform.position;
        foreach (GeoCoord coord in MissionController.Instance.Coords)
        {
            Vector3 spherePoint = coord.ToVector3UnitSphere();
            Vector3 worldPosPoint = origin + spherePoint * _spherreScale;
            InstantiatePoint(_spherePrefab, worldPosPoint, coord.Label);
        }
    }

    private void InstantiatePoint(GameObject prefab, Vector3 coords, string name)
    {
        GameObject point = Instantiate(prefab, coords, Quaternion.identity);
        TMP_Text tmpText = point.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
            tmpText.text = name;
    }

    public Vector3 GetOriginPosition()
    {
        return transform.position;
    }

    public float GetSphereScale()
    {
        return _spherreScale;
    }
}
