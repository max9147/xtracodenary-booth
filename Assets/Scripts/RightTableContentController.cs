using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightTableContentController : MonoBehaviour
{
    private const int ID = 2;

    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Image _textBackground;
    [SerializeField] private Image[] _textBackgroundLogos;
    [SerializeField] private TextMeshPro _rightTableText;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private Transform _textCanvas;

    [SerializeField] private Color _backgroundColor;
    [SerializeField] private Color _backgroundLogoColor;

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
        _textCanvas.transform.LookAt(_mainText.transform.position * 2f - _mainCamera.position);
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

        DOTween.To(() => _rightTableText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _rightTableText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 1f, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StoppingHover()
    {
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 0f, 0.5f);

        DOTween.To(() => _rightTableText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _rightTableText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        _quickOutline.enabled = false;
    }

    private IEnumerator SelectingArea()
    {
        yield return new WaitForSeconds(1f);

        _textBackground.DOColor(_backgroundColor, 0.5f);
        foreach (var _bakcgroundLogo in _textBackgroundLogos)
            _bakcgroundLogo.DOColor(_backgroundLogoColor, 0.5f);

        yield return new WaitForSeconds(0.5f);

        foreach (char _character in "We are an events\ntechnology company that\nencourages you to dream,\nto bring out those\nextraordinary thoughts\nwhich you had labelled\nridiculous and through\ncreativity and ingenuity we\nwill make them a reality.")
        {
            _mainText.text += _character;

            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator UnselectingArea()
    {
        _textBackground.DOColor(new Color(_backgroundColor.r, _backgroundColor.g, _backgroundColor.b, 0f), 0.5f);
        foreach (var _bakcgroundLogo in _textBackgroundLogos)
            _bakcgroundLogo.DOColor(Color.clear, 0.5f);
        _mainText.DOColor(new Color(1, 1, 1, 0f), 0.5f);

        yield return new WaitForSeconds(0.5f);

        _mainText.text = string.Empty;
        _mainText.color = Color.white;
    }
}