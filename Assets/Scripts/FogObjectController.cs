using System.Collections;
using UnityEngine;

public class FogObjectController : MonoBehaviour
{
    [SerializeField] private GameObject fogObject;

    public void EnableFog()
    {
        fogObject.SetActive(true);
    }

    public void DisableFog()
    {
        fogObject.SetActive(false);
    }

    public void FogForSeconds(float seconds)
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
