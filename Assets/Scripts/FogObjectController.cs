using System.Collections;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public void EnableFog()
    {
        RenderSettings.fog = true;
    }

    public void DisableFog()
    {
        RenderSettings.fog = false;
    }

    public void EnableFogForSeconds(float seconds)
    {
        StartCoroutine(FogCoroutine(seconds));
    }

private IEnumerator FogCoroutine(float seconds)
    {
        RenderSettings.fog = true;
        yield return new WaitForSeconds(seconds);
        RenderSettings.fog = false;
    }

}
