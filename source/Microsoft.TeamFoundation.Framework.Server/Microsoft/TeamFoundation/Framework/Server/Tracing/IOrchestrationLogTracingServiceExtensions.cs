// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Tracing.IOrchestrationLogTracingServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server.Tracing
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IOrchestrationLogTracingServiceExtensions
  {
    public static bool TraceOrchestrationLogNewOrchestration(
      this IOrchestrationLogTracingService service,
      IVssRequestContext requestContext,
      Guid orchestrationId,
      long endToEndExecutionTimeThreshold,
      string application,
      string feature)
    {
      return service.TraceOrchestrationLogNewOrchestration(requestContext, orchestrationId.ToString(), endToEndExecutionTimeThreshold, application, feature);
    }

    public static bool TraceOrchestrationLogPhaseStarted(
      this IOrchestrationLogTracingService service,
      IVssRequestContext requestContext,
      Guid orchestrationId,
      long executionTimeThreshold,
      string application,
      string feature,
      string command)
    {
      return service.TraceOrchestrationLogPhaseStarted(requestContext, orchestrationId.ToString(), executionTimeThreshold, application, feature, command);
    }

    public static bool TraceOrchestrationLogCompletion(
      this IOrchestrationLogTracingService service,
      IVssRequestContext requestContext,
      Guid orchestrationId,
      string application,
      string feature,
      string command)
    {
      return service.TraceOrchestrationLogCompletion(requestContext, orchestrationId.ToString(), application, feature, command);
    }

    public static bool TraceOrchestrationLogCompletionWithError(
      this IOrchestrationLogTracingService service,
      IVssRequestContext requestContext,
      Guid orchestrationId,
      string application,
      string feature,
      string command,
      string exceptionType,
      string exceptionMessage,
      bool isExceptionExpected)
    {
      return service.TraceOrchestrationLogCompletionWithError(requestContext, orchestrationId.ToString(), application, feature, command, exceptionType, exceptionMessage, isExceptionExpected);
    }
  }
}
