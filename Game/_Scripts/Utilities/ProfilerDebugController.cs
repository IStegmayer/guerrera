using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using UnityEngine;

namespace Dzajna {
public class ProfilerDebugController : MonoBehaviour {
    string statsText;
    ProfilerRecorder systemMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder drawCallsCountRecorder;
    ProfilerRecorder setPassCallsCountRecorder;
    ProfilerRecorder triCountRecorder;
    ProfilerRecorder vertCountRecorder;

    public TMPro.TextMeshProUGUI m_statsText;

    static double GetRecorderFrameAverage(ProfilerRecorder recorder) {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        unsafe {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;
        }

        return r;
    }

    void OnEnable() {
        systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        drawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        setPassCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
        triCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        vertCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
    }

    void OnDisable() {
        systemMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();
        drawCallsCountRecorder.Dispose();
        setPassCallsCountRecorder.Dispose();
        triCountRecorder.Dispose();
        vertCountRecorder.Dispose();
    }

    void Update() {
        var sb = new StringBuilder(500);
        sb.AppendLine($"{GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms");
        sb.AppendLine($"{gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"{systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"{drawCallsCountRecorder.LastValue}");
        sb.AppendLine($"{setPassCallsCountRecorder.LastValue}");
        sb.AppendLine($"{Mathf.RoundToInt(triCountRecorder.LastValue/1000f)}k");
        sb.AppendLine($"{Mathf.RoundToInt(vertCountRecorder.LastValue/1000f)}k");
        m_statsText.text = sb.ToString();
    }

    // void OnGUI() {
    //     GUI.TextArea(new Rect(10, 30, 250, 50), statsText);
    // }
}
}