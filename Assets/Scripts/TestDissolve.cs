using UnityEngine;

public class TestDissolve : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;
    [SerializeField] private float speed = 0.5f;

    void Update()
    {
        float amount = Mathf.PingPong(Time.time * speed, 1.0f);

        // Send dissolve amount to Shader Graph.
        targetMaterial.SetFloat("_DissolveAmount", amount);
    }
}