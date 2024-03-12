using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class LeftTableContentController : MonoBehaviour
{
    private const int ID = 1;

    [SerializeField] private CameraController _cameraController;
    [SerializeField] private TextMeshPro _leftTableText;
    [SerializeField] private TextMeshPro _mainText;
    [SerializeField] private Transform _mainCamera;

    [SerializeField] private string _mainTextString;

    private Coroutine _startingHoverCoroutine;
    private Coroutine _stoppingHoverCoroutine;
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
            StopAllCoroutines();
            StartCoroutine(SelectingArea());
        }
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            StopAllCoroutines();
            StartCoroutine(UnselectingArea());
        }
    }

    private IEnumerator StartingHover()
    {
        _quickOutline.enabled = true;
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 10f, 0.5f);

        DOTween.To(() => _leftTableText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _leftTableText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 1f, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StoppingHover()
    {
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 0f, 0.5f);

        DOTween.To(() => _leftTableText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _leftTableText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        _quickOutline.enabled = false;
    }

    private IEnumerator SelectingArea()
    {
        yield return new WaitForSeconds(1f);

        foreach (char _character in _mainTextString)
        {
            _mainText.text += _character;

            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator UnselectingArea()
    {
        while (_mainText.text.Length > 0)
        {
            _mainText.text = _mainText.text.Substring(0, _mainText.text.Length - 1);

            yield return new WaitForSeconds(0.005f);
        }
    }
}