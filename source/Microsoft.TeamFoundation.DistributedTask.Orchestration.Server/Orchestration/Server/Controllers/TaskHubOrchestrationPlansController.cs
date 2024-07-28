// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubOrchestrationPlansController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "plans", ResourceVersion = 2)]
  public sealed class TaskHubOrchestrationPlansController : TaskHubApiController
  {
    [HttpGet]
    public TaskOrchestrationPlan GetPlan(Guid planId) => this.Hub.GetPlan(this.TfsRequestContext, this.ScopeIdentifier, planId) ?? throw new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) planId));
  }
}
