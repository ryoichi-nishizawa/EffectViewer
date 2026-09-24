using UnityEngine;

public class TapEffectController : MonoBehaviour
{
    [SerializeField] private Material tapMaterial;

    [Header("Radius Settings")]
    [SerializeField] private float maxRadius = 0.04f;
    [SerializeField] private float appearDuration = 0.2f;

    [Header("Pulse Settings (Loop)")]
    [SerializeField] private float pulseFrequency = 6.0f;

    private float currentRadius = 0.0f;
    private bool isTouching = false;
    private float touchTimer = 0.0f;
    private Vector2 currentScreenPos = Vector2.zero;

    private static readonly int CenterID = Shader.PropertyToID("_Center");
    private static readonly int RadiusID = Shader.PropertyToID("_Radius");

    private void OnEnable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.OnTouchBegan += HandleTouchBegan;
            TouchManager.Instance.OnTouchMoved += HandleTouchMoved;
            TouchManager.Instance.OnTouchEnded += HandleTouchEnded;
        }
    }

    private void OnDisable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.OnTouchBegan -= HandleTouchBegan;
            TouchManager.Instance.OnTouchMoved -= HandleTouchMoved;
            TouchManager.Instance.OnTouchEnded -= HandleTouchEnded;
        }
    }

    private void HandleTouchBegan(Vector2 screenPosition)
    {
        isTouching = true;
        touchTimer = 0.0f;
        currentRadius = -0.1f;
        currentScreenPos = screenPosition;
    }

    private void HandleTouchMoved(Vector2 screenPosition)
    {
        currentScreenPos = screenPosition;
    }

    private void HandleTouchEnded(Vector2 screenPosition)
    {
        isTouching = false;
    }

    private void Update()
    {
        if (isTouching)
        {
            touchTimer += Time.deltaTime;

            Vector2 uvPos = new Vector2(currentScreenPos.x / Screen.width, currentScreenPos.y / Screen.height);
            tapMaterial.SetVector(CenterID, uvPos);

            if (touchTimer < appearDuration)
            {
                // [Phase 1] Linearly interpolate from 0 to maxRadius over the specified duration (appearDuration)
                float progress = touchTimer / appearDuration;
                currentRadius = Mathf.Lerp(0.0f, maxRadius, progress);
            }
            else
            {
                // [Phase 2] After appearDuration has elapsed: Loop between max and half radius
                float minRadius = maxRadius * 0.5f;
                float pulseTime = (touchTimer - appearDuration) * pulseFrequency;

                // Smoothly starts shrinking from 1 (max) down to half (0.5)
                float wave = (Mathf.Cos(pulseTime) + 1.0f) * 0.5f;
                currentRadius = Mathf.Lerp(minRadius, maxRadius, wave);
            }
        }
        else
        {
            // Rapidly shrink to 0 when touch is released
            currentRadius = Mathf.Lerp(currentRadius, 0.0f, Time.deltaTime * 15.0f);
            if (currentRadius < 0.001f)
            {
                currentRadius = -0.1f;
            }
        }

        tapMaterial.SetFloat(RadiusID, currentRadius);
    }
}