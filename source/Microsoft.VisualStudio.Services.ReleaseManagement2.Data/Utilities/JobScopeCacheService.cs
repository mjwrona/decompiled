// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.JobScopeCacheService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class JobScopeCacheService : 
    VssMemoryCacheService<string, bool>,
    IJobScopeCacheService,
    IVssFrameworkService
  {
    [StaticSafe("Grandfathered")]
    private static MemoryCacheConfiguration<string, bool> defaultCacheConfiguration = new MemoryCacheConfiguration<string, bool>().WithCleanupInterval(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Constants.JobScopeSettingCacheCleanupInterval).WithExpiryInterval(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Constants.JobScopeSettingCacheExpirationInterval).WithInactivityInterval(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Constants.JobScopeSettingCacheExpirationInterval).WithMaxElements(10000);

    public JobScopeCacheService()
      : base((IEqualityComparer<string>) EqualityComparer<string>.Default, JobScopeCacheService.defaultCacheConfiguration)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext) => base.ServiceStart(systemRequestContext);

    public bool TryGetJobScope(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      out bool jobScopeSetting)
    {
      jobScopeSetting = false;
      if (plan != null)
      {
        string jobScopeCacheKey = JobScopeCacheService.GetJobScopeCacheKey(plan);
        bool flag;
        if (this.TryGetValue(requestContext, jobScopeCacheKey, out flag))
        {
          jobScopeSetting = flag;
          return true;
        }
      }
      return false;
    }

    public void Add(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      bool jobScopeSetting)
    {
      if (plan == null)
        return;
      string jobScopeCacheKey = JobScopeCacheService.GetJobScopeCacheKey(plan);
      this.Set(requestContext, jobScopeCacheKey, jobScopeSetting);
    }

    public bool RemoveCachedValue(IVssRequestContext requestContext, TaskOrchestrationPlan plan)
    {
      if (plan == null)
        return false;
      string jobScopeCacheKey = JobScopeCacheService.GetJobScopeCacheKey(plan);
      return this.Remove(requestContext, jobScopeCacheKey);
    }

    private static string GetJobScopeCacheKey(TaskOrchestrationPlan plan) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.JobScopeCacheKeyFormat, (object) plan.PlanId.ToString(), (object) plan.Owner.Name);
  }
}
