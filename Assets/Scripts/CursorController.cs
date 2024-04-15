using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private RectTransform _cursor;

    private float _lastPositionX;
    private float _targetRotation;

    private void Awake()
    {
        Cursor.visible = false;

        _lastPositionX = 0f;
        _targetRotation = 0f;
    }

    private void Update()
    {
        _targetRotation += _lastPositionX - Input.mousePosition.x / 10f;
        _lastPositionX = Input.mousePosition.x / 10f;
        _targetRotation = Mathf.Lerp(_targetRotation, 0f, 10f * Time.deltaTime);
        _cursor.eulerAngles = new Vector3(0f, 0f, _targetRotation);

        _cursor.anchoredPosition = new Vector2((_mainCamera.ScreenToViewportPoint(Input.mousePosition).x - 0.5f) * Screen.width, (_mainCamera.ScreenToViewportPoint(Input.mousePosition).y - 0.5f) * Screen.height);
    }
}