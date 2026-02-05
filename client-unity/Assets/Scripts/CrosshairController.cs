using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#nullable enable 
public class CrosshairController : MonoBehaviour
{
    [SerializeField] Image HitMarkerTL;
    [SerializeField] Image HitMarkerBL;
    [SerializeField] Image HitMarkerTR;
    [SerializeField] Image HitMarkerBR;

    Coroutine? HitMarkerCoroutine;

    public void ShowHitMarker()
    {
        if (HitMarkerCoroutine != null) 
            StopCoroutine(HitMarkerCoroutine);

        HitMarkerCoroutine = StartCoroutine(HitMarkerFade());
    }

    IEnumerator HitMarkerFade()
    {
        float DurationSeconds = 0.25f;

        SetHitMarkerAlpha(1f);

        float StartTime = Time.time;
        while (Time.time - StartTime < DurationSeconds)
        {
            float T = (Time.time - StartTime) / DurationSeconds;
            SetHitMarkerAlpha(1f - T);
            yield return null;
        }

        SetHitMarkerAlpha(0f);
        HitMarkerCoroutine = null;
    }

    void SetHitMarkerAlpha(float Alpha)
    {
        if (HitMarkerTL != null) { var C = HitMarkerTL.color; C.a = Alpha; HitMarkerTL.color = C; }
        if (HitMarkerBL != null) { var C = HitMarkerBL.color; C.a = Alpha; HitMarkerBL.color = C; }
        if (HitMarkerTR != null) { var C = HitMarkerTR.color; C.a = Alpha; HitMarkerTR.color = C; }
        if (HitMarkerBR != null) { var C = HitMarkerBR.color; C.a = Alpha; HitMarkerBR.color = C; }
    }
}
