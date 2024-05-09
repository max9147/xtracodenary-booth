using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RightGameContentController : MonoBehaviour
{
    private const int ID = 6;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _gameCanvas;
    [SerializeField] private RectTransform _player;
    [SerializeField] private RectTransform _projectilePrefab;
    [SerializeField] private RectTransform[] _enemies;

    private Coroutine _startingHoverCoroutine;
    private Coroutine _stoppingHoverCoroutine;
    private QuickOutline _quickOutline;

    private bool _isPlaying;

    private void Awake()
    {
        _quickOutline = GetComponent<QuickOutline>();

        _isPlaying = false;
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
        if (!_isPlaying)
            return;

        _player.anchoredPosition = new Vector2(Mathf.Clamp((Input.mousePosition.x - (Screen.width / 2)) / 2, -300f, 300f), -220f);

        bool _hasWon = true;

        foreach (var item in _enemies)
        {
            if (item.gameObject.activeInHierarchy)
                _hasWon = false;
        }

        if (_hasWon)
        {
            _isPlaying = false;
            _cameraController.UnselectArea(true);
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

            foreach (var item in _enemies)
            {
                item.gameObject.SetActive(true);
            }

            _isPlaying = true;

            StartCoroutine(Shoot());
        }
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            _gameCanvas.SetActive(false);

            _isPlaying = false;
        }
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

    private IEnumerator Shoot()
    {
        while (_isPlaying)
        {
            yield return new WaitForSeconds(1f);

            RectTransform _curProjectile = Instantiate(_projectilePrefab, _gameCanvas.transform);
            _curProjectile.anchoredPosition = _player.anchoredPosition + new Vector2(0f, 20f);
            _curProjectile.DOAnchorPos(new Vector2(_curProjectile.anchoredPosition.x, 240f), 1f).SetEase(Ease.Linear);
        }
    }
}