// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectPropertyCacheService
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.Server.Types
{
  internal class ProjectPropertyCacheService : 
    VssMemoryCacheService<ProjectPropertyName, ProjectProperty>
  {
    private static readonly long s_defaultMaxSizeHosted = 1048576;
    private static readonly long s_defaultMaxSizeOnPremises = 5242880;
    private static readonly TimeSpan s_defaultCleanupInterval = TimeSpan.FromMinutes(1.0);
    private static readonly TimeSpan s_defaultInactivityInterval = TimeSpan.FromMinutes(5.0);
    private static readonly TimeSpan s_defaultExpiryInterval = TimeSpan.FromHours(1.0);
    private static readonly MemoryCacheConfiguration<ProjectPropertyName, ProjectProperty> s_defaultCacheConfiguration = new MemoryCacheConfiguration<ProjectPropertyName, ProjectProperty>().WithMaxSize(long.MaxValue, (ISizeProvider<ProjectPropertyName, ProjectProperty>) new ProjectPropertyCacheService.ProjectPropertyCacheSizeProvider()).WithCleanupInterval(ProjectPropertyCacheService.s_defaultCleanupInterval).WithInactivityInterval(ProjectPropertyCacheService.s_defaultInactivityInterval).WithExpiryInterval(ProjectPropertyCacheService.s_defaultExpiryInterval);
    private IVssMemoryCacheGrouping<ProjectPropertyName, ProjectProperty, Guid> m_projectIdGrouping;

    public ProjectPropertyCacheService()
      : base((IEqualityComparer<ProjectPropertyName>) EqualityComparer<ProjectPropertyName>.Default, ProjectPropertyCacheService.s_defaultCacheConfiguration)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      systemRequestContext.CheckProjectCollectionRequestContext();
      this.MaxCacheSize.Value = systemRequestContext.ExecutionEnvironment.IsHostedDeployment ? ProjectPropertyCacheService.s_defaultMaxSizeHosted : ProjectPropertyCacheService.s_defaultMaxSizeOnPremises;
      this.m_projectIdGrouping = VssMemoryCacheGroupingFactory.Create<ProjectPropertyName, ProjectProperty, Guid>(systemRequestContext, this.MemoryCache, (Func<ProjectPropertyName, ProjectProperty, IEnumerable<Guid>>) ((key, value) => (IEnumerable<Guid>) new Guid[1]
      {
        key.ProjectId
      }));
    }

    public IEnumerable<string> GetCachedPropertyNames(Guid projectId)
    {
      IEnumerable<ProjectPropertyName> keys;
      if (this.m_projectIdGrouping.TryGetKeys(projectId, out keys))
      {
        foreach (ProjectPropertyName projectPropertyName in keys)
          yield return projectPropertyName.PropertyName;
      }
    }

    private class ProjectPropertyCacheSizeProvider : 
      ISizeProvider<ProjectPropertyName, ProjectProperty>
    {
      public long GetSize(ProjectPropertyName key, ProjectProperty value)
      {
        long num = 16;
        Encoding unicode = Encoding.Unicode;
        long size = num + (long) unicode.GetByteCount(key.PropertyName);
        if (value != null)
        {
          size += (long) unicode.GetByteCount(value.Name);
          if (value.Value != null)
          {
            if (value.Value is string s)
              size += (long) unicode.GetByteCount(s);
            else
              size += 8L;
          }
        }
        return size;
      }
    }
  }
}
