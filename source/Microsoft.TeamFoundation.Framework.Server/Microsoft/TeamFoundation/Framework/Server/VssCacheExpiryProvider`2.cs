// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssCacheExpiryProvider`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssCacheExpiryProvider<TKey, TValue> : 
    VssCacheExpiryProvider,
    IExpiryProvider<TKey, TValue>
  {
    private readonly Capture<TimeSpan> m_expiryInterval;
    private readonly Capture<TimeSpan> m_inactivityInterval;
    private readonly Capture<DateTime> m_expiryDelay;
    protected readonly ITimeProvider m_timeProvider;

    public VssCacheExpiryProvider(
      Capture<TimeSpan> expiryInterval,
      Capture<TimeSpan> inactivityInterval,
      Capture<DateTime> expiryDelay = null)
      : this(expiryInterval, inactivityInterval, expiryDelay, (ITimeProvider) null)
    {
    }

    internal VssCacheExpiryProvider(
      Capture<TimeSpan> expiryInterval,
      Capture<TimeSpan> inactivityInterval,
      Capture<DateTime> expiryDelay,
      ITimeProvider timeProvider)
    {
      ArgumentUtility.CheckForNull<Capture<TimeSpan>>(expiryInterval, nameof (expiryInterval));
      ArgumentUtility.CheckForNull<Capture<TimeSpan>>(inactivityInterval, nameof (inactivityInterval));
      this.m_expiryInterval = expiryInterval;
      this.m_inactivityInterval = inactivityInterval;
      this.m_expiryDelay = expiryDelay;
      this.m_timeProvider = timeProvider ?? (ITimeProvider) new DefaultTimeProvider();
    }

    public virtual bool IsExpired(
      TKey key,
      TValue value,
      DateTime modifiedTimestamp,
      DateTime accessedTimestamp)
    {
      DateTime now = this.m_timeProvider.Now;
      if (this.m_expiryDelay != null && !(now >= this.m_expiryDelay.Value))
        return false;
      return now - modifiedTimestamp >= this.m_expiryInterval.Value || now - accessedTimestamp >= this.m_inactivityInterval.Value;
    }
  }
}
