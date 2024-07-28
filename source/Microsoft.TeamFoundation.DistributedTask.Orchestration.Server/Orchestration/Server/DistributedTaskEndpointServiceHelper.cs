// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DistributedTaskEndpointServiceHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class DistributedTaskEndpointServiceHelper
  {
    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint GetServiceEndpointAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId)
    {
      IServiceEndpointService2 endpointService = requestContext.GetService<IServiceEndpointService2>();
      return requestContext.RunSynchronously<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Task<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>) (() => endpointService.GetServiceEndpointAsync(requestContext, scopeIdentifier, endpointId))).ToLegacyServiceEndpoint();
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, endpointId, actionFilter);
      return serviceEndpoint == null ? (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint) null : serviceEndpoint.ToLegacyServiceEndpoint();
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, scopeIdentifier, endpointId, actionFilter);
      return serviceEndpoint == null ? (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint) null : serviceEndpoint.ToLegacyServiceEndpoint();
    }

    public static IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> QueryServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      bool includeFailed,
      bool includeDetails,
      ServiceEndpointActionFilter actionFilter)
    {
      IServiceEndpointService2 endpointService = requestContext.GetService<IServiceEndpointService2>();
      return requestContext.RunSynchronously<IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>((Func<Task<IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>>) (() => endpointService.QueryServiceEndpointsAsync(requestContext, scopeIdentifier, type, authSchemes, endpointIds, (string) null, includeFailed, includeDetails, actionFilter))).FilterEndpoints().Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToLegacyServiceEndpoint()));
    }

    public static IEnumerable<ServiceEndpointProjectReference> FindServiceEndpointProjectReferences(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None)
    {
      return (IEnumerable<ServiceEndpointProjectReference>) requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, scopeIdentifier, endpointId, actionFilter)?.ServiceEndpointProjectReferences ?? (IEnumerable<ServiceEndpointProjectReference>) new List<ServiceEndpointProjectReference>();
    }

    public static IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> QueryServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<Guid> endpointIds,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthenticationParameters)
    {
      IServiceEndpointService2 endpointService = requestContext.GetService<IServiceEndpointService2>();
      return requestContext.RunSynchronously<IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>((Func<Task<IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>>) (() => endpointService.QueryServiceEndpointsAsync(requestContext, scopeIdentifier, endpointIds, refreshAuthenticationParameters))).FilterEndpoints().Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToLegacyServiceEndpoint()));
    }

    public static IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> QueryServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<string> endpointNames,
      bool includeFailed,
      bool includeDetails,
      ServiceEndpointActionFilter actionFilter)
    {
      IServiceEndpointService2 endpointService = requestContext.GetService<IServiceEndpointService2>();
      return requestContext.RunSynchronously<IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>((Func<Task<IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>>) (() => endpointService.QueryServiceEndpointsAsync(requestContext, scopeIdentifier, type, authSchemes, endpointNames, (string) null, includeFailed, includeDetails, actionFilter))).FilterEndpoints().Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToLegacyServiceEndpoint()));
    }

    public static async Task<IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      bool includeFailed,
      bool includeDetails)
    {
      return (await requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpointsAsync(requestContext, scopeIdentifier, type, authSchemes, endpointIds, (string) null, includeFailed, includeDetails)).FilterEndpoints().Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToLegacyServiceEndpoint()));
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint GetAdditionalServiceEndpointDetails(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint,
      IVssRequestContext requestContext)
    {
      if (endpoint.Type.Equals("Azure", StringComparison.InvariantCultureIgnoreCase))
      {
        if (!endpoint.Data.ContainsKey("SubscriptionName"))
          endpoint.Data.Add("SubscriptionName", endpoint.Name);
        if (!endpoint.Data.ContainsKey("Environment"))
          endpoint.Data.Add("Environment", "AzureCloud");
      }
      else if (endpoint.Type.Equals("AzureRM", StringComparison.InvariantCultureIgnoreCase) && !endpoint.Data.ContainsKey("environment"))
        endpoint.Data.Add("environment", "AzureCloud");
      endpoint.PopulateDependencyData(requestContext);
      return endpoint;
    }

    private static void PopulateDependencyData(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint,
      IVssRequestContext requestContext)
    {
      string str1 = "AzureCloud";
      if (endpoint.Data.TryGetValue("environment", out str1))
      {
        if ("AzureStack".Equals(str1, StringComparison.InvariantCultureIgnoreCase))
        {
          try
          {
            using (IEnumerator<KeyValuePair<string, string>> enumerator = endpoint.ToServiceEndpoint().GetAzureStackDependencyData().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<string, string> current = enumerator.Current;
                endpoint.Data[current.Key] = current.Value;
              }
              return;
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceWarning(10015527, "ServiceEndpoint", TaskResources.UnableToPopulateAzureStackData(), (object) ex.ToString());
            return;
          }
        }
      }
      IServiceEndpointTypesService2 service = requestContext.GetService<IServiceEndpointTypesService2>();
      if (endpoint.Authorization == null || endpoint.Authorization.Scheme == null || endpoint.Type == null)
        return;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType serviceEndpointType = service.GetServiceEndpointTypes(requestContext, endpoint.Type, endpoint.Authorization.Scheme).FirstOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>();
      if (serviceEndpointType == null || serviceEndpointType.DependencyData == null)
        return;
      foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData dependencyData in serviceEndpointType.DependencyData)
      {
        string input = dependencyData.Input;
        string str2;
        if (!string.IsNullOrEmpty(input) && endpoint.Data.TryGetValue(input, out str2) && !string.IsNullOrEmpty(str2))
        {
          foreach (KeyValuePair<string, List<KeyValuePair<string, string>>> keyValuePair in dependencyData.Map)
          {
            if (keyValuePair.Key.Equals(str2, StringComparison.InvariantCultureIgnoreCase))
            {
              using (List<KeyValuePair<string, string>>.Enumerator enumerator = keyValuePair.Value.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  KeyValuePair<string, string> current = enumerator.Current;
                  if (!string.IsNullOrEmpty(current.Key))
                    endpoint.Data[current.Key] = current.Value;
                }
                break;
              }
            }
          }
        }
      }
    }

    public static GitHubAuthentication GetGitHubAuthentication(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint legacyServiceEndpoint,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return legacyServiceEndpoint.ToServiceEndpoint().GetGitHubAuthentication(requestContext, projectId);
    }

    public static bool IsBasic(this Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization legacyAuthorization) => legacyAuthorization.ToEndpointAuthorization().IsBasic();

    public static bool IsToken(this Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization legacyAuthorization) => legacyAuthorization.ToEndpointAuthorization().IsToken();

    private static IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> FilterEndpoints(
      this IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> endpoints)
    {
      return endpoints.Where<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (e => !e.IsJiraEndpoint()));
    }

    private static bool IsJiraEndpoint(this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint) => string.Equals(serviceEndpoint?.Type, "Jira", StringComparison.OrdinalIgnoreCase);
  }
}
