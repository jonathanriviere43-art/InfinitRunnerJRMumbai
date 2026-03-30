using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DistanceTracker : MonoBehaviour
{
    [Header("Références")]
    public ChunkManager chunkManager;
    public TextMeshProUGUI distanceText;

    [Header("Options")]
    public float distanceMultiplier = 1f;

    private float runDistance = 0f;
    private float totalDistanceSaved = 0f;

    // 🔥 Historique des runs
    private List<int> runs = new List<int>();

    void Update()
    {
        if (chunkManager == null || distanceText == null)
            return;

        runDistance += chunkManager.moveSpeed * Time.deltaTime * distanceMultiplier;

        distanceText.text = "Distance : " + Mathf.FloorToInt(runDistance) + " m";
    }

    // 🔹 Sauvegarde du run + reset
    public void SaveAndResetRun()
    {
        int run = Mathf.FloorToInt(runDistance);

        runs.Add(run);
        totalDistanceSaved += run;

        runDistance = 0f;
    }

    // 🔹 Score final
    public int GetFinalScore()
    {
        return Mathf.FloorToInt(totalDistanceSaved + runDistance);
    }

    // 🔹 Accès runs
    public List<int> GetRuns()
    {
        return runs;
    }

    // ✅ AJOUT : distance actuelle
    public float GetCurrentDistance()
    {
        return runDistance;
    }
}
