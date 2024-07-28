// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentCloudExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Client;
using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.MachineManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskAgentCloudExtensions
  {
    private const string c_secretsStrongboxFormatString = "distributedtask.agentcloud.{0}";
    private const string c_cloudtestHost = "cloudtest.microsoft.com";
    private const string c_cloudtestHostMC = "cloudtest.azure.cn";
    private const string c_cloudtestHostFF = "cloudtest.azure.us";
    private const string c_managedDevopsHost = "manageddevops.microsoft.com";
    private static readonly Guid s_cloudtestServicePrincipal = new Guid("00000051-0000-8888-8000-000000000000");
    private const string c_layer = "TaskAgentCloudExtensions";

    public static async Task<RemoteProviderHttpClient> GetPoolProviderClient(
      this TaskAgentCloud agentCloud,
      IVssRequestContext requestContext,
      PoolProviderConfiguration poolProviderConfiguration = null)
    {
      VssCredentials credentials = (VssCredentials) null;
      if (poolProviderConfiguration != null)
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        string str = (requestContext.ExecutionEnvironment.IsDevFabricDeployment ? (DeploymentRealm) 3 : (DeploymentRealm) 5).ToString();
        CrossRealmService service = context.GetService<CrossRealmService>();
        Uri mmsServiceUrl = requestContext.GetService<MMSUrlConfigurationService>().GetMMSServiceUrl();
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        string realm1 = str;
        Uri mmsUrl = mmsServiceUrl;
        (CrossRealmService.RequestContextAdapter requestContextAdapter, Microsoft.TeamFoundation.DistributedTask.Pipelines.CrossRealm crossRealm) realm2 = service.GetRealm(requestContext1, realm1, mmsUrl);
        credentials = (VssCredentials) (FederatedCredential) await realm2.crossRealm.CreateOAuthCreds((Microsoft.TeamFoundation.DistributedTask.Pipelines.CrossRealm.RequestContextAdapter) realm2.requestContextAdapter, MmsResourceIds.InstanceTypeGuid);
      }
      else if (agentCloud.Internal.GetValueOrDefault())
        credentials = agentCloud.GetInternalAgentCloudCredentials(requestContext);
      else if (agentCloud.HasCloudTestHost())
      {
        if (requestContext.IsFeatureEnabled("DistributedTask.AadAuthFor1ES"))
        {
          try
          {
            if (requestContext.IsMicrosoftTenant())
            {
              IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
              credentials = vssRequestContext.GetService<IS2SCredentialsService>().GetS2SCredentials(vssRequestContext, TaskAgentCloudExtensions.s_cloudtestServicePrincipal);
            }
            else
              requestContext.TraceError(10015271, nameof (TaskAgentCloudExtensions), "Org not linked to Microsoft AAD Tenant is creating client for 1ES PoolProvider!");
          }
          catch (Exception ex)
          {
            requestContext.TraceError(10015271, nameof (TaskAgentCloudExtensions), string.Format("Failed to generate AAD token for the CloudTest PoolProvider: {0}: {1}", (object) ex.GetType(), (object) ex.Message));
          }
        }
      }
      HttpMessageHandler messageHandler = credentials != null ? (HttpMessageHandler) new VssHttpMessageHandler(credentials, new VssHttpRequestSettings()) : (HttpMessageHandler) null;
      List<DelegatingHandler> delegatingHandlerList = new List<DelegatingHandler>();
      delegatingHandlerList.Add((DelegatingHandler) new HMACMessageHandler(agentCloud.GetSharedSecretBytes()));
      delegatingHandlerList.AddRange((IEnumerable<DelegatingHandler>) ClientProviderHelper.GetMinimalDelegatingHandlers(requestContext, typeof (RemoteProviderHttpClient), ClientProviderHelper.Options.CreateDefault(requestContext), "RemoteAgentPoolProvider"));
      return poolProviderConfiguration == null ? new RemoteProviderHttpClient(agentCloud.AcquireAgentEndpoint, agentCloud.ReleaseAgentEndpoint, agentCloud.GetAgentDefinitionEndpoint, agentCloud.GetAgentRequestStatusEndpoint, agentCloud.GetAccountParallelismEndpoint, messageHandler, (IEnumerable<DelegatingHandler>) delegatingHandlerList) : new RemoteProviderHttpClient(poolProviderConfiguration.AcquireAgentUrl, poolProviderConfiguration.ReleaseAgentUrl, poolProviderConfiguration.GetAgentDefinitionsUrl, poolProviderConfiguration.GetAgentRequestStatusUrl, poolProviderConfiguration.GetAccountParallelismEndpoint, messageHandler, (IEnumerable<DelegatingHandler>) delegatingHandlerList);
    }

    public static void Validate(this TaskAgentCloud cloud, IVssRequestContext requestContext)
    {
      if (cloud == null)
        return;
      if (cloud.Type == null)
        throw new InvalidTaskAgentCloudException(TaskResources.InvalidAgentCloudType());
      cloud.ValidateInternalPermissions(requestContext);
      cloud.ValidateSharedSecret();
      if (!cloud.Internal.GetValueOrDefault())
      {
        TaskAgentCloudExtensions.ValidateUrl(cloud.AcquireAgentEndpoint, "AcquireAgentEndpoint", false);
        TaskAgentCloudExtensions.ValidateUrl(cloud.ReleaseAgentEndpoint, "ReleaseAgentEndpoint", false);
      }
      else
      {
        TaskAgentCloudExtensions.ValidateUrl(cloud.AcquireAgentEndpoint, "AcquireAgentEndpoint", true);
        TaskAgentCloudExtensions.ValidateUrl(cloud.ReleaseAgentEndpoint, "ReleaseAgentEndpoint", true);
      }
      TaskAgentCloudExtensions.ValidateUrl(cloud.GetAgentDefinitionEndpoint, "GetAgentDefinitionEndpoint", true);
      TaskAgentCloudExtensions.ValidateUrl(cloud.GetAgentRequestStatusEndpoint, "GetAgentRequestStatusEndpoint", true);
      TaskAgentCloudExtensions.ValidateUrl(cloud.GetAccountParallelismEndpoint, "GetAccountParallelismEndpoint", true);
    }

    public static void ValidatePartialCloud(
      this TaskAgentCloud cloud,
      IVssRequestContext requestContext)
    {
      if (cloud == null)
        return;
      cloud.ValidateInternalPermissions(requestContext);
      if (cloud.SharedSecret != null)
      {
        cloud.ValidateSharedSecret();
        if (cloud.AcquireAgentEndpoint != null || cloud.ReleaseAgentEndpoint != null || cloud.GetAgentDefinitionEndpoint != null || cloud.GetAgentRequestStatusEndpoint != null || cloud.GetAccountParallelismEndpoint != null || cloud.Name != null || cloud.Type != null || cloud.MaxParallelism.HasValue || cloud.AcquisitionTimeout.HasValue || cloud != null && cloud.Internal.HasValue)
          throw new ArgumentException(TaskResources.InvalidAgentCloudUpdateOfSharedSecret());
      }
      if (cloud.AcquireAgentEndpoint != null)
        TaskAgentCloudExtensions.ValidateUrl(cloud.AcquireAgentEndpoint, "AcquireAgentEndpoint", true);
      if (cloud.ReleaseAgentEndpoint != null)
        TaskAgentCloudExtensions.ValidateUrl(cloud.ReleaseAgentEndpoint, "ReleaseAgentEndpoint", true);
      if (cloud.GetAgentDefinitionEndpoint != null)
        TaskAgentCloudExtensions.ValidateUrl(cloud.GetAgentDefinitionEndpoint, "GetAgentDefinitionEndpoint", true);
      if (cloud.GetAgentRequestStatusEndpoint != null)
        TaskAgentCloudExtensions.ValidateUrl(cloud.GetAgentRequestStatusEndpoint, "GetAgentRequestStatusEndpoint", true);
      if (cloud.GetAccountParallelismEndpoint == null)
        return;
      TaskAgentCloudExtensions.ValidateUrl(cloud.GetAccountParallelismEndpoint, "GetAccountParallelismEndpoint", true);
    }

    public static void ValidateInternalPermissions(
      this TaskAgentCloud cloud,
      IVssRequestContext requestContext)
    {
      if (cloud != null && cloud.Internal.GetValueOrDefault() && !requestContext.IsSystemContext)
        throw new InvalidTaskAgentCloudException(TaskResources.InvalidOperationOnInternalAgentCloud((object) cloud.Name));
    }

    public static bool HasCloudTestHost(this TaskAgentCloud cloud)
    {
      if (cloud == null || cloud.AcquireAgentEndpoint == null)
        return false;
      return cloud.AcquireAgentEndpoint.IndexOf("cloudtest.microsoft.com", StringComparison.OrdinalIgnoreCase) >= 0 || cloud.AcquireAgentEndpoint.IndexOf("manageddevops.microsoft.com", StringComparison.OrdinalIgnoreCase) >= 0 || cloud.AcquireAgentEndpoint.IndexOf("cloudtest.azure.cn", StringComparison.OrdinalIgnoreCase) >= 0 || cloud.AcquireAgentEndpoint.IndexOf("cloudtest.azure.us", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    internal static void SaveSharedSecret(
      this TaskAgentCloud cloud,
      IVssRequestContext requestContext)
    {
      if (cloud == null || cloud.AgentCloudId <= 0 || cloud.SharedSecret == null)
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, cloud.GetStrongBoxName(), false);
      if (drawerId == Guid.Empty)
      {
        try
        {
          drawerId = service.CreateDrawer(vssRequestContext, cloud.GetStrongBoxName());
        }
        catch (StrongBoxDrawerExistsException ex)
        {
          drawerId = service.UnlockDrawer(vssRequestContext, cloud.GetStrongBoxName(), false);
        }
      }
      service.AddString(vssRequestContext, drawerId, "SharedSecret", cloud.SharedSecret);
    }

    internal static void LoadSharedSecret(
      this TaskAgentCloud cloud,
      IVssRequestContext requestContext)
    {
      if (cloud == null || cloud.AgentCloudId <= 0 || cloud.SharedSecret != null)
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid guid = service.UnlockDrawer(vssRequestContext, cloud.GetStrongBoxName(), false);
      if (object.Equals((object) Guid.Empty, (object) guid))
        return;
      cloud.SharedSecret = service.GetString(vssRequestContext, guid, "SharedSecret");
    }

    internal static void DeleteSharedSecret(
      this TaskAgentCloud cloud,
      IVssRequestContext requestContext)
    {
      if (cloud == null || cloud.AgentCloudId <= 0)
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid guid = service.UnlockDrawer(vssRequestContext, cloud.GetStrongBoxName(), false);
      if (object.Equals((object) Guid.Empty, (object) guid))
        return;
      service.DeleteDrawer(vssRequestContext, guid);
    }

    internal static byte[] GetSharedSecretBytes(this TaskAgentCloud cloud) => !string.IsNullOrEmpty(cloud.SharedSecret) ? Encoding.UTF8.GetBytes(cloud.SharedSecret) : (byte[]) null;

    private static void ValidateUrl(string url, string paramName, bool allowNull)
    {
      if (allowNull && string.IsNullOrEmpty(url))
        return;
      ArgumentUtility.CheckForNull<string>(url, paramName);
      if (url.Length > 128)
        throw new ArgumentException(TaskResources.InvalidUrlLength((object) url), paramName);
      if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out Uri _))
        throw new ArgumentException(TaskResources.InvalidUrl((object) url), paramName);
    }

    private static void ValidateSharedSecret(this TaskAgentCloud cloud)
    {
      byte[] sharedSecretBytes = cloud.GetSharedSecretBytes();
      if (sharedSecretBytes == null || sharedSecretBytes.Length < 16)
        throw new InvalidTaskAgentCloudException(TaskResources.InvalidAgentCloudSharedSecret());
    }

    private static string GetStrongBoxName(this TaskAgentCloud cloud) => string.Format("distributedtask.agentcloud.{0}", (object) cloud.AgentCloudId);
  }
}
