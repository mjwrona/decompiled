// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.IServiceEndpointService2
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  [DefaultServiceImplementation(typeof (FrameworkServiceEndpointService))]
  public interface IServiceEndpointService2 : IVssFrameworkService
  {
    ServiceEndpoint CreateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint);

    void DeleteServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      bool isDeepDelete = true);

    ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null);

    Task<ServiceEndpoint> GetServiceEndpointAsync(
      IVssRequestContext requestContext,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null);

    ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null);

    Task<ServiceEndpoint> GetServiceEndpointAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null);

    List<ServiceEndpoint> QueryServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed,
      bool includeDetails = false,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null);

    List<ServiceEndpoint> QueryServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string namePattern,
      IEnumerable<Guid> createdByFilter,
      int top,
      string owner,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      string continuationToken = null,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null);

    Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed,
      bool includeDetails = false,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null);

    Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<Guid> endpointIds,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthenticationParameters);

    Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<string> endpointNames,
      string owner,
      bool includeFailed,
      bool includeDetails = false,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null);

    ServiceEndpoint UpdateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      string operation);

    void UpdateEndpointAccessToken(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      string accessTokenKey,
      out string errorMessage);

    IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> GetServiceEndpointExecutionRecords(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      int maxCount,
      long continuationToken = 0);

    IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> AddServiceEndpointExecutionRecords(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpointExecutionRecordsInput input);

    void HandleOAuthConfigurationDelete(IVssRequestContext requestContext, Guid configurationId);

    IEnumerable<ServiceEndpoint> UpdateServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<ServiceEndpoint> endpoints);

    void DeleteTeamProject(IVssRequestContext systemRequestContext, Guid projectId);

    bool TryUpgradeServiceEndpointScheme(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string targetAuthorizationScheme,
      UpgradeServiceEndpointSuccessCriteria successCriteria,
      out ServiceEndpoint upgradedEndpoint);
  }
}
