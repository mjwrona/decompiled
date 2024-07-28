// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IEnvironmentExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [InheritedExport]
  public interface IEnvironmentExtension
  {
    EnvironmentResourceType ResourceType { get; }

    void OnDeleteEnvironment(IVssRequestContext requestContext, Guid projectId, int environmentId);

    IList<EnvironmentResourceReference> PopulateLinkedResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      IList<EnvironmentResourceReference> resources);

    IList<EnvironmentResourceReferenceObject> GetResourcesViewData(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      IList<int> resourceIds);
  }
}
