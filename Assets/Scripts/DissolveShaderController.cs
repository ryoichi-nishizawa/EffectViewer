using UnityEngine;
using System.Collections;

public class DissolveShaderController : MonoBehaviour
{
    [SerializeField]
    Material targetMaterial = null;

    [SerializeField][Range(0.0f, 1.0f)]
    float progress = 0.0f;

    [SerializeField]
    float duration = 1.0f;

    Coroutine progressCoroutine = null;
    public bool IsProcessing => progressCoroutine != null;

    void Update()
    {
        targetMaterial.SetFloat("_DissolveProgress", progress);
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
        if (!IsProcessing)
        {
            return;
        }

        StopCoroutine(progressCoroutine);
        progressCoroutine = null;
        progress = 0.0f;
    }

    IEnumerator ProgressCoroutine()
    {
        float elapsedTime = 0.0f;
        progress = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // 経過時間に応じて 0.0 から 1.0 の値を計算
            progress = Mathf.Clamp01(elapsedTime / duration);

            yield return null;
        }

        progressCoroutine = null;
        progress = 1.0f;
    }
}