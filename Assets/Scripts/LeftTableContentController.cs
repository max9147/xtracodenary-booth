using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class LeftTableContentController : MonoBehaviour
{
    private const int ID = 1;

    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _leftTableCanvas;
    [SerializeField] private GameObject _descriptionText;
    [SerializeField] private GameObject[] _teamTexts;
    [SerializeField] private TextMeshPro _leftTableText;
    [SerializeField] private Transform _mainCamera;

    private Coroutine _startingHoverCoroutine;
    private Coroutine _stoppingHoverCoroutine;
    private QuickOutline _quickOutline;

    private int _selectedID;

    private void Awake()
    {
        _quickOutline = GetComponent<QuickOutline>();
        _selectedID = -1;
    }

    private void OnEnable()
    {
        _cameraController.StartedHover += StartHover;
        _cameraController.StoppedHover += StopHover;
        _cameraController.SelectedArea += SelectArea;
        _cameraController.UnselectedArea += UnselectArea;

        _descriptionText.SetActive(true);
        foreach (var _teamText in _teamTexts)
            _teamText.SetActive(false);
    }

    private void OnDisable()
    {
        _cameraController.StartedHover -= StartHover;
        _cameraController.StoppedHover -= StopHover;
        _cameraController.SelectedArea -= SelectArea;
        _cameraController.UnselectedArea -= UnselectArea;
    }

    public void SelectTeamMember(int _setSelectedID)
    {
        if (_selectedID != -1)
            _teamTexts[_selectedID].SetActive(false);
        else
            _descriptionText.SetActive(false);

        _selectedID = _setSelectedID;

        _teamTexts[_selectedID].SetActive(true);
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
            _leftTableCanvas.SetActive(true);
    }

    private void UnselectArea(int _currentPoint)
    {
        if (_currentPoint == ID)
            _leftTableCanvas.SetActive(false);
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
}