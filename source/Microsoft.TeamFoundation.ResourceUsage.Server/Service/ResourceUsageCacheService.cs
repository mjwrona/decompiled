// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ResourceUsage.Server.Service.ResourceUsageCacheService
// Assembly: Microsoft.TeamFoundation.ResourceUsage.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55568389-A340-4F60-8DD1-887E0E3F1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ResourceUsage.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ResourceUsage.Server.Service
{
  internal class ResourceUsageCacheService : VssMemoryCacheService<Guid, Dictionary<string, Usage>>
  {
    private static readonly MemoryCacheConfiguration<Guid, Dictionary<string, Usage>> s_configuration = new MemoryCacheConfiguration<Guid, Dictionary<string, Usage>>().WithInactivityInterval(TimeSpan.FromMinutes(60.0)).WithExpiryInterval(TimeSpan.FromMinutes(30.0)).WithCleanupInterval(TimeSpan.FromMinutes(10.0));

    public ResourceUsageCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, ResourceUsageCacheService.s_configuration)
    {
    }
  }
}
