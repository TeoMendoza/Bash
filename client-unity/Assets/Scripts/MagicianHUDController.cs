using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MagicianHUDController : MonoBehaviour
{
    [SerializeField] public Canvas HudCanvas;

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

    void Start()
    {
        HealthBarWidth = HealthBar.sizeDelta.x;
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

}
