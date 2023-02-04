using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private Transform mapMarkerSphereTransform;
    private RaycastHit hit;
    private GeoCoord newCoordinate;
    private Vector3 worldPositionCoord;
    private Vector2 coordLatLong;
    private Vector2 tempMissionCoords = new Vector2(54.6872f, 25.2797f); // Misija - Vilnius.
    private bool canDisplayMarker;
    private bool canChooseNewLocation = true;
    private GeoCoordToCart coordTransformator;
    public static InputManager instance;

    [Header("UI Settings")]
    [SerializeField] private TMP_Text selectedCoordText;
    [SerializeField] private GameObject mapMarker;
    [SerializeField] private GameObject missionCompletionScreen;
    [SerializeField] private Slider pointsSlider;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text currentMissionText;
    [SerializeField] private TMP_Text totalPointsText;

    [SerializeField] private GameObject _gameCompletionScreen;
    [SerializeField] private GameObject _misionButton;
    [SerializeField] private GameObject _gameFinishButton;

    [Header("Score settings")]
    [SerializeField] private float missionMaxPoints;
    [SerializeField] private float missionMaxDistance;

    private float guessDistance;
    private MissionController _missionController;
    private float _totalScore;
    private int _currentMissionIndex;
    private int _missionsCount;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!SceneDataContainer.Instance.TryGetData(RoundSelectionPanel.COUNT_KEY, out _missionsCount))
            _missionsCount = 5;
        _missionController = MissionController.Instance;
        coordTransformator = GeoCoordToCart.Instance;
        pointsSlider.maxValue = missionMaxPoints;
        pointsSlider.value = 0;
        pointsText.text = "0 Points";
        PickNewMission();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canChooseNewLocation)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                coordLatLong = (ToGPSCoords(hit.point, coordTransformator.GetSphereScale()) - new Vector2(90, 0)) * -1;
                newCoordinate = new GeoCoord
                {
                    Latitude = coordLatLong.x,
                    Longitude = coordLatLong.y,
                };

                CreateMarker();
            }
        }

        if (canDisplayMarker)
        {
            DisplayMapMarker(worldPositionCoord);
        }
    }

    public void CompleteMission()
    {
        canDisplayMarker = false;
        mapMarker.gameObject.SetActive(false);
        missionCompletionScreen.SetActive(true);
        guessDistance = GetDistanceBetweenTwoPoints(coordLatLong, tempMissionCoords);
        selectedCoordText.text =
            @$"Selected location coordinates ({coordLatLong.x}, {coordLatLong.y})
            Your guess was {guessDistance} km away from location";

        float calculatedScore = CalculatePointsBasedOnDistance();
        pointsSlider.value = calculatedScore;
        pointsText.text = $"{calculatedScore} POINTS"; 
        _totalScore += calculatedScore;
    }

    public void PickNewMission()
    {
        _currentMissionIndex++;
        canChooseNewLocation = true;
        missionCompletionScreen.SetActive(false);
        mapMarkerSphereTransform.gameObject.SetActive(false);
        GeoCoord city = _missionController.GetRandomCity();
        tempMissionCoords = new Vector2(city.Latitude, city.Longitude);
        currentMissionText.text = 
            $"Location {_currentMissionIndex} / {_missionsCount} - {city.Label}";

        if (_currentMissionIndex == _missionsCount)
        {
            _misionButton.SetActive(false);
            _gameFinishButton.SetActive(true);
        }
    }

    public void FinishGame()
    {
        missionCompletionScreen.SetActive(false);
        _gameCompletionScreen.SetActive(true);
        totalPointsText.text = $"Total points: {_totalScore}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CreateMarker()
    {
        mapMarker.gameObject.SetActive(true);
        Vector3 spherePoint = newCoordinate.ToVector3UnitSphere();
        worldPositionCoord = coordTransformator.GetOriginPosition() + spherePoint * coordTransformator.GetSphereScale();
        mapMarkerSphereTransform.position = worldPositionCoord;
        canDisplayMarker = true;
    }

    private void DisplayMapMarker(Vector3 position)
    {
        mapMarker.transform.position = Camera.main.WorldToScreenPoint(position);
    }

    private float GetDistanceBetweenTwoPoints(Vector2 clickedLatLong, Vector2 actualLocationLatLong)
    {
        float lat1InRads = clickedLatLong.x * Mathf.PI / 180;
        float lat2InRads = actualLocationLatLong.x * Mathf.PI / 180;
        float latDiffInRads = (actualLocationLatLong.x - clickedLatLong.x) * Mathf.PI / 180;
        float longDiffInRads = (actualLocationLatLong.y - clickedLatLong.y) * Mathf.PI / 180;
        float a = Mathf.Sin(latDiffInRads / 2) * Mathf.Sin(latDiffInRads / 2) + Mathf.Cos(lat1InRads) * Mathf.Cos(lat2InRads) *
          Mathf.Sin(longDiffInRads / 2) * Mathf.Sin(longDiffInRads / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float distance = 6371000 * c;
        return distance / 1000;
    }

    private float CalculatePointsBasedOnDistance()
    {
        if (guessDistance > missionMaxDistance)
            return 0;
        return missionMaxPoints - guessDistance * missionMaxPoints / missionMaxDistance;
    }

    private Vector2 ToGPSCoords(Vector3 position, float sphereRadius)
    {
        float lat = (float)Mathf.Acos(position.y / sphereRadius);
        float lon = (float)Mathf.Atan2(position.x, position.z);
        lat *= Mathf.Rad2Deg;
        lon *= Mathf.Rad2Deg;
        return new Vector2(lat, lon);
    }

    public void SetCanConfirm(bool canChooseNewLocation)
    {
        this.canChooseNewLocation = canChooseNewLocation;
    }
}
