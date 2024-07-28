// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementReleaseEnvironmentCacheService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestManagementReleaseEnvironmentCacheService : 
    VssMemoryCacheService<Guid, List<CachedReleaseEnvironmentData>>,
    ITestManagementReleaseEnvironmentCacheService,
    IVssFrameworkService
  {
    private static MemoryCacheConfiguration<Guid, List<CachedReleaseEnvironmentData>> s_defaultCacheConfiguration = new MemoryCacheConfiguration<Guid, List<CachedReleaseEnvironmentData>>().WithCleanupInterval(ReleaseEnvironmentsCacheConstants.ReleaseEnvironmentsCacheCleanupInterval).WithExpiryInterval(ReleaseEnvironmentsCacheConstants.ReleaseEnvironmentsCacheExpirationInterval).WithInactivityInterval(ReleaseEnvironmentsCacheConstants.ReleaseEnvironmentsCacheExpirationInterval).WithMaxElements(5000);

    public TestManagementReleaseEnvironmentCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, TestManagementReleaseEnvironmentCacheService.s_defaultCacheConfiguration)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      base.ServiceStart(systemRequestContext);
    }

    public bool TryUpdateReleaseEnvironmentCache(
      IVssRequestContext requestContext,
      Guid projectId,
      List<CachedReleaseEnvironmentData> releaseEnvironments)
    {
      if (!(projectId != Guid.Empty) || releaseEnvironments == null || !releaseEnvironments.Any<CachedReleaseEnvironmentData>())
        return false;
      this.Set(requestContext, projectId, releaseEnvironments);
      return true;
    }

    public bool TryGetCachedReleaseEnvironmentData(
      IVssRequestContext requestContext,
      Guid projectId,
      out List<CachedReleaseEnvironmentData> releaseEnvironments)
    {
      releaseEnvironments = new List<CachedReleaseEnvironmentData>();
      List<CachedReleaseEnvironmentData> releaseEnvironmentDataList;
      if (!this.TryGetValue(requestContext, projectId, out releaseEnvironmentDataList))
        return false;
      releaseEnvironments = releaseEnvironmentDataList;
      return true;
    }
  }
}
