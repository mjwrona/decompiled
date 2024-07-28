// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.IEnvironmentResourceComponent`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  public interface IEnvironmentResourceComponent<T> : IDisposable where T : EnvironmentResource
  {
    T AddEnvironmentResource(Guid projectId, T resource);

    T GetEnvironmentResource(Guid projectId, int environmentId, int resourceId);

    IList<T> GetEnvironmentResourcesById(
      Guid projectId,
      int environmentId,
      IEnumerable<int> resourceIds);

    T UpdateEnvironmentResource(Guid projectId, T resource);

    T DeleteEnvironmentResource(Guid projectId, int environmentId, int resourceId, Guid deletedBy);

    IList<T> DeleteEnvironmentResources(Guid projectId, int environmentId, Guid deletedBy);
  }
}
