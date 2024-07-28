// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.WITCodeMarkerStartEnd
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.Internal.Performance;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WITCodeMarkerStartEnd : IDisposable
  {
    private TfCodeMarkerStartEnd m_codeMarker;
    private Mark m_endMarker;

    public WITCodeMarkerStartEnd(Mark begin, Mark end)
    {
      this.m_endMarker = end;
      int codeMarker1;
      int codeMarker2;
      if (Marker.TryTranslate(begin, out codeMarker1) && Marker.TryTranslate(end, out codeMarker2))
        this.m_codeMarker = new TfCodeMarkerStartEnd(codeMarker1, codeMarker2);
      Marker.TraceMarker(begin);
    }

    public void Dispose()
    {
      if (this.m_codeMarker != null)
        this.m_codeMarker.Dispose();
      Marker.TraceMarker(this.m_endMarker);
    }
  }
}
