// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssRequestContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IVssRequestContext : IDisposable
  {
    Guid ActivityId { get; }

    CancellationToken CancellationToken { get; }

    IDisposable LinkTokenSource(CancellationTokenSource toLink);

    long ContextId { get; }

    Guid UniqueIdentifier { get; }

    Guid E2EId { get; }

    string OrchestrationId { get; }

    string UserAgent { get; }

    string DomainUserName { get; }

    string AuthenticatedUserName { get; }

    TeamFoundationExecutionEnvironment ExecutionEnvironment { get; }

    ISqlConnectionInfo FrameworkConnectionInfo { get; }

    bool IsSystemContext { get; }

    bool IsServicingContext { get; }

    bool IsUserContext { get; }

    bool IsImpersonating { get; }

    bool IsCanceled { get; }

    bool IsTracked { get; set; }

    IDictionary<string, object> Items { get; }

    MethodInformation Method { get; }

    VssRequestContextPriority RequestPriority { get; }

    TimeSpan RequestTimeout { get; set; }

    int ResponseCode { get; }

    IVssRequestContext RootContext { get; }

    IVssServiceHost ServiceHost { get; }

    string ServiceName { get; set; }

    Exception Status { get; set; }

    IdentityDescriptor UserContext { get; }

    long CPUCycles { get; }

    long AllocatedBytes { get; }

    long CPUCyclesAsync { get; }

    long AllocatedBytesAsync { get; }

    double TSTUs { get; set; }

    RequestDetails GetRequestDetails(
      TeamFoundationLoggingLevel loggingLevel = TeamFoundationLoggingLevel.Normal,
      long executionTimeThreshold = 10000000,
      bool isExceptionExpected = false,
      bool canAggregate = true);

    Guid DataspaceIdentifier { get; }

    void AddDisposableResource(IDisposable disposable);

    [Obsolete("Use CreateUserContext instead.", false)]
    IVssRequestContext CreateImpersonationContext(
      IdentityDescriptor identity,
      RequestContextType? newType = null);

    void Cancel(string reason);

    void Cancel(string reason, HttpStatusCode statusCode);

    void EnterCancelableRegion(ICancelable cancelable);

    void ExitCancelableRegion(ICancelable cancelable);

    void LinkCancellation(IVssRequestContext childContext);

    void EnterMethod(MethodInformation methodInformation);

    void LeaveMethod();

    IVssRequestContext Elevate(bool throwIfShutdown = true);

    IVssRequestContext To(TeamFoundationHostType hostType);

    ISqlComponentCreator SqlComponentCreator { get; }

    IVssFrameworkServiceProvider ServiceProvider { get; }

    IVssHttpClientProvider ClientProvider { get; }

    IVssLockManager LockManager { get; }

    ILogRequest RequestLogger { get; }

    ITraceRequest RequestTracer { get; }

    ITimeRequest RequestTimer { get; }
  }
}
