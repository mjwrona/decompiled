// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkerExStartEnd
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.Internal.Performance
{
  internal sealed class CodeMarkerExStartEnd : IDisposable
  {
    private int _end;
    private byte[] _aBuff;

    public CodeMarkerExStartEnd(int begin, int end, byte[] aBuff)
    {
      CodeMarkers.Instance.CodeMarkerEx(begin, aBuff);
      this._end = end;
      this._aBuff = aBuff;
    }

    public CodeMarkerExStartEnd(int begin, int end, Guid guidData)
      : this(begin, end, guidData.ToByteArray())
    {
    }

    public void Dispose()
    {
      if (this._end == 0)
        return;
      CodeMarkers.Instance.CodeMarkerEx(this._end, this._aBuff);
      this._end = 0;
      this._aBuff = (byte[]) null;
    }
  }
}
