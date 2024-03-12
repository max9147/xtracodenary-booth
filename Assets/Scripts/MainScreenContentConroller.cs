using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainScreenContentConroller : MonoBehaviour
{
    private const int ID = 0;

    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _selectionCanvasLeft;
    [SerializeField] private GameObject _selectionCanvasRight;
    [SerializeField] private Image[] _selecitonOutlines;
    [SerializeField] private RawImage _video;
    [SerializeField] private RenderTexture[] _videoTextures;
    [SerializeField] private TextMeshPro _mainScreenText;
    [SerializeField] private VideoPlayer[] _videoPlayers;

    private Coroutine _startingHoverCoroutine;
    private Coroutine _stoppingHoverCoroutine;
    private QuickOutline _quickOutline;

    private bool _canChangeVideo;
    private int _currentVideo;

    private void Awake()
    {
        _quickOutline = GetComponent<QuickOutline>();
        _canChangeVideo = true;
        _currentVideo = 0;
        _selecitonOutlines[_currentVideo].color = Color.green;
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

    public void SelectVideo(int _videoID)
    {
        if (_currentVideo == _videoID || !_canChangeVideo)
            return;

        _canChangeVideo = false;

        StartCoroutine(ChangingVideo(_videoID));
    }

    private IEnumerator StartingHover()
    {
        _quickOutline.enabled = true;
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 10f, 0.5f);

        _mainScreenText.DOColor(Color.white, 0.5f);
        DOTween.To(() => _mainScreenText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _mainScreenText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 1f, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StoppingHover()
    {
        DOTween.To(() => _quickOutline.OutlineWidth, x => _quickOutline.OutlineWidth = x, 0f, 0.5f);

        _mainScreenText.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
        DOTween.To(() => _mainScreenText.fontMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _mainScreenText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        _quickOutline.enabled = false;
    }

    private IEnumerator SelectingArea()
    {
        yield return new WaitForSeconds(1f);

        _selectionCanvasLeft.transform.localPosition = new Vector3(1f, 1f, 1.35f);
        _selectionCanvasRight.transform.localPosition = new Vector3(-1f, 1f, 1.35f);

        _selectionCanvasLeft.SetActive(true);
        _selectionCanvasRight.SetActive(true);

        _selectionCanvasLeft.transform.DOLocalMove(new Vector3(2.3f, -1f, 1.35f), 1f).SetEase(Ease.InOutSine);
        _selectionCanvasRight.transform.DOLocalMove(new Vector3(-2.3f, -1f, 1.35f), 1f).SetEase(Ease.InOutSine);

        _video.texture = _videoTextures[_currentVideo];
        _videoPlayers[_currentVideo].Play();

        yield return new WaitForSeconds(0.1f);

        _video.DOColor(Color.white, 0.5f);
    }

    private IEnumerator UnselectingArea()
    {
        _video.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);

        _selectionCanvasLeft.transform.DOLocalMove(new Vector3(1f, 1f, 1.35f), 1f).SetEase(Ease.InOutSine);
        _selectionCanvasRight.transform.DOLocalMove(new Vector3(-1f, 1f, 1.35f), 1f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0.5f);

        _videoPlayers[_currentVideo].Stop();

        yield return new WaitForSeconds(0.5f);

        _selectionCanvasLeft.SetActive(false);
        _selectionCanvasRight.SetActive(false);
    }

    private IEnumerator ChangingVideo(int _videoID)
    {
        _video.DOColor(new Color(1f, 1f, 1f, 0f), 0.2f);
        _selecitonOutlines[_currentVideo].DOColor(Color.white, 0.2f);

        yield return new WaitForSeconds(0.2f);

        _videoPlayers[_currentVideo].Stop();
        _currentVideo = _videoID;
        _video.texture = _videoTextures[_currentVideo];
        _videoPlayers[_currentVideo].Play();

        yield return new WaitForSeconds(0.1f);

        _video.DOColor(Color.white, 0.2f);
        _selecitonOutlines[_currentVideo].DOColor(Color.green, 0.2f);

        yield return new WaitForSeconds(0.2f);

        _canChangeVideo = true;
    }
}