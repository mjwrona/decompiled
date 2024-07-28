// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.DomainEntryBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  public abstract class DomainEntryBase : IDomainEntry
  {
    private int IsInitialized;
    private readonly ILockName LockName;
    protected readonly SecurityIdentifier DomainSecurityId;
    protected long CacheRefreshTimeStampTicks;
    protected long MinTimeSpanBetweenCacheRefreshesTicks = TimeSpan.FromMinutes(5.0).Ticks;

    public string DnsDomainName { get; protected set; }

    public string DomainRootPath { get; protected set; }

    public string NetbiosName { get; protected set; }

    public DomainEntryBase(IVssRequestContext requestContext, SecurityIdentifier domainSecurityId)
    {
      this.DomainSecurityId = domainSecurityId;
      this.LockName = requestContext.To(TeamFoundationHostType.Deployment).ServiceHost.CreateUniqueLockName(string.Format("{0}.{1}", (object) "DomainEntry", (object) domainSecurityId));
    }

    public void RecordAccess(IVssRequestContext requestContext)
    {
      if (Interlocked.CompareExchange(ref this.IsInitialized, 1, 0) == 0)
      {
        this.Initialize(requestContext);
      }
      else
      {
        if (DateTime.UtcNow.Ticks - Interlocked.Read(ref this.CacheRefreshTimeStampTicks) <= Interlocked.Read(ref this.MinTimeSpanBetweenCacheRefreshesTicks))
          return;
        using (requestContext.To(TeamFoundationHostType.Deployment).Lock(this.LockName))
        {
          long ticks = DateTime.UtcNow.Ticks;
          if (ticks - this.CacheRefreshTimeStampTicks <= this.MinTimeSpanBetweenCacheRefreshesTicks)
            return;
          this.RefreshCache(ticks);
        }
      }
    }

    protected abstract void Initialize(IVssRequestContext requestContext);

    protected abstract void RefreshCache(long cacheRefreshTimeStampTicks);
  }
}
