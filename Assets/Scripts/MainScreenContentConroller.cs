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
    [SerializeField] private RawImage _video;
    [SerializeField] private TextMeshPro _mainScreenText;
    [SerializeField] private VideoPlayer _videoPlayer;

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
            StartCoroutine(PlayingVideo());
        }
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
        {
            StopAllCoroutines();
            StartCoroutine(HidingVideo());
        }
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

    private IEnumerator PlayingVideo()
    {
        yield return new WaitForSeconds(1f);

        _selectionCanvasLeft.transform.localPosition = new Vector3(1f, 1f, 1.35f);
        _selectionCanvasRight.transform.localPosition = new Vector3(-1f, 1f, 1.35f);

        _selectionCanvasLeft.SetActive(true);
        _selectionCanvasRight.SetActive(true);

        _selectionCanvasLeft.transform.DOLocalMove(new Vector3(2.3f, -1f, 1.35f), 1f).SetEase(Ease.InOutSine);
        _selectionCanvasRight.transform.DOLocalMove(new Vector3(-2.3f, -1f, 1.35f), 1f).SetEase(Ease.InOutSine);

        _video.DOColor(Color.white, 0.5f);
        _videoPlayer.Play();
    }

    private IEnumerator HidingVideo()
    {
        _video.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);

        _selectionCanvasLeft.transform.DOLocalMove(new Vector3(1f, 1f, 1.35f), 1f).SetEase(Ease.InOutSine);
        _selectionCanvasRight.transform.DOLocalMove(new Vector3(-1f, 1f, 1.35f), 1f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0.5f);

        _videoPlayer.Stop();

        yield return new WaitForSeconds(0.5f);

        _selectionCanvasLeft.SetActive(false);
        _selectionCanvasRight.SetActive(false);
    }
}