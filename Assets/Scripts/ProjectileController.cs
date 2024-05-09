using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}