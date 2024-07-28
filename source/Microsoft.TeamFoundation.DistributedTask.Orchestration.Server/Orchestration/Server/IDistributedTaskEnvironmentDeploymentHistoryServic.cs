// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IDistributedTaskEnvironmentDeploymentHistoryService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (FrameworkEnvironmentDeploymentHistoryService))]
  public interface IDistributedTaskEnvironmentDeploymentHistoryService : IVssFrameworkService
  {
    IList<EnvironmentDeploymentExecutionRecord> QueryEnvironmentDeploymentExecutionRecordsWithFilters(
      IVssRequestContext requestContext,
      int environmentId,
      Guid scopeId,
      int? resourceId,
      string continuationToken,
      int maxRecords = 25);

    IList<TaskOrchestrationOwner> QueryEnvironmentPreviousDeployments(
      IVssRequestContext requestContext,
      int environmentId,
      string planType,
      Guid scopeId,
      int definitionId,
      int ownerId,
      int maxRecords = 500,
      int daysToLookBack = 365);
  }
}
