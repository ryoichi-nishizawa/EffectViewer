using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    DissolveShaderController dissolveShaderController = null;

    public void OnClick()
    {
        dissolveShaderController.StartProgress();
    }
}