using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _cameraOutside;
    [SerializeField] private CinemachineVirtualCamera[] _selectionCameras;
    [SerializeField] private Transform[] _selectionPoints;
    [SerializeField] private QuickOutline[] _selectionOutlines;

    private float _posX;
    private int _selectedArea;

    private void Awake()
    {
        _selectedArea = -1;
    }

    private void Update()
    {
        if (_selectedArea == -1)
        {
            MoveOutsideCamera();
            CheckHover();
            CheckSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            UnselectArea();
    }

    private void MoveOutsideCamera()
    {
        _posX = Mathf.Lerp(_posX, (Input.mousePosition.x / Screen.width * 2f - 1f) * -50f, 0.01f);
        _cameraOutside.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = _posX;
    }

    private void CheckSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hit))
            {
                for (int i = 0; i < _selectionPoints.Length; i++)
                {
                    if (_selectionPoints[i] == _hit.transform)
                        SelectArea(i);
                }
            }
        }
    }

    private void CheckHover()
    {
        if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hit))
        {
            for (int i = 0; i < _selectionPoints.Length; i++)
                _selectionOutlines[i].enabled = _selectionPoints[i] == _hit.transform;
        }
        else
        {
            for (int i = 0; i < _selectionPoints.Length; i++)
                _selectionOutlines[i].enabled = false;
        }
    }

    private void SelectArea(int _selectionID)
    {
        _selectedArea = _selectionID;
        _selectionCameras[_selectedArea].Priority = 2;

        for (int i = 0; i < _selectionPoints.Length; i++)
            _selectionOutlines[i].enabled = false;
    }

    private void UnselectArea()
    {
        _selectionCameras[_selectedArea].Priority = 0;
        _selectedArea = -1;
    }
}