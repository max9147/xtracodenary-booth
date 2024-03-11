using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RightScreenContentController : MonoBehaviour
{
    private const int ID = 4;

    [SerializeField] private CameraController _cameraController;

    private Coroutine _outlineCoroutine;
    private QuickOutline _quickOutline;

    private void Awake()
    {
        _quickOutline = GetComponent<QuickOutline>();
    }

    private void OnEnable()
    {
        _cameraController.StartHover += StartHover;
        _cameraController.StopHover += StopHover;
        _cameraController.SelectArea += SelectArea;
        _cameraController.UnselectArea += UnselectArea;
    }

    private void OnDisable()
    {
        _cameraController.StartHover -= StartHover;
        _cameraController.StopHover -= StopHover;
        _cameraController.SelectArea -= SelectArea;
        _cameraController.UnselectArea -= UnselectArea;
    }

    private void StartHover(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            if (_outlineCoroutine != null)
                StopCoroutine(_outlineCoroutine);
            _quickOutline.enabled = true;
            DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 10f, 0.5f);
        }
    }

    private void StopHover(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 0f, 0.5f);
            _outlineCoroutine = StartCoroutine(DisablingOutline());
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

    private IEnumerator DisablingOutline()
    {
        yield return new WaitForSeconds(0.5f);

        _quickOutline.enabled = false;
    }
}