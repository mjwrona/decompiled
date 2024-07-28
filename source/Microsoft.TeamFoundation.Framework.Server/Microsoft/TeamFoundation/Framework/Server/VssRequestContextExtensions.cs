// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssRequestContextExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.TeamFoundation.Framework.Server.SmartRouterExtensions;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class VssRequestContextExtensions
  {
    private static Lazy<VssHttpClientOptions> s_eventualConsistencyHttpClientOptions = new Lazy<VssHttpClientOptions>((Func<VssHttpClientOptions>) (() => new VssHttpClientOptions()
    {
      ReadConsistencyLevel = new VssReadConsistencyLevel?(VssReadConsistencyLevel.Eventual)
    }));
    private const int c_chunkLen = 8000;
    private const string s_Area = "VssRequestContextExtensions";
    private const string s_Layer = "HostManagement";
    private const string c_performReadIdentityDataValidation = "VisualStudio.Services.Identity.PerformRequestContextReadIdentityDataIntegrityValidation";
    private const string s_unknownCommand = "Unknown Command";

    public static bool IsCanceled(this IVssRequestContext requestContext) => requestContext.IsCanceled;

    public static IVssRequestContext ToDeploymentHostContext(this IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment);

    public static VssRequestContextHolder ToCollection(
      this IVssRequestContext requestContext,
      Guid hostId)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ServiceHost.InstanceId == hostId)
        return new VssRequestContextHolder(requestContext, false);
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext != null && rootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && rootContext.ServiceHost.InstanceId == hostId)
        return new VssRequestContextHolder(requestContext.IsSystemContext ? rootContext.Elevate() : rootContext, false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new VssRequestContextHolder(vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, hostId, VssRequestContextExtensions.GeRequestContextType(requestContext)), true);
    }

    private static RequestContextType GeRequestContextType(IVssRequestContext requestContext)
    {
      if (requestContext.IsUserContext)
        return RequestContextType.UserContext;
      return requestContext.IsServicingContext ? RequestContextType.ServicingContext : RequestContextType.SystemContext;
    }

    internal static void ThrowIfCanceled(this IVssRequestContext requestContext) => requestContext.RequestContextInternal().CheckCanceled();

    internal static IRequestContextInternal RequestContextInternal(
      this IVssRequestContext requestContext,
      bool throwIfNull = true)
    {
      IRequestContextInternal requestContextInternal = requestContext as IRequestContextInternal;
      return !(requestContextInternal == null & throwIfNull) ? requestContextInternal : throw new InvalidCastException("Attempt to cast IVssRequestContext to IRequestContextInternal failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test.");
    }

    public static bool IsVirtualServiceHost(this IVssRequestContext requestContext) => requestContext.ServiceHost.ServiceHostInternal().IsVirtualServiceHost;

    public static Guid ServiceInstanceType(this IVssRequestContext requestContext) => requestContext.ServiceHost.DeploymentServiceHost.ServiceInstanceType;

    public static Guid ServiceInstanceId(this IVssRequestContext requestContext) => requestContext.ServiceHost.DeploymentServiceHost.InstanceId;

    public static TComponent CreateComponent<TComponent>(this IVssRequestContext context) where TComponent : class, ISqlResourceComponent, new() => context.SqlComponentCreator.CreateComponent<TComponent>(context);

    public static TComponent CreateComponent<TComponent>(
      this IVssRequestContext context,
      string databaseCategory = null,
      DatabaseConnectionType? connectionType = null,
      ITFLogger logger = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      return context.SqlComponentCreator.CreateComponent<TComponent>(context, databaseCategory, new Guid?(Guid.Empty), connectionType, logger);
    }

    public static TComponent CreateComponent<TComponent>(
      this IVssRequestContext context,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      DatabaseConnectionType? connectionType = null,
      ITFLogger logger = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      return context.SqlComponentCreator.CreateComponent<TComponent>(context, dataspaceCategory, new Guid?(dataspaceIdentifier), connectionType, logger);
    }

    public static TInterface CreateComponent<TInterface, TComponent>(this IVssRequestContext context) where TComponent : class, ISqlResourceComponent, TInterface, new() => (TInterface) context.CreateComponent<TComponent>();

    public static TInterface CreateComponent<TInterface, TComponent>(
      this IVssRequestContext context,
      string databaseCategory)
      where TComponent : class, ISqlResourceComponent, TInterface, new()
    {
      return (TInterface) context.CreateComponent<TComponent>(databaseCategory);
    }

    public static TComponent CreateReadReplicaAwareComponent<TComponent>(
      this IVssRequestContext requestContext,
      string serviceName,
      string databaseCategory = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      DatabaseConnectionType? nullable1 = requestContext.GetService<IVssDatabaseReadReplicaSettingsService>().IsReadReplicaEnabled(requestContext, serviceName) ? new DatabaseConnectionType?(DatabaseConnectionType.IntentReadOnly) : new DatabaseConnectionType?();
      IVssRequestContext context = requestContext;
      DatabaseConnectionType? nullable2 = nullable1;
      string databaseCategory1 = databaseCategory;
      DatabaseConnectionType? connectionType = nullable2;
      return context.CreateComponent<TComponent>(databaseCategory1, connectionType);
    }

    public static TService GetService<TService>(this IVssRequestContext context) where TService : class, IVssFrameworkService => context.ServiceProvider.GetService<TService>(context);

    public static TService GetService<TService>(
      this IVssRequestContext context,
      Func<IVssRequestContext, TService> factory)
      where TService : class, IVssFrameworkService
    {
      return context.ServiceProvider.GetService<TService>(context, factory);
    }

    public static TService GetParentService<TService>(
      this IVssRequestContext context,
      bool throwIfNoParent = false)
      where TService : class, IVssFrameworkService
    {
      TService parentService = default (TService);
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Parent);
      if (context1 != null)
        parentService = context1.GetService<TService>();
      else if (throwIfNoParent)
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
      return parentService;
    }

    public static TClient GetClient<TClient>(
      this IVssRequestContext context,
      VssHttpClientOptions httpClientOptions = null)
      where TClient : class, IVssHttpClient
    {
      if (httpClientOptions == null)
        httpClientOptions = context.GetHttpClientOptions();
      return context.ClientProvider.GetClient<TClient>(context, httpClientOptions);
    }

    public static TClient GetClient<TClient>(
      this IVssRequestContext context,
      Guid serviceAreaId,
      VssHttpClientOptions httpClientOptions = null)
      where TClient : class, IVssHttpClient
    {
      return context.GetClient<TClient>(serviceAreaId, Guid.Empty, httpClientOptions);
    }

    public static TClient GetClient<TClient>(
      this IVssRequestContext context,
      Guid serviceAreaId,
      Guid serviceIdentifier,
      VssHttpClientOptions httpClientOptions = null)
      where TClient : class, IVssHttpClient
    {
      if (serviceIdentifier == context.ServiceHost.InstanceId)
        serviceIdentifier = Guid.Empty;
      if (httpClientOptions == null)
        httpClientOptions = context.GetHttpClientOptions();
      return context.ClientProvider.GetClient<TClient>(context, serviceAreaId, serviceIdentifier, httpClientOptions);
    }

    public static VssHttpClientOptions GetHttpClientOptionsForEventualReadConsistencyLevel(
      this IVssRequestContext requestContext,
      string featureFlagName)
    {
      return requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled(featureFlagName) ? VssRequestContextExtensions.s_eventualConsistencyHttpClientOptions.Value : (VssHttpClientOptions) null;
    }

    public static VssHttpClientOptions GetHttpClientOptions(this IVssRequestContext context)
    {
      VssHttpClientOptions httpClientOptions = (VssHttpClientOptions) null;
      if (context.IsFeatureEnabled(FrameworkServerConstants.EnableHttpClientEventualReadConsistencyForwarding) && context.HasEventualReadConsistencyLevel())
      {
        httpClientOptions = context.GetHttpClientOptionsForEventualReadConsistencyLevel(FrameworkServerConstants.EnableHttpClientEventualReadConsistencyForwarding);
        context.Trace(2145658, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", "Initialized VssHttpClientOptions with VssReadConsistencyLevel.Eventual");
      }
      return httpClientOptions;
    }

    private static bool HasEventualReadConsistencyLevel(this IVssRequestContext context)
    {
      bool flag = false;
      if (!context.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        VssReadConsistencyLevel consistencyLevel;
        int num1 = context.Items.TryGetValue<VssReadConsistencyLevel>("ReadConsistencyLevel", out consistencyLevel) ? 1 : 0;
        int num2;
        if (num1 == 0 && context.IsFeatureEnabled(FrameworkServerConstants.ReadReplicaOptInElevatedFeatureFlag))
        {
          int hostType1 = (int) context.ServiceHost.HostType;
          TeamFoundationHostType? hostType2 = context.RootContext?.ServiceHost.HostType;
          int valueOrDefault = (int) hostType2.GetValueOrDefault();
          if (hostType1 == valueOrDefault & hostType2.HasValue)
          {
            IVssRequestContext rootContext = context.RootContext;
            bool? nullable;
            if (rootContext == null)
            {
              nullable = new bool?();
            }
            else
            {
              IDictionary<string, object> items = rootContext.Items;
              nullable = items != null ? new bool?(items.TryGetValue<VssReadConsistencyLevel>("ReadConsistencyLevel", out consistencyLevel)) : new bool?();
            }
            num2 = nullable.GetValueOrDefault() ? 1 : 0;
            goto label_8;
          }
        }
        num2 = 0;
label_8:
        if ((num1 | (num2 != 0 ? 1 : 0)) != 0 && consistencyLevel == VssReadConsistencyLevel.Eventual)
          flag = true;
      }
      return flag;
    }

    public static bool IsTracing(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags = null)
    {
      return requestContext.RequestTracer.IsTracing(tracepoint, level, area, layer, tags);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Exception exception)
    {
      requestContext.RequestTracer.TraceException(tracepoint, TraceLevel.Error, area, layer, exception);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Exception exception)
    {
      requestContext.RequestTracer.TraceException(tracepoint, level, area, layer, exception);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Exception exception,
      string format,
      params object[] args)
    {
      requestContext.RequestTracer.TraceException(tracepoint, level, area, layer, exception, format, args);
    }

    public static void TraceCatch(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Exception exception)
    {
      requestContext.TraceCatch(tracepoint, TraceLevel.Verbose, area, layer, exception);
    }

    public static void TraceCatch(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Exception exception)
    {
      if (exception == null)
        return;
      requestContext.RequestTracer.TraceException(tracepoint, level, area, layer, exception, "Exception caught and handled: {0}", (object) exception);
    }

    public static TException TraceThrow<TException>(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      TException exception)
      where TException : Exception
    {
      requestContext.TraceThrow<Exception>(tracepoint, TraceLevel.Warning, area, layer, (Exception) exception);
      return exception;
    }

    public static TException TraceThrow<TException>(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      TException exception)
      where TException : Exception
    {
      requestContext.RequestTracer.TraceException(tracepoint, level, area, layer, (Exception) exception, "Exception thrown: {0}", (object) exception);
      return exception;
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      [CallerMemberName] string methodName = null)
    {
      requestContext.RequestTracer.TraceEnter(tracepoint, area, layer, methodName);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      [CallerMemberName] string methodName = null)
    {
      requestContext.RequestTracer.TraceLeave(tracepoint, area, layer, methodName);
    }

    public static void Trace(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message)
    {
      requestContext.RequestTracer.Trace(tracepoint, level, area, layer, (string[]) null, message);
    }

    public static void Trace(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      object arg0)
    {
      requestContext.RequestTracer.Trace(tracepoint, level, area, layer, (string[]) null, message, arg0);
    }

    public static void Trace(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      object arg0,
      object arg1)
    {
      requestContext.RequestTracer.Trace(tracepoint, level, area, layer, (string[]) null, message, arg0, arg1);
    }

    public static void Trace(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      object arg0,
      object arg1,
      object arg2)
    {
      requestContext.RequestTracer.Trace(tracepoint, level, area, layer, (string[]) null, message, arg0, arg1, arg2);
    }

    public static void Trace(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      requestContext.RequestTracer.Trace(tracepoint, level, area, layer, (string[]) null, format, args);
    }

    public static void Trace(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      params object[] args)
    {
      requestContext.RequestTracer.Trace(tracepoint, level, area, layer, tags, format, args);
    }

    public static void TraceAlways(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      requestContext.RequestTracer.TraceAlways(tracepoint, level, area, layer, (string[]) null, format, args);
    }

    internal static void TraceAlwaysInChunks(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      StringBuilder stringBuilder)
    {
      if (stringBuilder.Length <= 8192)
      {
        requestContext.TraceAlways(tracepoint, level, area, layer, stringBuilder.ToString());
      }
      else
      {
        for (int startIndex = 0; startIndex < stringBuilder.Length; startIndex += 8000)
        {
          string str = stringBuilder.ToString(startIndex, Math.Min(8000, stringBuilder.Length - startIndex));
          requestContext.TraceAlways(tracepoint, level, area, layer, "(Start Index {0}) {1}", (object) startIndex.ToString(), (object) str);
        }
      }
    }

    public static void TraceAlways(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      params object[] args)
    {
      requestContext.RequestTracer.TraceAlways(tracepoint, level, area, layer, tags, format, args);
    }

    [DebuggerStepThrough]
    public static void TraceBlock(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      string area,
      string layer,
      string methodName,
      Action action)
    {
      requestContext.TraceBlock(enterTracepoint, leaveTracepoint, leaveTracepoint, area, layer, methodName, action);
    }

    [DebuggerStepThrough]
    public static void TraceBlock(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      int exceptionTracepoint,
      string area,
      string layer,
      string methodName,
      Action action)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Action>(action, nameof (action));
      requestContext.TraceEnter(enterTracepoint, area, layer, methodName);
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        action();
        requestContext.TraceLeave(leaveTracepoint, area, layer, methodName);
        stopwatch.Stop();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exceptionTracepoint, area, layer, ex);
        throw;
      }
      finally
      {
        requestContext.RequestTimer.LastTracedBlockSpan = stopwatch.Elapsed;
      }
    }

    [DebuggerStepThrough]
    public static void TraceBlock(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      int exceptionTracepoint,
      string area,
      string layer,
      Action action,
      [CallerMemberName] string methodName = null)
    {
      requestContext.TraceBlock(enterTracepoint, leaveTracepoint, exceptionTracepoint, area, layer, methodName, action);
    }

    [DebuggerStepThrough]
    public static T TraceBlock<T>(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      string area,
      string layer,
      string methodName,
      Func<T> action)
    {
      return requestContext.TraceBlock<T>(enterTracepoint, leaveTracepoint, leaveTracepoint, area, layer, methodName, action);
    }

    [DebuggerStepThrough]
    public static T TraceBlock<T>(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      int exceptionTracepoint,
      string area,
      string layer,
      string methodName,
      Func<T> action)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<T>>(action, nameof (action));
      requestContext.TraceEnter(enterTracepoint, area, layer, methodName);
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        T obj = action();
        requestContext.TraceLeave(leaveTracepoint, area, layer, methodName);
        stopwatch.Stop();
        return obj;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exceptionTracepoint, area, layer, ex);
        throw;
      }
      finally
      {
        requestContext.RequestTimer.LastTracedBlockSpan = stopwatch.Elapsed;
      }
    }

    [DebuggerStepThrough]
    public static T TraceBlock<T>(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      int exceptionTracepoint,
      string area,
      string layer,
      Func<T> action,
      [CallerMemberName] string methodName = null)
    {
      return requestContext.TraceBlock<T>(enterTracepoint, leaveTracepoint, exceptionTracepoint, area, layer, methodName, action);
    }

    [DebuggerStepThrough]
    public static IDisposable TraceBlock(
      this ITraceRequest traceRequest,
      int startTracepoint,
      int endTracepoint,
      string area,
      string layer,
      [CallerMemberName] string methodName = null)
    {
      ArgumentUtility.CheckForNull<ITraceRequest>(traceRequest, nameof (traceRequest));
      traceRequest.TraceEnter(startTracepoint, area, layer, methodName);
      return (IDisposable) new VssRequestContextExtensions.TraceBlockDisposable(traceRequest, endTracepoint, area, layer, methodName);
    }

    [DebuggerStepThrough]
    public static IDisposable TraceBlock(
      this IVssRequestContext requestContext,
      int startTracepoint,
      int endTracepoint,
      string area,
      string layer,
      [CallerMemberName] string methodName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.RequestTracer.TraceBlock(startTracepoint, endTracepoint, area, layer, methodName);
    }

    public static async Task TraceBlockAsync(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      int exceptionTracepoint,
      string area,
      string layer,
      Func<CancellationToken, Task> action,
      [CallerMemberName] string methodName = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<CancellationToken, Task>>(action, nameof (action));
      requestContext.TraceEnter(enterTracepoint, area, layer, methodName);
      Stopwatch sw = Stopwatch.StartNew();
      try
      {
        await action(cancellationToken);
        requestContext.TraceLeave(leaveTracepoint, area, layer, methodName);
        sw.Stop();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exceptionTracepoint, area, layer, ex);
        throw;
      }
      finally
      {
        requestContext.RequestTimer.LastTracedBlockSpan = sw.Elapsed;
      }
      sw = (Stopwatch) null;
    }

    public static async Task TraceBlockAsync(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      int exceptionTracepoint,
      string area,
      string layer,
      Func<Task> action,
      [CallerMemberName] string methodName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<Task>>(action, nameof (action));
      requestContext.TraceEnter(enterTracepoint, area, layer, methodName);
      Stopwatch sw = Stopwatch.StartNew();
      try
      {
        await action();
        requestContext.TraceLeave(leaveTracepoint, area, layer, methodName);
        sw.Stop();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exceptionTracepoint, area, layer, ex);
        throw;
      }
      finally
      {
        requestContext.RequestTimer.LastTracedBlockSpan = sw.Elapsed;
      }
      sw = (Stopwatch) null;
    }

    public static async Task<T> TraceBlockAsync<T>(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      int exceptionTracepoint,
      string area,
      string layer,
      Func<CancellationToken, Task<T>> action,
      [CallerMemberName] string methodName = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<CancellationToken, Task<T>>>(action, nameof (action));
      requestContext.TraceEnter(enterTracepoint, area, layer, methodName);
      Stopwatch sw = Stopwatch.StartNew();
      T obj1;
      try
      {
        T obj2 = await action(cancellationToken);
        requestContext.TraceLeave(leaveTracepoint, area, layer, methodName);
        sw.Stop();
        obj1 = obj2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exceptionTracepoint, area, layer, ex);
        throw;
      }
      finally
      {
        requestContext.RequestTimer.LastTracedBlockSpan = sw.Elapsed;
      }
      sw = (Stopwatch) null;
      return obj1;
    }

    public static async Task<T> TraceBlockAsync<T>(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      int exceptionTracepoint,
      string area,
      string layer,
      Func<Task<T>> action,
      [CallerMemberName] string methodName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<Task<T>>>(action, nameof (action));
      requestContext.TraceEnter(enterTracepoint, area, layer, methodName);
      Stopwatch sw = Stopwatch.StartNew();
      T obj1;
      try
      {
        T obj2 = await action();
        requestContext.TraceLeave(leaveTracepoint, area, layer, methodName);
        sw.Stop();
        obj1 = obj2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exceptionTracepoint, area, layer, ex);
        throw;
      }
      finally
      {
        requestContext.RequestTimer.LastTracedBlockSpan = sw.Elapsed;
      }
      sw = (Stopwatch) null;
      return obj1;
    }

    public static void TraceConditionally(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<string> message)
    {
      requestContext.RequestTracer.TraceConditionally(tracepoint, level, area, layer, message);
    }

    public static void TraceConditionally(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      Func<string> message)
    {
      requestContext.RequestTracer.TraceConditionally(tracepoint, level, area, layer, tags, message);
    }

    public static void TraceSerializedConditionally(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      requestContext.RequestTracer.TraceSerializedConditionally(tracepoint, level, area, layer, true, format, args);
    }

    public static void TraceSerializedConditionally(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      bool includeStackTrace,
      string format,
      params object[] args)
    {
      ITraceRequest requestTracer = requestContext.RequestTracer;
      int tracepoint1 = tracepoint;
      int level1 = (int) level;
      string area1 = area;
      string layer1 = layer;
      string str = format;
      int num = includeStackTrace ? 1 : 0;
      string format1 = str;
      object[] objArray = args;
      requestTracer.TraceSerializedConditionally(tracepoint1, (TraceLevel) level1, area1, layer1, num != 0, format1, objArray);
    }

    public static void TraceDataConditionally(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string traceMessage,
      Func<object> traceData = null,
      [CallerMemberName] string methodName = null)
    {
      requestContext.RequestTracer.TraceDataConditionally(tracepoint, level, area, layer, traceMessage, traceData, methodName);
    }

    public static void TraceEnumerableConditionally<T>(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string traceMessage,
      IEnumerable<T> traceEnumerable,
      bool isContentTracing = true,
      Func<object> traceAdditionalData = null,
      int batchSize = 10,
      [CallerMemberName] string methodName = null)
    {
      if (!requestContext.IsTracing(tracepoint, level, area, layer))
        return;
      if (traceEnumerable == null)
      {
        TraceNullEnumerable();
      }
      else
      {
        if (!(traceEnumerable is IReadOnlyList<T> objList1))
          objList1 = (IReadOnlyList<T>) traceEnumerable.ToList<T>();
        IReadOnlyList<T> objList2 = objList1;
        if (!objList2.Any<T>())
          TraceEmptyEnumerable();
        else if (isContentTracing)
          TraceEnumerableContents(objList2);
        else
          TraceEnumerableCount(objList2.Count);
      }

      void TraceNullEnumerable() => requestContext.TraceDataConditionally(tracepoint, level, area, layer, traceMessage, new Func<object>((object) this, __methodptr(\u003CTraceEnumerableConditionally\u003Eg__GetTraceData\u007C4)), methodName);

      void TraceEmptyEnumerable() => requestContext.TraceDataConditionally(tracepoint, level, area, layer, traceMessage, new Func<object>((object) this, __methodptr(\u003CTraceEnumerableConditionally\u003Eg__GetTraceData\u007C5)), methodName);

      void TraceEnumerableCount(int count)
      {
        requestContext.TraceDataConditionally(tracepoint, level, area, layer, traceMessage, new Func<object>(GetTraceData), methodName);

        object GetTraceData() => traceAdditionalData == null ? (object) new
        {
          count = count
        } : (object) new
        {
          count = count,
          additionalData = traceAdditionalData()
        };
      }

      void TraceEnumerableContents(IReadOnlyList<T> contents)
      {
        int count = contents.Count;
        int lastBatchIndex = (int) Math.Ceiling((double) count / (double) batchSize) - 1;
        Guid batchCorrelationId = Guid.NewGuid();
        int batchIndex = 0;
        foreach (IList<T> objList in contents.Batch<T>(batchSize))
        {
          IList<T> batchValues = objList;
          requestContext.TraceDataConditionally(tracepoint, level, area, layer, traceMessage, new Func<object>(GetTraceData), methodName);
          batchIndex++;

          object GetTraceData() => traceAdditionalData == null ? (object) new
          {
            count = count,
            batchCorrelationId = batchCorrelationId,
            batchIndex = batchIndex,
            lastBatchIndex = lastBatchIndex,
            batchValues = batchValues
          } : (object) new
          {
            count = count,
            batchCorrelationId = batchCorrelationId,
            batchIndex = batchIndex,
            lastBatchIndex = lastBatchIndex,
            batchValues = batchValues,
            additionalData = traceAdditionalData()
          };
        }
      }
    }

    internal static ITraceRequestInternal RequestTracerInternal(
      this ITraceRequest traceRequest,
      bool throwIfNull = true)
    {
      ITraceRequestInternal traceRequestInternal = traceRequest as ITraceRequestInternal;
      return !(traceRequestInternal == null & throwIfNull) ? traceRequestInternal : throw new InvalidCastException("Attempt to cast ITraceRequest to ITraceRequestInternal failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test.");
    }

    internal static TeamFoundationTracingService TracingService(
      this IVssRequestContext requestContext)
    {
      return requestContext.RequestTracer.RequestTracerInternal().TracingService;
    }

    public static IDisposable Lock(this IVssRequestContext context, ILockName lockName) => (IDisposable) context.LockManager.Lock(lockName);

    public static bool IsLockHeld(this IVssRequestContext context, ILockName lockName) => context.LockManager.IsLockHeld(lockName);

    public static NamedLockFrame AcquireReaderLock(
      this IVssRequestContext context,
      ILockName lockName)
    {
      return context.LockManager.AcquireReaderLock(lockName);
    }

    public static bool IsReaderLockHeld(this IVssRequestContext context, ILockName lockName) => context.LockManager.IsReaderLockHeld(lockName);

    public static NamedLockFrame AcquireWriterLock(
      this IVssRequestContext context,
      ILockName lockName)
    {
      return context.LockManager.AcquireWriterLock(lockName);
    }

    public static bool IsWriterLockHeld(this IVssRequestContext context, ILockName lockName) => context.LockManager.IsWriterLockHeld(lockName);

    public static IDisposable AcquireConnectionLock(
      this IVssRequestContext context,
      ConnectionLockNameType type)
    {
      return (IDisposable) context.LockManager.AcquireConnectionLock(type);
    }

    public static IDisposable AcquireExemptionLock(this IVssRequestContext context) => (IDisposable) context.LockManager.AcquireExemptionLock();

    internal static IVssLockManagerInternal LockManagerInternal(
      this IVssRequestContext context,
      bool throwIfNull = true)
    {
      IVssLockManagerInternal lockManager = context.LockManager as IVssLockManagerInternal;
      return !(lockManager == null & throwIfNull) ? lockManager : throw new InvalidCastException("Attempt to cast IVssLockManager to IVssLockManagerInternal failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test.");
    }

    internal static ITimeRequestInternal RequestTimerInternal(
      this ITimeRequest timeRequest,
      bool throwIfNull = true)
    {
      ITimeRequestInternal timeRequestInternal = timeRequest as ITimeRequestInternal;
      return !(timeRequestInternal == null & throwIfNull) ? timeRequestInternal : throw new InvalidCastException("Attempt to cast ITimeRequest to ITimeRequestInternal failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test.");
    }

    public static DateTime StartTime(this IVssRequestContext requestContext) => requestContext.RequestTimer.StartTime;

    public static DateTime EndTime(this IVssRequestContext requestContext) => requestContext.RequestTimer.EndTime;

    public static long DelayTime(this IVssRequestContext requestContext) => requestContext.RequestTimer.DelayTime;

    public static long ConcurrencySemaphoreTime(this IVssRequestContext requestContext) => requestContext.RequestTimer.ConcurrencySemaphoreTime;

    public static long ExecutionTime(this IVssRequestContext requestContext) => requestContext.RequestTimer.ExecutionTime;

    public static long QueueTime(this IVssRequestContext requestContext) => requestContext.RequestTimer.QueueTime;

    public static long TimeToFirstPage(this IVssRequestContext requestContext) => requestContext.RequestTimer.TimeToFirstPage;

    public static void UpdateTimeToFirstPage(this IVssRequestContext requestContext) => requestContext.RequestTimer.SetTimeToFirstPageEnd();

    public static double LastTracedBlockElapsedMilliseconds(this IVssRequestContext requestContext) => requestContext.RequestTimer.LastTracedBlockSpan.TotalMilliseconds;

    public static string GetTraceTimingAsString(this IVssRequestContext requestContext)
    {
      if (!(requestContext.RequestTimer is VssRequestContext.TimeRequest requestTimer))
        return string.Empty;
      KeyValuePair<DiagnosticLocation, int>[] traceTimings = requestTimer.TraceTimings;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < traceTimings.Length; ++index)
      {
        KeyValuePair<DiagnosticLocation, int> keyValuePair = traceTimings[index];
        if (keyValuePair.Key.IsInitialized())
          stringBuilder.AppendFormat("{0}:{1} ", (object) keyValuePair.Key, (object) keyValuePair.Value);
        else
          break;
      }
      return stringBuilder.ToString();
    }

    public static IEnumerable<KeyValuePair<string, int>> GetTraceTimingKeyValueList(
      this IVssRequestContext requestContext)
    {
      if (!(requestContext.RequestTimer is VssRequestContext.TimeRequest requestTimer))
        return (IEnumerable<KeyValuePair<string, int>>) null;
      List<KeyValuePair<string, int>> timingKeyValueList = new List<KeyValuePair<string, int>>();
      foreach (KeyValuePair<DiagnosticLocation, int> traceTiming in requestTimer.TraceTimings)
      {
        DiagnosticLocation key = traceTiming.Key;
        if (key.IsInitialized())
        {
          List<KeyValuePair<string, int>> keyValuePairList = timingKeyValueList;
          key = traceTiming.Key;
          KeyValuePair<string, int> keyValuePair = new KeyValuePair<string, int>(key.ToString(), traceTiming.Value);
          keyValuePairList.Add(keyValuePair);
        }
        else
          break;
      }
      return (IEnumerable<KeyValuePair<string, int>>) timingKeyValueList;
    }

    public static IDisposable CreateTimeToFirstPageExclusionBlock(
      this IVssRequestContext requestContext)
    {
      return requestContext.RequestTimer.CreateTimeToFirstPageExclusionBlock();
    }

    public static string AuthenticationType(this IVssRequestContext requestContext) => requestContext.RootContext is IVssWebRequestContext rootContext ? rootContext.AuthenticationType : string.Empty;

    public static Uri RequestUri(this IVssRequestContext requestContext) => requestContext.RootContext is IVssWebRequestContext rootContext ? rootContext.RequestUri : (Uri) null;

    public static IUrlTracer RequestUriForTracing(this IVssRequestContext requestContext) => requestContext.RootContext is IVssWebRequestContext rootContext ? rootContext.RequestUriForTracing : (IUrlTracer) null;

    public static string RawUrl(this IVssRequestContext requestContext) => requestContext.RootContext is IVssWebRequestContext rootContext ? rootContext.RawUrl : string.Empty;

    public static string RemoteIPAddress(this IVssRequestContext requestContext) => requestContext.RootContext is IVssWebRequestContext rootContext ? rootContext.RemoteIPAddress : string.Empty;

    public static string RemotePort(this IVssRequestContext requestContext) => requestContext.RootContext is IVssWebRequestContext rootContext ? rootContext.RemotePort : string.Empty;

    public static string Command(this IVssRequestContext requestContext) => requestContext.RootContext is IVssWebRequestContext rootContext ? rootContext.Command : string.Empty;

    public static string UniqueAgentIdentifier(this IVssRequestContext requestContext) => requestContext.RootContext is IVssWebRequestContext rootContext ? rootContext.UniqueAgentIdentifier : string.Empty;

    public static string Title(this IVssRequestContext requestContext)
    {
      if (requestContext.RootContext.Method != null)
        return requestContext.RootContext.Method.Name;
      if (!(requestContext is IVssWebRequestContext))
        return string.Empty;
      requestContext.Trace(15040006, TraceLevel.Verbose, nameof (VssRequestContextExtensions), nameof (Title), "IVssWebRequestContext call has been made");
      return "Unknown Command";
    }

    public static string RequestPath(this IVssRequestContext requestContext)
    {
      string str = requestContext.RawUrl();
      int length;
      if ((length = str.IndexOf('?')) != -1)
        str = str.Substring(0, length);
      return str;
    }

    public static string RelativePath(this IVssRequestContext requestContext)
    {
      string rawUrl = requestContext.RequestPath();
      return requestContext.RemoveVirtualDirectory(rawUrl);
    }

    public static string RelativeUrl(this IVssRequestContext requestContext)
    {
      string rawUrl = requestContext.RawUrl();
      return requestContext.RemoveVirtualDirectory(rawUrl);
    }

    public static string HttpMethod(this IVssRequestContext requestContext) => requestContext is IVssWebRequestContext webRequestContext ? webRequestContext.HttpMethod : string.Empty;

    internal static string RemoveVirtualDirectory(
      this IVssRequestContext requestContext,
      string rawUrl)
    {
      string path = requestContext.VirtualPath();
      if (!string.IsNullOrEmpty(rawUrl) && !string.IsNullOrEmpty(path))
      {
        string pathIfNeeded1 = UriUtility.AppendSlashToPathIfNeeded(rawUrl);
        string pathIfNeeded2 = UriUtility.AppendSlashToPathIfNeeded(path);
        string str = pathIfNeeded2;
        if (pathIfNeeded1.StartsWith(str, StringComparison.OrdinalIgnoreCase))
          rawUrl = rawUrl.Substring(pathIfNeeded2.Length - 1);
      }
      return rawUrl;
    }

    public static void PartialResultsReady(this IVssRequestContext requestContext)
    {
      if (!(requestContext.RootContext is IVssWebRequestContext rootContext))
        return;
      rootContext.PartialResultsReady();
    }

    public static void SetSessionValue(
      this IVssRequestContext requestContext,
      string key,
      string value)
    {
      if (!(requestContext.RootContext is IVssWebRequestContext rootContext))
        return;
      rootContext.SetSessionValue(key, value);
    }

    public static bool GetSessionValue(
      this IVssRequestContext requestContext,
      string key,
      out string value)
    {
      value = (string) null;
      return requestContext.RootContext is IVssWebRequestContext rootContext && rootContext.GetSessionValue(key, out value);
    }

    public static bool GetSessionTrackingState(
      this IVssRequestContext requestContext,
      out SessionTrackingState sessionState)
    {
      return requestContext.RootContext.Items.TryGetValue<SessionTrackingState>("VstsSession", out sessionState);
    }

    public static string VirtualPath(this IVssRequestContext requestContext) => (requestContext.ServiceHost.InstanceId == requestContext.RootContext.ServiceHost.InstanceId ? requestContext.RootContext : requestContext) is IVssWebRequestContext webRequestContext && webRequestContext.VirtualPath != null ? webRequestContext.VirtualPath : requestContext.WebApplicationPath();

    public static string WebApplicationPath(this IVssRequestContext requestContext) => requestContext?.RootContext is IVssWebRequestContext rootContext && rootContext.WebApplicationPath != null ? rootContext.WebApplicationPath : VirtualPathUtility.AppendTrailingSlash(UrlHostResolutionService.ApplicationVirtualPath);

    internal static HostRouteContext RouteContext(this IVssRequestContext requestContext)
    {
      HostRouteContext hostRouteContext;
      return requestContext.TryGetItem<HostRouteContext>(RequestContextItemsKeys.ServiceHostRouteContext, out hostRouteContext) ? hostRouteContext : (HostRouteContext) null;
    }

    internal static IWebRequestContextInternal WebRequestContextInternal(
      this IVssRequestContext requestContext,
      bool throwIfNull = true)
    {
      IWebRequestContextInternal requestContextInternal = requestContext as IWebRequestContextInternal;
      return !(requestContextInternal == null & throwIfNull) ? requestContextInternal : throw new InvalidCastException("Attempt to cast IVssRequestContext to IWebRequestContextInternal failed.");
    }

    internal static Microsoft.TeamFoundation.Framework.Server.RequestRestrictions RequestRestrictions(
      this IVssRequestContext requestContext)
    {
      return requestContext.RootContext.WebRequestContextInternal().RequestRestrictions;
    }

    public static bool TryGetItem<T>(
      this IVssRequestContext requestContext,
      string key,
      out T value)
    {
      object obj1 = (object) null;
      if (requestContext.Items.TryGetValue(key, out obj1))
      {
        if (obj1 is T obj2)
        {
          value = obj2;
          return true;
        }
        value = default (T);
        return false;
      }
      value = default (T);
      return false;
    }

    public static AccessIntent GetAccessIntent(
      this IVssRequestContext requestContext,
      string databaseCategory)
    {
      AccessIntent accessIntent;
      return VssRequestContextExtensions.GetAccessIntentCache(requestContext).TryGetValue(databaseCategory, out accessIntent) ? accessIntent : AccessIntent.NotSpecified;
    }

    public static void SetAccessIntent(
      this IVssRequestContext requestContext,
      string databaseCategory,
      AccessIntent accessIntent)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseCategory, nameof (databaseCategory));
      IDictionary<string, AccessIntent> accessIntentCache = VssRequestContextExtensions.GetAccessIntentCache(requestContext);
      AccessIntent accessIntent1;
      if (accessIntentCache.TryGetValue(databaseCategory, out accessIntent1))
      {
        if (accessIntent > accessIntent1)
          accessIntentCache[databaseCategory] = accessIntent;
      }
      else
        accessIntentCache[databaseCategory] = accessIntent;
    }

    private static IDictionary<string, AccessIntent> GetAccessIntentCache(
      IVssRequestContext requestContext)
    {
      IDictionary<string, AccessIntent> accessIntentCache;
      if (!requestContext.RootContext.TryGetItem<IDictionary<string, AccessIntent>>(RequestContextItemsKeys.AccessIntentCache, out accessIntentCache))
      {
        accessIntentCache = (IDictionary<string, AccessIntent>) new Dictionary<string, AccessIntent>();
        requestContext.RootContext.Items[RequestContextItemsKeys.AccessIntentCache] = (object) accessIntentCache;
      }
      return accessIntentCache;
    }

    internal static bool TryGetCachedFeatureAvailability(
      this IVssRequestContext requestContext,
      string featureName,
      out FeatureAvailabilityInformation state)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(featureName, nameof (featureName));
      return VssRequestContextExtensions.GetFeatureAvailabilityCache(requestContext).TryGetValue(featureName, out state);
    }

    internal static void CacheFeatureAvailability(
      this IVssRequestContext requestContext,
      string featureName,
      FeatureAvailabilityInformation state)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(featureName, nameof (featureName));
      VssRequestContextExtensions.GetFeatureAvailabilityCache(requestContext)[featureName] = state;
    }

    private static IDictionary<string, FeatureAvailabilityInformation> GetFeatureAvailabilityCache(
      IVssRequestContext requestContext)
    {
      IDictionary<string, FeatureAvailabilityInformation> availabilityCache;
      if (!requestContext.TryGetItem<IDictionary<string, FeatureAvailabilityInformation>>(RequestContextItemsKeys.FeatureAvailabilityCache, out availabilityCache))
      {
        availabilityCache = (IDictionary<string, FeatureAvailabilityInformation>) new Dictionary<string, FeatureAvailabilityInformation>();
        requestContext.Items[RequestContextItemsKeys.FeatureAvailabilityCache] = (object) availabilityCache;
      }
      return availabilityCache;
    }

    public static T GetExtension<T>(
      this IVssRequestContext requestContext,
      ExtensionLifetime lifetime = ExtensionLifetime.Instance,
      string strategy = null,
      bool throwOnError = false)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssExtensionManagementService>().GetExtension<T>(requestContext, lifetime, strategy, throwOnError);
    }

    public static T GetExtension<T>(
      this IVssRequestContext requestContext,
      Func<T, bool> filter,
      bool throwOnError = false)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssExtensionManagementService>().GetExtension<T>(requestContext, filter, throwOnError);
    }

    public static IDisposableReadOnlyList<T> GetExtensions<T>(
      this IVssRequestContext requestContext,
      ExtensionLifetime lifetime = ExtensionLifetime.Instance,
      string strategy = null,
      bool throwOnError = false)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssExtensionManagementService>().GetExtensions<T>(requestContext, lifetime, strategy, throwOnError);
    }

    public static IDisposableReadOnlyList<T> GetExtensions<T>(
      this IVssRequestContext requestContext,
      Func<T, bool> filter,
      bool throwOnError = false)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssExtensionManagementService>().GetExtensions<T>(requestContext, filter, throwOnError);
    }

    public static void SetClientAccessMapping(
      this IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      requestContext.Items[RequestContextItemsKeys.ClientAccessMapping] = (object) accessMapping;
    }

    internal static ILogRequestInternal RequestLoggerInternal(
      this ILogRequest logger,
      bool throwIfNull = true)
    {
      ILogRequestInternal logRequestInternal = logger as ILogRequestInternal;
      return !(logRequestInternal == null & throwIfNull) ? logRequestInternal : throw new InvalidCastException("Attempt to cast ILogRequest to ILogRequestInternal failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test.");
    }

    public static void LogItem(this IVssRequestContext requestContext, string key, string value) => requestContext.RequestLogger.LogItem(key, value);

    internal static RequestDetails GetBasicRequestDetails(
      this IVssRequestContext requestContext,
      TeamFoundationLoggingLevel loggingLevel = TeamFoundationLoggingLevel.Normal,
      long executionTimeThreshold = 10000000,
      bool isExceptionExpected = false,
      bool canAggregate = true)
    {
      RequestDetails basicRequestDetails = new RequestDetails()
      {
        StartTime = requestContext.StartTime()
      };
      basicRequestDetails.EndTime = basicRequestDetails.StartTime.AddMilliseconds(0.001 * (double) requestContext.ExecutionTime());
      basicRequestDetails.ExecutionTime = requestContext.ExecutionTime();
      basicRequestDetails.DelayTime = requestContext.DelayTime();
      basicRequestDetails.ConcurrencySemaphoreTime = requestContext.ConcurrencySemaphoreTime();
      basicRequestDetails.QueueTime = requestContext.QueueTime();
      basicRequestDetails.Title = requestContext.Title();
      basicRequestDetails.OrchestrationId = requestContext.OrchestrationId;
      basicRequestDetails.UniqueIdentifier = requestContext.UniqueIdentifier;
      basicRequestDetails.AuthenticatedUserName = requestContext.AuthenticatedUserName;
      basicRequestDetails.RemoteIPAddress = requestContext.RemoteIPAddress();
      basicRequestDetails.UserAgent = requestContext.UserAgent;
      basicRequestDetails.Command = requestContext.Command();
      basicRequestDetails.ActivityId = requestContext.ActivityId;
      basicRequestDetails.InstanceId = requestContext.ServiceHost.InstanceId;
      basicRequestDetails.Status = requestContext.Status;
      basicRequestDetails.ServiceName = requestContext.ServiceName;
      basicRequestDetails.DomainUserName = requestContext.DomainUserName;
      basicRequestDetails.Method = requestContext.Method;
      if (loggingLevel == TeamFoundationLoggingLevel.All)
        basicRequestDetails.RecursiveSqlCalls = requestContext.RequestLogger.RequestLoggerInternal().RecursiveSqlCalls;
      basicRequestDetails.Count = 1;
      basicRequestDetails.AuthenticationType = requestContext.AuthenticationType();
      basicRequestDetails.UniqueAgentIdentifier = requestContext.UniqueAgentIdentifier();
      basicRequestDetails.ResponseCode = requestContext.ResponseCode;
      basicRequestDetails.VSID = requestContext.GetUserId();
      basicRequestDetails.IdentityTracingItems = requestContext.GetUserIdentityTracingItemsWithReadIdentityIfNotPresent();
      basicRequestDetails.AuthenticationMechanism = requestContext.GetAuthenticationMechanism();
      basicRequestDetails.E2EId = requestContext.E2EId;
      basicRequestDetails.TimeToFirstPage = requestContext.TimeToFirstPage() == 0L ? basicRequestDetails.ExecutionTime : requestContext.TimeToFirstPage();
      basicRequestDetails.ExecutionTimeThreshold = executionTimeThreshold;
      basicRequestDetails.IsExceptionExpected = isExceptionExpected;
      basicRequestDetails.CanAggregate = canAggregate;
      basicRequestDetails.ActivityStatus = EvaluateActivityStatus(basicRequestDetails.Status, basicRequestDetails.TimeToFirstPage);
      basicRequestDetails.HostStartTime = requestContext.ServiceHost.ServiceHostInternal().StartTime;
      basicRequestDetails.HostType = requestContext.ServiceHost.HostType;
      if ((basicRequestDetails.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        basicRequestDetails.HostType = TeamFoundationHostType.Deployment;
      basicRequestDetails.HostId = requestContext.ServiceHost.InstanceId;
      basicRequestDetails.HostName = requestContext.ServiceHost.Name;
      if (requestContext.FrameworkConnectionInfo != null)
      {
        basicRequestDetails.DatabaseServerName = requestContext.FrameworkConnectionInfo.DataSource;
        basicRequestDetails.DatabaseName = requestContext.FrameworkConnectionInfo.InitialCatalog;
      }
      basicRequestDetails.ParentHostId = requestContext.ServiceHost.ParentServiceHost != null ? requestContext.ServiceHost.ParentServiceHost.InstanceId : Guid.Empty;
      basicRequestDetails.AnonymousIdentifier = requestContext.GetAnonymousIdentifier();
      SessionTrackingState sessionState = (SessionTrackingState) null;
      if (requestContext.GetSessionTrackingState(out sessionState) && sessionState != null)
      {
        basicRequestDetails.PersistentSessionId = sessionState.PersistentSessionId;
        basicRequestDetails.PendingAuthenticationSessionId = sessionState.PendingAuthenticationSessionId;
        basicRequestDetails.CurrentAuthenticationSessionId = sessionState.CurrentAuthenticationSessionId;
      }
      if (requestContext.RequestLogger != null && requestContext.RequestLogger.RequestLoggerInternal() != null)
      {
        ILogRequestInternal logRequestInternal = requestContext.RequestLogger.RequestLoggerInternal();
        basicRequestDetails.LogicalReads = logRequestInternal.LogicalReads;
        basicRequestDetails.PhysicalReads = logRequestInternal.PhysicalReads;
        basicRequestDetails.CpuTime = logRequestInternal.CpuTime;
        basicRequestDetails.ElapsedTime = logRequestInternal.ElapsedTime;
      }
      WellKnownPerformanceTimings performanceTimings = PerformanceTimer.GetWellKnownParsedPerformanceTimings(requestContext);
      basicRequestDetails.SqlExecutionTime = performanceTimings.SqlExecutionTime;
      basicRequestDetails.SqlExecutionCount = performanceTimings.SqlExecutionCount;
      basicRequestDetails.FinalSqlCommandExecutionTime = performanceTimings.FinalSqlCommandExecutionTime;
      basicRequestDetails.SqlRetryExecutionTime = performanceTimings.SqlRetryExecutionTime;
      basicRequestDetails.SqlRetryExecutionCount = performanceTimings.SqlRetryExecutionCount;
      basicRequestDetails.SqlReadOnlyExecutionTime = performanceTimings.SqlReadOnlyExecutionTime;
      basicRequestDetails.SqlReadOnlyExecutionCount = performanceTimings.SqlReadOnlyExecutionCount;
      basicRequestDetails.RedisExecutionTime = performanceTimings.RedisExecutionTime;
      basicRequestDetails.RedisExecutionCount = performanceTimings.RedisExecutionCount;
      basicRequestDetails.AadTokenExecutionTime = performanceTimings.AadTokenExecutionTime;
      basicRequestDetails.AadTokenExecutionCount = performanceTimings.AadTokenExecutionCount;
      basicRequestDetails.AadGraphExecutionTime = performanceTimings.AadGraphExecutionTime;
      basicRequestDetails.AadGraphExecutionCount = performanceTimings.AadGraphExecutionCount;
      basicRequestDetails.BlobStorageExecutionTime = performanceTimings.BlobStorageExecutionTime;
      basicRequestDetails.BlobStorageExecutionCount = performanceTimings.BlobStorageExecutionCount;
      basicRequestDetails.TableStorageExecutionTime = performanceTimings.TableStorageExecutionTime;
      basicRequestDetails.TableStorageExecutionCount = performanceTimings.TableStorageExecutionCount;
      basicRequestDetails.ServiceBusExecutionTime = performanceTimings.ServiceBusExecutionTime;
      basicRequestDetails.ServiceBusExecutionCount = performanceTimings.ServiceBusExecutionCount;
      basicRequestDetails.VssClientExecutionTime = performanceTimings.VssClientExecutionTime;
      basicRequestDetails.VssClientExecutionCount = performanceTimings.VssClientExecutionCount;
      basicRequestDetails.DocDBExecutionCount = performanceTimings.DocDBExecutionCount;
      basicRequestDetails.DocDBExecutionTime = performanceTimings.DocDBExecutionTime;
      basicRequestDetails.DocDBRUsConsumed = performanceTimings.DocDBRUsConsumed;
      object obj;
      if (requestContext.Items.TryGetValue(RequestContextItemsKeys.LogItems, out obj))
        basicRequestDetails.LogItemsObject = obj;
      try
      {
        string feature;
        if (!requestContext.To(TeamFoundationHostType.Deployment).GetService<FeatureMappingService>().TryGetCommandFeatureMapping(requestContext, basicRequestDetails.ServiceName, basicRequestDetails.Title, out feature))
          feature = "Unknown";
        basicRequestDetails.Feature = feature;
      }
      catch (Exception ex)
      {
        requestContext.Trace(15040003, TraceLevel.Error, nameof (VssRequestContextExtensions), nameof (GetBasicRequestDetails), "Caught Exception {0} while calling FeatureMappingService", (object) ex);
        basicRequestDetails.Feature = "Unknown";
      }
      basicRequestDetails.CPUCycles = requestContext.CPUCycles;
      basicRequestDetails.AllocatedBytes = requestContext.AllocatedBytes;
      basicRequestDetails.TSTUs = requestContext.TSTUs;
      string str;
      if (requestContext.RootContext.TryGetItem<string>(RequestContextItemsKeys.ThrottleReason, out str))
        basicRequestDetails.ThrottleReason = str;
      SupportsPublicAccess supportsPublicAccess;
      if (requestContext.RootContext.Items.TryGetValue<SupportsPublicAccess>(RequestContextItemsKeys.SupportsPublicAccess, out supportsPublicAccess))
        basicRequestDetails.SupportsPublicAccess = supportsPublicAccess;
      Guid guid;
      if (requestContext.RootContext.Items.TryGetValue<Guid>(RequestContextItemsKeys.AuthorizationId, out guid))
        basicRequestDetails.AuthorizationId = guid;
      basicRequestDetails.OAuthAppId = requestContext.GetOAuthAppId();
      RequestDetails requestDetails = basicRequestDetails;
      MethodInformation method = requestContext.Method;
      long num = (method != null ? (method.IsLongRunning ? 1 : 0) : 0) != 0 ? -1L : (long) requestContext.RequestTimeout.TotalSeconds;
      requestDetails.MethodInformationTimeout = num;
      basicRequestDetails.PreControllerTime = requestContext.RequestTimer.PreControllerTime;
      basicRequestDetails.ControllerTime = requestContext.RequestTimer.ControllerTime;
      if (basicRequestDetails.PreControllerTime > 0L)
      {
        requestContext.RequestTimer.SetPostControllerTime();
        basicRequestDetails.PostControllerTime = requestContext.RequestTimer.PostControllerTime;
      }
      SmartRouterContext smartRouterContext = requestContext.TryGetSmartRouterContext();
      basicRequestDetails.SmartRouterStatus = smartRouterContext?.Status;
      basicRequestDetails.SmartRouterReason = smartRouterContext?.Reason;
      basicRequestDetails.SmartRouterTarget = smartRouterContext?.TargetServer;
      return basicRequestDetails;

      ActivityStatus EvaluateActivityStatus(Exception ex, long timeToFirstPage)
      {
        if (ex != null && !isExceptionExpected)
          return ActivityStatus.Failed;
        return timeToFirstPage <= executionTimeThreshold ? ActivityStatus.Success : ActivityStatus.Slow;
      }
    }

    public static IVssRequestContext CreateUserContext(
      this IVssRequestContext requestContext,
      IdentityDescriptor user)
    {
      return requestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(requestContext, requestContext.ServiceHost.InstanceId, user);
    }

    public static IVssRequestContext CreateUserContext(
      this IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<IdentityDescriptor> scopes = null)
    {
      ITeamFoundationHostManagementService service = requestContext.GetService<ITeamFoundationHostManagementService>();
      if (scopes == null)
        return service.BeginUserRequest(requestContext, requestContext.ServiceHost.InstanceId, identity);
      IRequestActor requestActor = RequestActor.CreateRequestActor(requestContext, identity.Descriptor, identity.Id);
      requestActor.TryAppendPrincipal(Microsoft.TeamFoundation.Framework.Server.Authorization.SubjectType.Scope, new EvaluationPrincipal(new IdentityDescriptor("System:Scope", "Scope"), (IdentityDescriptor) null, scopes));
      bool flag = false;
      IVssRequestContext childContext = (IVssRequestContext) null;
      try
      {
        IInternalTeamFoundationHostManagementService managementService = service as IInternalTeamFoundationHostManagementService;
        IVssRequestContext requestContext1 = requestContext;
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        List<IRequestActor> actors = new List<IRequestActor>();
        actors.Add(requestActor);
        object[] objArray = Array.Empty<object>();
        childContext = managementService.BeginRequest(requestContext1, instanceId, RequestContextType.UserContext, true, false, (IReadOnlyList<IRequestActor>) actors, HostRequestType.Default, objArray);
        requestContext.LinkCancellation(childContext);
        flag = true;
      }
      finally
      {
        if (!flag && childContext != null)
          childContext.Dispose();
      }
      return childContext;
    }

    public static void RunAsUserContext(
      this IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Action<IVssRequestContext> action)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      using (IVssRequestContext userContext = requestContext.CreateUserContext(identity))
        action(userContext);
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      this IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      return requestContext.ReadIdentity(descriptor, true);
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      this IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool fallbackToIdentityService)
    {
      if (!(descriptor != (IdentityDescriptor) null))
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      bool flag = requestContext.IsTracing(36301, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement");
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      if (requestContext.TryGetItem<Microsoft.VisualStudio.Services.Identity.Identity>(descriptor.ToString(), out identity))
      {
        if (flag)
          requestContext.Trace(36301, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", string.Format("Identity for descriptor {0}, found cached on context with id {1}.", (object) descriptor.ToString(), (object) identity.Id));
        VssRequestContextExtensions.ValidateAndTraceIdentity(requestContext, identity, 1754829, 1754830);
        return identity;
      }
      if (requestContext.RootContext != null && requestContext.ServiceHost.InstanceId == requestContext.RootContext.ServiceHost.InstanceId && requestContext.RootContext.TryGetItem<Microsoft.VisualStudio.Services.Identity.Identity>(descriptor.ToString(), out identity))
      {
        if (flag)
          requestContext.Trace(36301, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", string.Format("Identity for descriptor {0}, found cached on root context with id {1}.", (object) descriptor.ToString(), (object) identity.Id));
        VssRequestContextExtensions.ValidateAndTraceIdentity(requestContext, identity, 1754831, 1754832);
        return identity;
      }
      if (!fallbackToIdentityService)
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (flag)
        requestContext.Trace(36301, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", "Context cache miss for identity descriptor " + descriptor.ToString() + ", calling identity service.");
      if (descriptor == requestContext.UserContext && (requestContext.ServiceHost == null || requestContext.RootContext == null || !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.RootContext.IsDeploymentFallbackIdentityReadAllowed() ? 0 : (!ServicePrincipals.IsServicePrincipal(requestContext, descriptor) ? 1 : 0)) != 0)
      {
        IVssRequestContext rootContext = requestContext.RootContext;
        Microsoft.VisualStudio.Services.Identity.Identity authorizedIdentity = rootContext.GetService<IdentityService>().ReadIdentities(rootContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptor
        }, QueryMembership.None, (IEnumerable<string>) new string[1]
        {
          "UserId"
        }).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (authorizedIdentity != null)
        {
          authorizedIdentity.Id = authorizedIdentity.GetProperty<Guid>("UserId", Guid.Empty);
          if (authorizedIdentity.MasterId != Guid.Empty && authorizedIdentity.MasterId != IdentityConstants.LinkedId)
            authorizedIdentity.MasterId = authorizedIdentity.Id;
          if (flag)
            requestContext.TraceDataConditionally(36301, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", "Identity retrieved from root context", (Func<object>) (() => (object) new
            {
              descriptor = descriptor,
              authorizedIdentity = authorizedIdentity
            }), nameof (ReadIdentity));
          identity = authorizedIdentity;
        }
        else if (flag)
          requestContext.Trace(36301, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", "Identity for descriptor " + descriptor.ToString() + " could not be retrieved from root context");
      }
      if (identity == null)
        identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity != null)
      {
        if (flag)
          requestContext.Trace(36301, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", string.Format("Identity retrieved for {0}, identity id: {1}.", (object) descriptor.ToString(), (object) identity.Id));
        requestContext.Items.Add(descriptor.ToString(), (object) identity);
      }
      else if (flag)
        requestContext.Trace(36301, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", "Identity not found for " + descriptor.ToString() + ", returning null.");
      VssRequestContextExtensions.ValidateAndTraceIdentity(requestContext, identity, 1754833, 1754834);
      return identity;
    }

    private static void ValidateAndTraceIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      int tracePointForAadBacked,
      int tracePointForMsaBacked)
    {
      IdentityHelper.ValidateIdentityTranslation(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      }, tracePointForAadBacked, tracePointForMsaBacked, "VisualStudio.Services.Identity.PerformRequestContextReadIdentityDataIntegrityValidation");
    }

    public static IdentityDescriptor GetAuthenticatedDescriptor(
      this IVssRequestContext requestContext)
    {
      return VssRequestContextExtensions.GetAuthenticatedActor(requestContext)?.Descriptor;
    }

    internal static Guid GetAuthenticatedId(this IVssRequestContext requestContext)
    {
      IRequestActor authenticatedActor = VssRequestContextExtensions.GetAuthenticatedActor(requestContext);
      return authenticatedActor != null ? authenticatedActor.Identifier : Guid.Empty;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetAuthenticatedIdentity(
      this IVssRequestContext requestContext)
    {
      return requestContext.ReadIdentity(requestContext.GetAuthenticatedDescriptor());
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetUserIdentity(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetUserIdentity(true);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetUserIdentity(
      this IVssRequestContext requestContext,
      bool fallbackToIdentityService)
    {
      return requestContext.ReadIdentity(requestContext.UserContext, fallbackToIdentityService);
    }

    public static Guid GetUserId(this IVssRequestContext requestContext, bool resolveToHost = false)
    {
      if (!resolveToHost || requestContext.RequestContextInternal().IsRootContext)
      {
        IRequestActor userActor = requestContext.GetUserActor();
        if (userActor != null)
          return userActor.Identifier;
      }
      else
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (userIdentity != null)
          return userIdentity.Id;
      }
      return Guid.Empty;
    }

    public static Guid GetUserCuid(this IVssRequestContext requestContext)
    {
      IdentityTracingItems identityTracingItems = requestContext.GetUserIdentityTracingItems();
      return identityTracingItems == null ? Guid.Empty : identityTracingItems.Cuid;
    }

    public static Guid GetTenantId(this IVssRequestContext requestContext)
    {
      IdentityTracingItems identityTracingItems = requestContext.GetUserIdentityTracingItems();
      return identityTracingItems == null ? Guid.Empty : identityTracingItems.TenantId;
    }

    public static IdentityTracingItems GetUserIdentityTracingItems(
      this IVssRequestContext requestContext)
    {
      IdentityTracingItems identityTracingItems;
      requestContext.RootContext.TryGetItem<IdentityTracingItems>(RequestContextItemsKeys.IdentityTracingItems, out identityTracingItems);
      return identityTracingItems;
    }

    public static IdentityTracingItems GetUserIdentityTracingItemsWithReadIdentityIfNotPresent(
      this IVssRequestContext requestContext)
    {
      IdentityTracingItems identityIfNotPresent = requestContext.GetUserIdentityTracingItems();
      if (identityIfNotPresent == null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (userIdentity == null)
        {
          identityIfNotPresent = new IdentityTracingItems(Guid.Empty, Guid.Empty, Guid.Empty);
          requestContext.RootContext.Items[RequestContextItemsKeys.IdentityTracingItems] = (object) identityIfNotPresent;
        }
        else
        {
          identityIfNotPresent = requestContext.SetUserIdentityTracingItems(userIdentity);
          requestContext.TraceSerializedConditionally(60077, TraceLevel.Info, nameof (VssRequestContextExtensions), "HostManagement", "Couldn't find identity tracing items on requestContext for {0}", (object) userIdentity);
        }
      }
      return identityIfNotPresent;
    }

    internal static IdentityTracingItems SetUserIdentityTracingItems(
      this IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userContextIdentity)
    {
      try
      {
        IdentityTracingItems identityTracingItems = IdentityHelper.GenerateIdentityTracingItems(requestContext, (IReadOnlyVssIdentity) userContextIdentity);
        requestContext.RootContext.Items[RequestContextItemsKeys.IdentityTracingItems] = (object) identityTracingItems;
        return identityTracingItems;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60074, TraceLevel.Warning, nameof (VssRequestContextExtensions), "HostManagement", ex);
      }
      return (IdentityTracingItems) null;
    }

    public static string GetUserIdentityDisplayName(this IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity == null)
        return string.Empty;
      return !string.IsNullOrEmpty(userIdentity.CustomDisplayName) ? userIdentity.CustomDisplayName : userIdentity.DisplayName;
    }

    public static string GetSecretValueFromStrongBox(
      this IVssRequestContext requestContext,
      string strongBoxDrawerName,
      string strongBoxKeyName)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, strongBoxDrawerName, strongBoxKeyName, false);
      return service.GetString(requestContext, itemInfo);
    }

    public static string GetAuthenticationMechanism(this IVssRequestContext requestContext)
    {
      string str;
      return requestContext.RootContext.TryGetItem<string>(RequestContextItemsKeys.AuthenticationMechanism, out str) ? str : string.Empty;
    }

    public static string GetAuthorizationId(this IVssRequestContext requestContext)
    {
      object obj;
      return requestContext.RootContext.TryGetItem<object>(RequestContextItemsKeys.AuthorizationId, out obj) ? obj.ToString() : (string) null;
    }

    public static Guid GetOAuthAppId(this IVssRequestContext requestContext)
    {
      Guid guid;
      return requestContext.RootContext.TryGetItem<Guid>(RequestContextItemsKeys.OAuthAppId, out guid) ? guid : Guid.Empty;
    }

    [Obsolete("Use GetAuthenticatedDescriptor() instead.", true)]
    public static void GetAuthenticatedIdentity(
      this IVssRequestContext requestContext,
      out IdentityDescriptor descriptor)
    {
      descriptor = requestContext.GetAuthenticatedDescriptor();
    }

    [Obsolete("Use GetUserId instead.", true)]
    public static Guid UserIdentityId(this IVssRequestContext requestContext) => requestContext.GetUserId();

    private static IRequestActor GetAuthenticatedActor(IVssRequestContext requestContext)
    {
      IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
      return actors == null ? (IRequestActor) null : actors.FirstOrDefault<IRequestActor>();
    }

    internal static IRequestActor GetUserActor(this IVssRequestContext requestContext)
    {
      IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
      return actors == null ? (IRequestActor) null : actors.LastOrDefault<IRequestActor>();
    }

    [Obsolete("Use ServicePrincipals.IsServicePrincipal instead.")]
    public static bool IsServicePrincipal(this IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment && ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext);

    [Obsolete("Do not use this method, do a proper security check instead.")]
    public static void CheckServicePrincipal(this IVssRequestContext requestContext)
    {
      if (!requestContext.IsServicePrincipal())
        throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
    }

    public static bool IsCollectionAdministrator(this IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
      IdentityService service = requestContext.GetService<IdentityService>();
      return authenticatedDescriptor != (IdentityDescriptor) null && service.IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, authenticatedDescriptor);
    }

    public static IdentityValidationResult IsValidIdentity(this IVssRequestContext requestContext)
    {
      IRequestContextInternal requestContextInternal = requestContext.RequestContextInternal();
      requestContextInternal.IdentityValidationStatus |= IdentityValidationStatus.DelayedIdentityValidation;
      if (requestContextInternal.IdentityValidationStatus.HasFlag((Enum) IdentityValidationStatus.Validated) || !requestContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation") && (requestContextInternal.IsRootContext || !requestContext.RootContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation")))
        return IdentityValidationResult.Success;
      IdentityValidationResult validationResult = requestContext.To(TeamFoundationHostType.Deployment).GetService<IIdentityValidationService>().ValidateRequestIdentity(requestContext);
      if (!validationResult.IsSuccess)
        return validationResult;
      TeamFoundationApplicationCore.ApplyLicensePrincipals(requestContext);
      TeamFoundationRequestFilterHelper.PostAuthorizeRequest(requestContext);
      return validationResult;
    }

    public static void ValidateIdentity(this IVssRequestContext requestContext)
    {
      IdentityValidationResult validationResult = requestContext.IsValidIdentity();
      if (validationResult.IsSuccess)
        return;
      if (validationResult.Exception != null)
        throw validationResult.Exception;
      throw new InvalidOperationException();
    }

    internal static bool IsPipelineIdentity(this IVssRequestContext requestContext)
    {
      bool flag;
      if (!requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.IsPipelineIdentity, out flag))
      {
        string role;
        flag = IdentityHelper.TryParseFrameworkServiceIdentityDescriptor(requestContext.UserContext, out Guid _, out role, out string _) && string.Equals(role, "Build", StringComparison.OrdinalIgnoreCase);
        requestContext.RootContext.Items.Add(RequestContextItemsKeys.IsPipelineIdentity, (object) flag);
      }
      return flag;
    }

    internal static bool IsProxyIdentity(this IVssRequestContext requestContext)
    {
      bool flag;
      if (!requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.IsProxyIdentity, out flag))
      {
        string role;
        flag = IdentityHelper.TryParseFrameworkServiceIdentityDescriptor(requestContext.UserContext, out Guid _, out role, out string _) && string.Equals(role, "ProxyService", StringComparison.OrdinalIgnoreCase);
        requestContext.RootContext.Items.Add(RequestContextItemsKeys.IsProxyIdentity, (object) flag);
      }
      return flag;
    }

    internal static bool IsAnonymousPrincipal(this IVssRequestContext requestContext)
    {
      if (requestContext.IsSystemContext)
        return false;
      IRequestContextInternal requestContextInternal = requestContext.RequestContextInternal();
      Guid anonymousSubjectId = AnonymousAccessConstants.AnonymousSubjectId;
      ref Guid local1 = ref anonymousSubjectId;
      IReadOnlyList<IRequestActor> actors = requestContextInternal.Actors;
      // ISSUE: variable of a boxed type
      __Boxed<Guid?> local2 = (ValueType) (actors != null ? actors.LastOrDefault<IRequestActor>()?.Identifier : new Guid?());
      return local1.Equals((object) local2);
    }

    internal static bool IsAnonymous(this IVssRequestContext requestContext) => requestContext.UserContext == (IdentityDescriptor) null || requestContext.IsAnonymousPrincipal();

    internal static bool IsRootContextAnonymous(this IVssRequestContext requestContext) => requestContext.RootContext.IsAnonymous();

    internal static bool IsPublicUser(this IVssRequestContext requestContext) => !requestContext.IsSystemContext && requestContext.IsRootContextPublicUser();

    internal static bool IsRootContextPublicUser(this IVssRequestContext requestContext)
    {
      bool flag;
      return requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.IsPublicUser, out flag) && flag;
    }

    internal static void ComputePublicUser(this IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || requestContext.IsAnonymous() || requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.IsPublicUser))
        return;
      requestContext.RootContext.Items[RequestContextItemsKeys.IsPublicUser] = (object) (bool) (ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) ? 0 : (!requestContext.GetService<IdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, requestContext.UserContext) ? 1 : 0));
    }

    internal static void RecomputePublicUser(this IVssRequestContext requestContext)
    {
      requestContext.RootContext.Items.Remove(RequestContextItemsKeys.IsPublicUser);
      requestContext.ComputePublicUser();
    }

    internal static bool IsPublicResourceLicense(this IVssRequestContext requestContext)
    {
      bool flag;
      return requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.IsPublicResourceLicense, out flag) && flag;
    }

    public static void SetLicenseForPublicResource(this IVssRequestContext requestContext)
    {
      if (requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.IsPublicResourceLicense))
        return;
      requestContext.RootContext.Items[RequestContextItemsKeys.IsPublicResourceLicense] = (object) true;
      TeamFoundationApplicationCore.ReplaceStakeholderPrincipal(requestContext);
    }

    internal static bool HasWriteAccess(this IVssRequestContext requestContext)
    {
      if (requestContext.IsAnonymousPrincipal() && !requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.AllowAnonymousWrites))
        return false;
      return !requestContext.IsPublicUser() || requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.AllowPublicUserWrites);
    }

    internal static void CheckWriteAccess(this IVssRequestContext requestContext)
    {
      if (!requestContext.HasWriteAccess())
        throw new UnauthorizedWriteException();
    }

    public static IEnumerable<EvaluationPrincipal> GetUserEvaluationPrincipals(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetUserActor().Principals.Values;
    }

    public static bool IsFeatureEnabled(this IVssRequestContext requestContext, string featureName) => requestContext.IsFeatureEnabled(featureName, FeatureFlagService.UseNewFeatureService);

    public static bool IsFeatureEnabled(
      this IVssRequestContext requestContext,
      string featureName,
      bool useNewFeatureService)
    {
      if (useNewFeatureService)
        return requestContext.GetFeature(featureName).IsFeatureEnabled<IVssRequestContext>(requestContext);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(vssRequestContext, featureName);
    }

    public static VssFeature GetFeature(this IVssRequestContext requestContext, string featureName) => requestContext.GetService<FeatureFlagService>().GetFeature(requestContext, featureName);

    public static void CheckDeploymentRequestContext(this IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
    }

    public static void CheckOrganizationRequestContext(this IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ServiceHost.Is(TeamFoundationHostType.Application))
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
    }

    public static void CheckOrganizationOnlyRequestContext(this IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ServiceHost.IsOnly(TeamFoundationHostType.Application))
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
    }

    public static void CheckProjectCollectionRequestContext(this IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
    }

    public static void CheckProjectCollectionOrOrganizationRequestContext(
      this IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && !context.ServiceHost.IsOnly(TeamFoundationHostType.Application))
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
    }

    public static void CheckServicingRequestContext(this IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      context.CheckDeploymentRequestContext();
      if (!context.IsServicingContext && !context.IsSystemContext)
        throw new UnexpectedRequestContextTypeException(RequestContextType.ServicingContext);
    }

    public static void CheckHostedDeployment(this IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
    }

    public static void CheckOnPremisesDeployment(
      this IVssRequestContext requestContext,
      bool throwInvalidAccessException = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      if (throwInvalidAccessException)
        throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
      throw new InvalidOperationException(FrameworkResources.ServiceAvailableInOnPremTfsOnly());
    }

    public static void CheckSystemRequestContext(this IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.IsSystemContext)
        throw new UnexpectedRequestContextTypeException(RequestContextType.SystemContext);
    }

    public static void CheckServiceHostType(
      this IVssRequestContext requestContext,
      TeamFoundationHostType hostType,
      string paramName = "requestContext")
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, paramName);
      if (hostType == TeamFoundationHostType.Deployment)
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return;
      }
      else if (requestContext.ServiceHost.IsOnly(hostType))
        return;
      string message;
      switch (hostType)
      {
        case TeamFoundationHostType.Deployment:
          message = FrameworkResources.DeploymentHostRequired();
          break;
        case TeamFoundationHostType.Application:
          message = FrameworkResources.ApplicationHostRequired();
          break;
        case TeamFoundationHostType.Application | TeamFoundationHostType.Deployment:
          message = FrameworkResources.DeploymentOrApplicationHostRequired();
          break;
        default:
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      }
      throw new InvalidRequestContextHostException(message);
    }

    public static void CheckServiceHostId(
      this IVssRequestContext requestContext,
      Guid serviceHostId,
      IVssFrameworkService service,
      string paramName = "requestContext")
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, paramName);
      if (serviceHostId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.ServiceRequestContextHostMessage((object) service.GetType().Name, (object) serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    public static AccountEntitlement GetAccountEntitlement(
      this IVssRequestContext requestContext,
      Guid userId)
    {
      AccountEntitlement accountEntitlement;
      return !requestContext.TryGetAccountEntitlement(userId, out accountEntitlement) ? (AccountEntitlement) null : accountEntitlement;
    }

    internal static bool TryGetAccountEntitlement(
      this IVssRequestContext requestContext,
      Guid userId,
      out AccountEntitlement accountEntitlement)
    {
      accountEntitlement = (AccountEntitlement) null;
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext?.ServiceHost == null || rootContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return false;
      string key = VssRequestContextExtensions.GetKey(userId);
      return rootContext.TryGetItem<AccountEntitlement>(key, out accountEntitlement);
    }

    internal static void SetAccountEntitlement(
      this IVssRequestContext requestContext,
      Guid userId,
      AccountEntitlement accountEntitlement)
    {
      string key = VssRequestContextExtensions.GetKey(userId);
      requestContext.RootContext.Items[key] = (object) accountEntitlement;
    }

    private static string GetKey(Guid userId) => "$vss:AccountEntitlement/" + userId.ToString();

    public static string GetAnonymousIdentifier(this IVssRequestContext requestContext)
    {
      object obj;
      return requestContext.Items.TryGetValue(RequestContextItemsKeys.AnonymousIdentifier, out obj) && obj != null ? obj.ToString() : string.Empty;
    }

    internal static void SetAnonymousIdentifier(
      this IVssRequestContext requestContext,
      string anonymousIdentifier)
    {
      requestContext.Items[RequestContextItemsKeys.AnonymousIdentifier] = (object) anonymousIdentifier;
    }

    public static void AssertAsyncExecutionEnabled(this IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.IsProduction && SynchronizationContext.Current == null)
        throw new InvalidOperationException("Asynchronous execution is not enabled for this request context");
    }

    public static IDisposable CreateAsyncTimeOutScope(
      this IVssRequestContext requestContext,
      TimeSpan delay)
    {
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(delay);
      IDisposable second = requestContext.LinkTokenSource(cancellationTokenSource);
      return (IDisposable) new VssRequestContextExtensions.LinkedDisposable((IDisposable) cancellationTokenSource, second);
    }

    public static IDisposable CreateOrchestrationIdScope(
      this IVssRequestContext requestContext,
      string orchestrationId)
    {
      return string.IsNullOrEmpty(orchestrationId) ? (IDisposable) null : (IDisposable) new VssRequestContextExtensions.OrchestrationIdScope(requestContext, orchestrationId);
    }

    public static IDisposable AllowAnonymousWrites(
      this IVssRequestContext requestContext,
      params ISecuredObject[] securedObjects)
    {
      return VssRequestContextExtensions.AllowWrites(requestContext, securedObjects, RequestContextItemsKeys.AllowAnonymousWrites);
    }

    public static IDisposable AllowAnonymousOrPublicUserWrites(
      this IVssRequestContext requestContext,
      params ISecuredObject[] securedObjects)
    {
      return VssRequestContextExtensions.AllowWrites(requestContext, securedObjects, RequestContextItemsKeys.AllowAnonymousWrites, RequestContextItemsKeys.AllowPublicUserWrites);
    }

    public static IDisposable AllowPublicUserWrites(
      this IVssRequestContext requestContext,
      params ISecuredObject[] securedObjects)
    {
      return VssRequestContextExtensions.AllowWrites(requestContext, securedObjects, RequestContextItemsKeys.AllowPublicUserWrites);
    }

    public static void CheckPermissionToReadPublicIdentityInfo(
      this IVssRequestContext requestContext)
    {
      GraphSecurityHelper.CheckPermissionToReadIdentity(requestContext, 1);
    }

    public static void CheckPermissionToReadPersonalIdentityInfo(
      this IVssRequestContext requestContext)
    {
      GraphSecurityHelper.CheckPermissionToReadIdentity(requestContext, 2);
    }

    public static bool HasPermissionToReadPublicIdentityInfo(this IVssRequestContext requestContext) => GraphSecurityHelper.HasPermissionToReadIdentity(requestContext, 1);

    public static bool HasPermissionToReadPersonalIdentityInfo(
      this IVssRequestContext requestContext)
    {
      return GraphSecurityHelper.HasPermissionToReadIdentity(requestContext, 2);
    }

    private static IDisposable AllowWrites(
      IVssRequestContext requestContext,
      ISecuredObject[] securedObjects,
      params string[] keyNames)
    {
      TrackedSecurityCollection securityCollection;
      if (requestContext.RootContext.Items.TryGetValue<TrackedSecurityCollection>(RequestContextItemsKeys.SecurityTracking, out securityCollection) && securedObjects != null)
      {
        foreach (ISecuredObject securedObject in securedObjects)
          securityCollection.Validate(securedObject);
      }
      return (IDisposable) new VssRequestContextExtensions.RequestContextItemReference(requestContext, keyNames);
    }

    public static IDisposable AllowCrossDataspaceAccess(this IVssRequestContext requestContext) => (IDisposable) new VssRequestContextExtensions.RequestContextItemReference(requestContext, new string[1]
    {
      RequestContextItemsKeys.AllowCrossDataspaceAccess
    });

    public static IDisposable SetDisposableContextItem(
      this IVssRequestContext requestContext,
      string key,
      object value)
    {
      return (IDisposable) new VssRequestContextExtensions.RequestContextItemReference(requestContext, key, value);
    }

    public static void LogAuditEvent(
      this IVssRequestContext requestContext,
      string actionId,
      Dictionary<string, object> data,
      Guid targetHostId = default (Guid),
      Guid projectId = default (Guid))
    {
      requestContext.GetService<IAuditLogService>().Log(requestContext, actionId, (IDictionary<string, object>) data, targetHostId, projectId);
    }

    public static void LogAuditEvent(
      this IVssRequestContext requestContext,
      string actionId,
      Dictionary<string, object> data,
      AuditLogContextOverride contextOverride)
    {
      requestContext.GetService<IAuditLogService>().Log(requestContext, actionId, (IDictionary<string, object>) data, contextOverride);
    }

    public static bool IsHostProcessType(
      this IVssRequestContext requestContext,
      HostProcessType processType)
    {
      return requestContext.ServiceHost.DeploymentServiceHost.ProcessType == processType;
    }

    private class TraceBlockDisposable : IDisposable
    {
      private readonly ITraceRequest m_tracer;
      private readonly int m_endTracepoint;
      private readonly string m_area;
      private readonly string m_layer;
      private readonly string m_methodName;
      private bool m_disposed;

      public TraceBlockDisposable(
        ITraceRequest tracer,
        int endTracepoint,
        string area,
        string layer,
        string methodName)
      {
        this.m_tracer = tracer;
        this.m_endTracepoint = endTracepoint;
        this.m_area = area;
        this.m_layer = layer;
        this.m_methodName = methodName;
      }

      public void Dispose()
      {
        if (this.m_disposed)
          return;
        this.m_disposed = true;
        this.m_tracer.TraceLeave(this.m_endTracepoint, this.m_area, this.m_layer, this.m_methodName);
      }
    }

    private struct LinkedDisposable : IDisposable
    {
      private readonly IDisposable m_first;
      private readonly IDisposable m_second;

      public LinkedDisposable(IDisposable first, IDisposable second)
      {
        this.m_first = first;
        this.m_second = second;
      }

      public void Dispose()
      {
        this.m_first.Dispose();
        this.m_second.Dispose();
      }
    }

    private struct OrchestrationIdScope : IDisposable
    {
      private readonly string m_previousOrchestrationId;
      private readonly IRequestContextInternal m_rootRequestContext;

      public OrchestrationIdScope(IVssRequestContext requestContext, string orchestrationId)
      {
        this.m_rootRequestContext = (IRequestContextInternal) null;
        this.m_previousOrchestrationId = (string) null;
        if (string.Equals(orchestrationId, requestContext.OrchestrationId, StringComparison.OrdinalIgnoreCase))
          return;
        IVssRequestContext rootContext = requestContext.RootContext;
        this.m_previousOrchestrationId = rootContext.OrchestrationId;
        this.m_rootRequestContext = rootContext.RequestContextInternal();
        this.m_rootRequestContext.SetOrchestrationId(orchestrationId);
      }

      public void Dispose() => this.m_rootRequestContext?.SetOrchestrationId(this.m_previousOrchestrationId);
    }

    private struct RequestContextItemReference : IDisposable
    {
      private readonly IVssRequestContext m_requestContext;
      private readonly string[] m_keyNames;

      public RequestContextItemReference(
        IVssRequestContext requestContext,
        params string[] keyNames)
      {
        this.m_requestContext = requestContext;
        this.m_keyNames = keyNames;
        foreach (string keyName in this.m_keyNames)
          requestContext.RootContext.Items[keyName] = (object) true;
      }

      public RequestContextItemReference(
        IVssRequestContext requestContext,
        string key,
        object value)
      {
        this.m_requestContext = requestContext;
        this.m_keyNames = new string[1]{ key };
        requestContext.RootContext.Items[key] = value;
      }

      public void Dispose()
      {
        foreach (string keyName in this.m_keyNames)
          this.m_requestContext.RootContext.Items.Remove(keyName);
      }
    }
  }
}
