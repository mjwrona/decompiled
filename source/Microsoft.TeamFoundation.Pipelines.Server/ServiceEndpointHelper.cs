// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ServiceEndpointHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class ServiceEndpointHelper
  {
    private const string c_layer = "ServiceEndpointHelper";
    private static readonly IEnumerable<string> s_defaultInstallationAuthSchemes = (IEnumerable<string>) new List<string>()
    {
      "InstallationToken"
    };

    public static string GetNextAvailableEndpointName(
      IVssRequestContext context,
      Guid teamProjectId,
      string resourceName,
      bool includeFailed = false)
    {
      context.TraceInfo(TracePoints.Provider.CreateConnection, nameof (ServiceEndpointHelper), string.Format("{0}({1},{2})", (object) nameof (GetNextAvailableEndpointName), (object) teamProjectId, (object) resourceName));
      HashSet<string> hashSet = context.GetService<IServiceEndpointService2>().QueryServiceEndpoints(context.Elevate(), teamProjectId, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) null, (string) null, includeFailed).Select<ServiceEndpoint, string>((Func<ServiceEndpoint, string>) (e => e.Name)).Distinct<string>().ToHashSet<string>();
      return ServiceEndpointHelper.GetArtifactName(resourceName, hashSet);
    }

    public static string GetUserLogin(ServiceEndpoint endpoint, string propertyName)
    {
      string str;
      return endpoint != null && endpoint.Data != null && endpoint.Data.TryGetValue(propertyName, out str) ? str : (string) null;
    }

    public static bool TryExtractProviderData(
      IVssRequestContext requestContext,
      Guid projectId,
      IPipelineSourceProvider provider,
      Guid endpointId,
      out string userLogin,
      out string installationId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ServiceEndpoint serviceEndpoint = vssRequestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(vssRequestContext, projectId, endpointId);
      userLogin = ServiceEndpointHelper.GetUserLogin(serviceEndpoint, provider.ExternalApp.UserLoginPropertyName);
      installationId = provider.ConnectionCreator.GetInstallationId(requestContext, serviceEndpoint);
      return !string.IsNullOrEmpty(userLogin) || !string.IsNullOrEmpty(installationId);
    }

    public static void MarkStaleServiceEndpoints(
      IVssRequestContext requestContext,
      Guid projectId,
      IPipelineSourceProvider provider,
      string removedInstallationId,
      IEnumerable<string> schemes = null)
    {
      requestContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (ServiceEndpointHelper), string.Format("{0}({1},{2},{3})", (object) nameof (MarkStaleServiceEndpoints), (object) projectId, (object) provider.ProviderId, (object) removedInstallationId));
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IServiceEndpointService2 service = vssRequestContext.GetService<IServiceEndpointService2>();
      foreach (ServiceEndpoint endpoint in ServiceEndpointHelper.GetProviderEndpointsInternal(vssRequestContext, service, projectId, provider, true, removedInstallationId, schemes: schemes))
      {
        endpoint.Data["installationStatus"] = "uninstalled";
        service.UpdateServiceEndpoint(vssRequestContext, projectId, endpoint, (string) null);
        requestContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (ServiceEndpointHelper), string.Format("{0}- the endpoint {1}, {2} marked as uninstalled.", (object) nameof (MarkStaleServiceEndpoints), (object) endpoint.Id, (object) endpoint.Name));
      }
    }

    public static ServiceEndpoint GetProviderEndpointForUser(
      IVssRequestContext requestContext,
      Guid projectId,
      IPipelineSourceProvider provider,
      string specificInstallationId)
    {
      IEnumerable<ServiceEndpoint> providerEndpoints = ServiceEndpointHelper.GetProviderEndpoints(requestContext.Elevate(), projectId, provider, specificInstallationId: specificInstallationId);
      return providerEndpoints != null && providerEndpoints.Any<ServiceEndpoint>() ? requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(requestContext, projectId, provider.ConnectionCreator.RepositoryType, ServiceEndpointHelper.s_defaultInstallationAuthSchemes, providerEndpoints.Select<ServiceEndpoint, Guid>((Func<ServiceEndpoint, Guid>) (e => e.Id)), (string) null, false, actionFilter: ServiceEndpointActionFilter.Use).FirstOrDefault<ServiceEndpoint>() : (ServiceEndpoint) null;
    }

    public static ServiceEndpoint GetProviderEndpoint(
      IVssRequestContext elevatedContext,
      Guid projectId,
      Guid endpointId,
      IPipelineSourceProvider provider,
      bool ignoreUninstalledEndpoints = true,
      string specificInstallationId = null)
    {
      IServiceEndpointService2 service = elevatedContext.GetService<IServiceEndpointService2>();
      return ServiceEndpointHelper.GetProviderEndpointsInternal(elevatedContext, service, projectId, provider, (ignoreUninstalledEndpoints ? 1 : 0) != 0, specificInstallationId, (IEnumerable<Guid>) new Guid[1]
      {
        endpointId
      }).FirstOrDefault<ServiceEndpoint>();
    }

    public static IEnumerable<ServiceEndpoint> GetProviderEndpoints(
      IVssRequestContext elevatedContext,
      Guid projectId,
      IPipelineSourceProvider provider,
      bool ignoreUninstalledEndpoints = true,
      string specificInstallationId = null)
    {
      IServiceEndpointService2 service = elevatedContext.GetService<IServiceEndpointService2>();
      return ServiceEndpointHelper.GetProviderEndpointsInternal(elevatedContext, service, projectId, provider, ignoreUninstalledEndpoints, specificInstallationId);
    }

    public static IEnumerable<ServiceEndpoint> GetFilteredEndpoints(
      IVssRequestContext userContext,
      Guid projectId,
      IPipelineSourceProvider provider,
      bool elevateAndIncludeDetails,
      IEnumerable<string> schemes = null,
      string dataKey = null,
      string dataValue = null)
    {
      userContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (ServiceEndpointHelper), string.Format("{0}({1},{2})", (object) "GetProviderEndpoints", (object) projectId, (object) provider.ProviderId));
      List<ServiceEndpoint> source1 = userContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(userContext, projectId, provider.ConnectionCreator.RepositoryType, schemes ?? ServiceEndpointHelper.s_defaultInstallationAuthSchemes, (IEnumerable<Guid>) null, (string) null, false, actionFilter: ServiceEndpointActionFilter.Manage);
      userContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (ServiceEndpointHelper), string.Format("{0} - found {1} endpoints before filter.", (object) nameof (GetFilteredEndpoints), (object) source1.Count));
      bool flag = !string.IsNullOrEmpty(dataKey) && !string.IsNullOrEmpty(dataValue);
      List<ServiceEndpoint> source2;
      if (elevateAndIncludeDetails | flag && source1.Any<ServiceEndpoint>())
      {
        IVssRequestContext vssRequestContext = userContext.Elevate();
        source2 = vssRequestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(vssRequestContext, projectId, provider.ConnectionCreator.RepositoryType, (IEnumerable<string>) null, source1.Select<ServiceEndpoint, Guid>((Func<ServiceEndpoint, Guid>) (e => e.Id)), (string) null, false, true);
      }
      else
        source2 = source1;
      if (flag && source2.Any<ServiceEndpoint>())
      {
        source2 = source2.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x =>
        {
          IDictionary<string, string> data = x.Data;
          return string.Equals(data != null ? data.GetValueOrDefault<string, string>(dataKey) : (string) null, dataValue, StringComparison.OrdinalIgnoreCase);
        })).ToList<ServiceEndpoint>();
        userContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (ServiceEndpointHelper), string.Format("{0} - found {1} endpoints after filter.", (object) nameof (GetFilteredEndpoints), (object) source2.Count));
        if (!elevateAndIncludeDetails)
        {
          Dictionary<Guid, ServiceEndpoint> dict = source2.ToDictionary<ServiceEndpoint, Guid>((Func<ServiceEndpoint, Guid>) (e => e.Id));
          return source1.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (ue => dict.ContainsKey(ue.Id)));
        }
      }
      return (IEnumerable<ServiceEndpoint>) source2;
    }

    public static string GetArtifactName(string resourceName, HashSet<string> hash)
    {
      int num = 1;
      string artifactName = PipelineHelper.CreateArtifactName(resourceName);
      while (hash.Contains(artifactName))
      {
        artifactName = PipelineHelper.CreateArtifactName(resourceName, num.ToString());
        ++num;
      }
      return artifactName;
    }

    private static IEnumerable<ServiceEndpoint> GetProviderEndpointsInternal(
      IVssRequestContext elevatedContext,
      IServiceEndpointService2 endpointService,
      Guid projectId,
      IPipelineSourceProvider provider,
      bool ignoreUninstalledEndpoints,
      string specificInstallationId,
      IEnumerable<Guid> endpointIds = null,
      IEnumerable<string> schemes = null)
    {
      elevatedContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (ServiceEndpointHelper), string.Format("{0}({1},{2},{3})", (object) "GetProviderEndpoints", (object) projectId, (object) provider.ProviderId, (object) specificInstallationId));
      List<ServiceEndpoint> source = endpointService.QueryServiceEndpoints(elevatedContext, projectId, provider.ConnectionCreator.RepositoryType, schemes ?? ServiceEndpointHelper.s_defaultInstallationAuthSchemes, endpointIds, (string) null, false, true);
      elevatedContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (ServiceEndpointHelper), string.Format("{0} - found {1} provider endpoints.", (object) "GetProviderEndpoints", (object) source.Count));
      return source.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (endpoint =>
      {
        string a;
        if (!ignoreUninstalledEndpoints || !endpoint.Data.TryGetValue("installationStatus", out a) || !string.Equals(a, "uninstalled"))
          return provider.ConnectionCreator.IsProviderEndpoint(elevatedContext, endpoint, specificInstallationId);
        elevatedContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (ServiceEndpointHelper), string.Format("{0} - the endpoint {1} was uninstalled", (object) "GetProviderEndpoints", (object) endpoint.Id));
        return false;
      }));
    }

    public static class Constants
    {
      public const string InstallationStatus = "installationStatus";
      public const string StatusUninstalled = "uninstalled";
    }
  }
}
