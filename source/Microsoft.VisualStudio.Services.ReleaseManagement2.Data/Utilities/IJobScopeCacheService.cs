// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.IJobScopeCacheService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  [DefaultServiceImplementation(typeof (JobScopeCacheService))]
  public interface IJobScopeCacheService : IVssFrameworkService
  {
    bool TryGetJobScope(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      out bool jobScopeSetting);

    void Add(IVssRequestContext requestContext, TaskOrchestrationPlan plan, bool jobScopeSetting);

    bool RemoveCachedValue(IVssRequestContext requestContext, TaskOrchestrationPlan plan);
  }
}
