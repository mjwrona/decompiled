// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostRoutingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostRoutingService : IHostRoutingService, IVssFrameworkService
  {
    internal static readonly string Area = "HostRouting";
    internal static readonly string Layer = nameof (HostRoutingService);
    private static readonly RegistryQuery c_cacheFlushDelayRegistryQuery = (RegistryQuery) "/Configuration/Routing/CacheFlushDelay";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.RoutingCache = (IHostRoutingCache) systemRequestContext.GetService<HostRoutingCacheService>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public (IVssRequestContext requestContext, HostProxyData hostProxyData) BeginRequest(
      IVssRequestContext deploymentContext,
      HttpContextBase httpContext)
    {
      deploymentContext.TraceEnter(6473477, HostRoutingService.Area, HostRoutingService.Layer, nameof (BeginRequest));
      try
      {
        HostRouteContext routeContext = (HostRouteContext) httpContext.Items[(object) HttpContextConstants.ServiceHostRouteContext];
        if (routeContext == null)
          return ((IVssRequestContext) null, (HostProxyData) null);
        if (deploymentContext.ServiceHost.HasDatabaseAccess)
          ArgumentUtility.CheckForEmptyGuid(routeContext.HostId, "routeContext.HostId");
        HostProxyData hostProxyData = (HostProxyData) null;
        HostRoutingService.BeginContextResult beginContextResult = this.BeginLocalContext(deploymentContext, httpContext, routeContext.HostId);
        IVssRequestContext vssRequestContext;
        switch (beginContextResult)
        {
          case HostRoutingService.StartedResult startedResult:
            vssRequestContext = startedResult.Context;
            break;
          case HostRoutingService.NonexistentResult _:
          case HostRoutingService.MigratingResult _:
            if (deploymentContext.ExecutionEnvironment.IsHostedDeployment)
            {
              if (beginContextResult is HostRoutingService.MigratingResult migratingResult && deploymentContext.IsFeatureEnabled("Microsoft.AzureDevOps.Routing.DisableHostMigrationArr"))
                throw migratingResult.Exception;
              vssRequestContext = this.ResolveHostContext(deploymentContext, httpContext, routeContext, out hostProxyData);
              break;
            }
            vssRequestContext = (IVssRequestContext) null;
            break;
          case HostRoutingService.StoppedResult stoppedResult:
            throw stoppedResult.Exception;
          default:
            throw new InvalidOperationException();
        }
        if (deploymentContext.ExecutionEnvironment.IsHostedDeployment)
          deploymentContext.GetService<IAfdRouteKeyService>().SetAfdRouteKeyHeaders(deploymentContext, routeContext, httpContext, (IProxyData) hostProxyData);
        if (vssRequestContext == null)
          return ((IVssRequestContext) null, hostProxyData);
        string header = httpContext.Request.Headers["X-VSS-ClientAccessMapping"];
        if (string.IsNullOrEmpty(header))
        {
          if (routeContext.AccessMappingMonikers != null)
            vssRequestContext.Items.Add(RequestContextItemsKeys.ClientAccessMappingMonikers, (object) routeContext.AccessMappingMonikers);
        }
        else
          vssRequestContext.Items.Add(RequestContextItemsKeys.ClientAccessMappingMonikers, (object) header);
        return (vssRequestContext, (HostProxyData) null);
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5019641, HostRoutingService.Area, HostRoutingService.Layer, ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(4728014, HostRoutingService.Area, HostRoutingService.Layer, nameof (BeginRequest));
      }
    }

    private IVssRequestContext ResolveHostContext(
      IVssRequestContext deploymentContext,
      HttpContextBase httpContext,
      HostRouteContext routeContext,
      out HostProxyData hostProxyData,
      bool flushImsOnLocalHostFailures = true)
    {
      deploymentContext.TraceEnter(2231131, HostRoutingService.Area, HostRoutingService.Layer, nameof (ResolveHostContext));
      try
      {
        hostProxyData = this.RoutingCache.Get(deploymentContext, routeContext.HostId, routeContext.RouteFlags);
        if (hostProxyData == null)
          return (IVssRequestContext) null;
        Guid targetInstanceId = hostProxyData.TargetInstanceId;
        Guid instanceId = deploymentContext.ServiceHost.InstanceId;
        if (targetInstanceId == instanceId)
        {
          deploymentContext.GetService<IHostSyncService>().EnsureHostUpdated(deploymentContext, routeContext.HostId);
          HostRoutingService.BeginContextResult beginContextResult = this.BeginLocalContext(deploymentContext, httpContext, routeContext.HostId);
          switch (beginContextResult)
          {
            case HostRoutingService.StartedResult startedResult:
              return startedResult.Context;
            case HostRoutingService.StoppedResult stoppedResult:
              throw stoppedResult.Exception;
            case HostRoutingService.NonexistentResult _:
            case HostRoutingService.MigratingResult _:
              TimeSpan timeSpan = flushImsOnLocalHostFailures && !deploymentContext.IsFeatureEnabled("VisualStudio.Services.Routing.DisableCacheFlushing") ? deploymentContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(deploymentContext, in HostRoutingService.c_cacheFlushDelayRegistryQuery, TimeSpan.FromMinutes(1.0)) : throw ((HostRoutingService.ExceptionalResult) beginContextResult).Exception;
              if (DateTime.UtcNow - hostProxyData.CreatedUtc < timeSpan)
                throw ((HostRoutingService.ExceptionalResult) beginContextResult).Exception;
              deploymentContext.Trace(112103490, TraceLevel.Error, HostRoutingService.Area, HostRoutingService.Layer, string.Format("Flushing routing and IMS caches for host: {0}", (object) routeContext.HostId));
              deploymentContext.GetService<InstanceManagementService>().FlushLocationServiceData(deploymentContext, routeContext.HostId);
              this.RoutingCache.Remove(deploymentContext, routeContext.HostId);
              return this.ResolveHostContext(deploymentContext, httpContext, routeContext, out hostProxyData, false);
            default:
              throw new InvalidOperationException();
          }
        }
        else
        {
          if (!deploymentContext.IsFeatureEnabled("VisualStudio.Services.Framework.DisableArr"))
            return this.BeginRoutedContext(deploymentContext, httpContext, hostProxyData);
          string header = httpContext.Request.Headers["X-FD-RouteKey"];
          deploymentContext.Trace(111111, TraceLevel.Error, HostRoutingService.Area, HostRoutingService.Layer, string.Format("ARR is disabled so we cannot route this request. Route key: {0}. Current deployment ID: {1}. Target deployment ID: {2}.", (object) header, (object) instanceId, (object) targetInstanceId));
          return (IVssRequestContext) null;
        }
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(4242046, HostRoutingService.Area, HostRoutingService.Layer, ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(5688510, HostRoutingService.Area, HostRoutingService.Layer, nameof (ResolveHostContext));
      }
    }

    private IVssRequestContext BeginRoutedContext(
      IVssRequestContext deploymentContext,
      HttpContextBase httpContext,
      HostProxyData hostProxyData)
    {
      string authority = new Uri(hostProxyData.TargetInstanceUrl).Authority;
      if (!deploymentContext.IsFeatureEnabled("VisualStudio.Services.Routing.DisableArrCircuitBreaker"))
      {
        HostRoutingService.ArrCircuitBreakerRef circuitBreakerRef = HostRoutingService.ArrCircuitBreakerRef.Initialize(deploymentContext, authority);
        httpContext.Items[(object) "ArrCircuitBreakerRef"] = (object) circuitBreakerRef;
      }
      httpContext.Request.ServerVariables[HttpContextConstants.VssRewriteUrl] = new HostRoutingService.AuthorityUriBuilder(httpContext.Request.Url)
      {
        Authority = authority
      }.Uri.AbsoluteUri;
      httpContext.Response.Headers.Set("X-VSS-RequestRouted", authority);
      httpContext.Items[(object) HttpContextConstants.ArrRequestRouted] = (object) true;
      httpContext.Request.Headers.Set("X-VSS-RequestRouted", bool.TrueString);
      httpContext.Items[(object) HttpContextConstants.ServiceHostRouteContext] = (object) new HostRouteContext()
      {
        HostId = deploymentContext.ServiceHost.InstanceId,
        VirtualPath = UrlHostResolutionService.ApplicationVirtualPath,
        RouteFlags = RouteFlags.DeploymentHost
      };
      HostRoutingService.BeginContextResult beginContextResult = this.BeginLocalContext(deploymentContext, httpContext, deploymentContext.ServiceHost.InstanceId);
      IVssRequestContext vssRequestContext = !(beginContextResult is HostRoutingService.ExceptionalResult exceptionalResult) ? ((HostRoutingService.StartedResult) beginContextResult).Context : throw exceptionalResult.Exception;
      HostRoutingService.SetSessionHeaders(httpContext, vssRequestContext);
      HttpCookie cookie1 = httpContext.Request.Cookies["X-VSS-UseRequestRouting"];
      if (cookie1 == null || !cookie1.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
      {
        TeamFoundationAuthenticationService service = vssRequestContext.GetService<ITeamFoundationAuthenticationService>() as TeamFoundationAuthenticationService;
        HttpCookie cookie2 = new HttpCookie("X-VSS-UseRequestRouting", bool.TrueString)
        {
          Domain = service != null ? service.AuthenticationServiceInternal()?.GetCookieRootDomain(vssRequestContext) : (string) null,
          Path = "/",
          HttpOnly = true,
          Secure = deploymentContext.ExecutionEnvironment.IsSslOnly,
          Expires = DateTime.UtcNow.AddHours(1.0)
        };
        httpContext.Response.Cookies.Set(cookie2);
      }
      httpContext.Request.Headers.Set("X-VSS-E2EID", vssRequestContext.E2EId.ToString("D"));
      if (deploymentContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableIpSpoofingPrevention"))
      {
        httpContext.Request.Headers.Set("X-Arr-Forwarded-For", IpHelper.ResolveClientIp(deploymentContext, httpContext.Request));
        ExpiringIssuedToken sapplicationToken = deploymentContext.GetService<IS2SArrTokenService>().GetS2SApplicationToken(deploymentContext);
        httpContext.Request.Headers.Set("X-Arr-Authorization", sapplicationToken.Value);
      }
      if (!string.IsNullOrEmpty(vssRequestContext.OrchestrationId))
        httpContext.Request.Headers.Set("X-VSS-OrchestrationId", vssRequestContext.OrchestrationId);
      if (httpContext.Request.ReadEntityBodyMode == ReadEntityBodyMode.Classic || httpContext.Request.ReadEntityBodyMode == ReadEntityBodyMode.Buffered)
        httpContext.Request.InsertEntityBody();
      vssRequestContext.Items.Add(RequestContextItemsKeys.HostProxyData, (object) hostProxyData);
      return vssRequestContext;
    }

    public void EndRoutedContext(IVssRequestContext deploymentContext, HttpContextBase context)
    {
      if (!(context.Items[(object) "ArrCircuitBreakerRef"] is HostRoutingService.ArrCircuitBreakerRef circuitBreakerRef))
        return;
      circuitBreakerRef.EndRequest(deploymentContext, context.Response);
    }

    private HostRoutingService.BeginContextResult BeginLocalContext(
      IVssRequestContext deploymentContext,
      HttpContextBase httpContext,
      Guid hostId)
    {
      HostProperties hostProperties = deploymentContext.GetService<IInternalTeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(deploymentContext, hostId);
      if (hostProperties == null || hostProperties.IsVirtualServiceHost())
        return (HostRoutingService.BeginContextResult) HostRoutingService.BeginContextResult.Nonexistent(hostId);
      if (hostProperties.Status == TeamFoundationServiceHostStatus.Stopped && (hostProperties.SubStatus == ServiceHostSubStatus.Migrating || hostProperties.SubStatus == ServiceHostSubStatus.Idle))
        return (HostRoutingService.BeginContextResult) HostRoutingService.BeginContextResult.Migrating(hostId);
      HostRequestType requestType = HostingEnvironment.IsHosted ? HostRequestType.AspNet : HostRequestType.GenericHttp;
      object[] additionalParameters = new object[1]
      {
        (object) httpContext
      };
      if (deploymentContext is SshRequestContext)
      {
        requestType = HostRequestType.Ssh;
        additionalParameters = new object[2]
        {
          (object) httpContext,
          (object) deploymentContext.UserAgent
        };
      }
      try
      {
        return (HostRoutingService.BeginContextResult) HostRoutingService.BeginContextResult.Started(this.AttemptBeginRequest(deploymentContext, hostId, requestType, additionalParameters));
      }
      catch (HostShutdownException ex1)
      {
        if (!deploymentContext.ExecutionEnvironment.IsHostedDeployment)
          return (HostRoutingService.BeginContextResult) HostRoutingService.BeginContextResult.Stopped(ex1);
        deploymentContext.GetService<IInternalHostSyncService>().RetryFailedSync(deploymentContext, hostProperties);
        try
        {
          return (HostRoutingService.BeginContextResult) HostRoutingService.BeginContextResult.Started(this.AttemptBeginRequest(deploymentContext, hostId, requestType, additionalParameters));
        }
        catch (HostShutdownException ex2)
        {
          return (HostRoutingService.BeginContextResult) HostRoutingService.BeginContextResult.Stopped(ex2);
        }
      }
      catch (HostDoesNotExistException ex)
      {
        return (HostRoutingService.BeginContextResult) HostRoutingService.BeginContextResult.Nonexistent(hostId);
      }
    }

    private IVssRequestContext AttemptBeginRequest(
      IVssRequestContext deploymentContext,
      Guid hostId,
      HostRequestType requestType,
      object[] additionalParameters)
    {
      return deploymentContext.GetService<IInternalTeamFoundationHostManagementService>().BeginRequest(deploymentContext, hostId, RequestContextType.UserContext, true, true, (IReadOnlyList<IRequestActor>) null, requestType, additionalParameters);
    }

    internal static void SetSessionHeaders(
      HttpContextBase httpContext,
      IVssRequestContext localDeploymentContext)
    {
      if (!string.IsNullOrEmpty(httpContext.Request.Headers.Get("X-TFS-Session")))
        return;
      httpContext.Request.Headers.Set("X-TFS-Session", localDeploymentContext.UniqueIdentifier.ToString("D"));
    }

    internal IHostRoutingCache RoutingCache { get; set; }

    internal class ArrCircuitBreakerRef
    {
      private static readonly CommandPropertiesSetter s_defaults = new CommandPropertiesSetter().WithFallbackDisabled(true).WithExecutionTimeout(TimeSpan.MaxValue);

      internal ArrCircuitBreakerRef(
        TaskCompletionSource<object> tcs,
        CommandAsync command,
        Task<object> circuitBreakerTask)
      {
        this.TaskCompletionSource = tcs;
        this.Command = command;
        this.CircuitBreakerTask = circuitBreakerTask;
      }

      public TaskCompletionSource<object> TaskCompletionSource { get; }

      public CommandAsync Command { get; }

      public Task<object> CircuitBreakerTask { get; }

      public static HostRoutingService.ArrCircuitBreakerRef Initialize(
        IVssRequestContext deploymentContext,
        string destinationAuthority)
      {
        string str = "ARR.Proxy-" + destinationAuthority;
        CommandSetter commandSetter = new CommandSetter((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) str);
        TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
        CommandAsync<object> commandAsync = new CommandServiceFactory(deploymentContext, (CommandKey) str, HostRoutingService.ArrCircuitBreakerRef.s_defaults).CreateCommandAsync<object>(commandSetter, (Func<Task<object>>) (() => tcs.Task), (Func<Task<object>>) null, false);
        Task<object> circuitBreakerTask = commandAsync.Execute();
        if (circuitBreakerTask.Exception != null)
          circuitBreakerTask.ConfigureAwait(false).GetAwaiter().GetResult();
        return new HostRoutingService.ArrCircuitBreakerRef(tcs, (CommandAsync) commandAsync, circuitBreakerTask);
      }

      public void EndRequest(IVssRequestContext deploymentContext, HttpResponseBase response)
      {
        if (response.StatusCode == 0 || response.StatusCode >= 500)
          this.TaskCompletionSource.SetException((Exception) new VssServiceResponseException((HttpStatusCode) response.StatusCode, string.Format("ARR failed with {0}.{1} status code.", (object) response.StatusCode, (object) response.SubStatusCode), (Exception) null));
        else
          this.TaskCompletionSource.SetResult((object) null);
        try
        {
          this.CircuitBreakerTask.Wait();
        }
        catch (Exception ex)
        {
          deploymentContext.TraceException(865604704, HostRoutingService.Area, HostRoutingService.Layer, ex);
        }
      }
    }

    private class AuthorityUriBuilder : UriBuilder
    {
      public AuthorityUriBuilder(Uri uri)
        : base(uri)
      {
      }

      public string Authority
      {
        set
        {
          string[] strArray = value.Split(new char[1]{ ':' }, 2);
          this.Host = strArray[0];
          if (strArray.Length <= 1)
            return;
          this.Port = int.Parse(strArray[1]);
        }
      }
    }

    private abstract class BeginContextResult
    {
      public static HostRoutingService.StartedResult Started(IVssRequestContext context) => new HostRoutingService.StartedResult(context);

      public static HostRoutingService.StoppedResult Stopped(HostShutdownException exc) => new HostRoutingService.StoppedResult(exc);

      public static HostRoutingService.NonexistentResult Nonexistent(Guid hostId) => new HostRoutingService.NonexistentResult(hostId);

      public static HostRoutingService.MigratingResult Migrating(Guid hostId) => new HostRoutingService.MigratingResult(hostId);
    }

    private class StartedResult : HostRoutingService.BeginContextResult
    {
      public StartedResult(IVssRequestContext context) => this.Context = context;

      public IVssRequestContext Context { get; }
    }

    private abstract class ExceptionalResult : HostRoutingService.BeginContextResult
    {
      public abstract Exception Exception { get; }
    }

    private class StoppedResult : HostRoutingService.ExceptionalResult
    {
      private readonly HostShutdownException _exception;

      public StoppedResult(HostShutdownException exc) => this._exception = exc;

      public override Exception Exception => (Exception) this._exception;
    }

    private class NonexistentResult : HostRoutingService.ExceptionalResult
    {
      private readonly Guid _hostId;

      public NonexistentResult(Guid hostId) => this._hostId = hostId;

      public override Exception Exception => (Exception) new HostDoesNotExistException(this._hostId);
    }

    private class MigratingResult : HostRoutingService.ExceptionalResult
    {
      private readonly Guid _hostId;

      public MigratingResult(Guid hostId) => this._hostId = hostId;

      public override Exception Exception => (Exception) new HostShutdownException(FrameworkResources.HostOfflineForMigration((object) this._hostId));
    }
  }
}
