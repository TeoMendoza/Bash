using UnityEngine;
using TMPro; // Required for TextMeshProUGUI

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Drag your UI TextMeshPro object here in the Inspector
    private float pollingTime = 0.5f; // Update interval
    private float time;
    private int frameCount;
    private int currentFPS;

    private void Update()
    {
        time += Time.deltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            currentFPS = Mathf.RoundToInt(frameCount / time);
            fpsText.text = currentFPS.ToString() + " FPS";

            time -= pollingTime; // Reset time for the next interval
            frameCount = 0; // Reset frame count
        }
    }
}
