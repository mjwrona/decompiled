// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PoolProviderAgentRequestHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class PoolProviderAgentRequestHelper
  {
    private static readonly ApiResourceVersion ApiVersion = new ApiResourceVersion(VssRestApiVersion.v5_0.ToVersion())
    {
      IsPreview = true
    };
    private const string CustomPoolProviderAgentSettingsPath = "/Service/DistributedTask/Settings/PoolProvider/CustomAgentSettings/*";
    private const string OrchestrationIdJwtClaim = "orchid";

    internal static AgentRequest GetAgentRequest(
      IVssRequestContext requestContext,
      TaskAgentCloud cloud,
      TaskAgentCloudRequest request,
      TaskAgentPoolReference pool,
      TaskAgentReference agent,
      TaskAgentJobRequest agentJobRequest)
    {
      return new AgentRequest()
      {
        AgentId = request.RequestId,
        AgentPool = pool.Name,
        AccountId = requestContext.ServiceHost.InstanceId,
        AgentCloudId = cloud.Id,
        AgentConfiguration = PoolProviderAgentRequestHelper.GetAgentConfiguration(requestContext, cloud, request, pool, agent, agentJobRequest?.Demands),
        AgentSpecification = request.AgentSpecification,
        AuthenticationToken = PoolProviderAgentRequestHelper.GetAuthenticationToken(requestContext, request, pool),
        AppendRequestMessageUrl = PoolProviderAgentRequestHelper.GetAppendRequestMessageUrl(requestContext, request),
        GetAssociatedJobUrl = PoolProviderAgentRequestHelper.GetGetAssociatedJobUrl(requestContext, request),
        UpdateRequestUrl = PoolProviderAgentRequestHelper.GetUpdateRequestUrl(requestContext, request),
        AssociatedJob = PoolProviderAgentRequestHelper.GetAssociatedJob(requestContext, pool, agentJobRequest),
        Demands = PoolProviderAgentRequestHelper.GetDemands(requestContext, agentJobRequest)
      };
    }

    private static AgentConfiguration GetAgentConfiguration(
      IVssRequestContext requestContext,
      TaskAgentCloud cloud,
      TaskAgentCloudRequest request,
      TaskAgentPoolReference pool,
      TaskAgentReference agent,
      IList<Demand> requestDemands)
    {
      return new AgentConfiguration()
      {
        AgentSettings = PoolProviderAgentRequestHelper.GetAgentSettings(requestContext, cloud, pool, agent),
        AgentCredentials = PoolProviderAgentRequestHelper.GetAgentCredentials(requestContext, request, pool),
        AgentVersion = PoolProviderAgentRequestHelper.GetLatestAgentVersion(requestContext),
        MinimumAgentVersion = PoolProviderAgentRequestHelper.GetMinimumAgentVersion(requestContext, requestDemands),
        AgentDownloadUrls = PoolProviderAgentRequestHelper.GetAgentDownloadUrls(requestContext)
      };
    }

    private static Dictionary<string, string> GetAgentSettings(
      IVssRequestContext requestContext,
      TaskAgentCloud cloud,
      TaskAgentPoolReference pool,
      TaskAgentReference agent)
    {
      Dictionary<string, string> agentSettings = new Dictionary<string, string>();
      if (cloud.Internal.GetValueOrDefault())
        agentSettings.Add("AcceptTeeEula", "True");
      agentSettings.Add("AgentId", agent.Id.ToString());
      agentSettings.Add("AgentName", agent.Name);
      agentSettings.Add("AutoUpdate", "False");
      agentSettings.Add("AgentCloudId", cloud.Id.ToString());
      agentSettings.Add("PoolId", pool.Id.ToString());
      agentSettings.Add("ServerUrl", requestContext.BuildHyperlink());
      agentSettings.Add("SkipCapabilitiesScan", "True");
      agentSettings.Add("SkipSessionRecover", "True");
      agentSettings.Add("workFolder", "_work");
      foreach (RegistryEntry registryEntry in requestContext.GetService<ICachedRegistryService>().ReadEntriesFallThru(requestContext, new RegistryQuery("/Service/DistributedTask/Settings/PoolProvider/CustomAgentSettings/*")))
        agentSettings.Add(registryEntry.Name, registryEntry.Value);
      return agentSettings;
    }

    private static AgentCredentialData GetAgentCredentials(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request,
      TaskAgentPoolReference pool)
    {
      Microsoft.VisualStudio.Services.Identity.Identity poolIdentity = requestContext.GetService<DistributedTaskResourceService>().ProvisionServiceIdentity(requestContext, pool.Id, AgentPoolServiceAccountRoles.AgentPoolService);
      return PoolProviderAgentRequestHelper.GetAgentCredentials(requestContext, request, poolIdentity);
    }

    internal static AgentCredentialData GetAgentCredentials(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request,
      Microsoft.VisualStudio.Services.Identity.Identity poolIdentity)
    {
      AgentCredentialData agentCredentials = new AgentCredentialData()
      {
        Scheme = "PAT"
      };
      string scope = string.Join(" ", new string[2]
      {
        ScopeHelpers.ConstructScope("LocationService.Connect"),
        DistributedTaskScopeHelper.GenerateAgentCloudRequestListenScope(request.AgentCloudId, request.RequestId)
      });
      Dictionary<string, string> customClaims = new Dictionary<string, string>();
      if (requestContext.OrchestrationId != null)
        customClaims.Add("orchid", requestContext.OrchestrationId);
      SessionToken selfDescribingJwt = SessionTokenGenerator.GenerateSelfDescribingJwt(requestContext, TimeSpan.FromDays(2.0), poolIdentity, scope, (IDictionary<string, string>) customClaims);
      agentCredentials.Data.Add("token", selfDescribingJwt.Token);
      return agentCredentials;
    }

    private static string GetMinimumAgentVersion(
      IVssRequestContext requestContext,
      IList<Demand> demands)
    {
      string semanticVersion2 = TaskAgentConstants.MinimumPoolProviderAgentVersion.ToString();
      if (demands != null && demands.Count > 0)
      {
        DemandMinimumVersion demandMinimumVersion = DemandMinimumVersion.Max((IEnumerable<Demand>) demands);
        if (demandMinimumVersion != null && DemandMinimumVersion.CompareVersion(demandMinimumVersion.Value, semanticVersion2) > 0)
          semanticVersion2 = demandMinimumVersion.Value;
      }
      return semanticVersion2;
    }

    public static string GetLatestAgentVersion(IVssRequestContext requestContext)
    {
      IPackageMetadataService service = requestContext.GetService<IPackageMetadataService>();
      PackageVersion latestAgentVersion = (PackageVersion) null;
      foreach (PackageVersion other in new List<PackageVersion>()
      {
        service.GetLatestPackageVersion(requestContext, TaskAgentConstants.AgentPackageType, TaskAgentConstants.CoreV2WindowsPlatformName),
        service.GetLatestPackageVersion(requestContext, TaskAgentConstants.AgentPackageType, TaskAgentConstants.CoreV2LinuxPlatformName),
        service.GetLatestPackageVersion(requestContext, TaskAgentConstants.AgentPackageType, TaskAgentConstants.CoreV2OSXPlatformName)
      })
      {
        if (latestAgentVersion == null)
          latestAgentVersion = other;
        else if (latestAgentVersion.CompareTo(other) > 0)
          latestAgentVersion = other;
      }
      return (string) latestAgentVersion;
    }

    private static Dictionary<string, string> GetAgentDownloadUrls(IVssRequestContext requestContext) => requestContext.GetService<IPackageMetadataService>().GetLatestPackageDownloadUrls(requestContext, TaskAgentConstants.AgentPackageType);

    private static string GetAuthenticationToken(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request,
      TaskAgentPoolReference pool)
    {
      string scope = string.Join(" ", new string[2]
      {
        ScopeHelpers.ConstructScope("LocationService.Connect"),
        DistributedTaskScopeHelper.GenerateAgentCloudRequestUseScope(request.AgentCloudId, request.RequestId)
      });
      Microsoft.VisualStudio.Services.Identity.Identity toBeResolvedIdentity = requestContext.GetService<DistributedTaskResourceService>().ProvisionServiceIdentity(requestContext, pool.Id, AgentPoolServiceAccountRoles.AgentPoolService);
      Dictionary<string, string> customClaims = (Dictionary<string, string>) null;
      if (requestContext.IsFeatureEnabled("DistributedTask.TraceExternalAgentProvisioning") && requestContext.OrchestrationId != null)
      {
        customClaims = new Dictionary<string, string>();
        customClaims.Add("orchid", requestContext.OrchestrationId);
      }
      return SessionTokenGenerator.GenerateSelfDescribingJwt(requestContext, TimeSpan.FromDays(2.0), toBeResolvedIdentity, scope, (IDictionary<string, string>) customClaims).Token;
    }

    private static string GetUpdateRequestUrl(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request)
    {
      return requestContext.GetService<ILocationService>().GetLocationData(requestContext, requestContext.ServiceInstanceType()).GetResourceUri(requestContext, "distributedtask", TaskResourceIds.AgentCloudRequests, (object) new Dictionary<string, object>()
      {
        {
          "agentCloudId",
          (object) request.AgentCloudId
        },
        {
          "agentCloudRequestId",
          (object) request.RequestId
        }
      }).ToString() + ("?api-version=" + PoolProviderAgentRequestHelper.ApiVersion.ToString());
    }

    private static string GetAppendRequestMessageUrl(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request)
    {
      return requestContext.GetService<ILocationService>().GetLocationData(requestContext, requestContext.ServiceInstanceType()).GetResourceUri(requestContext, "distributedtask", TaskResourceIds.AgentCloudRequestMessages, (object) new Dictionary<string, object>()
      {
        {
          "agentCloudId",
          (object) request.AgentCloudId
        },
        {
          "agentCloudRequestId",
          (object) request.RequestId
        }
      }).ToString() + ("?api-version=" + PoolProviderAgentRequestHelper.ApiVersion.ToString());
    }

    private static string GetGetAssociatedJobUrl(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request)
    {
      try
      {
        return requestContext.GetService<ILocationService>().GetLocationData(requestContext, requestContext.ServiceInstanceType()).GetResourceUri(requestContext, "distributedtask", TaskResourceIds.AgentCloudRequestJob, (object) new Dictionary<string, object>()
        {
          {
            "agentCloudId",
            (object) request.AgentCloudId
          },
          {
            "agentCloudRequestId",
            (object) request.RequestId
          }
        }).ToString() + ("?api-version=" + PoolProviderAgentRequestHelper.ApiVersion.ToString());
      }
      catch (VssResourceNotFoundException ex)
      {
        return (string) null;
      }
    }

    private static AgentRequestJob GetAssociatedJob(
      IVssRequestContext requestContext,
      TaskAgentPoolReference pool,
      TaskAgentJobRequest agentJobRequest)
    {
      return requestContext.IsFeatureEnabled("DistributedTask.IncludeJobInNonMmsAgentCloudAcquireRequests") && !pool.IsHosted ? requestContext.RunSynchronously<AgentRequestJob>((Func<Task<AgentRequestJob>>) (() => agentJobRequest.GetAgentRequestJobAsync(requestContext))) : (AgentRequestJob) null;
    }

    internal static IList<string> GetDemands(
      IVssRequestContext requestContext,
      TaskAgentJobRequest agentJobRequest)
    {
      return requestContext.IsFeatureEnabled("DistributedTask.IncludeDemandsInAgentCloudAcquireRequests") && agentJobRequest?.Demands != null ? (IList<string>) agentJobRequest.Demands.Select<Demand, string>((Func<Demand, string>) (d => d.ToString())).ToList<string>() : (IList<string>) null;
    }
  }
}
