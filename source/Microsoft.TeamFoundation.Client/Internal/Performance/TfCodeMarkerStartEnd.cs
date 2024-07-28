// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.TfCodeMarkerStartEnd
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.Internal.Performance
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfCodeMarkerStartEnd : IDisposable
  {
    private CodeMarkerStartEnd m_codeMarker;

    public TfCodeMarkerStartEnd(int begin, int end) => this.m_codeMarker = new CodeMarkerStartEnd(begin, end);

    public void Dispose() => this.m_codeMarker.Dispose();
  }
}
