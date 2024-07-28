// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Tracing.IOrchestrationLogTracingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.Tracing
{
  [DefaultServiceImplementation(typeof (OrchestrationLogTracingService))]
  public interface IOrchestrationLogTracingService : IVssFrameworkService
  {
    bool TraceOrchestrationLogNewOrchestration(
      IVssRequestContext requestContext,
      string orchestrationId,
      long endToEndExecutionTimeThreshold,
      string application,
      string feature);

    bool TraceOrchestrationLogPhaseStarted(
      IVssRequestContext requestContext,
      string orchestrationId,
      long executionTimeThreshold,
      string application,
      string feature,
      string command);

    bool TraceOrchestrationLogCompletion(
      IVssRequestContext requestContext,
      string orchestrationId,
      string application,
      string feature,
      string command);

    bool TraceOrchestrationLogCompletionWithError(
      IVssRequestContext requestContext,
      string orchestrationId,
      string application,
      string feature,
      string command,
      string exceptionType,
      string exceptionMessage,
      bool isExceptionExpected);
  }
}
