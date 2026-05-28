using UnityEngine;
using System.Collections;

public class DissolveShaderController : MonoBehaviour
{
    [SerializeField]
    Material targetMaterial = null;

    [SerializeField]
    Transform targetTransform = null;

    [SerializeField][Range(0.0f, 1.0f)]
    float progress = 0.0f;

    [SerializeField]
    float duration = 1.0f;

    [SerializeField]
    float scaleRange = 1.0f;

    [SerializeField]
    AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField]
    AnimationCurve dissolveCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField]
    ParticleSystem flashImpulse = null;

    Coroutine progressCoroutine = null;
    public bool IsProcessing => progressCoroutine != null;

    void Update()
    {
        float easedProgress = dissolveCurve.Evaluate(progress);
        targetMaterial?.SetFloat("_DissolveProgress", easedProgress);

        float scaleMultiplier = IsProcessing ? scaleCurve.Evaluate(progress) * scaleRange + 1.0f : 1.0f;
        if (targetTransform != null)
        {
            targetTransform.localScale = Vector3.one * scaleMultiplier;
        }
    }

    public void StartProgress()
    {
        if (IsProcessing)
        {
            return;
        }

        progressCoroutine = StartCoroutine(ProgressCoroutine());
    }

    public void StopProgress()
    {
        if (IsProcessing)
        {
            StopCoroutine(progressCoroutine);
        }

        progressCoroutine = null;
        progress = 0.0f;
    }

    IEnumerator ProgressCoroutine()
    {
        float elapsedTime = 0.0f;
        progress = 0.0f;

        flashImpulse?.Play();
        yield return null;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // A value between 0.0 and 1.0 is calculated based on the elapsed time.
            progress = Mathf.Clamp01(elapsedTime / duration);

            yield return null;
        }

        progressCoroutine = null;
        progress = 1.0f;
    }
}