// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class OrchestrationServiceExtensions
  {
    public static void RaiseEvent(
      this OrchestrationService service,
      IVssRequestContext requestContext,
      string hubName,
      string instanceId,
      string eventName,
      object eventData,
      bool ensureOrchestrationExist = false)
    {
      OrchestrationService service1 = service;
      IVssRequestContext requestContext1 = requestContext;
      string hubName1 = hubName;
      OrchestrationInstance instance = new OrchestrationInstance();
      instance.InstanceId = instanceId;
      string eventName1 = eventName;
      object eventData1 = eventData;
      int num = ensureOrchestrationExist ? 1 : 0;
      service1.RaiseEvent(requestContext1, hubName1, instance, eventName1, eventData1, num != 0);
    }

    public static async Task RaiseEventAsync(
      this OrchestrationService service,
      IVssRequestContext requestContext,
      string hubName,
      string instanceId,
      string eventName,
      object eventData,
      bool ensureOrchestrationExist = false)
    {
      OrchestrationService orchestrationService = service;
      IVssRequestContext requestContext1 = requestContext;
      string hubName1 = hubName;
      OrchestrationInstance instance = new OrchestrationInstance();
      instance.InstanceId = instanceId;
      string eventName1 = eventName;
      object eventData1 = eventData;
      int num = ensureOrchestrationExist ? 1 : 0;
      DateTime? fireAt = new DateTime?();
      await orchestrationService.RaiseEventAsync(requestContext1, hubName1, instance, eventName1, eventData1, num != 0, fireAt);
    }

    public static void RaiseEvent(
      this OrchestrationService service,
      IVssRequestContext requestContext,
      string hubName,
      OrchestrationInstance instance,
      string eventName,
      object eventData,
      bool ensureOrchestrationExists = false)
    {
      requestContext.RunSynchronously((Func<Task>) (() => service.RaiseEventAsync(requestContext, hubName, instance, eventName, eventData, ensureOrchestrationExists, new DateTime?())));
    }

    public static void TerminateOrchestrationInstance(
      this OrchestrationService service,
      IVssRequestContext requestContext,
      string hubName,
      string instanceId)
    {
      requestContext.RunSynchronously((Func<Task>) (() => service.TerminateOrchestrationInstanceAsync(requestContext, hubName, instanceId, string.Empty)));
    }

    public static void TerminateOrchestrationInstance(
      this OrchestrationService service,
      IVssRequestContext requestContext,
      string hubName,
      string instanceId,
      string reason)
    {
      requestContext.RunSynchronously((Func<Task>) (() =>
      {
        OrchestrationService orchestrationService = service;
        IVssRequestContext requestContext1 = requestContext;
        string hubName1 = hubName;
        OrchestrationInstance instance = new OrchestrationInstance();
        instance.InstanceId = instanceId;
        string reason1 = reason;
        return orchestrationService.TerminateOrchestrationInstanceAsync(requestContext1, hubName1, instance, reason1);
      }));
    }

    public static void TerminateOrchestrationInstance(
      this OrchestrationService service,
      IVssRequestContext requestContext,
      string hubName,
      OrchestrationInstance instance,
      string reason)
    {
      requestContext.RunSynchronously((Func<Task>) (() => service.TerminateOrchestrationInstanceAsync(requestContext, hubName, instance, reason)));
    }
  }
}
