using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using TMPro;
using UnityEngine;

public class GeoCoordToCart : MonoBehaviour
{
    public static GeoCoordToCart Instance { get; private set; }

    [SerializeField] private float _spherreScale = 10;
    [SerializeField] private GameObject _spherePrefab;

    private const string JSON_FILE_NAME = "Cities";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        string json = Resources.Load<TextAsset>(JSON_FILE_NAME).text;
        List<GeoCoord> coords = new List<GeoCoord>(json.Split("\n")
            .Take(500)
            .Select(x => JsonUtility.FromJson<GeoCoord>(x))
            .Where(x => x != null));

        Vector3 origin = transform.position;
        foreach (GeoCoord coord in coords)
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
