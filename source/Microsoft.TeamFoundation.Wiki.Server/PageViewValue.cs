// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.PageViewValue
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class PageViewValue
  {
    private int m_viewCountDelta;

    public PageViewValue(
      PageViewKey key,
      int viewCountBase,
      int viewCountDelta,
      DateTime lastViewedAt)
    {
      this.Key = key;
      this.ViewCountBase = viewCountBase;
      this.ViewCountDelta = viewCountDelta;
      this.LastViewedAt = lastViewedAt;
    }

    public PageViewKey Key { get; }

    public int ViewCountBase { get; }

    public int ViewCountDelta
    {
      get => this.m_viewCountDelta;
      private set => this.m_viewCountDelta = value;
    }

    public DateTime LastViewedAt { get; }

    public void IncrementViewCount() => Interlocked.Increment(ref this.m_viewCountDelta);
  }
}
