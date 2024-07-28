// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.TfCodeMarkers
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;

namespace Microsoft.Internal.Performance
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TfCodeMarkers
  {
    public static void CodeMarker(int nTimerID) => CodeMarkers.Instance.CodeMarker(nTimerID);

    public static void InitPerformanceDll(int iApp, string strRegRoot) => CodeMarkers.Instance.InitPerformanceDll(iApp, strRegRoot);

    public static void UninitializePerformanceDLL(int iApp) => CodeMarkers.Instance.UninitializePerformanceDLL(iApp);
  }
}
