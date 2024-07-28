// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeletedAgentSessionCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class DeletedAgentSessionCacheService : 
    VssMemoryCacheService<Guid, TaskAgentSessionData>
  {
    private static readonly MemoryCacheConfiguration<Guid, TaskAgentSessionData> s_defaultConfiguration = new MemoryCacheConfiguration<Guid, TaskAgentSessionData>().WithMaxElements(500).WithCleanupInterval(TimeSpan.FromMinutes(5.0)).WithInactivityInterval(TimeSpan.FromMinutes(1.0));

    public DeletedAgentSessionCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, DeletedAgentSessionCacheService.s_defaultConfiguration)
    {
    }
  }
}
