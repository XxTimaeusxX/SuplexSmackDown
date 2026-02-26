using System.IO;
using Unity.Profiling;
using UnityEngine;

public class FrameTimeLogger : MonoBehaviour
{
    private FrameTiming[] frameTimings = new FrameTiming[1];
    private string csvPath;

    void Start()
    {
        csvPath = Path.Combine(Application.persistentDataPath, "Scripts/Profiling");
        Directory.CreateDirectory(csvPath);
        csvPath = Path.Combine(csvPath, "frame_times.csv");
        // Ensure header is written
        File.WriteAllText(csvPath, "Frame,TimeMs\n");
    }

    void Update()
    {
        FrameTimingManager.CaptureFrameTimings();
        var count = FrameTimingManager.GetLatestTimings(1, frameTimings);

        if (count > 0)
        {
            var timing = frameTimings[0];
            var cpuTimeMs = timing.cpuMainThreadFrameTime;

            // Log every frame
            string line = $"{Time.frameCount},{cpuTimeMs:F2}\n";
            File.AppendAllText(csvPath, line);
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log($"Logged frame times to: {csvPath}");
    }
}
