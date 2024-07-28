// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.ServerNodeHealthService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal class ServerNodeHealthService : 
    SmartRouterFrameworkServiceBase,
    IServerNodeHealthService,
    IVssFrameworkService,
    IHasSmartRouterBackgroundJob
  {
    public ServerNodeHealthService()
      : base(SmartRouterBase.TraceLayer.BackEnd, true)
    {
    }

    public bool? IsHealthy(IVssRequestContext requestContext, ServerNode server)
    {
      requestContext = this.CheckRequestContext(requestContext);
      ServerNodeHealthService.HealthEntry healthEntry;
      if (this.ServerHealthMap.TryGetValue(ServerNodeHealthService.HealthEntry.MakeKey(server), out healthEntry))
      {
        TimeSpan timeToLiveSetting = this.GetSmartRouterServerHealthTimeToLiveSetting(requestContext);
        if (healthEntry.Updated + timeToLiveSetting > DateTime.UtcNow)
        {
          this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.GetServerHealth, "Server health: server={0}, isHealthy={1}", (object) healthEntry.Server.RoleInstance, (object) healthEntry.IsHealthy);
          return new bool?(healthEntry.IsHealthy);
        }
        this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.GetServerHealthExpired, "Server health expired: server={0}, isHealthy={1}", (object) healthEntry.Server.RoleInstance, (object) healthEntry.IsHealthy);
        return new bool?();
      }
      this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.GetServerHealthUnknown, "Server health unknown: server={0}", (object) server.RoleInstance);
      return new bool?();
    }

    protected override void OnServiceStart(IVssRequestContext requestContext) => this.HealthProbeJob.Start(requestContext, new Func<IVssRequestContext, Task>(this.ProbeHealthJobCallbackAsync), this.GetSmartRouterHealthProbePeriodSetting(requestContext));

    protected override void OnServiceEnd(IVssRequestContext systemRequestContext) => this.HealthProbeJob.Stop(systemRequestContext);

    private async Task ProbeHealthJobCallbackAsync(IVssRequestContext systemRequestContext)
    {
      ServerNodeHealthService nodeHealthService = this;
      List<Task<ServerNodeHealthService.HealthProbeResult>> healthProbeTasks;
      if (!nodeHealthService.IsEnabled(systemRequestContext))
      {
        healthProbeTasks = (List<Task<ServerNodeHealthService.HealthProbeResult>>) null;
      }
      else
      {
        IReadOnlyList<ServerNodeWithHash> discoveredServerNodes = nodeHealthService.GetDiscoveredServerNodes(systemRequestContext);
        if (!discoveredServerNodes.Any<ServerNodeWithHash>())
        {
          nodeHealthService.Tracer.TraceInfo(systemRequestContext, SmartRouterBase.TracePoint.HealthProbeNoDiscoveredServers, "health probe: no discovered servers");
          healthProbeTasks = (List<Task<ServerNodeHealthService.HealthProbeResult>>) null;
        }
        else
        {
          string hostName = nodeHealthService.GetPublicAccessHostName(systemRequestContext);
          healthProbeTasks = discoveredServerNodes.Select<ServerNodeWithHash, ServerNode>((Func<ServerNodeWithHash, ServerNode>) (item => item.Server)).Select<ServerNode, Task<ServerNodeHealthService.HealthProbeResult>>((Func<ServerNode, Task<ServerNodeHealthService.HealthProbeResult>>) (async server => await this.HttpGetHealthEndpointAsync(server, hostName))).ToList<Task<ServerNodeHealthService.HealthProbeResult>>();
          ServerNodeHealthService.HealthProbeResult[] healthProbeResultArray = await Task.WhenAll<ServerNodeHealthService.HealthProbeResult>((IEnumerable<Task<ServerNodeHealthService.HealthProbeResult>>) healthProbeTasks);
          using (List<Task<ServerNodeHealthService.HealthProbeResult>>.Enumerator enumerator = healthProbeTasks.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Task<ServerNodeHealthService.HealthProbeResult> current = enumerator.Current;
              if (current.Status == TaskStatus.RanToCompletion)
              {
                ServerNodeHealthService.HealthProbeResult result = current.Result;
                if (result.Exception == null)
                {
                  nodeHealthService.UpdateHealth(result.Server, result.StatusCode == HttpStatusCode.OK);
                  nodeHealthService.Tracer.TraceInfo(systemRequestContext, SmartRouterBase.TracePoint.ProbeHealthJob, "Smart router health probe: server={0}, endpoint={1}, status={2}", (object) result.Server.RoleInstance, (object) result.Endpoint, (object) result.StatusCode.ToString());
                }
                else if (result.Exception is OperationCanceledException)
                  nodeHealthService.Tracer.TraceWarning(systemRequestContext, SmartRouterBase.TracePoint.ProbeHealthJobCanceled, "Smart router health probe canceled: server={0}, endpoint={1}", (object) result.Server.RoleInstance, (object) result.Endpoint);
                else if (result.Exception is SocketException)
                {
                  nodeHealthService.UpdateHealth(result.Server, false);
                  nodeHealthService.Tracer.TraceWarning(systemRequestContext, SmartRouterBase.TracePoint.ProbeHealthSocketException, "Smart router health probe socket error: server={0}, endpoint={1}, exception={2}", (object) result.Server.RoleInstance, (object) result.Endpoint, (object) nodeHealthService.Tracer.GetLazyStackTrace(result.Exception));
                }
                else
                {
                  nodeHealthService.UpdateHealth(result.Server, false);
                  nodeHealthService.Tracer.TraceException(systemRequestContext, SmartRouterBase.TracePoint.ProbeHealthJobException, result.Exception, "Smart router health probe error: server={0}, endpoint={1}, exception={2}", (object) result.Server.RoleInstance, (object) result.Endpoint, (object) nodeHealthService.Tracer.GetLazyStackTrace(result.Exception));
                }
              }
            }
            healthProbeTasks = (List<Task<ServerNodeHealthService.HealthProbeResult>>) null;
          }
        }
      }
    }

    protected internal virtual IReadOnlyList<ServerNodeWithHash> GetDiscoveredServerNodes(
      IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentHostContext = requestContext.ToDeploymentHostContext();
      return deploymentHostContext.GetServerNodeDiscoveryService().GetDiscoveredServerNodes(deploymentHostContext);
    }

    protected internal virtual string GetPublicAccessHostName(IVssRequestContext requestContext) => new Uri(requestContext.GetPublicAccessMappingAccessPoint()).Host;

    protected internal virtual TimeSpan GetSmartRouterServerHealthTimeToLiveSetting(
      IVssRequestContext requestContext)
    {
      return requestContext.GetSmartRouterServerHealthTimeToLiveSetting();
    }

    protected internal virtual TimeSpan GetSmartRouterHealthProbePeriodSetting(
      IVssRequestContext requestContext)
    {
      return requestContext.GetSmartRouterHealthProbePeriodSetting();
    }

    protected internal virtual async Task<ServerNodeHealthService.HealthProbeResult> HttpGetHealthEndpointAsync(
      ServerNode server,
      string hostName)
    {
      Uri healthEndpoint = new UriBuilder()
      {
        Scheme = Uri.UriSchemeHttps,
        Host = server.IPAddress,
        Path = "_apis/health"
      }.Uri;
      try
      {
        return new ServerNodeHealthService.HealthProbeResult(server, healthEndpoint, (await this.HttpClient.Value.SendAsync(new HttpRequestMessage(HttpMethod.Get, healthEndpoint)
        {
          Headers = {
            Host = hostName
          }
        })).StatusCode);
      }
      catch (Exception ex)
      {
        return new ServerNodeHealthService.HealthProbeResult(server, healthEndpoint, (HttpStatusCode) 0, ex);
      }
    }

    SmartRouterBackgroundJob IHasSmartRouterBackgroundJob.BackgroundJob => this.HealthProbeJob;

    private void UpdateHealth(ServerNode server, bool isHealthy)
    {
      ServerNodeHealthService.HealthEntry healthEntry = new ServerNodeHealthService.HealthEntry(server, isHealthy);
      this.ServerHealthMap[healthEntry.Key] = healthEntry;
    }

    private ConcurrentDictionary<string, ServerNodeHealthService.HealthEntry> ServerHealthMap { get; } = new ConcurrentDictionary<string, ServerNodeHealthService.HealthEntry>();

    private SmartRouterBackgroundJob HealthProbeJob { get; } = new SmartRouterBackgroundJob(nameof (HealthProbeJob));

    private Lazy<System.Net.Http.HttpClient> HttpClient { get; } = new Lazy<System.Net.Http.HttpClient>((Func<System.Net.Http.HttpClient>) (() => new System.Net.Http.HttpClient()));

    private class HealthEntry
    {
      public HealthEntry(ServerNode server, bool isHealthy)
      {
        server = server.CheckArgumentIsNotNull<ServerNode>(nameof (server));
        this.Key = ServerNodeHealthService.HealthEntry.MakeKey(server);
        this.Server = server;
        this.IsHealthy = isHealthy;
        this.Updated = DateTime.UtcNow;
      }

      public string Key { get; }

      public ServerNode Server { get; }

      public bool IsHealthy { get; }

      public DateTime Updated { get; }

      public static string MakeKey(ServerNode server) => server.RoleName + "|" + server.RoleInstance;
    }

    public struct HealthProbeResult
    {
      public HealthProbeResult(
        ServerNode serverNode,
        Uri endpoint,
        HttpStatusCode statusCode,
        Exception? ex = null)
      {
        this.Server = serverNode;
        this.Endpoint = endpoint;
        this.StatusCode = statusCode;
        this.Exception = ex;
      }

      public ServerNode Server { get; private set; }

      public Uri Endpoint { get; set; }

      public HttpStatusCode StatusCode { get; private set; }

      public Exception? Exception { get; private set; }
    }
  }
}
