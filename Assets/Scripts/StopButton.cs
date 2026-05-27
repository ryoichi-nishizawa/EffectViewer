using UnityEngine;

public class StopButton : MonoBehaviour
{
    [SerializeField]
    DissolveShaderController dissolveShaderController = null;

    public void OnClick()
    {
        dissolveShaderController.StopProgress();
    }
}