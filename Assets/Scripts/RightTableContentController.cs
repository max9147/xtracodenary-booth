using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class RightTableContentController : MonoBehaviour
{
    private const int ID = 2;

    [SerializeField] private CameraController _cameraController;
    [SerializeField] private TextMeshPro _mainText;
    [SerializeField] private Transform _mainCamera;

    [SerializeField] private string _mainTextString;

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

    private void Update()
    {
        _mainText.transform.LookAt(_mainText.transform.position * 2f - _mainCamera.position);
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
            StopAllCoroutines();
            StartCoroutine(TypingText());
        }
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            StopAllCoroutines();
            StartCoroutine(HidingText());
        }
    }

    private IEnumerator DisablingOutline()
    {
        yield return new WaitForSeconds(0.5f);

        _quickOutline.enabled = false;
    }

    private IEnumerator TypingText()
    {
        yield return new WaitForSeconds(1f);

        foreach (char _character in _mainTextString)
        {
            _mainText.text += _character;

            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator HidingText()
    {
        while (_mainText.text.Length > 0)
        {
            _mainText.text = _mainText.text.Substring(0, _mainText.text.Length - 1);

            yield return new WaitForSeconds(0.005f);
        }
    }
}