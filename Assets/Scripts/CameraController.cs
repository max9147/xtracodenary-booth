using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public event Action<int> StartedHover;
    public event Action<int> StoppedHover;
    public event Action<int> SelectedArea;
    public event Action<int> UnselectedArea;

    [SerializeField] private Color _blueColor;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _cameraOutside;
    [SerializeField] private CinemachineVirtualCamera[] _selectionCameras;
    [SerializeField] private ParticleSystem[] _fireworks;
    [SerializeField] private TextMeshProUGUI[] _navigationTexts;
    [SerializeField] private Transform _logo;
    [SerializeField] private Transform[] _selectionPoints;

    private bool _blockRotation;
    private bool _canPlaySecret;
    private float _posX;
    private float _posXSelection;
    private float _posYSelection;
    private int _selectedArea;
    private int _hoveredArea;

    private void Awake()
    {
        _blockRotation = false;
        _canPlaySecret = true;
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
            CheckLogoClick();
        }
        else
        {
            MoveInsideCamera();

            if (Input.GetKeyDown(KeyCode.Escape))
                StartCoroutine(UnselectingArea());
        }
    }

    public void SelectArea(int _selectionID)
    {
        if (_selectionID == _selectedArea)
            StartCoroutine(UnselectingArea());
        else if (_selectedArea != -1)
            StartCoroutine(UnselectingArea(_selectionID));
        else
            SelectingArea(_selectionID);
    }

    private void MoveOutsideCamera()
    {
        if (!_blockRotation)
        {
            _posX = Mathf.Lerp(_posX, (Input.mousePosition.x / Screen.width * 2f - 1f) * -50f, 0.01f);
            _cameraOutside.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = _posX;
        }
    }

    private void MoveInsideCamera()
    {
        if (!_blockRotation)
        {
            _posXSelection = Mathf.Lerp(_posXSelection, (Input.mousePosition.x / Screen.width * 2f - 1f) * 0.2f, 0.01f);
            _posYSelection = Mathf.Lerp(_posYSelection, (Input.mousePosition.y / Screen.height * 2f - 1f) * 0.2f, 0.01f);
            _selectionCameras[_selectedArea].transform.localPosition = new Vector3(_posXSelection, _posYSelection, 0f);
        }
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

    private void CheckLogoClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hit))
            {
                if (_hit.transform == _logo)
                    PlaySecret();
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
                        StoppedHover?.Invoke(_hoveredArea);

                    _hoveredArea = _currentPoint;
                    StartedHover?.Invoke(_hoveredArea);
                }
            }
        }
        else if (_hoveredArea > -1)
        {
            StoppedHover?.Invoke(_hoveredArea);
            _hoveredArea = -1;
        }
    }

    private void SelectingArea(int _selectionID)
    {
        _selectedArea = _selectionID;

        if (_selectedArea < 5)
            _navigationTexts[_selectedArea].color = Color.white;

        if (_selectedArea == 1 || _selectedArea == 2)
        {
            _blockRotation = true;
            DOTween.To(() => _cameraOutside.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value, x => _cameraOutside.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = x, _selectedArea == 1 ? 180f : -180f, 2f).SetEase(Ease.InOutSine);
        }
        else
            _selectionCameras[_selectedArea].Priority = 2;
        SelectedArea?.Invoke(_selectedArea);

        StoppedHover?.Invoke(_hoveredArea);
        _hoveredArea = -1;
    }

    private void PlaySecret()
    {
        if (!_canPlaySecret)
            return;

        _canPlaySecret = false;

        StartCoroutine(RefreshSecret());

        foreach (var _firework in _fireworks)
            _firework.Play();
    }

    private IEnumerator RefreshSecret()
    {
        yield return new WaitForSeconds(10f);

        _canPlaySecret = true;
    }

    private IEnumerator UnselectingArea(int _reselectID = -1)
    {
        if (_selectedArea == 1 || _selectedArea == 2)
        {
            DOTween.To(() => _cameraOutside.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value, x => _cameraOutside.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = x, 0f, 2f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(2f);

            _blockRotation = false;
            _posX = 0;
        }
        else
            _selectionCameras[_selectedArea].Priority = 0;

        if (_selectedArea < 5)
            _navigationTexts[_selectedArea].color = _blueColor;

        UnselectedArea?.Invoke(_selectedArea);
        _selectedArea = -1;

        if (_reselectID != -1)
        {
            if (_reselectID == 1 || _reselectID == 2)
                yield return new WaitForSeconds(1f);

            SelectingArea(_reselectID);
        }
    }
}