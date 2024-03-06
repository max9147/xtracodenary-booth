using System.Collections;
using TMPro;
using UnityEngine;

public class TeamContentController : MonoBehaviour
{
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private TextMeshPro _mainText;

    [SerializeField] private string _mainTextString;

    private void OnEnable()
    {
        _mainText.text = string.Empty;

        StartCoroutine(TypingText());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        _mainText.transform.LookAt(_mainText.transform.position * 2f - _mainCamera.position);
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
}