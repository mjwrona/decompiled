// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Marker
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.Internal.Performance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class Marker
  {
    private static Dictionary<int, int> s_codeMarkerMap;
    private const string s_format = "WIT PerfMonitor: Marker={0}, Thread={1}, Tick={2}";
    private static BooleanSwitch s_switchPerf = new BooleanSwitch("PerfMonitor", "TFS Perfromance monitoring and reporting");

    public static void Process(Mark mark)
    {
      int codeMarker;
      if (Marker.TryTranslate(mark, out codeMarker))
        TfCodeMarkers.CodeMarker(codeMarker);
      Marker.TraceMarker(mark);
    }

    internal static void TraceMarker(Mark mark)
    {
      if (!Marker.s_switchPerf.Enabled && !TeamFoundationTrace.IsTracingEnabled)
        return;
      TeamFoundationTrace.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WIT PerfMonitor: Marker={0}, Thread={1}, Tick={2}", (object) mark, (object) Environment.CurrentManagedThreadId, (object) Environment.TickCount));
    }

    internal static bool TryTranslate(Mark mark, out int codeMarker)
    {
      Marker.EnsureList();
      return Marker.s_codeMarkerMap.TryGetValue((int) mark, out codeMarker);
    }

    private static void EnsureList()
    {
      if (Marker.s_codeMarkerMap != null)
        return;
      Marker.s_codeMarkerMap = new Dictionary<int, int>()
      {
        [101] = 9500,
        [102] = 9501,
        [103] = 9502,
        [104] = 9503,
        [105] = 9504,
        [106] = 9505,
        [107] = 9506,
        [108] = 9507,
        [109] = 9508,
        [110] = 9509,
        [111] = 9510,
        [112] = 9511,
        [113] = 9512,
        [114] = 9513,
        [115] = 9514,
        [116] = 9515,
        [117] = 9516,
        [118] = 9517,
        [119] = 9518,
        [120] = 9519,
        [121] = 9520,
        [122] = 9521,
        [123] = 9522,
        [124] = 9523,
        [125] = 9524,
        [126] = 9525,
        [167] = 9560,
        [168] = 9561,
        [157] = 9548,
        [158] = 9549,
        [159] = 9550,
        [160] = 9551,
        [161] = 9552,
        [162] = 9553,
        [169] = 9558,
        [170] = 9559,
        [201] = 9526,
        [202] = 9527,
        [203] = 9528,
        [204] = 9529,
        [209] = 9530,
        [210] = 9531,
        [211] = 9532,
        [212] = 9533,
        [(int) byte.MaxValue] = 9544,
        [256] = 9545,
        [301] = 9534,
        [302] = 9535,
        [303] = 9536,
        [304] = 9537,
        [309] = 9538,
        [310] = 9539,
        [345] = 9554,
        [346] = 9555,
        [351] = 9556,
        [352] = 9557,
        [311] = 9540,
        [312] = 9541,
        [355] = 9546,
        [356] = 9547
      };
    }
  }
}
