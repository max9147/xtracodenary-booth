using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LeftGameContentController : MonoBehaviour
{
    private const int ID = 5;

    [SerializeField] private CameraController _cameraController;

    private Coroutine _startingHoverCoroutine;
    private Coroutine _stoppingHoverCoroutine;
    private QuickOutline _quickOutline;

    private void Awake()
    {
        _quickOutline = GetComponent<QuickOutline>();
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

        }
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {

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
}