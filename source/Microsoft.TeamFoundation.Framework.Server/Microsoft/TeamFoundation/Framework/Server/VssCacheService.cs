// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class VssCacheService : VssCacheBase, IVssFrameworkService
  {
    public static readonly TimeSpan NoCleanup = TimeSpan.Zero;
    private Guid m_serviceHostId;
    private readonly HashSet<TeamFoundationTask> m_cleanupTasks = new HashSet<TeamFoundationTask>();

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.InternalInitialize(systemRequestContext);
      this.ServiceStart(systemRequestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.ServiceEnd(systemRequestContext);
      this.InternalFinalize(systemRequestContext);
    }

    protected virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    protected virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal virtual void InternalInitialize(IVssRequestContext systemRequestContext) => this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;

    internal virtual void InternalFinalize(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationTaskService service = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      lock (this.m_cleanupTasks)
      {
        foreach (TeamFoundationTask cleanupTask in this.m_cleanupTasks)
          service.RemoveTask(this.m_serviceHostId, cleanupTask);
        this.m_cleanupTasks.Clear();
      }
    }

    public ILockName CreateLockName(IVssRequestContext requestContext, string name) => requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) name));

    protected void RegisterCacheServicing<TKey, TValue>(
      IVssRequestContext requestContext,
      IMemoryCacheList<TKey, TValue> cache,
      TimeSpan period)
    {
      if (period == VssCacheService.NoCleanup)
        return;
      TeamFoundationTask task = new TeamFoundationTask((TeamFoundationTaskCallback) ((rq, state) => cache.Sweep()), (object) null, (int) period.TotalMilliseconds);
      lock (this.m_cleanupTasks)
        this.m_cleanupTasks.Add(task);
      try
      {
        requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(this.m_serviceHostId, task);
      }
      catch
      {
        lock (this.m_cleanupTasks)
          this.m_cleanupTasks.Remove(task);
        throw;
      }
    }

    protected void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHostId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.CacheServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }
  }
}
