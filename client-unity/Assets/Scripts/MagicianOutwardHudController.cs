using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using SpacetimeDB.Types;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagicianOutwardHudController : MonoBehaviour
{

    [SerializeField] Image OutwardDustCloud;
    [SerializeField] float DustCloudFadeSeconds = 0.1f;
    Coroutine? ActiveOutwardDustCloudCoroutine;
    [SerializeField] Material LocalInvisibleMaterial;
    [SerializeField] Material NonLocalInvisibleMaterial;
    [SerializeField] Material DefaultMaterial;
    [SerializeField] Material EmptyMaterial;
    [SerializeField] Material InvincibleMaterial;
    [SerializeField] SkinnedMeshRenderer MagicianSkin;

    public void SetOutwardDustCloudActive(bool IsActive)
    {
        if (OutwardDustCloud == null) return;

        if (ActiveOutwardDustCloudCoroutine != null)
        {
            StopCoroutine(ActiveOutwardDustCloudCoroutine);
            ActiveOutwardDustCloudCoroutine = null;
        }

        float TargetAlpha01 = IsActive ? 1f : 0f;
        ActiveOutwardDustCloudCoroutine = StartCoroutine(AnimateOutwardDustCloudAlpha(TargetAlpha01, DustCloudFadeSeconds));
    }

    IEnumerator AnimateOutwardDustCloudAlpha(float TargetAlpha01, float DurationSeconds)
    {
        if (OutwardDustCloud == null) yield break;

        Color CurrentColor = OutwardDustCloud.color;
        float StartAlpha01 = CurrentColor.a;

        float ElapsedSeconds = 0f;

        while (ElapsedSeconds < DurationSeconds)
        {
            ElapsedSeconds += Time.deltaTime;
            float Time01 = Mathf.Clamp01(ElapsedSeconds / DurationSeconds);

            float NewAlpha01 = Mathf.Lerp(StartAlpha01, TargetAlpha01, Time01);
            OutwardDustCloud.color = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, NewAlpha01);

            yield return null;
        }

        OutwardDustCloud.color = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, TargetAlpha01);
        ActiveOutwardDustCloudCoroutine = null;
    }

    public void SetInvisible(bool Local)
    {
        Material[] Materials = MagicianSkin.materials;
        Materials[0] = Local ? LocalInvisibleMaterial : NonLocalInvisibleMaterial;
        MagicianSkin.materials = Materials;
    }

    public void ResetInvisible()
    {
        Material[] Materials = MagicianSkin.materials;
        Materials[0] = DefaultMaterial;
        MagicianSkin.materials = Materials;
    }

    public void SetInvincible()
    {
        Material[] Materials = MagicianSkin.materials;
        Materials[1] = InvincibleMaterial;
        MagicianSkin.materials = Materials;
    }

    public void ResetInvincible()
    {
        Material[] Materials = MagicianSkin.materials;
        Materials[1] = EmptyMaterial;
        MagicianSkin.materials = Materials;
    }
}
