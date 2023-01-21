using UnityEngine;

public class AlwaysLookToCamera : MonoBehaviour
{
    [SerializeField] private float _distance;

    private void Start()
    {
        transform.localPosition = transform.parent.position.normalized * _distance;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform, Camera.main.transform.up);
    }
}
