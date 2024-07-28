// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PoolProviderConfigurationHelpers
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class PoolProviderConfigurationHelpers
  {
    public static void AddProviderConfigurationToAgentRequestResponse(
      this AgentRequestResponse agentRequestResponse,
      PoolProviderConfiguration poolProviderConfiguration)
    {
      if (poolProviderConfiguration == null)
        return;
      if (agentRequestResponse.AgentData == null)
        agentRequestResponse.AgentData = new JObject();
      string lower = "PoolProviderConfiguration".ToLower();
      string str = lower + "_new";
      if (!agentRequestResponse.AgentData.ContainsKey(lower))
        agentRequestResponse.AgentData.Add((object) new JProperty(lower, (object) JsonConvert.SerializeObject((object) poolProviderConfiguration)));
      if (agentRequestResponse.AgentData.ContainsKey(str))
        return;
      agentRequestResponse.AgentData.Add((object) new JProperty(str, (object) JObject.FromObject((object) poolProviderConfiguration)));
    }

    public static PoolProviderConfiguration GetPoolProviderConfiguration(
      this JObject agentData,
      IVssRequestContext requestContext)
    {
      string lower = "PoolProviderConfiguration".ToLower();
      string propertyName = lower + "_new";
      if (agentData == null)
        return (PoolProviderConfiguration) null;
      PoolProviderConfiguration providerConfiguration1 = (PoolProviderConfiguration) null;
      PoolProviderConfiguration providerConfiguration2 = (PoolProviderConfiguration) null;
      if (agentData.ContainsKey(propertyName))
      {
        try
        {
          providerConfiguration2 = agentData.GetValue(propertyName).ToObject<PoolProviderConfiguration>();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10015239, "DistributedTask", nameof (PoolProviderConfigurationHelpers), ex);
        }
      }
      if (agentData.ContainsKey(lower))
      {
        try
        {
          providerConfiguration1 = JsonConvert.DeserializeObject<PoolProviderConfiguration>(agentData.GetValue(lower).ToString());
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10015239, "DistributedTask", nameof (PoolProviderConfigurationHelpers), ex);
        }
      }
      return providerConfiguration2 ?? providerConfiguration1;
    }

    public static async Task<PoolProviderConfiguration> GetDeploymentPoolProviderConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      string str = (requestContext.ExecutionEnvironment.IsDevFabricDeployment ? (DeploymentRealm) 3 : (DeploymentRealm) 5).ToString();
      CrossRealmService service = context.GetService<CrossRealmService>();
      Uri mmsServiceUrl = requestContext.GetService<MMSUrlConfigurationService>().GetMMSServiceUrl();
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      string realm = str;
      Uri mmsUrl = mmsServiceUrl;
      (CrossRealmService.RequestContextAdapter requestContextAdapter, CrossRealm crossRealm) = service.GetRealm(requestContext1, realm, mmsUrl);
      Dictionary<string, object> routeValues = new Dictionary<string, object>()
      {
        {
          "requestType",
          (object) "hosted"
        },
        {
          "api-version",
          (object) TaskConstants.DeploymentApiVersion.ToString()
        }
      };
      PoolProviderConfiguration poolProviderConfiguration = new PoolProviderConfiguration();
      PoolProviderConfiguration providerConfiguration = poolProviderConfiguration;
      providerConfiguration.AcquireAgentUrl = (await crossRealm.GetResourceUri((CrossRealm.RequestContextAdapter) requestContextAdapter, PoolProviderResourceIds.DeploymentAreaId, PoolProviderResourceIds.DeploymentAcquireAgentLocationId, (object) new Dictionary<string, object>((IDictionary<string, object>) routeValues), true, false, false, requestContext.CancellationToken)).ToString();
      providerConfiguration = (PoolProviderConfiguration) null;
      providerConfiguration = poolProviderConfiguration;
      providerConfiguration.ReleaseAgentUrl = (await crossRealm.GetResourceUri((CrossRealm.RequestContextAdapter) requestContextAdapter, PoolProviderResourceIds.DeploymentAreaId, PoolProviderResourceIds.DeploymentReleaseAgentLocationId, (object) new Dictionary<string, object>((IDictionary<string, object>) routeValues), true, false, false, requestContext.CancellationToken)).ToString();
      providerConfiguration = (PoolProviderConfiguration) null;
      providerConfiguration = poolProviderConfiguration;
      providerConfiguration.GetAgentRequestStatusUrl = (await crossRealm.GetResourceUri((CrossRealm.RequestContextAdapter) requestContextAdapter, PoolProviderResourceIds.DeploymentAreaId, PoolProviderResourceIds.DeploymentAgentRequestStatusLocationId, (object) new Dictionary<string, object>((IDictionary<string, object>) routeValues), true, false, false, requestContext.CancellationToken)).ToString();
      providerConfiguration = (PoolProviderConfiguration) null;
      PoolProviderConfiguration providerConfiguration1 = poolProviderConfiguration;
      requestContextAdapter = (CrossRealmService.RequestContextAdapter) null;
      crossRealm = (CrossRealm) null;
      routeValues = (Dictionary<string, object>) null;
      poolProviderConfiguration = (PoolProviderConfiguration) null;
      return providerConfiguration1;
    }
  }
}
