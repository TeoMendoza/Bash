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

#nullable enable

public class MagicianHUDController : MonoBehaviour
{
    [SerializeField] public Canvas HudCanvas;
    [SerializeField] public CrosshairController CrosshairController;
    [SerializeField] private RectTransform HealthBar;
    [SerializeField] private TextMeshProUGUI Health;
    private float HealthBarWidth;

    [SerializeField] private TextMeshProUGUI Ammo;
    [SerializeField] private Image AmmoIcon;

    [SerializeField] private TextMeshProUGUI TarotTime;
    [SerializeField] private Image TarotIcon;

    [SerializeField] private TextMeshProUGUI DustTime;
    [SerializeField] private Image DustIcon;

    [SerializeField] private TextMeshProUGUI CloakTime;
    [SerializeField] private Image CloakIcon;

    [SerializeField] private TextMeshProUGUI HypnosisTime;
    [SerializeField] private Image HypnosisIcon;

    private HudEffect?[] HudEffects = new HudEffect?[5];
    private Coroutine?[] FlashCoroutines = new Coroutine?[5];

    private Dictionary<int, Image> HudEffectsIcons = new();

    [SerializeField] private Image EffectIcon1;
    [SerializeField] private Image EffectIcon2;
    [SerializeField] private Image EffectIcon3;
    [SerializeField] private Image EffectIcon4;
    [SerializeField] private Image EffectIcon5;

    [SerializeField] private Sprite CloakEffectIcon;
    [SerializeField] private Sprite SpeedEffectIcon;
    [SerializeField] private Sprite InvincibleEffectIcon;
    [SerializeField] private Sprite HypnosisEffectIcon;
    [SerializeField] private Sprite TarotEffectIcon;
    [SerializeField] private Sprite NoneEffectIcon;

    void OnEnable()
    {
        HealthBarWidth = HealthBar.sizeDelta.x;
        HudEffectsIcons = new Dictionary<int, Image> { {0, EffectIcon1}, {1, EffectIcon2}, {2, EffectIcon3}, {3, EffectIcon4}, {4, EffectIcon5} };
        HudEffects = new HudEffect?[] { null, null, null, null, null };
    }

    public void UpdateHealth(int newHealth)
    {
        Health.text = newHealth.ToString();
        float HealthPercent = Mathf.Clamp01(newHealth / 200f);
        HealthBar.sizeDelta = new Vector2(HealthBarWidth * HealthPercent, HealthBar.sizeDelta.y);   
    }

    public void UpdateAmmo(int newAmmo)
    {
        Ammo.text = newAmmo.ToString();
        if (newAmmo == 0) {
            Color Color = AmmoIcon.color;
            Color.a = 50f / 255f;
            AmmoIcon.color = Color;
        }

        else {
            Color Color = AmmoIcon.color;
            Color.a = 1f;
            AmmoIcon.color = Color;
        }      
    }

    public void StartTarotCooldown()
    {
        Color Color = TarotIcon.color;
        Color.a = 50f / 255f;
        TarotIcon.color = Color;
    }

    public void UpdateTarot(int Time)
    {
        TarotTime.text = Time.ToString();
    }

    public void EndTarotCooldown()
    {
        Color Color = TarotIcon.color;
        Color.a = 1f;
        TarotIcon.color = Color;

        TarotTime.text = "";
    }


    public void StartDustCooldown()
    {
        Color Color = DustIcon.color;
        Color.a = 50f / 255f;
        DustIcon.color = Color;
    }

    public void UpdateDust(int Time)
    {
        DustTime.text = Time.ToString();
    }

    public void EndDustCooldown()
    {
        Color Color = DustIcon.color;
        Color.a = 1f;
        DustIcon.color = Color;

        DustTime.text = "";
    }

    public void StartCloakCooldown()
    {
        Color Color = CloakIcon.color;
        Color.a = 50f / 255f;
        CloakIcon.color = Color;
    }

    public void UpdateCloak(int Time)
    {
        CloakTime.text = Time.ToString();
    }

    public void EndCloakCooldown()
    {
        Color Color = CloakIcon.color;
        Color.a = 1f;
        CloakIcon.color = Color;

        CloakTime.text = "";
    }

    public void StartHypnosisCooldown()
    {
        Color Color = HypnosisIcon.color;
        Color.a = 50f / 255f;
        HypnosisIcon.color = Color;
    }

    public void UpdateHypnosis(int Time)
    {
        HypnosisTime.text = Time.ToString();
    }

    public void EndHypnosisCooldown()
    {
        Color Color = HypnosisIcon.color;
        Color.a = 1f;
        HypnosisIcon.color = Color;

        HypnosisTime.text = "";
    }

    public void HandleEffectAsTarget(PlayerEffect Effect)
    {
        if (Effect.EffectType == EffectType.Damage) {
            // Apply Red Tint
        }

        else if (Effect.EffectType == EffectType.Dust) {
            // Apply Dust Blind - Also Possibly On Magician Model (Could Be Done Through Animation Tree)
        }
        
        else if (Effect.EffectType == EffectType.Stunned) {
            // Apply Visual "You Are Stunned!" Kind Of Effect
        }

        else {
            AddEffectToHud(Effect);
        }
    }

    public void HandleEffectAsSender(PlayerEffect Effect)
    {
        if (Effect.EffectType == EffectType.Damage) {
            CrosshairController.ShowHitMarker();
        }
    }

    public void HandleEffectRemoveAsTarget(PlayerEffect Effect)
    {
        if (Effect.EffectType == EffectType.Damage) { } // Do Nothing - Red Tints Resolves Itself

        else if (Effect.EffectType == EffectType.Dust) {
            // Remove Dust Blind
        }
        
        else if (Effect.EffectType == EffectType.Stunned) {
            // Remove "You Are Stunned" Effect Stuff
        }

        else {
            RemoveEffectFromHud(Effect);
        }
    }

    public void AddEffectToHud(PlayerEffect Effect)
    {
        for (int Index = 0; Index < HudEffects.Length; Index++)
            if (HudEffects[Index]?.type == Effect.EffectType)
            {
                StopFlash(Index);
                HudEffects[Index]!.id = Effect.Id;
                return;
            }

        for (int Index = 0; Index < HudEffects.Length; Index++)
            if (HudEffects[Index] == null)
            {
                HudEffects[Index] = new HudEffect { type = Effect.EffectType, id = Effect.Id};
                switch (Effect.EffectType)
                {
                    case EffectType.Cloak:
                        HudEffectsIcons[Index].sprite = CloakEffectIcon;
                        break;

                    case EffectType.Invincible:
                        HudEffectsIcons[Index].sprite = InvincibleEffectIcon;
                        break;
                    
                    case EffectType.Speed:
                        HudEffectsIcons[Index].sprite = SpeedEffectIcon;
                        break;
                    
                    case EffectType.Hypnosis:
                        HudEffectsIcons[Index].sprite = HypnosisEffectIcon;
                        break;
                    
                    case EffectType.Tarot:
                        HudEffectsIcons[Index].sprite = TarotEffectIcon;
                        break;

                    default: 
                        break;
                }

                Color color = HudEffectsIcons[Index].color;
                color.a = 1f;
                HudEffectsIcons[Index].color = color;
                return;
            }
    }

    public void TryHudIconFlash(PlayerEffect Effect)
    {
        for (int Index = 0; Index < HudEffects.Length; Index++)
            if (HudEffects[Index]?.type == Effect.EffectType && HudEffects[Index]?.id == Effect.Id)
            {
                if (FlashCoroutines[Index] != null) return;
                FlashCoroutines[Index] = StartCoroutine(FlashHudIcon(Index));
                return;
            }
    }

    public void RemoveEffectFromHud(PlayerEffect Effect)
    {
        for (int Index = 0; Index < HudEffects.Length; Index++)
            if (HudEffects[Index]?.type == Effect.EffectType && HudEffects[Index]?.id == Effect.Id)
            {
                StopFlash(Index);
                HudEffects[Index] = null;
                HudEffectsIcons[Index].sprite = NoneEffectIcon;
                Color color = HudEffectsIcons[Index].color;
                color.a = 0f;
                HudEffectsIcons[Index].color = color;
                return;
            }
    }

    private IEnumerator FlashHudIcon(int Index)
    {
        float FlashStepSeconds = 0.25f;
        float MinAlpha = 0.25f;

        while (true)
        {
            if (HudEffects[Index] == null || HudEffectsIcons[Index] == null) break;

            var Icon = HudEffectsIcons[Index];
            var ColorValue = Icon.color;
            ColorValue.a = MinAlpha;
            Icon.color = ColorValue;
            yield return new WaitForSeconds(FlashStepSeconds);

            if (HudEffects[Index] == null || HudEffectsIcons[Index] == null) break;

            Icon = HudEffectsIcons[Index];
            ColorValue = Icon.color;
            ColorValue.a = 1f;
            Icon.color = ColorValue;
            yield return new WaitForSeconds(FlashStepSeconds);
        }

        FlashCoroutines[Index] = null;
    }

    private void StopFlash(int Index)
    {
        if (FlashCoroutines[Index] == null) return;
        StopCoroutine(FlashCoroutines[Index]);
        FlashCoroutines[Index] = null;

        if (HudEffectsIcons[Index] != null)
        {
            var ColorValue = HudEffectsIcons[Index].color;
            ColorValue.a = 1f;
            HudEffectsIcons[Index].color = ColorValue;
        }
    }

    public class HudEffect
    {
        public ulong id;
        public EffectType type;
    }

}
