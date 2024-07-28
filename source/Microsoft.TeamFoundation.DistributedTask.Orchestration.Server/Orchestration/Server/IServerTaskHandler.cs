// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IServerTaskHandler
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [InheritedExport]
  public interface IServerTaskHandler
  {
    string HandlerName { get; }

    ExecuteTaskResponse ExecuteTask(
      IVssRequestContext requestContext,
      ServerTaskExecutionContext executionContext,
      ServerTaskRequestMessage taskRequest);

    Task<ExecuteTaskResponse> ExecuteTaskAsync(
      IVssRequestContext requestContext,
      ServerTaskExecutionContext executionContext,
      ServerTaskRequestMessage taskRequest);

    CancelTaskResponse CancelTask(
      IVssRequestContext requestContext,
      ServerTaskExecutionContext executionContext,
      ServerTaskRequestMessage taskRequest,
      TaskCanceledReasonType reasonType);

    Task<CancelTaskResponse> CancelTaskAsync(
      IVssRequestContext requestContext,
      ServerTaskExecutionContext executionContext,
      ServerTaskRequestMessage taskRequest,
      TaskCanceledReasonType reasonType);

    IList<ServerTaskSectionExecutionOptions> GetTaskSectionExecutionOptions(
      IVssRequestContext requestContext,
      JobEnvironment jobEnvironment,
      TaskDefinition taskDefinition,
      TaskInstance taskInstance);
  }
}
