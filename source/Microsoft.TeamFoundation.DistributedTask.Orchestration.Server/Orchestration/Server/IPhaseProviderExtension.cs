// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IPhaseProviderExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [InheritedExport]
  public interface IPhaseProviderExtension : IPhaseProvider
  {
    Task StartPhaseAsync(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest,
      PhaseExecutionContext phaseExecutionContext);

    Task CancelPhaseAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      string reason);

    Task JobStartedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance job,
      JobStartedEventData data);

    Task JobCompletedAsync(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance job);
  }
}
