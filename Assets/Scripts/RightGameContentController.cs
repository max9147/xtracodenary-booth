using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightGameContentController : MonoBehaviour
{
    private const int ID = 6;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Image[] _tileLights;
    [SerializeField] private GameObject _gameCanvas;
    [SerializeField] private TextMeshProUGUI[] _tileNumbers;
    [SerializeField] private Transform[] _tileColliders;

    private Coroutine _startingHoverCoroutine;
    private Coroutine _stoppingHoverCoroutine;
    private QuickOutline _quickOutline;

    private bool[] _usedTiles = new bool[9] { false, false, false, false, false, false, false, false, false };
    private int _activeTile;

    private void Awake()
    {
        _quickOutline = GetComponent<QuickOutline>();

        _activeTile = -1;
    }

    private void OnEnable()
    {
        _cameraController.StartedHover += StartHover;
        _cameraController.StoppedHover += StopHover;
        _cameraController.SelectedArea += SelectArea;
        _cameraController.UnselectedArea += UnselectArea;
    }

    private void OnDisable()
    {
        _cameraController.StartedHover -= StartHover;
        _cameraController.StoppedHover -= StopHover;
        _cameraController.SelectedArea -= SelectArea;
        _cameraController.UnselectedArea -= UnselectArea;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hit))
            {
                for (int _currentTile = 0; _currentTile < _tileColliders.Length; _currentTile++)
                {
                    if (_tileColliders[_currentTile] == _hit.transform && _activeTile > -1 && _activeTile == _currentTile)
                    {
                        _tileLights[_currentTile].gameObject.SetActive(false);
                        _tileNumbers[_currentTile].color = Color.red;
                        ActivateTile();
                    }
                }
            }
        }
    }

    private void StartHover(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            if (_stoppingHoverCoroutine != null)
                StopCoroutine(_stoppingHoverCoroutine);
            _startingHoverCoroutine = StartCoroutine(StartingHover());
        }
    }

    private void StopHover(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            if (_startingHoverCoroutine != null)
                StopCoroutine(_startingHoverCoroutine);
            _stoppingHoverCoroutine = StartCoroutine(StoppingHover());
        }
    }

    private void SelectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            _gameCanvas.SetActive(true);

            Invoke(nameof(ActivateTile), 1f);
        }
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            _gameCanvas.SetActive(false);
        }
    }

    private void ActivateTile()
    {
        List<int> _availableTiles = new List<int>();

        for (int i = 0; i < _usedTiles.Length; i++)
        {
            if (!_usedTiles[i])
                _availableTiles.Add(i);
        }

        if (_availableTiles.Count == 0)
        {
            _cameraController.UnselectArea(true);
            return;
        }

        int _selectedTile = _availableTiles[Random.Range(0, _availableTiles.Count)];
        _activeTile = _selectedTile;

        _usedTiles[_selectedTile] = true;

        _tileLights[_selectedTile].DOColor(Color.red, 1f);
    }

    private IEnumerator StartingHover()
    {
        _quickOutline.enabled = true;
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 10f, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StoppingHover()
    {
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        _quickOutline.enabled = false;
    }
}