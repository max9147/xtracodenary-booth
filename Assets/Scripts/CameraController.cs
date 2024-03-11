using Cinemachine;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public event Action<int> StartHover;
    public event Action<int> StopHover;
    public event Action<int> SelectArea;
    public event Action<int> UnselectArea;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _cameraOutside;
    [SerializeField] private CinemachineVirtualCamera[] _selectionCameras;
    [SerializeField] private Transform[] _selectionPoints;

    private float _posX;
    private float _posXSelection;
    private float _posYSelection;
    private int _selectedArea;
    private int _hoveredArea;

    private void Awake()
    {
        _selectedArea = -1;
        _hoveredArea = -1;
    }

    private void Update()
    {
        if (_selectedArea == -1)
        {
            MoveOutsideCamera();
            CheckHover();
            CheckSelection();
        }
        else
        {
            MoveInsideCamera();

            if (Input.GetKeyDown(KeyCode.Escape))
                UnselectingArea();
        }
    }

    private void MoveOutsideCamera()
    {
        _posX = Mathf.Lerp(_posX, (Input.mousePosition.x / Screen.width * 2f - 1f) * -50f, 0.01f);
        _cameraOutside.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = _posX;
    }

    private void MoveInsideCamera()
    {
        _posXSelection = Mathf.Lerp(_posXSelection, (Input.mousePosition.x / Screen.width * 2f - 1f) * 0.2f, 0.01f);
        _posYSelection = Mathf.Lerp(_posYSelection, (Input.mousePosition.y / Screen.height * 2f - 1f) * 0.2f, 0.01f);
        _selectionCameras[_selectedArea].transform.localPosition = new Vector3(_posXSelection, _posYSelection, 0f);
    }

    private void CheckSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hit))
            {
                for (int _currentPoint = 0; _currentPoint < _selectionPoints.Length; _currentPoint++)
                {
                    if (_selectionPoints[_currentPoint] == _hit.transform)
                        SelectingArea(_currentPoint);
                }
            }
        }
    }

    private void CheckHover()
    {
        if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hit))
        {
            for (int _currentPoint = 0; _currentPoint < _selectionPoints.Length; _currentPoint++)
            {
                if (_selectionPoints[_currentPoint] == _hit.transform && _hoveredArea != _currentPoint)
                {
                    if (_hoveredArea > -1)
                        StopHover?.Invoke(_hoveredArea);

                    _hoveredArea = _currentPoint;
                    StartHover?.Invoke(_hoveredArea);
                }
            }
        }
        else if (_hoveredArea > -1)
        {
            StopHover?.Invoke(_hoveredArea);
            _hoveredArea = -1;
        }
    }

    private void SelectingArea(int _selectionID)
    {
        _selectedArea = _selectionID;
        _selectionCameras[_selectedArea].Priority = 2;
        SelectArea?.Invoke(_selectedArea);

        StopHover?.Invoke(_hoveredArea);
        _hoveredArea = -1;
    }

    private void UnselectingArea()
    {
        _selectionCameras[_selectedArea].Priority = 0;
        UnselectArea?.Invoke(_selectedArea);
        _selectedArea = -1;
    }
}