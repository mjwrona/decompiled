// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.RolloutCounter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  internal sealed class RolloutCounter : IMatcher
  {
    private readonly long _percentage;
    private long _rolloutCount;
    private long _requestCount;

    public RolloutCounter(long percentage)
    {
      this._percentage = percentage >= 0L && percentage <= 100L ? percentage : throw new ArgumentException("percentage must be in <0, 100>");
      this._rolloutCount = 0L;
      this._requestCount = (long) (percentage == 100L);
    }

    public bool Match(in Query query)
    {
      if (Interlocked.Read(ref this._rolloutCount) < Interlocked.Read(ref this._requestCount))
      {
        Interlocked.Add(ref this._rolloutCount, 100L);
        Interlocked.Add(ref this._requestCount, this._percentage);
        return true;
      }
      Interlocked.Add(ref this._requestCount, this._percentage);
      return false;
    }

    bool IMatcher.Match(in Query query) => this.Match(in query);
  }
}
