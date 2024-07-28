// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkerStartEnd
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.Internal.Performance
{
  internal sealed class CodeMarkerStartEnd : IDisposable
  {
    private int _end;

    public CodeMarkerStartEnd(int begin, int end)
    {
      CodeMarkers.Instance.CodeMarker(begin);
      this._end = end;
    }

    public void Dispose()
    {
      if (this._end == 0)
        return;
      CodeMarkers.Instance.CodeMarker(this._end);
      this._end = 0;
    }
  }
}
