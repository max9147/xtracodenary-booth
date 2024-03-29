using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightScreenContentController : MonoBehaviour
{
    private const int ID = 4;

    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Image _tv1Image;
    [SerializeField] private Image _tv2Image;
    [SerializeField] private Image _tv3Image;
    [SerializeField] private Sprite[] _slideshow1Sprites;
    [SerializeField] private Sprite[] _slideshow2Sprites;
    [SerializeField] private Sprite[] _slideshow3Sprites;
    [SerializeField] private TextMeshPro _rightScreenText;
    [SerializeField] private TextMeshProUGUI _tv1Text;
    [SerializeField] private TextMeshProUGUI _tv2Text;
    [SerializeField] private TextMeshProUGUI _tv3Text;

    [SerializeField] private string[] _slideshow1Names;
    [SerializeField] private string[] _slideshow2Names;
    [SerializeField] private string[] _slideshow3Names;

    private Coroutine _startingHoverCoroutine;
    private Coroutine _stoppingHoverCoroutine;
    private QuickOutline _quickOutline;

    private int _currentSlide;

    private void Awake()
    {
        _quickOutline = GetComponent<QuickOutline>();
        _currentSlide = 0;
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
            _currentSlide = 0;
            StopAllCoroutines();
            StartCoroutine(ShowingSlides());
        }
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            StopAllCoroutines();

            _tv1Image.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv1Text.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv2Image.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv2Text.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv3Image.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv3Text.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
        }
    }

    private IEnumerator StartingHover()
    {
        _quickOutline.enabled = true;
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 10f, 0.5f);

        _rightScreenText.DOColor(Color.white, 0.5f);
        DOTween.To(() => _rightScreenText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _rightScreenText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 1f, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StoppingHover()
    {
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 0f, 0.5f);

        _rightScreenText.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
        DOTween.To(() => _rightScreenText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _rightScreenText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        _quickOutline.enabled = false;
    }

    private IEnumerator ShowingSlides()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            _tv1Image.sprite = _slideshow1Sprites[_currentSlide];
            _tv1Text.text = _slideshow1Names[_currentSlide];
            _tv2Image.sprite = _slideshow2Sprites[_currentSlide];
            _tv2Text.text = _slideshow2Names[_currentSlide];
            _tv3Image.sprite = _slideshow3Sprites[_currentSlide];
            _tv3Text.text = _slideshow3Names[_currentSlide];

            _tv1Image.DOColor(Color.white, 0.5f);
            _tv1Text.DOColor(Color.white, 0.5f);
            _tv2Image.DOColor(Color.white, 0.5f);
            _tv2Text.DOColor(Color.white, 0.5f);
            _tv3Image.DOColor(Color.white, 0.5f);
            _tv3Text.DOColor(Color.white, 0.5f);

            yield return new WaitForSeconds(3f);

            _tv1Image.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv1Text.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv2Image.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv2Text.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv3Image.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
            _tv3Text.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);

            yield return new WaitForSeconds(0.5f);

            _currentSlide++;
            if (_currentSlide >= _slideshow1Names.Length)
                _currentSlide = 0;
        }
    }
}