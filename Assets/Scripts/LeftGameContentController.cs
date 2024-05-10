using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeftGameContentController : MonoBehaviour
{
    private const int ID = 5;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _gameCanvas;
    [SerializeField] private Image[] _imageTiles;
    [SerializeField] private Sprite _cross;
    [SerializeField] private Sprite _crossOutline;
    [SerializeField] private Sprite _circle;
    [SerializeField] private TextMeshPro _leftGameText;
    [SerializeField] private Transform[] _tileColliders;

    private Coroutine _startingHoverCoroutine;
    private Coroutine _stoppingHoverCoroutine;
    private QuickOutline _quickOutline;

    private bool _playerTurn;
    private bool[] _usedTiles = new bool[9] { false, false, false, false, false, false, false, false, false };
    private bool[] _crossTiles = new bool[9] { false, false, false, false, false, false, false, false, false };
    private int _hoveredTile;

    private void Awake()
    {
        _quickOutline = GetComponent<QuickOutline>();

        _playerTurn = false;
        _hoveredTile = -1;
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
        if (!_playerTurn)
            return;

        if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hit))
        {
            for (int _currentTile = 0; _currentTile < _tileColliders.Length; _currentTile++)
            {
                if (_tileColliders[_currentTile] == _hit.transform && _hoveredTile != _currentTile)
                {
                    if (_currentTile != _hoveredTile && _hoveredTile > -1 && !_usedTiles[_hoveredTile])
                        _imageTiles[_hoveredTile].gameObject.SetActive(false);

                    _hoveredTile = _currentTile;
                    if (!_usedTiles[_hoveredTile])
                    {
                        _imageTiles[_hoveredTile].gameObject.SetActive(true);
                        _imageTiles[_hoveredTile].sprite = _crossOutline;
                    }
                }
            }
        }
        else if (_hoveredTile > -1)
        {
            if (!_usedTiles[_hoveredTile])
                _imageTiles[_hoveredTile].gameObject.SetActive(false);
            _hoveredTile = -1;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_hoveredTile > -1 && !_usedTiles[_hoveredTile])
            {
                _usedTiles[_hoveredTile] = true;
                _crossTiles[_hoveredTile] = true;
                _imageTiles[_hoveredTile].sprite = _cross;

                if ((_crossTiles[0] && _crossTiles[1] && _crossTiles[2]) || (_crossTiles[3] && _crossTiles[4] && _crossTiles[5]) || (_crossTiles[6] && _crossTiles[7] && _crossTiles[8]) ||
                    (_crossTiles[0] && _crossTiles[3] && _crossTiles[6]) || (_crossTiles[1] && _crossTiles[4] && _crossTiles[7]) || (_crossTiles[2] && _crossTiles[5] && _crossTiles[8]) ||
                    (_crossTiles[0] && _crossTiles[4] && _crossTiles[8]) || (_crossTiles[2] && _crossTiles[4] && _crossTiles[6]))
                {
                    _cameraController.UnselectArea(true);
                    return;
                }

                List<int> _availableTiles = new List<int>();

                for (int i = 0; i < _usedTiles.Length; i++)
                {
                    if (!_usedTiles[i])
                        _availableTiles.Add(i);
                }

                if (_availableTiles.Count == 0)
                {
                    _cameraController.UnselectArea();
                    return;
                }

                StartCoroutine(EnemyTurn());
            }
        }
    }

    private IEnumerator EnemyTurn()
    {
        _playerTurn = false;

        yield return new WaitForSeconds(0.5f);

        List<int> _availableTiles = new List<int>();

        for (int i = 0; i < _usedTiles.Length; i++)
        {
            if (!_usedTiles[i])
                _availableTiles.Add(i);
        }

        int _selectedTile = _availableTiles[Random.Range(0, _availableTiles.Count)];
        _usedTiles[_selectedTile] = true;
        _imageTiles[_selectedTile].gameObject.SetActive(true);
        _imageTiles[_selectedTile].sprite = _circle;

        yield return new WaitForSeconds(0.5f);

        _playerTurn = true;
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

            for (int i = 0; i < _usedTiles.Length; i++)
            {
                _usedTiles[i] = false;
                _crossTiles[i] = false;
                _imageTiles[i].sprite = null;
                _imageTiles[i].gameObject.SetActive(false);
            }

            Invoke(nameof(ActivatePlayer), 1f);
        }
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            _gameCanvas.SetActive(false);

            _playerTurn = false;
        }
    }

    private void ActivatePlayer()
    {
        _playerTurn = true;
    }

    private IEnumerator StartingHover()
    {
        _quickOutline.enabled = true;
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 10f, 0.5f);

        _leftGameText.DOColor(Color.white, 0.5f);
        DOTween.To(() => _leftGameText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _leftGameText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 1f, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StoppingHover()
    {
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 0f, 0.5f);

        _leftGameText.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
        DOTween.To(() => _leftGameText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _leftGameText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        _quickOutline.enabled = false;
    }
}