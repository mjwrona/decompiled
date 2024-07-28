// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.IOrchestrationService
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (OrchestrationService))]
  public interface IOrchestrationService : IVssFrameworkService
  {
    OrchestrationHubDescription CreateHub(
      IVssRequestContext requestContext,
      OrchestrationHubDescription description);

    OrchestrationInstance CreateOrchestrationInstance(
      IVssRequestContext requestContext,
      string hubName,
      string name,
      string version,
      string instanceId,
      object input);

    OrchestrationHubDescription GetHubDescription(IVssRequestContext requestContext, string hubName);

    OrchestrationSession GetOrchestrationSession(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId);

    IList<OrchestrationState> GetOrchestrationState(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId);

    OrchestrationState GetOrchestrationState(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId,
      string executionId);

    IList<OrchestrationState> GetRunningOrchestrationState(
      IVssRequestContext requestContext,
      string hubName);

    OrchestrationState GetRunningOrchestrationState(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId);

    Task RaiseEventAsync(
      IVssRequestContext requestContext,
      string hubName,
      OrchestrationInstance instance,
      string eventName,
      object eventData,
      bool ensureOrchestrationExists = false,
      DateTime? fireAt = null);

    OrchestrationHubDescription RenameHub(
      IVssRequestContext requestContext,
      string hubName,
      string newHubName);

    void RemovePoisonedOrchestrations(
      IVssRequestContext requestContext,
      string hubName,
      IList<string> orchestrationIds,
      TimeSpan? timeout = null);

    void ScheduleDipatcherJobForDeliverableMessages(
      IVssRequestContext requestContext,
      IEnumerable<string> hubNames);

    Task TerminateOrchestrationInstanceAsync(
      IVssRequestContext requestContext,
      string hubName,
      OrchestrationInstance instance,
      string reason);

    Task TerminateOrchestrationInstanceAsync(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId);

    Task TerminateOrchestrationInstanceAsync(
      IVssRequestContext requestContext,
      string hubName,
      string instanceId,
      string reason);
  }
}
