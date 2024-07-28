// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Tracing.OrchestrationLogTracingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.Tracing
{
  public class OrchestrationLogTracingService : IOrchestrationLogTracingService, IVssFrameworkService
  {
    private const string s_NewOrchestration = "NewOrchestration";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool TraceOrchestrationLogNewOrchestration(
      IVssRequestContext requestContext,
      string orchestrationId,
      long endToEndExecutionTimeThreshold,
      string application,
      string feature)
    {
      DateTime resolutionUtcNow = DateTimeUtility.GetHighResolutionUtcNow();
      return this.TraceOrchestrationLog(requestContext, orchestrationId, resolutionUtcNow, TeamFoundationTracingService.s_zeroDate, endToEndExecutionTimeThreshold, OrchestrationLogOrchestrationStatus.Success, application, feature, "NewOrchestration", (string) null, (string) null, false);
    }

    public bool TraceOrchestrationLogPhaseStarted(
      IVssRequestContext requestContext,
      string orchestrationId,
      long executionTimeThreshold,
      string application,
      string feature,
      string command)
    {
      DateTime resolutionUtcNow = DateTimeUtility.GetHighResolutionUtcNow();
      return this.TraceOrchestrationLog(requestContext, orchestrationId, resolutionUtcNow, TeamFoundationTracingService.s_zeroDate, executionTimeThreshold, OrchestrationLogOrchestrationStatus.Success, application, feature, command, (string) null, (string) null, false);
    }

    public bool TraceOrchestrationLogCompletion(
      IVssRequestContext requestContext,
      string orchestrationId,
      string application,
      string feature,
      string command)
    {
      DateTime resolutionUtcNow = DateTimeUtility.GetHighResolutionUtcNow();
      return this.TraceOrchestrationLog(requestContext, orchestrationId, TeamFoundationTracingService.s_zeroDate, resolutionUtcNow, -1L, OrchestrationLogOrchestrationStatus.Success, application, feature, command, (string) null, (string) null, false);
    }

    public bool TraceOrchestrationLogCompletionWithError(
      IVssRequestContext requestContext,
      string orchestrationId,
      string application,
      string feature,
      string command,
      string exceptionType,
      string exceptionMessage,
      bool isExceptionExpected)
    {
      DateTime resolutionUtcNow = DateTimeUtility.GetHighResolutionUtcNow();
      return this.TraceOrchestrationLog(requestContext, orchestrationId, TeamFoundationTracingService.s_zeroDate, resolutionUtcNow, -1L, OrchestrationLogOrchestrationStatus.Failed, application, feature, command, exceptionType, exceptionMessage, isExceptionExpected);
    }

    private bool TraceOrchestrationLog(
      IVssRequestContext requestContext,
      string orchestrationId,
      DateTime startTime,
      DateTime endTime,
      long executionTimeThreshold,
      OrchestrationLogOrchestrationStatus orchestrationStatus,
      string application,
      string feature,
      string command,
      string exceptionType,
      string exceptionMessage,
      bool isExceptionExpected)
    {
      TeamFoundationHostType hostType = this.GetHostType(requestContext);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      Guid parentHostId = requestContext.ServiceHost.ParentServiceHost != null ? requestContext.ServiceHost.ParentServiceHost.InstanceId : Guid.Empty;
      Guid userId = requestContext.RootContext.GetUserId();
      IdentityTracingItems identityTracingItems = requestContext.RootContext.GetUserIdentityTracingItems();
      string anonymousIdentifier = requestContext.GetAnonymousIdentifier();
      return requestContext.TracingService().TraceOrchestrationLog(orchestrationId, startTime, endTime, executionTimeThreshold, (byte) orchestrationStatus, application, feature, command, exceptionType, exceptionMessage, isExceptionExpected, (byte) hostType, instanceId, parentHostId, userId, identityTracingItems != null ? identityTracingItems.Cuid : Guid.Empty, anonymousIdentifier);
    }

    private TeamFoundationHostType GetHostType(IVssRequestContext requestContext)
    {
      TeamFoundationHostType hostType = requestContext.ServiceHost.HostType;
      if ((hostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        hostType = TeamFoundationHostType.Deployment;
      return hostType;
    }
  }
}
