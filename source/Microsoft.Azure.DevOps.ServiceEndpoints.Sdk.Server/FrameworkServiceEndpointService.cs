// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.FrameworkServiceEndpointService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class FrameworkServiceEndpointService : IServiceEndpointService2, IVssFrameworkService
  {
    private const string c_layer = "FrameworkServiceEndpointService";

    public ServiceEndpoint CreateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (CreateServiceEndpoint)))
      {
        this.FillServiceEndpointProjectReferencesForEndpoint(endpoint, scopeIdentifier);
        return requestContext.GetClient<ServiceEndpointHttpClient>().CreateServiceEndpointAsync(endpoint).SyncResult<ServiceEndpoint>();
      }
    }

    public ServiceEndpoint UpdateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      string operation)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (UpdateServiceEndpoint)))
      {
        this.FillServiceEndpointProjectReferencesForEndpoint(endpoint, scopeIdentifier);
        return requestContext.GetClient<ServiceEndpointHttpClient>().UpdateServiceEndpointAsync(endpoint.Id, endpoint, operation).SyncResult<ServiceEndpoint>();
      }
    }

    public List<ServiceEndpoint> QueryServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed,
      bool includeDetails,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (QueryServiceEndpoints)))
        return requestContext.GetClient<ServiceEndpointHttpClient>().GetServiceEndpointsAsync(scopeIdentifier, type, authSchemes, endpointIds, owner, new bool?(includeFailed), new bool?(includeDetails)).SyncResult<List<ServiceEndpoint>>();
    }

    public async Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed,
      bool includeDetails,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null)
    {
      IEnumerable<ServiceEndpoint> serviceEndpointsAsync;
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (QueryServiceEndpointsAsync)))
        serviceEndpointsAsync = (IEnumerable<ServiceEndpoint>) await requestContext.GetClient<ServiceEndpointHttpClient>().GetServiceEndpointsAsync(scopeIdentifier, type, authSchemes, endpointIds, owner, new bool?(includeFailed), new bool?(includeDetails));
      return serviceEndpointsAsync;
    }

    public async Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<Guid> endpointIds,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthenticationParameters)
    {
      IEnumerable<ServiceEndpoint> authenticationAsync;
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (QueryServiceEndpointsAsync)))
        authenticationAsync = (IEnumerable<ServiceEndpoint>) await requestContext.GetClient<ServiceEndpointHttpClient>().GetServiceEndpointsWithRefreshedAuthenticationAsync(scopeIdentifier, endpointIds, refreshAuthenticationParameters);
      return authenticationAsync;
    }

    public async Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<string> endpointNames,
      string owner,
      bool includeFailed,
      bool includeDetails,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null)
    {
      IEnumerable<ServiceEndpoint> endpointsByNamesAsync;
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (QueryServiceEndpointsAsync)))
        endpointsByNamesAsync = (IEnumerable<ServiceEndpoint>) await requestContext.GetClient<ServiceEndpointHttpClient>().GetServiceEndpointsByNamesAsync(scopeIdentifier, endpointNames, type, authSchemes, owner, new bool?(includeFailed), new bool?(includeDetails));
      return endpointsByNamesAsync;
    }

    public List<ServiceEndpoint> QueryServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string searchText,
      IEnumerable<Guid> createdByFilter,
      int top,
      string owner,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      string continuationToken = null,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null)
    {
      throw new NotImplementedException();
    }

    public ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (GetServiceEndpoint)))
        return requestContext.GetClient<ServiceEndpointHttpClient>().GetServiceEndpointDetailsAsync(scopeIdentifier, endpointId).SyncResult<ServiceEndpoint>();
    }

    public ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      throw new NotImplementedException();
    }

    public async Task<ServiceEndpoint> GetServiceEndpointAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      ServiceEndpoint endpointDetailsAsync;
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (GetServiceEndpointAsync)))
        endpointDetailsAsync = await requestContext.GetClient<ServiceEndpointHttpClient>().GetServiceEndpointDetailsAsync(scopeIdentifier, endpointId);
      return endpointDetailsAsync;
    }

    public async Task<ServiceEndpoint> GetServiceEndpointAsync(
      IVssRequestContext requestContext,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      TaskCompletionSource<ServiceEndpoint> completionSource = new TaskCompletionSource<ServiceEndpoint>();
      completionSource.SetException((Exception) new NotImplementedException());
      return await completionSource.Task;
    }

    public void DeleteServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      bool isDeepDelete = true)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (DeleteServiceEndpoint)))
      {
        ServiceEndpointHttpClient client = requestContext.GetClient<ServiceEndpointHttpClient>();
        Guid endpointId1 = endpointId;
        List<string> projectIds = new List<string>();
        projectIds.Add(scopeIdentifier.ToString());
        bool? deep = new bool?();
        CancellationToken cancellationToken = new CancellationToken();
        client.DeleteServiceEndpointAsync(endpointId1, (IEnumerable<string>) projectIds, deep, cancellationToken: cancellationToken).SyncResult();
      }
    }

    public void UpdateEndpointAccessToken(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      string accessTokenKey,
      out string errorMessage)
    {
      throw new NotImplementedException();
    }

    public bool TryUpgradeServiceEndpointScheme(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string targetAuthorizationScheme,
      UpgradeServiceEndpointSuccessCriteria successCriteria,
      out ServiceEndpoint upgradedEndpoint)
    {
      throw new NotImplementedException();
    }

    public IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> GetServiceEndpointExecutionRecords(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      int maxCount,
      long continuationToken)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (GetServiceEndpointExecutionRecords)))
        return (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>) requestContext.GetClient<ServiceEndpointHttpClient>().GetServiceEndpointExecutionRecordsAsync(scopeIdentifier, endpointId).SyncResult<PagedList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>>();
    }

    public IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> AddServiceEndpointExecutionRecords(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpointExecutionRecordsInput input)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (AddServiceEndpointExecutionRecords)))
        return (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>) requestContext.GetClient<ServiceEndpointHttpClient>().AddServiceEndpointExecutionRecordsAsync(scopeIdentifier, input).SyncResult<List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>>();
    }

    public void HandleOAuthConfigurationDelete(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
    }

    public IEnumerable<ServiceEndpoint> UpdateServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<ServiceEndpoint> endpoints)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointService), nameof (UpdateServiceEndpoints)))
      {
        this.FillServiceEndpointProjectReferencesForEndpoints(endpoints, scopeIdentifier);
        return (IEnumerable<ServiceEndpoint>) requestContext.GetClient<ServiceEndpointHttpClient>().UpdateServiceEndpointsAsync(endpoints).SyncResult<List<ServiceEndpoint>>();
      }
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid scopeIdentifier) => requestContext.CheckSystemRequestContext();

    private void FillServiceEndpointProjectReferencesForEndpoints(
      IEnumerable<ServiceEndpoint> endpoints,
      Guid scopeIdentifier)
    {
      foreach (ServiceEndpoint endpoint in endpoints)
        this.FillServiceEndpointProjectReferencesForEndpoint(endpoint, scopeIdentifier);
    }

    private void FillServiceEndpointProjectReferencesForEndpoint(
      ServiceEndpoint endpoint,
      Guid scopeIdentifier)
    {
      endpoint.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) new List<ServiceEndpointProjectReference>()
      {
        new ServiceEndpointProjectReference()
        {
          Name = endpoint.Name,
          Description = endpoint.Description,
          ProjectReference = new ProjectReference()
          {
            Id = scopeIdentifier
          }
        }
      };
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
