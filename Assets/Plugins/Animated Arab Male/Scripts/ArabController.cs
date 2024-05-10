using System.Collections;
using UnityEngine;

namespace ArabDemo
{
    public class ArabController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] SkinnedMeshRenderer face;
        [SerializeField] private int blinkBlendshape = 10;
        [SerializeField] private int talkBlendshapeEndIndx = 9;

        [Header("Settings")]
        [SerializeField] private float lipsSpeedDelay = .1f;
        [SerializeField] private Vector2 blinkDelayRange = new Vector2(1f, 3f);
        public bool TalkAllowed = false;
        public bool BlinkAllowed = false;



        private void Start()
        {
            if (TalkAllowed)
                StartCoroutine(talkIE());

            if (BlinkAllowed)
                StartCoroutine(BlinkIE());
        }
        IEnumerator BlinkIE()
        {
            while (BlinkAllowed)
            {
                float randomDelay = Random.Range(blinkDelayRange.x, blinkDelayRange.y);
                face.SetBlendShapeWeight(blinkBlendshape, 100);
                yield return new WaitForSeconds(.1f);
                face.SetBlendShapeWeight(blinkBlendshape, 0);
                yield return new WaitForSeconds(randomDelay);
            }
        }

        IEnumerator talkIE()
        {
            while (TalkAllowed)
            {
                int randomPhonem = Random.Range(0, talkBlendshapeEndIndx);
                float targetWeight = UnityEngine.Random.Range(60, 100);

                // Gradually increase blendshape weight
                for (float t = 0; t < 1; t += Time.deltaTime / lipsSpeedDelay)
                {
                    float newWeight = Mathf.Lerp(0, targetWeight, t);
                    face.SetBlendShapeWeight(randomPhonem, newWeight);
                    yield return null;
                }

                // Wait before resetting blendshape weight
                yield return new WaitForSeconds(lipsSpeedDelay);

                // Gradually decrease blendshape weight back to 0
                for (float t = 0; t < 1; t += Time.deltaTime / lipsSpeedDelay)
                {
                    float newWeight = Mathf.Lerp(targetWeight, 0, t);
                    face.SetBlendShapeWeight(randomPhonem, newWeight);
                    yield return null;
                }
            }
        }

    }
}
