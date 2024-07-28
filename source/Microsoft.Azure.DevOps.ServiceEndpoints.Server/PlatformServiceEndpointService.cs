// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.PlatformServiceEndpointService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal class PlatformServiceEndpointService : IServiceEndpointService2, IVssFrameworkService
  {
    private const string c_layer = "PlatformServiceEndpointService";
    private const int Max_Update_Count = 5;

    public PlatformServiceEndpointService() => this.ServiceEndpointSecurity = new ServiceEndpointSecurity();

    protected ServiceEndpointSecurity ServiceEndpointSecurity { get; set; }

    public ServiceEndpoint CreateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (CreateServiceEndpoint)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForNull<ServiceEndpoint>(endpoint, nameof (endpoint));
        endpoint.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) new List<ServiceEndpointProjectReference>()
        {
          new ServiceEndpointProjectReference()
          {
            ProjectReference = new ProjectReference()
            {
              Id = scopeIdentifier
            },
            Name = endpoint.Name,
            Description = endpoint.Description
          }
        };
        this.CreateServiceEndpointForCollection(requestContext, endpoint);
        return endpoint;
      }
    }

    public ServiceEndpoint UpdateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      string operation)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (UpdateServiceEndpoint)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForNull<ServiceEndpoint>(endpoint, nameof (endpoint));
        if (endpoint.ServiceEndpointProjectReferences == null || endpoint.ServiceEndpointProjectReferences.Count<ServiceEndpointProjectReference>() == 0)
          endpoint.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) new List<ServiceEndpointProjectReference>()
          {
            new ServiceEndpointProjectReference()
            {
              ProjectReference = new ProjectReference()
              {
                Id = scopeIdentifier
              },
              Name = endpoint.Name,
              Description = endpoint.Description
            }
          };
        return this.UpdateServiceEndpointForCollection(requestContext, endpoint, operation);
      }
    }

    public IEnumerable<ServiceEndpoint> UpdateServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<ServiceEndpoint> endpoints)
    {
      IDictionary<string, ServiceEndpointType> endpointTypesMap = PlatformServiceEndpointTypesService.GetServiceEndpointTypesMap(requestContext);
      return this.UpdateServiceEndpoints(requestContext, scopeIdentifier, endpoints, endpointTypesMap);
    }

    public IEnumerable<ServiceEndpoint> UpdateServiceEndpointsForCollection(
      IVssRequestContext requestContext,
      IEnumerable<ServiceEndpoint> endpoints)
    {
      Guid scopeIdentifier = PlatformServiceEndpointService.ValidateForUpdateServiceEndpoints(endpoints);
      IDictionary<string, ServiceEndpointType> endpointTypesMap = PlatformServiceEndpointTypesService.GetServiceEndpointTypesMap(requestContext);
      return this.UpdateServiceEndpoints(requestContext, scopeIdentifier, endpoints, endpointTypesMap);
    }

    private static Guid ValidateForUpdateServiceEndpoints(IEnumerable<ServiceEndpoint> endpoints)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      if (endpoints == null)
        return Guid.Empty;
      foreach (ServiceEndpoint endpoint in endpoints)
      {
        if (endpoint.ServiceEndpointProjectReferences == null || !endpoint.ServiceEndpointProjectReferences.Any<ServiceEndpointProjectReference>((Func<ServiceEndpointProjectReference, bool>) (i => i.ProjectReference != null)))
          throw new ArgumentException(ServiceEndpointResources.AtleastOneProjectReferenceRequired());
        foreach (ServiceEndpointProjectReference projectReference in (IEnumerable<ServiceEndpointProjectReference>) endpoint.ServiceEndpointProjectReferences)
        {
          if (projectReference.ProjectReference == null)
            throw new ArgumentException(ServiceEndpointResources.ProjectReferenceMissing());
          source.Add(projectReference.ProjectReference.Id);
        }
      }
      return source.Count == 1 ? source.First<Guid>() : throw new ArgumentException(ServiceEndpointResources.MultipleEndpointUpdateShouldBeFromSameProject());
    }

    public IEnumerable<ServiceEndpoint> UpdateServiceEndpoints(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<ServiceEndpoint> endpoints,
      IDictionary<string, ServiceEndpointType> endpointTypesMap)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (UpdateServiceEndpoints)))
      {
        ArgumentUtility.CheckForNull<IEnumerable<ServiceEndpoint>>(endpoints, nameof (endpoints));
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        if (endpoints.Count<ServiceEndpoint>() > 5)
          throw new ArgumentException(string.Format(ServiceEndpointResources.UpdateCountExceeded((object) 5)));
        List<ServiceEndpoint> serviceEndpointList1 = new List<ServiceEndpoint>();
        IEnumerable<Guid> endpointIds = endpoints.Select<ServiceEndpoint, Guid>((Func<ServiceEndpoint, Guid>) (x => x.Id));
        List<ServiceEndpoint> serviceEndpointList2 = this.QueryServiceEndpointsInternal(requestContext, scopeIdentifier, string.Empty, (IEnumerable<string>) new List<string>(), endpointIds, (string) null, true);
        List<Guid> existingEndpointIds = serviceEndpointList2.Select<ServiceEndpoint, Guid>((Func<ServiceEndpoint, Guid>) (x => x.Id)).ToList<Guid>();
        List<ServiceEndpoint> list1 = endpoints.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => existingEndpointIds.Contains(x.Id))).ToList<ServiceEndpoint>();
        List<ServiceEndpoint> list2 = endpoints.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => !existingEndpointIds.Contains(x.Id))).ToList<ServiceEndpoint>();
        this.PreUpdateOperations(requestContext, scopeIdentifier, list1, serviceEndpointList2, endpointTypesMap);
        this.PreCreateOperations(requestContext, scopeIdentifier, list2);
        serviceEndpointList1.AddRange((IEnumerable<ServiceEndpoint>) list1);
        serviceEndpointList1.AddRange((IEnumerable<ServiceEndpoint>) list2);
        this.UpdateServiceEndpointsInternal(requestContext, scopeIdentifier, serviceEndpointList1, endpointTypesMap);
        this.PostUpdateOperations(requestContext, scopeIdentifier, list1, serviceEndpointList2, endpointTypesMap);
        this.PostCreateOperations(requestContext, scopeIdentifier, (IList<ServiceEndpoint>) list2, endpointTypesMap);
        serviceEndpointList1.ConvertGitHubEndpointsForMigrations(requestContext);
        return (IEnumerable<ServiceEndpoint>) serviceEndpointList1;
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
      bool includeDetails = false,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null)
    {
      return requestContext.RunSynchronously<IEnumerable<ServiceEndpoint>>((Func<Task<IEnumerable<ServiceEndpoint>>>) (() => this.QueryServiceEndpointsAsync(requestContext, scopeIdentifier, type, authSchemes, endpointIds, owner, includeFailed, includeDetails, actionFilter, refreshAuthorizationQuery))).ToList<ServiceEndpoint>();
    }

    public async Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed,
      bool includeDetails = false,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null)
    {
      IEnumerable<ServiceEndpoint> serviceEndpoints;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (QueryServiceEndpointsAsync)))
      {
        try
        {
          bool isServicePrincipal = this.ServiceEndpointSecurity.IsCallerServicePrincipal(requestContext, scopeIdentifier.ToString("D"));
          if (!isServicePrincipal)
            ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
          List<ServiceEndpoint> endpoints = (await this.QueryServiceEndpointsInternalAsync(requestContext, scopeIdentifier, type, authSchemes, endpointIds, owner, includeFailed)).ToList<ServiceEndpoint>();
          if (endpoints.Count > 0)
          {
            if (!isServicePrincipal)
            {
              includeDetails = false;
              requestContext.TraceInfo(34001206, nameof (PlatformServiceEndpointService), "Excluded endpoint details for non service principals");
            }
            endpoints = this.FilterServiceEndpointsAndGetDetails(requestContext, scopeIdentifier, endpointIds, (IEnumerable<ServiceEndpoint>) endpoints, includeDetails, actionFilter, refreshAuthorizationQuery).ToList<ServiceEndpoint>();
            await this.PopulateServiceEndpointSharedStatus(requestContext, (IList<ServiceEndpoint>) endpoints);
            endpoints.ResolveIdentityRefs(requestContext);
            endpoints.ConvertGitHubEndpointsForMigrations(requestContext);
          }
          serviceEndpoints = (IEnumerable<ServiceEndpoint>) endpoints;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
          throw;
        }
      }
      return serviceEndpoints;
    }

    public Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<Guid> endpointIds,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthenticationParameters)
    {
      return this.QueryServiceEndpointsAsync(requestContext, scopeIdentifier, (string) null, (IEnumerable<string>) null, endpointIds, (string) null, false, true, ServiceEndpointActionFilter.None, refreshAuthenticationParameters);
    }

    public async Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<string> endpointNames,
      string owner,
      bool includeFailed,
      bool includeDetails = false,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthorizationQuery = null)
    {
      IEnumerable<ServiceEndpoint> serviceEndpoints;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (QueryServiceEndpointsAsync)))
      {
        try
        {
          ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
          List<ServiceEndpoint> endpoints = (await this.QueryServiceEndpointsInternalAsync(requestContext, scopeIdentifier, type, authSchemes, endpointNames, owner, includeFailed)).ToList<ServiceEndpoint>();
          if (!this.ServiceEndpointSecurity.IsCallerServicePrincipal(requestContext, scopeIdentifier.ToString("D")))
          {
            includeDetails = false;
            requestContext.TraceInfo(34001207, nameof (PlatformServiceEndpointService), "Excluded endpoint details for non service principals");
          }
          endpoints = this.FilterServiceEndpointsAndGetDetails(requestContext, scopeIdentifier, (IEnumerable<Guid>) null, (IEnumerable<ServiceEndpoint>) endpoints, includeDetails, actionFilter, refreshAuthorizationQuery).ToList<ServiceEndpoint>();
          await this.PopulateServiceEndpointSharedStatus(requestContext, (IList<ServiceEndpoint>) endpoints);
          endpoints.ResolveIdentityRefs(requestContext);
          endpoints.ConvertGitHubEndpointsForMigrations(requestContext);
          serviceEndpoints = (IEnumerable<ServiceEndpoint>) endpoints;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
          throw;
        }
      }
      return serviceEndpoints;
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
      return requestContext.RunSynchronously<IEnumerable<ServiceEndpoint>>((Func<Task<IEnumerable<ServiceEndpoint>>>) (() => this.QueryServiceEndpointsAsync(requestContext, scopeIdentifier, searchText, createdByFilter, top, owner, actionFilter, continuationToken, refreshAuthorizationQuery))).ToList<ServiceEndpoint>();
    }

    public async Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string searchText,
      IEnumerable<Guid> createdByFilter,
      int top,
      string owner,
      ServiceEndpointActionFilter actionFilter,
      string continuationToken,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthPayload)
    {
      IEnumerable<ServiceEndpoint> serviceEndpoints1;
      try
      {
        MethodScope methodScope = new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (QueryServiceEndpointsAsync));
        List<ServiceEndpoint> result;
        try
        {
          using (ServiceEndpointComponent scs = requestContext.CreateComponent<ServiceEndpointComponent>())
          {
            if (createdByFilter == null)
              createdByFilter = (IEnumerable<Guid>) Array.Empty<Guid>();
            result = await scs.QueryServiceEndpointsAsync(scopeIdentifier, searchText, createdByFilter, 5000, owner, continuationToken);
          }
          await this.PopulateServiceEndpointSharedStatus(requestContext, (IList<ServiceEndpoint>) result);
        }
        finally
        {
          methodScope.Dispose();
        }
        methodScope = new MethodScope();
        if (result != null)
        {
          result = this.FilterServiceEndpointsAndGetDetails(requestContext, scopeIdentifier, (IEnumerable<Guid>) null, (IEnumerable<ServiceEndpoint>) result, actionFilter: actionFilter, refreshAuthPayload: refreshAuthPayload).ToList<ServiceEndpoint>();
          result = result.Take<ServiceEndpoint>(top).ToList<ServiceEndpoint>();
        }
        List<ServiceEndpoint> serviceEndpoints2 = result;
        if (serviceEndpoints2 != null)
          serviceEndpoints2.ConvertGitHubEndpointsForMigrations(requestContext);
        serviceEndpoints1 = (IEnumerable<ServiceEndpoint>) result;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
        throw;
      }
      return serviceEndpoints1;
    }

    public ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      return requestContext.RunSynchronously<ServiceEndpoint>((Func<Task<ServiceEndpoint>>) (() => this.GetServiceEndpointAsync(requestContext, scopeIdentifier, endpointId, actionFilter, refreshAuthenticationParameters)));
    }

    public ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      return requestContext.RunSynchronously<ServiceEndpoint>((Func<Task<ServiceEndpoint>>) (() => this.GetServiceEndpointAsync(requestContext, endpointId, actionFilter, refreshAuthenticationParameters)));
    }

    public async Task<ServiceEndpoint> GetServiceEndpointAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      PlatformServiceEndpointService serviceEndpointService = this;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (GetServiceEndpointAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(endpointId, nameof (endpointId));
        try
        {
          bool canViewEndpoint = false;
          int bitForActionFilter = PlatformServiceEndpointService.GetRequiredEndpointPermissionBitForActionFilter(actionFilter);
          canViewEndpoint = bitForActionFilter != 16 ? serviceEndpointService.ServiceEndpointSecurity.HasPermission(requestContext, scopeIdentifier.ToString("D"), endpointId.ToString("D"), bitForActionFilter, true) : serviceEndpointService.CheckViewEndpointPermission(requestContext, scopeIdentifier, endpointId, true);
          ServiceEndpoint endpoint = await serviceEndpointService.GetServiceEndpointInternalAsync(requestContext, endpointId, scopeIdentifier);
          if (endpoint == null)
            return (ServiceEndpoint) null;
          if (!canViewEndpoint)
          {
            if (actionFilter != ServiceEndpointActionFilter.None)
              return (ServiceEndpoint) null;
            return new ServiceEndpoint()
            {
              Name = endpoint.Name,
              Id = endpoint.Id
            };
          }
          ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpoint, false);
          endpoint = serviceEndpointService.GetEndpointDetails(requestContext, scopeIdentifier, endpoint, serviceEndpointType, refreshAuthenticationParameters);
          await serviceEndpointService.PopulateServiceEndpointSharedStatus(requestContext, (IList<ServiceEndpoint>) new List<ServiceEndpoint>()
          {
            endpoint
          });
          endpoint.ResolveIdentityRefs(requestContext);
          endpoint.ConvertGitHubEndpointsForMigrations(requestContext);
          if (endpoint.IsDisabled && !requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints"))
            endpoint.IsDisabled = false;
          return endpoint;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
          throw;
        }
      }
    }

    public async Task<ServiceEndpoint> GetServiceEndpointAsync(
      IVssRequestContext requestContext,
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      PlatformServiceEndpointService serviceEndpointService = this;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (GetServiceEndpointAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(endpointId, nameof (endpointId));
        try
        {
          serviceEndpointService.ServiceEndpointSecurity.CheckCallerIsServicePrincipal(requestContext, Guid.Empty.ToString("D"));
          Guid scopeIdentifier = Guid.Empty;
          ServiceEndpoint endpoint = await serviceEndpointService.GetServiceEndpointInternalAsync(requestContext, endpointId, scopeIdentifier);
          if (endpoint == null)
            return (ServiceEndpoint) null;
          ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpoint, false);
          endpoint = serviceEndpointService.GetEndpointDetails(requestContext, scopeIdentifier, endpoint, serviceEndpointType, refreshAuthenticationParameters);
          await serviceEndpointService.PopulateServiceEndpointSharedStatus(requestContext, (IList<ServiceEndpoint>) new List<ServiceEndpoint>()
          {
            endpoint
          });
          endpoint.ResolveIdentityRefs(requestContext);
          endpoint.ConvertGitHubEndpointsForMigrations(requestContext);
          if (endpoint.IsDisabled && !requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints"))
            endpoint.IsDisabled = false;
          return endpoint;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
          throw;
        }
      }
    }

    public static void FillServiceEndpointProjectReferencesProjectDetail(
      IVssRequestContext requestContext,
      IList<ServiceEndpoint> serviceEndpoints)
    {
      if (serviceEndpoints.IsNullOrEmpty<ServiceEndpoint>())
        return;
      IEnumerable<ProjectInfo> projects = requestContext.GetService<IProjectService>().GetProjects(requestContext.Elevate(), ProjectState.WellFormed);
      IDictionary<string, Guid> dictionary1 = (IDictionary<string, Guid>) new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IDictionary<Guid, string> dictionary2 = (IDictionary<Guid, string>) new Dictionary<Guid, string>();
      foreach (ProjectInfo projectInfo in projects)
      {
        if (!dictionary1.ContainsKey(projectInfo.Name))
          dictionary1.Add(projectInfo.Name, projectInfo.Id);
        if (!dictionary2.ContainsKey(projectInfo.Id))
          dictionary2.Add(projectInfo.Id, projectInfo.Name);
      }
      foreach (ServiceEndpoint serviceEndpoint in (IEnumerable<ServiceEndpoint>) serviceEndpoints)
      {
        if (serviceEndpoint != null && serviceEndpoint.ServiceEndpointProjectReferences != null)
        {
          List<ServiceEndpointProjectReference> source = new List<ServiceEndpointProjectReference>();
          foreach (ServiceEndpointProjectReference projectReference1 in (IEnumerable<ServiceEndpointProjectReference>) serviceEndpoint.ServiceEndpointProjectReferences)
          {
            ProjectReference projectReference2 = projectReference1.ProjectReference;
            if (projectReference2.Id == Guid.Empty && string.IsNullOrEmpty(projectReference2.Name))
            {
              requestContext.TraceError(34000856, "ServiceEndpoints", ServiceEndpointResources.ServiceEndpointProjectReferenceInformationNotProvided((object) serviceEndpoint.Id));
            }
            else
            {
              if (projectReference2.Id == Guid.Empty && dictionary1.ContainsKey(projectReference2.Name))
              {
                projectReference2.Id = dictionary1[projectReference2.Name];
                projectReference2.Name = dictionary2[projectReference2.Id];
              }
              if (string.IsNullOrEmpty(projectReference2.Name) && dictionary2.ContainsKey(projectReference2.Id))
                projectReference2.Name = dictionary2[projectReference2.Id];
              source.Add(projectReference1);
            }
          }
          serviceEndpoint.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) source;
          serviceEndpoint.IsShared = source.Count<ServiceEndpointProjectReference>() > 1;
        }
      }
    }

    private async Task PopulateServiceEndpointSharedStatus(
      IVssRequestContext requestContext,
      IList<ServiceEndpoint> endpoints)
    {
      if (endpoints.IsNullOrEmpty<ServiceEndpoint>())
        return;
      foreach (IGrouping<Guid, ServiceEndpointProjectReferenceResult> grouping in (await this.QueryServiceEndpointsProjectReferencesInternalAsync(requestContext, Guid.Empty, endpoints.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x != null)).Select<ServiceEndpoint, Guid>((Func<ServiceEndpoint, Guid>) (x => x.Id)))).GroupBy<ServiceEndpointProjectReferenceResult, Guid>((Func<ServiceEndpointProjectReferenceResult, Guid>) (x => x.ServiceEndpointId)))
      {
        IGrouping<Guid, ServiceEndpointProjectReferenceResult> serviceEndpointProjectReferences = grouping;
        ServiceEndpoint serviceEndpoint = endpoints.FirstOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x.Id.Equals(serviceEndpointProjectReferences.Key)));
        if (serviceEndpoint != null)
        {
          serviceEndpoint.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) serviceEndpointProjectReferences.Select<ServiceEndpointProjectReferenceResult, ServiceEndpointProjectReference>((Func<ServiceEndpointProjectReferenceResult, ServiceEndpointProjectReference>) (x => new ServiceEndpointProjectReference()
          {
            Name = x.Name,
            Description = x.Description,
            ProjectReference = new ProjectReference()
            {
              Id = x.ProjectId
            }
          })).ToList<ServiceEndpointProjectReference>();
          if (serviceEndpointProjectReferences.Count<ServiceEndpointProjectReferenceResult>() > 1)
            serviceEndpoint.IsShared = true;
        }
      }
      PlatformServiceEndpointService.FillServiceEndpointProjectReferencesProjectDetail(requestContext, endpoints);
    }

    private bool CheckViewEndpointPermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      bool checkFrameworkReadPermissions = false)
    {
      bool flag1 = this.ServiceEndpointSecurity.HasPermission(requestContext, scopeIdentifier.ToString("D"), endpointId.ToString("D"), 1, true);
      if (!flag1)
        flag1 = this.ServiceEndpointSecurity.HasPermission(requestContext, scopeIdentifier.ToString("D"), endpointId.ToString("D"), 16, true);
      bool flag2 = false;
      if (!flag1)
        flag2 = this.ServiceEndpointSecurity.HasPermission(requestContext, ServiceEndpointSecurity.Collection, endpointId.ToString("D"), 2, true);
      int num = flag1 | flag2 ? 1 : 0;
      if (!(num == 0 & checkFrameworkReadPermissions))
        return num != 0;
      this.ServiceEndpointSecurity.CheckFrameworkReadPermissions(requestContext);
      return num != 0;
    }

    private ServiceEndpoint GetEndpointDetails(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      RefreshAuthenticationParameters refreshAuthenticationParameters = null)
    {
      PlatformServiceEndpointService.MergeEndpointAuthorizationFromStrongBox(requestContext, endpoint);
      PlatformServiceEndpointService.MergeEndpointDataFromStrongBox(requestContext, endpoint);
      IServiceEndpointOperationsExtension operationsExtension = PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpoint);
      operationsExtension.PostGetOperations(requestContext, endpoint, scopeIdentifier, endpointType);
      if (this.ServiceEndpointSecurity.IsCallerServicePrincipal(requestContext, scopeIdentifier.ToString("D"), endpoint.Id.ToString("D")))
      {
        requestContext.TraceInfo(34001203, nameof (PlatformServiceEndpointService), "Auth data refreshing start");
        operationsExtension.RefreshAuthenticationDataIfRequired(requestContext, scopeIdentifier, endpoint, endpointType, refreshAuthenticationParameters);
      }
      else
      {
        requestContext.TraceInfo(34001204, nameof (PlatformServiceEndpointService), "Clearing confidential parameters");
        PlatformServiceEndpointService.ClearConfidentialParameters(endpoint, endpointType);
      }
      return endpoint;
    }

    private static void MergeEndpointAuthorizationFromStrongBox(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      IDictionary<string, string> strongBoxAuthorizationParameters;
      if (endpoint?.Authorization == null || !ServiceEndpointStrongBoxHelper.TryGetAuthorizationParametersFromStrongBox(requestContext, endpoint.Id, out strongBoxAuthorizationParameters))
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) strongBoxAuthorizationParameters)
      {
        if (string.IsNullOrEmpty(keyValuePair.Value))
          requestContext.Trace(34000847, TraceLevel.Warning, "ServiceEndpoints", nameof (PlatformServiceEndpointService), "Strong box null value for auth key='" + keyValuePair.Key + "'");
        string str;
        if (endpoint.Authorization.Parameters.TryGetValue(keyValuePair.Key, out str) && str != null)
          requestContext.Trace(34000847, TraceLevel.Warning, "ServiceEndpoints", nameof (PlatformServiceEndpointService), "Attempt to rewrite not-null value with key='" + keyValuePair.Key + "'");
        endpoint.Authorization.Parameters[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    private static void MergeEndpointDataFromStrongBox(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      Dictionary<string, string> strongBoxEndpointData;
      if (!ServiceEndpointStrongBoxHelper.TryGetEndpointDataFromStrongBox(requestContext, endpoint.Id, out strongBoxEndpointData))
        return;
      foreach (KeyValuePair<string, string> keyValuePair in strongBoxEndpointData)
      {
        if (string.IsNullOrEmpty(keyValuePair.Value))
          requestContext.Trace(34000847, TraceLevel.Warning, "ServiceEndpoints", nameof (PlatformServiceEndpointService), "Strong box null value for secret endpoint data key='" + keyValuePair.Key + "'");
        string str;
        if (endpoint.Data.TryGetValue(keyValuePair.Key, out str) && str != null)
          requestContext.Trace(34000847, TraceLevel.Warning, "ServiceEndpoints", nameof (PlatformServiceEndpointService), "Attempt to rewrite not-null value with key='" + keyValuePair.Key + "'");
        endpoint.Data[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public void DeleteServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      bool isDeepDelete = true)
    {
      IVssRequestContext requestContext1 = requestContext;
      Guid endpointId1 = endpointId;
      List<Guid> projectIdList = new List<Guid>();
      projectIdList.Add(scopeIdentifier);
      int num = isDeepDelete ? 1 : 0;
      this.DeleteServiceEndpointForCollection(requestContext1, endpointId1, projectIdList, num != 0);
    }

    public void DeleteServiceEndpointForCollection(
      IVssRequestContext requestContext,
      Guid endpointId,
      List<Guid> projectIdList,
      bool isDeepDelete = true)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (DeleteServiceEndpointForCollection)))
      {
        ArgumentUtility.CheckForEmptyGuid(endpointId, nameof (endpointId));
        this.DeleteSecurityChecks(requestContext, endpointId, (IList<Guid>) projectIdList);
        ServiceEndpoint endpoint = this.GetServiceEndpointInternal(requestContext, endpointId, Guid.Empty);
        requestContext.RunSynchronously((Func<Task>) (() => this.PopulateServiceEndpointSharedStatus(requestContext, (IList<ServiceEndpoint>) new List<ServiceEndpoint>()
        {
          endpoint
        })));
        if (projectIdList == null || projectIdList.Count == 0)
          projectIdList = endpoint.ServiceEndpointProjectReferences != null ? endpoint.ServiceEndpointProjectReferences.Select<ServiceEndpointProjectReference, Guid>((Func<ServiceEndpointProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>() : new List<Guid>();
        if (endpoint == null)
          return;
        IServiceEndpointOperationsExtension operationsExtension = PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpoint);
        operationsExtension?.PreDeleteOperations(requestContext, endpoint, projectIdList.FirstOrDefault<Guid>());
        if (AzureServicePrincipalHelper.IsSpnAutoCreateEndpoint(endpoint) & isDeepDelete && !endpoint.IsShared)
        {
          ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpoint);
          this.UpdateServiceEndpointInternal(requestContext, projectIdList.FirstOrDefault<Guid>(), endpoint, serviceEndpointType);
          operationsExtension?.PostDeleteOperations(requestContext, endpoint, projectIdList.FirstOrDefault<Guid>());
        }
        else
          this.DeleteEndpointAndStrongBox(requestContext.Elevate(), projectIdList, endpoint);
        foreach (Guid projectId in projectIdList)
          CustomerIntelligenceHelper.PublishServiceEndpointCreatedOrUpdatedOrDeletedTelemetry(requestContext, projectId, endpoint, "DeleteServiceEndpoint");
        foreach (Guid projectId in projectIdList)
        {
          Guid scopeIdentifier = projectId;
          try
          {
            IPipelineResourceAuthorizationProxyService authorizationProxyService = requestContext.GetService<IPipelineResourceAuthorizationProxyService>();
            requestContext.RunSynchronously((Func<Task>) (() => authorizationProxyService.DeletePipelinePermissionsForResource(requestContext.Elevate(), scopeIdentifier, endpointId.ToString(), "endpoint")));
          }
          catch (Exception ex)
          {
            string format = "Deleting pipeline permissions for service endpoint with ID: {0} failed with the following error: {1}";
            requestContext.TraceError(34000849, "ServiceEndpoints", format, (object) endpointId.ToString(), (object) ex.Message);
          }
        }
      }
    }

    private void CheckDeleteEndpointPermission(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId)
    {
      if (scopeIdentifier.Equals(Guid.Empty))
      {
        this.ServiceEndpointSecurity.CheckCallerIsServicePrincipal(requestContext, Guid.Empty.ToString("D"));
      }
      else
      {
        Func<IVssRequestContext, string> getErrorMessageFunc = (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.EndpointAccessDeniedForAdminOperation());
        this.ServiceEndpointSecurity.CheckPermission(requestContext, scopeIdentifier.ToString("D"), endpointId.ToString("D"), 2, false, getErrorMessageFunc);
      }
    }

    public void UpdateEndpointAccessToken(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      string accessTokenKey,
      out string errorMessage)
    {
      requestContext.TraceEnter(0, nameof (PlatformServiceEndpointService), nameof (UpdateEndpointAccessToken));
      errorMessage = (string) null;
      try
      {
        Func<IVssRequestContext, string> getErrorMessageFunc = (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.EndpointAccessDeniedForAdminOperation());
        this.ServiceEndpointSecurity.CheckPermission(requestContext, scopeIdentifier.ToString("D"), endpointId.ToString("D"), 2, false, getErrorMessageFunc);
        IDictionary<string, string> strongBoxAuthorizationParameters;
        if (!ServiceEndpointStrongBoxHelper.TryGetAuthorizationParametersFromStrongBox(requestContext, endpointId, out strongBoxAuthorizationParameters))
          return;
        strongBoxAuthorizationParameters["AccessToken"] = accessTokenKey;
      }
      catch (Exception ex)
      {
        errorMessage = ServiceEndpointResources.UpdateAuthorizationDataFailedError((object) endpointId);
        requestContext.TraceException(34000802, nameof (PlatformServiceEndpointService), ex);
      }
      finally
      {
        requestContext.TraceLeave(0, nameof (PlatformServiceEndpointService), nameof (UpdateEndpointAccessToken));
      }
    }

    public IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> AddServiceEndpointExecutionRecords(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpointExecutionRecordsInput input)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (AddServiceEndpointExecutionRecords)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        PlatformServiceEndpointService.ValidateServiceEndpointExecutionRecordsInput(input);
        if (input.EndpointIds.IsNullOrEmpty<Guid>())
          throw new ArgumentException(ServiceEndpointResources.EndpointIdsIsEmptyError());
        this.ServiceEndpointSecurity.CheckCallerIsServicePrincipal(requestContext, scopeIdentifier.ToString("D"));
        try
        {
          IList<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord> executionRecords = this.AddServiceEndpointExecutionHistoryInternal(requestContext, scopeIdentifier, input);
          CustomerIntelligenceHelper.PublishServiceEndpointExecutionHistory(requestContext, scopeIdentifier, executionRecords);
          return executionRecords.ToContract();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
          throw;
        }
      }
    }

    protected virtual IList<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord> AddServiceEndpointExecutionHistoryInternal(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpointExecutionRecordsInput input)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (AddServiceEndpointExecutionHistoryInternal)))
      {
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
          return component.AddServiceEndpointExecutionHistory(scopeIdentifier, input);
      }
    }

    private static void ValidateServiceEndpointExecutionRecordsInput(
      ServiceEndpointExecutionRecordsInput input)
    {
      ArgumentUtility.CheckForNull<ServiceEndpointExecutionRecordsInput>(input, nameof (input));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData>(input.Data, "Data");
      ArgumentUtility.CheckForNull<string>(input.Data.PlanType, "PlanType");
      ArgumentUtility.CheckForNull<ServiceEndpointExecutionOwner>(input.Data.Definition, "Definition");
      ArgumentUtility.CheckForNull<ServiceEndpointExecutionOwner>(input.Data.Owner, "Owner");
      ArgumentUtility.CheckForNull<DateTime>(input.Data.StartTime, "StartTime");
      ArgumentUtility.CheckForNull<DateTime>(input.Data.FinishTime, "FinishTime");
      ArgumentUtility.CheckForNull<ServiceEndpointExecutionResult>(input.Data.Result, "Result");
    }

    public IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> GetServiceEndpointExecutionRecords(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      int maxCount,
      long continuationToken = 0)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (GetServiceEndpointExecutionRecords)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeIdentifier, nameof (scopeIdentifier));
        ArgumentUtility.CheckForEmptyGuid(endpointId, nameof (endpointId));
        if (!this.CheckViewEndpointPermission(requestContext, scopeIdentifier, endpointId))
          return (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>) Array.Empty<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord>();
        try
        {
          return this.GetServiceEndpointExecutionRecordsInternal(requestContext, scopeIdentifier, endpointId, maxCount, continuationToken);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
          throw;
        }
      }
    }

    protected virtual IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord> GetServiceEndpointExecutionRecordsInternal(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid endpointId,
      int maxCount,
      long continuationToken)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (GetServiceEndpointExecutionRecordsInternal)))
      {
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
          return component.GetServiceEndpointExecutionHistory(scopeIdentifier, endpointId, maxCount, continuationToken).ToContract();
      }
    }

    internal void DeleteEndpointAndStrongBox(
      IVssRequestContext systemRequestContext,
      List<Guid> scopeIdentifiers,
      ServiceEndpoint endpoint)
    {
      using (new MethodScope(systemRequestContext, nameof (PlatformServiceEndpointService), nameof (DeleteEndpointAndStrongBox)))
      {
        bool isEndpointDeleted = this.DeleteServiceEndpointInternal(systemRequestContext, endpoint.Id, scopeIdentifiers);
        this.PostDeleteOperation(systemRequestContext, scopeIdentifiers, endpoint, isEndpointDeleted);
      }
    }

    private void PostDeleteOperation(
      IVssRequestContext systemRequestContext,
      List<Guid> scopeIdentifiers,
      ServiceEndpoint endpoint,
      bool isEndpointDeleted)
    {
      if (isEndpointDeleted)
      {
        if (!endpoint.Type.Equals("AzureRM"))
          PlatformServiceEndpointService.GetEndpointOperationsExtension(systemRequestContext, endpoint)?.PostDeleteOperations(systemRequestContext, endpoint, Guid.Empty);
        Guid drawer = ServiceEndpointStrongBoxHelper.GetDrawer(systemRequestContext, endpoint.Id);
        if (drawer == Guid.Empty)
          systemRequestContext.TraceInfo(nameof (PlatformServiceEndpointService), "No drawer id exists for service endpoint: {0}", (object) endpoint.Id);
        else
          ServiceEndpointStrongBoxHelper.DeleteStrongBoxDrawer(systemRequestContext, drawer);
        try
        {
          this.ServiceEndpointSecurity.RemoveServiceEndpointAccessControlList(systemRequestContext, endpoint.Id, "Collection");
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(34000803, "ResourceService", ex);
        }
      }
      foreach (Guid scopeIdentifier in scopeIdentifiers)
      {
        try
        {
          this.ServiceEndpointSecurity.RemoveServiceEndpointAccessControlList(systemRequestContext, endpoint.Id, scopeIdentifier.ToString());
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(34000803, "ResourceService", ex);
        }
        endpoint.Authorization = (EndpointAuthorization) null;
        systemRequestContext.GetService<ITeamFoundationEventService>().PublishNotification(systemRequestContext, (object) new ServiceEndpointDeletedEvent(scopeIdentifier, endpoint));
      }
    }

    internal void UpdateStrongBox(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (UpdateStrongBox)))
      {
        PlatformServiceEndpointService.ReplaceAuthorizationTokensForOAuth(requestContext, endpoint);
        ServiceEndpointAuthorizationHelper.StoreSecretsInStrongBox(requestContext, endpoint, endpointType, isUpdate);
      }
    }

    private void PreCreateOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      List<ServiceEndpoint> endpointsToCreate)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (PreCreateOperations)))
      {
        foreach (ServiceEndpoint serviceEndpoint in endpointsToCreate)
        {
          this.ProjectSecurityChecks(requestContext, scopeIdentifier, serviceEndpoint.GetServiceEndpointCreator(requestContext), serviceEndpoint.Id);
          this.PreCreateOperations(requestContext, serviceEndpoint, PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, serviceEndpoint));
        }
      }
    }

    private void PreCreateOperations(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (PreCreateOperations)))
      {
        bool isDraft = endpoint.IsDraftEndpoint(requestContext);
        if (!isDraft)
          PlatformServiceEndpointService.SetDefaultValues(endpoint, endpointType);
        new ServiceEndpointValidator(requestContext).ValidateServiceEndpoint(endpoint, (ServiceEndpoint) null, endpointType, false, isDraft);
        PlatformServiceEndpointService.ReplaceAuthorizationTokensForOAuth(requestContext, endpoint);
        endpoint.CreatedBy = this.GetCreatedByUser(requestContext);
        PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpoint)?.PreCreateOperations(requestContext, endpoint);
      }
    }

    private void ProjectSecurityChecks(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Microsoft.VisualStudio.Services.Identity.Identity endpointCreator,
      Guid endpointId)
    {
      this.CheckCreatePermissions(requestContext, scopeIdentifier);
      this.ServiceEndpointSecurity.InitializeServiceEndpointSecurity(requestContext, endpointId, scopeIdentifier, endpointCreator);
    }

    private void PostCreateOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IList<ServiceEndpoint> endpointsToCreate,
      IDictionary<string, ServiceEndpointType> endpointTypesMap)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (PostCreateOperations)))
      {
        foreach (ServiceEndpoint endpoint in (IEnumerable<ServiceEndpoint>) endpointsToCreate)
        {
          ServiceEndpointType endpointType;
          endpointTypesMap.TryGetValue(endpoint.Type, out endpointType);
          this.PostCreateOperations(requestContext, scopeIdentifier, endpoint, endpointType);
        }
      }
    }

    private void PostCreateOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (PostCreateOperations)))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        endpoint.CreatedBy = service.GetIdentity(requestContext, endpoint.CreatedBy.Id).ToIdentityRef(requestContext);
        ServiceEndpointAuthorizationHelper.StoreSecretsInStrongBox(requestContext, endpoint, endpointType, false);
        PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpoint)?.PostCreateOperations(requestContext, endpoint, scopeIdentifier, endpointType);
        PlatformServiceEndpointService.ClearConfidentialParameters(endpoint, endpointType);
        CustomerIntelligenceHelper.PublishServiceEndpointCreatedOrUpdatedOrDeletedTelemetry(requestContext, scopeIdentifier, endpoint, "CreateServiceEndpoint");
      }
    }

    private void PreUpdateOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      List<ServiceEndpoint> endpointsToUpdate,
      List<ServiceEndpoint> existingEndpoints,
      IDictionary<string, ServiceEndpointType> endpointTypesMap)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (PreUpdateOperations)))
      {
        foreach (ServiceEndpoint serviceEndpoint in endpointsToUpdate)
        {
          ServiceEndpoint endpointToUpdate = serviceEndpoint;
          ServiceEndpointType endpointType;
          endpointTypesMap.TryGetValue(endpointToUpdate.Type, out endpointType);
          ServiceEndpoint existingEndpoint = existingEndpoints.FirstOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x.Id == endpointToUpdate.Id));
          this.PreUpdateOperations(requestContext, scopeIdentifier, endpointToUpdate, existingEndpoint, endpointType, string.Empty);
          if (!scopeIdentifier.Equals(Guid.Empty))
          {
            IVssRequestContext requestContext1 = requestContext;
            List<Guid> scopeIdentifiers = new List<Guid>();
            scopeIdentifiers.Add(scopeIdentifier);
            ServiceEndpoint endpoint = endpointToUpdate;
            int num = !endpointToUpdate.Equals((object) existingEndpoint) ? 1 : 0;
            this.UpdateSecurityChecks(requestContext1, scopeIdentifiers, endpoint, num != 0);
          }
        }
      }
    }

    private void PreUpdateOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType,
      string operation)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (PreUpdateOperations)))
      {
        PlatformServiceEndpointService.PrepareNewEndpoint(requestContext, endpoint, existingEndpoint, endpointType);
        PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpoint)?.PreUpdateOperations(requestContext, scopeIdentifier, endpoint, existingEndpoint, operation);
      }
    }

    private static void PrepareNewEndpoint(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType)
    {
      if (existingEndpoint == null)
        throw new ArgumentException(string.Format(ServiceEndpointResources.EndpointNotFound((object) endpoint.Id)));
      if (endpoint.ServiceEndpointProjectReferences != null && endpoint.ServiceEndpointProjectReferences.Count<ServiceEndpointProjectReference>() > 0)
      {
        foreach (ServiceEndpointProjectReference projectReference in (IEnumerable<ServiceEndpointProjectReference>) endpoint.ServiceEndpointProjectReferences)
        {
          if (string.IsNullOrEmpty(projectReference.Name))
            throw new ArgumentException(string.Format(ServiceEndpointResources.ServiceEndpointProjectReferenceNameNull()));
        }
      }
      if (string.IsNullOrEmpty(endpoint.Name))
        endpoint.Name = existingEndpoint.Name;
      if (endpoint.Description == null)
        endpoint.Description = existingEndpoint.Description;
      ServiceEndpointValidator endpointValidator = new ServiceEndpointValidator(requestContext);
      ServiceEndpointType serviceEndpointType = endpointType ?? PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpoint);
      PlatformServiceEndpointService.MergeEndpointDataFromStrongBox(requestContext, existingEndpoint);
      PlatformServiceEndpointService.MergeEndpointAuthorizationFromStrongBox(requestContext, existingEndpoint);
      PlatformServiceEndpointService.PopulateUrlIfRequired(endpoint, serviceEndpointType);
      bool flag = endpoint.IsDraftEndpoint(requestContext);
      ServiceEndpoint endpoint1 = endpoint;
      ServiceEndpoint existingEndpoint1 = existingEndpoint;
      ServiceEndpointType endpointType1 = serviceEndpointType;
      int num = flag ? 1 : 0;
      endpointValidator.ValidateServiceEndpoint(endpoint1, existingEndpoint1, endpointType1, true, num != 0);
      ServiceEndpointValidator.ValidateUrlOrAuthSchemeChange(existingEndpoint, endpoint, endpointType);
      PlatformServiceEndpointService.PatchEndpointData(endpoint, existingEndpoint, endpointType);
      PlatformServiceEndpointService.PatchEndpointAuthorizationParameters(endpoint, existingEndpoint, endpointType);
      if (string.Compare(endpoint.Type, existingEndpoint.Type, true, CultureInfo.InvariantCulture) != 0)
        throw new ArgumentException(string.Format(ServiceEndpointResources.EndpointTypeCannotBeUpdated((object) existingEndpoint.Type)));
    }

    public bool TryUpgradeServiceEndpointScheme(
      IVssRequestContext requestContext,
      ServiceEndpoint existingEndpoint,
      string targetAuthenticationScheme,
      UpgradeServiceEndpointSuccessCriteria successCriteria,
      out ServiceEndpoint upgradedEndpoint)
    {
      upgradedEndpoint = (ServiceEndpoint) null;
      bool flag = false;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (TryUpgradeServiceEndpointScheme)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<ServiceEndpoint>(existingEndpoint, nameof (existingEndpoint));
        ArgumentUtility.CheckStringForNullOrEmpty(targetAuthenticationScheme, nameof (targetAuthenticationScheme));
        ArgumentUtility.CheckForNull<UpgradeServiceEndpointSuccessCriteria>(successCriteria, nameof (successCriteria));
        if (!requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, "ms.vss-distributedtask-web.workload-identity-federation"))
          throw new ServiceEndpointException(ServiceEndpointResources.InvalidSpnOperation((object) "ConvertAuthenticationScheme"));
        PlatformServiceEndpointService.EnsureStatusNotInProgress(existingEndpoint);
        ServiceEndpoint endpoint = existingEndpoint.Clone();
        this.SetupDefaultValues(endpoint);
        this.ValidateOwner(requestContext, endpoint);
        List<Guid> projectReferences = PlatformServiceEndpointService.GetProjectReferences(endpoint);
        this.UpdateSecurityChecks(requestContext, projectReferences, endpoint, true);
        List<string> droppedAuthParameters;
        endpoint.ConvertAuthenticationScheme(requestContext, targetAuthenticationScheme, out droppedAuthParameters);
        ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpoint);
        IServiceEndpointOperationsExtension endpointOperationsExtension = PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpoint);
        IServiceEndpointOperationsExtension operationsExtension = endpointOperationsExtension;
        if ((operationsExtension != null ? (!operationsExtension.SupportsUpgradeBetween(existingEndpoint.Authorization.Scheme, targetAuthenticationScheme) ? 1 : 0) : 1) != 0)
          throw new ArgumentException(ServiceEndpointResources.UpgradeAuthorizationSchemeNotSupported((object) endpoint.Type, (object) existingEndpoint.Authorization.Scheme, (object) targetAuthenticationScheme));
        if (successCriteria.Matches((Func<bool>) (() => endpointOperationsExtension.TryAuthenticateWithServiceEndpoint(requestContext, endpoint))))
        {
          endpoint.OperationStatus = JObject.FromObject((object) new AzureSpnOperationStatus("Ready", ""));
          TimeSpan upOldSecretsDelay = PlatformServiceEndpointService.GetCleanUpOldSecretsDelay(requestContext);
          endpoint.Data["revertSchemeDeadline"] = DateTime.UtcNow.Add(upOldSecretsDelay).ToString("O");
          OidcFederationClaims federationClaims = OidcFederationClaims.CreateNewOidcFederationClaims(requestContext, endpoint);
          endpoint.Authorization.Parameters["workloadIdentityFederationIssuer"] = federationClaims.Issuer;
          endpoint.Authorization.Parameters["workloadIdentityFederationSubject"] = federationClaims.Subject;
          this.UpdateServiceEndpointInternal(requestContext, projectReferences, endpoint, serviceEndpointType);
          IdentityService service = requestContext.GetService<IdentityService>();
          endpoint.CreatedBy = service.GetIdentity(requestContext, endpoint.CreatedBy.Id).ToIdentityRef(requestContext);
          upgradedEndpoint = endpoint;
          ServiceEndpointAuthorizationHelper.StoreAuthorizationSchemeInputsForRecovery(requestContext, existingEndpoint, droppedAuthParameters);
          this.UpdateStrongBox(requestContext, upgradedEndpoint, serviceEndpointType, true);
          requestContext.TraceInfo(34000863, nameof (PlatformServiceEndpointService), "Upgraded scheme for endpoint {0} from {1} to {2}", (object) existingEndpoint.Id, (object) existingEndpoint.Authorization.Scheme, (object) upgradedEndpoint.Authorization.Scheme);
          flag = true;
        }
        else
        {
          upgradedEndpoint = endpoint;
          requestContext.TraceInfo(34000864, nameof (PlatformServiceEndpointService), "Could not convert the scheme for endpoint {0} from {1} to {2}. Endpoint remains unchanged.", (object) existingEndpoint.Id, (object) existingEndpoint.Authorization.Scheme, (object) upgradedEndpoint.Authorization.Scheme);
        }
        PlatformServiceEndpointService.ClearConfidentialParameters(upgradedEndpoint, serviceEndpointType);
        return flag;
      }
    }

    private static TimeSpan GetCleanUpOldSecretsDelay(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return TimeSpan.FromMinutes((double) vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, in PlatformServiceEndpointConstants.s_cleanUpOldSecretsAfterUpgradeDelayInMinutes, 10080));
    }

    private static void EnsureStatusNotInProgress(ServiceEndpoint endpoint)
    {
      if (endpoint.OperationStatus == null)
        return;
      AzureSpnOperationStatus spnOperationStatus = endpoint.OperationStatus.ToObject<AzureSpnOperationStatus>();
      if (spnOperationStatus?.State == "InProgress" && spnOperationStatus.StatusMessage != "converting_scheme")
        throw new InvalidOperationException(ServiceEndpointResources.AnotherOperationAlreadyInProgress((object) spnOperationStatus.StatusMessage));
    }

    private void UpdateSecurityChecks(
      IVssRequestContext requestContext,
      List<Guid> scopeIdentifiers,
      ServiceEndpoint endpoint,
      bool isCollectionEndpointChanged)
    {
      bool flag = this.ServiceEndpointSecurity.HasPermission(requestContext, "Collection", endpoint.Id.ToString("D"), 2);
      if (isCollectionEndpointChanged && !flag && requestContext.IsFeatureEnabled("ServiceEndpoints.NewServiceEndpointAPIs"))
        throw new AccessCheckException(ServiceEndpointResources.ServiceEndpointAccessDeniedForCollectionAdminOperation());
      if (flag)
        return;
      foreach (Guid scopeIdentifier in scopeIdentifiers)
      {
        Func<IVssRequestContext, string> getErrorMessageFunc = (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.EndpointAccessDeniedForAdminOperation());
        this.ServiceEndpointSecurity.CheckPermission(requestContext, scopeIdentifier.ToString("D"), endpoint.Id.ToString("D"), 2, false, getErrorMessageFunc);
      }
    }

    private void PostUpdateOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      List<ServiceEndpoint> endpointsToUpdate,
      List<ServiceEndpoint> existingEndpoints,
      IDictionary<string, ServiceEndpointType> endpointTypesMap)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (PostUpdateOperations)))
      {
        foreach (ServiceEndpoint serviceEndpoint in endpointsToUpdate)
        {
          ServiceEndpoint endpointToUpdate = serviceEndpoint;
          ServiceEndpointType endpointType;
          endpointTypesMap.TryGetValue(endpointToUpdate.Type, out endpointType);
          this.PostUpdateOperations(requestContext, scopeIdentifier, endpointToUpdate, existingEndpoints.Single<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x.Id == endpointToUpdate.Id)), endpointType, string.Empty);
          CustomerIntelligenceHelper.PublishServiceEndpointCreatedOrUpdatedOrDeletedTelemetry(requestContext, scopeIdentifier, endpointToUpdate, "UpdateServiceEndpoint");
        }
      }
    }

    private void PostUpdateOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType,
      string operation)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (PostUpdateOperations)))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        endpoint.CreatedBy = service.GetIdentity(requestContext, existingEndpoint.CreatedBy.Id).ToIdentityRef(requestContext);
        this.UpdateStrongBox(requestContext, endpoint, endpointType, true);
        PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpoint)?.PostUpdateOperations(requestContext, endpoint, existingEndpoint, scopeIdentifier, operation, endpointType);
        PlatformServiceEndpointService.ClearConfidentialParameters(endpoint, endpointType);
      }
    }

    private static void PatchEndpointData(
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType)
    {
      if (existingEndpoint == null || endpointType?.InputDescriptors == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) existingEndpoint.Data)
      {
        InputDescriptor inputDescriptor;
        endpointType.TryGetInputDescriptor(keyValuePair.Key, out inputDescriptor);
        if (inputDescriptor != null && inputDescriptor.IsConfidential && endpoint.Data.ContainsKey(inputDescriptor.Id) && endpoint.Data[inputDescriptor.Id] == null)
          endpoint.Data[inputDescriptor.Id] = keyValuePair.Value;
      }
    }

    private static void PatchEndpointAuthorizationParameters(
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType)
    {
      List<InputDescriptor> inputDescriptors;
      if (existingEndpoint == null || !(endpoint.Authorization.Scheme == existingEndpoint.Authorization.Scheme) || endpoint.Authorization.Parameters == null || endpointType == null || !endpointType.TryGetAuthInputDescriptors(endpoint.Authorization.Scheme, out inputDescriptors))
        return;
      foreach (InputDescriptor inputDescriptor in inputDescriptors)
      {
        if (inputDescriptor != null && inputDescriptor.IsConfidential && endpoint.Authorization.Parameters.ContainsKey(inputDescriptor.Id) && endpoint.Authorization.Parameters[inputDescriptor.Id] == null && existingEndpoint.Authorization.Parameters.ContainsKey(inputDescriptor.Id))
          endpoint.Authorization.Parameters[inputDescriptor.Id] = existingEndpoint.Authorization.Parameters[inputDescriptor.Id];
      }
    }

    private static void SetDefaultValues(
      ServiceEndpoint serviceEndpoint,
      ServiceEndpointType endpointType)
    {
      if (endpointType?.InputDescriptors == null)
        return;
      foreach (InputDescriptor inputDescriptor in endpointType.InputDescriptors)
      {
        if (inputDescriptor.Values?.DefaultValue != null && !serviceEndpoint.Data.ContainsKey(inputDescriptor.Id))
          serviceEndpoint.Data.Add(new KeyValuePair<string, string>(inputDescriptor.Id, inputDescriptor.Values.DefaultValue));
      }
      PlatformServiceEndpointService.PopulateUrlIfRequired(serviceEndpoint, endpointType);
    }

    private static void PopulateUrlIfRequired(
      ServiceEndpoint serviceEndpoint,
      ServiceEndpointType serviceEndpointType)
    {
      if (serviceEndpoint.Authorization.IsOauth2() && EndpointAuthorizationExtensions.RequiresOAuth2Configuration(serviceEndpointType))
      {
        serviceEndpoint.Url = (Uri) null;
      }
      else
      {
        bool flag = serviceEndpointType?.EndpointUrl != null && !serviceEndpointType.EndpointUrl.IsVisible.IsNullOrEmpty<char>() && serviceEndpointType.EndpointUrl.IsVisible.Equals("false", StringComparison.InvariantCultureIgnoreCase);
        if ((serviceEndpointType?.EndpointUrl?.DependsOn == null || serviceEndpointType.EndpointUrl.DependsOn.Input.IsNullOrEmpty<char>() ? 0 : (!serviceEndpointType.EndpointUrl.DependsOn.Map.IsNullOrEmpty<DependencyBinding>() ? 1 : 0)) != 0)
          PlatformServiceEndpointService.PopulateUrlFromDependsOnProperties(serviceEndpoint, serviceEndpointType);
        else if (flag)
          PlatformServiceEndpointService.PopulateUrlFromEndpointType(serviceEndpoint, serviceEndpointType);
      }
      PlatformServiceEndpointService.SanitizeEndpointUrlIfRequired(serviceEndpoint);
    }

    private static void SanitizeEndpointUrlIfRequired(ServiceEndpoint serviceEndpoint)
    {
      Uri result;
      if (!(serviceEndpoint.Url != (Uri) null) || string.IsNullOrEmpty(serviceEndpoint.Url.OriginalString) || !Uri.TryCreate(serviceEndpoint.Url.OriginalString.Trim(), UriKind.Absolute, out result))
        return;
      serviceEndpoint.Url = result;
    }

    private static void PopulateUrlFromDependsOnProperties(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      string dependantValue;
      endpoint.Data.TryGetValue(endpointType.EndpointUrl.DependsOn.Input, out dependantValue);
      if (dependantValue.IsNullOrEmpty<char>())
        return;
      string uriString = endpointType.EndpointUrl.DependsOn.Map.FirstOrDefault<DependencyBinding>((Func<DependencyBinding, bool>) (i => i.Key == dependantValue))?.Value;
      Uri result;
      if (uriString == null || !Uri.TryCreate(uriString, UriKind.Absolute, out result) || result.Equals((object) endpoint.Url))
        return;
      endpoint.Url = result;
    }

    private static void PopulateUrlFromEndpointType(
      ServiceEndpoint endpoint,
      ServiceEndpointType serviceEndpointType)
    {
      if (serviceEndpointType == null)
        return;
      if (serviceEndpointType.EndpointUrl != null)
      {
        if (!string.IsNullOrEmpty(serviceEndpointType.EndpointUrl.Format))
        {
          EndpointStringResolver endpointStringResolver = new EndpointStringResolver(EndpointMustacheHelper.CreateReplacementContext(endpoint, (Dictionary<string, string>) null, Guid.Empty, initialContextTemplate: string.Empty, serviceEndpointType: serviceEndpointType));
          try
          {
            endpoint.Url = new Uri(endpointStringResolver.ResolveVariablesInMustacheFormat(serviceEndpointType.EndpointUrl.Format));
          }
          catch
          {
            if (!(endpoint.Url == (Uri) null) && !string.IsNullOrEmpty(endpoint.Url.AbsoluteUri))
              return;
            string lower = serviceEndpointType.EndpointUrl.Format.ToLower();
            endpoint.Url = new Uri(endpointStringResolver.ResolveVariablesInMustacheFormat(lower));
          }
        }
        else
          endpoint.Url = serviceEndpointType.EndpointUrl?.Value;
      }
      else
      {
        if (!(serviceEndpointType.EndpointUrl?.Value != (Uri) null) || serviceEndpointType.EndpointUrl.Value.Equals((object) endpoint.Url))
          return;
        endpoint.Url = serviceEndpointType.EndpointUrl.Value;
      }
    }

    private static void ReplaceAuthorizationTokensForOAuth(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      if (!endpoint.Authorization.IsOauth() && !endpoint.Authorization.IsOauth2())
        return;
      string str;
      string key;
      if (VssStringComparer.ServiceEndpointTypeCompararer.Equals(endpoint.Type, "Bitbucket"))
      {
        str = BitbucketConstants.StrongBox.OAuthCallbackDrawerName;
        key = "RefreshToken";
      }
      else if (string.Equals(endpoint.Authorization.Scheme, "OAuth2", StringComparison.OrdinalIgnoreCase) || VssStringComparer.ServiceEndpointTypeCompararer.Equals(endpoint.Type, "GitHub"))
      {
        str = "OAuth2Callback";
        key = "AccessToken";
      }
      else
      {
        if (!VssStringComparer.ServiceEndpointTypeCompararer.Equals(endpoint.Type, "GitHubBoards"))
          return;
        str = "GitHubOauthCallback";
        key = "AccessToken";
      }
      string a;
      if (!endpoint.Authorization.Parameters.TryGetValue("OAuthAccessTokenIsSupplied", out a) || !string.Equals(a, bool.TrueString, StringComparison.OrdinalIgnoreCase))
      {
        string lookUpKey;
        if (!endpoint.Authorization.Parameters.TryGetValue("AccessToken", out lookUpKey))
          throw new ArgumentException(ServiceEndpointResources.OauthArgumentError());
        string strongBoxContent = ServiceEndpointStrongBoxHelper.GetStrongBoxContent(requestContext, str, lookUpKey);
        endpoint.Authorization.Parameters.Remove("AccessToken");
        OAuth2TokenResult oauth2TokenResult;
        if (JsonUtilities.TryDeserialize<OAuth2TokenResult>(strongBoxContent, out oauth2TokenResult))
        {
          endpoint.Authorization.Parameters["AccessToken"] = oauth2TokenResult.AccessToken;
          endpoint.Authorization.Parameters["IssuedAt"] = DateTime.UtcNow.ToString();
          if (!string.IsNullOrEmpty(oauth2TokenResult.RefreshToken))
          {
            endpoint.Authorization.Parameters["RefreshToken"] = oauth2TokenResult.RefreshToken;
            endpoint.Authorization.Parameters["ExpiresIn"] = oauth2TokenResult.ExpiresIn;
          }
          else
            requestContext.Trace(34000847, TraceLevel.Warning, "ServiceEndpoints", nameof (PlatformServiceEndpointService), "Empty OAuth2 refreshToken during replace operation");
        }
        else
          endpoint.Authorization.Parameters[key] = strongBoxContent;
        ServiceEndpointStrongBoxHelper.DeleteStrongBoxDrawer(requestContext, str);
      }
      else
        endpoint.Authorization.Parameters.Remove("OAuthAccessTokenIsSupplied");
    }

    private IdentityRef GetCreatedByUser(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return new IdentityRef()
      {
        Id = userIdentity.Id.ToString("D")
      };
    }

    internal static ProjectInfo GetProjectInfo(
      IVssRequestContext requestContext,
      string projFilter,
      bool throwOnProjectNotFound)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projFilter, nameof (projFilter));
      IProjectService service = requestContext.GetService<IProjectService>();
      Guid result;
      ProjectInfo projectInfo;
      if (!Guid.TryParse(projFilter, out result))
      {
        try
        {
          projectInfo = service.GetProject(requestContext, projFilter);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          projectInfo = (ProjectInfo) null;
        }
      }
      else
      {
        try
        {
          projectInfo = service.GetProject(requestContext, result);
        }
        catch (ProjectDoesNotExistException ex)
        {
          projectInfo = (ProjectInfo) null;
        }
      }
      return !(projectInfo == null & throwOnProjectNotFound) ? projectInfo : throw new ProjectDoesNotExistException();
    }

    public void CheckCreatePermissions(IVssRequestContext requestContext, Guid scopeIdentifier)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (CheckCreatePermissions)))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        if (!this.QueryServiceEndpointsInternal(requestContext.Elevate(), scopeIdentifier, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) null, (string) null, true).ToList<ServiceEndpoint>().Any<ServiceEndpoint>())
        {
          IdentityScope scope = service.GetScope(requestContext, scopeIdentifier);
          this.ServiceEndpointSecurity.ProvisionProjectLevelGroups(requestContext, service, scope, scopeIdentifier);
        }
        Func<IVssRequestContext, string> getErrorMessageFunc = (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.EndpointAccessDeniedForCreate());
        this.ServiceEndpointSecurity.CheckCreateEndpointPermission(requestContext, scopeIdentifier.ToString("D"), string.Empty, true, getErrorMessageFunc);
      }
    }

    protected virtual ServiceEndpoint AddServiceEndpointInternal(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      IVssRequestContext requestContext1 = requestContext;
      List<Guid> scopeIdentifiers = new List<Guid>();
      scopeIdentifiers.Add(scopeIdentifier);
      ServiceEndpoint endpoint1 = endpoint;
      ServiceEndpointType endpointType1 = endpointType;
      this.AddServiceEndpointInternal(requestContext1, scopeIdentifiers, endpoint1, endpointType1);
      return endpoint;
    }

    protected virtual ServiceEndpoint AddServiceEndpointInternal(
      IVssRequestContext requestContext,
      List<Guid> scopeIdentifiers,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      ServiceEndpoint cloneWithOutSecrets = endpoint.GetEndpointCloneWithOutSecrets(endpointType);
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (AddServiceEndpointInternal)))
      {
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
          component.AddServiceEndpoint(scopeIdentifiers, cloneWithOutSecrets, true);
        return endpoint;
      }
    }

    public virtual ServiceEndpoint UpdateServiceEndpointInternal(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      List<Guid> guidList;
      if (!(scopeIdentifier != Guid.Empty))
      {
        guidList = new List<Guid>();
      }
      else
      {
        guidList = new List<Guid>();
        guidList.Add(scopeIdentifier);
      }
      List<Guid> scopeIdentifiers = guidList;
      if (scopeIdentifier != Guid.Empty)
        endpoint.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) new List<ServiceEndpointProjectReference>()
        {
          new ServiceEndpointProjectReference()
          {
            Description = endpoint.Description,
            Name = endpoint.Name,
            ProjectReference = new ProjectReference()
            {
              Id = scopeIdentifier
            }
          }
        };
      return this.UpdateServiceEndpointInternal(requestContext, scopeIdentifiers, endpoint, endpointType);
    }

    public virtual ServiceEndpoint UpdateServiceEndpointInternal(
      IVssRequestContext requestContext,
      List<Guid> scopeIdentifiers,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      ServiceEndpoint cloneWithOutSecrets = endpoint.GetEndpointCloneWithOutSecrets(endpointType);
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (UpdateServiceEndpointInternal)))
      {
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
          component.UpdateServiceEndpoint(scopeIdentifiers, cloneWithOutSecrets, true);
        endpoint.ConvertGitHubEndpointsForMigrations(requestContext);
        return endpoint;
      }
    }

    public virtual List<ServiceEndpoint> UpdateServiceEndpointsInternal(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      List<ServiceEndpoint> endpoints,
      IDictionary<string, ServiceEndpointType> endpointTypesMap)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (UpdateServiceEndpointsInternal)))
      {
        List<ServiceEndpoint> cloneWithOutSecrets = PlatformServiceEndpointService.GetEndpointsCloneWithOutSecrets(endpoints, endpointTypesMap);
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
          component.UpdateServiceEndpoints(scopeIdentifier, cloneWithOutSecrets, true);
        return endpoints;
      }
    }

    protected virtual List<ServiceEndpoint> QueryServiceEndpointsInternal(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed)
    {
      return requestContext.RunSynchronously<IEnumerable<ServiceEndpoint>>((Func<Task<IEnumerable<ServiceEndpoint>>>) (() => this.QueryServiceEndpointsInternalAsync(requestContext, scopeIdentifier, type, authSchemes, endpointIds, owner, includeFailed))).ToList<ServiceEndpoint>();
    }

    protected virtual async Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsInternalAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed)
    {
      if (!authSchemes.IsNullOrEmpty<string>() && authSchemes.Contains<string>("OAuth", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && !authSchemes.Contains<string>("OAuth2", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        authSchemes = authSchemes.Union<string>((IEnumerable<string>) new List<string>()
        {
          "OAuth2"
        });
      if (!authSchemes.IsNullOrEmpty<string>() && authSchemes.Contains<string>("PersonalAccessToken", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && !authSchemes.Contains<string>("Token", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        authSchemes = authSchemes.Union<string>((IEnumerable<string>) new List<string>()
        {
          "Token"
        });
      IEnumerable<ServiceEndpoint> serviceEndpointsAsync;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (QueryServiceEndpointsInternalAsync)))
      {
        using (ServiceEndpointComponent scs = requestContext.CreateComponent<ServiceEndpointComponent>())
          serviceEndpointsAsync = (IEnumerable<ServiceEndpoint>) await scs.GetServiceEndpointsAsync(scopeIdentifier, type, authSchemes, endpointIds, owner, includeFailed);
      }
      return serviceEndpointsAsync;
    }

    protected virtual async Task<List<ServiceEndpointProjectReferenceResult>> QueryServiceEndpointsProjectReferencesInternalAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<Guid> endpointIds)
    {
      List<ServiceEndpointProjectReferenceResult> projectReferenceResultList;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (QueryServiceEndpointsProjectReferencesInternalAsync)))
      {
        using (ServiceEndpointComponent scs = requestContext.CreateComponent<ServiceEndpointComponent>())
          projectReferenceResultList = await scs.QueryServiceEndpointSharedProjectsAsync(endpointIds, scopeIdentifier);
      }
      return projectReferenceResultList;
    }

    protected virtual async Task<IEnumerable<ServiceEndpoint>> QueryServiceEndpointsInternalAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<string> endpointNames,
      string owner,
      bool includeFailed)
    {
      IEnumerable<ServiceEndpoint> serviceEndpointsAsync;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (QueryServiceEndpointsInternalAsync)))
      {
        using (ServiceEndpointComponent scs = requestContext.CreateComponent<ServiceEndpointComponent>())
          serviceEndpointsAsync = (IEnumerable<ServiceEndpoint>) await scs.GetServiceEndpointsAsync(scopeIdentifier, type, authSchemes, endpointNames, owner, includeFailed);
      }
      return serviceEndpointsAsync;
    }

    protected virtual ServiceEndpoint GetServiceEndpointInternal(
      IVssRequestContext requestContext,
      Guid endpointId,
      Guid scopeIdentifier)
    {
      return requestContext.RunSynchronously<ServiceEndpoint>((Func<Task<ServiceEndpoint>>) (() => this.GetServiceEndpointInternalAsync(requestContext, endpointId, scopeIdentifier)));
    }

    protected virtual async Task<ServiceEndpoint> GetServiceEndpointInternalAsync(
      IVssRequestContext requestContext,
      Guid endpointId,
      Guid scopeIdentifier)
    {
      ServiceEndpoint serviceEndpointAsync;
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (GetServiceEndpointInternalAsync)))
      {
        using (ServiceEndpointComponent scs = requestContext.CreateComponent<ServiceEndpointComponent>())
          serviceEndpointAsync = await scs.GetServiceEndpointAsync(endpointId, scopeIdentifier);
      }
      return serviceEndpointAsync;
    }

    protected virtual bool DeleteServiceEndpointInternal(
      IVssRequestContext requestContext,
      Guid endpointId,
      Guid scopeIdentifier)
    {
      return this.DeleteServiceEndpointInternal(requestContext, endpointId, new List<Guid>()
      {
        scopeIdentifier
      });
    }

    protected virtual bool DeleteServiceEndpointInternal(
      IVssRequestContext requestContext,
      Guid endpointId,
      List<Guid> scopeIdentifiers)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (DeleteServiceEndpointInternal)))
      {
        ServiceEndpoint serviceDeleted;
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
          serviceDeleted = component.DeleteServiceEndpoint(endpointId, scopeIdentifiers);
        int num = serviceDeleted != null ? 1 : 0;
        if (num != 0)
          PlatformServiceEndpointService.DeleteScope(requestContext, serviceDeleted);
        return num != 0;
      }
    }

    private static List<ServiceEndpoint> GetEndpointsCloneWithOutSecrets(
      List<ServiceEndpoint> endpoints,
      IDictionary<string, ServiceEndpointType> endpointTypesMap)
    {
      List<ServiceEndpoint> cloneWithOutSecrets1 = new List<ServiceEndpoint>();
      if (endpoints != null && endpointTypesMap != null)
      {
        foreach (ServiceEndpoint endpoint in endpoints)
        {
          ServiceEndpointType endpointType;
          endpointTypesMap.TryGetValue(endpoint.Type, out endpointType);
          ServiceEndpoint cloneWithOutSecrets2 = endpoint.GetEndpointCloneWithOutSecrets(endpointType);
          cloneWithOutSecrets1.Add(cloneWithOutSecrets2);
        }
      }
      return cloneWithOutSecrets1;
    }

    private static void DeleteScope(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceDeleted)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (DeleteScope)))
      {
        if (!(serviceDeleted.GroupScopeId != Guid.Empty))
          return;
        try
        {
          requestContext.GetService<IdentityService>().DeleteScope(requestContext.Elevate(), serviceDeleted.GroupScopeId);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
        }
      }
    }

    private static void ClearConfidentialParameters(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      endpoint.ClearConfidentialDataEntries(endpointType);
      endpoint.ClearConfidentialAuthorizationParameters(endpointType);
    }

    private IEnumerable<ServiceEndpoint> FilterServiceEndpointsAndGetDetails(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      IEnumerable<Guid> requestedEndpointIds,
      IEnumerable<ServiceEndpoint> existingEndpoints,
      bool getDetails = false,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None,
      IEnumerable<RefreshAuthenticationParameters> refreshAuthPayload = null)
    {
      int permissions = PlatformServiceEndpointService.GetRequiredEndpointPermissionBitForActionFilter(actionFilter);
      requestContext.TraceConditionally(34001208, TraceLevel.Verbose, "ServiceEndpoints", nameof (PlatformServiceEndpointService), (Func<string>) (() => string.Format("Requested {0} permissions to filter endpoints.", (object) permissions)));
      bool hasFrameworkReadPermissions = this.ServiceEndpointSecurity.HasFrameworkReadPermissions(requestContext);
      IDictionary<string, ServiceEndpointType> serviceEndpointTypeMap = PlatformServiceEndpointTypesService.GetServiceEndpointTypesMap(requestContext);
      return existingEndpoints.Select<ServiceEndpoint, ServiceEndpoint>((Func<ServiceEndpoint, ServiceEndpoint>) (endpoint =>
      {
        if (endpoint.IsDisabled)
        {
          if (requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints"))
          {
            if (actionFilter == ServiceEndpointActionFilter.Use)
              return (ServiceEndpoint) null;
          }
          else
            endpoint.IsDisabled = false;
        }
        if (requestContext.IsFeatureEnabled(BitbucketFeatureFlags.BitbucketAzurePipelinesBackupOAuthClient))
          endpoint.IsOutdated = PlatformServiceEndpointService.IsOutdatedConnection(endpoint.Type, endpoint.Authorization);
        if (PlatformServiceEndpointService.IsAzureBoardsGitHubIntegrationEndpointType(endpoint.Type))
        {
          IEnumerable<Guid> source = requestedEndpointIds;
          if ((source != null ? (source.Contains<Guid>(endpoint.Id) ? 1 : 0) : 0) == 0)
            return (ServiceEndpoint) null;
        }
        bool hasEndpointPermissions = permissions != 16 ? this.ServiceEndpointSecurity.HasPermission(requestContext, scopeIdentifier.ToString("D"), endpoint.Id.ToString("D"), permissions, true) : this.CheckViewEndpointPermission(requestContext, scopeIdentifier, endpoint.Id);
        bool isUserCollectionLevelAdmin = false;
        if (!hasEndpointPermissions)
          isUserCollectionLevelAdmin = this.ServiceEndpointSecurity.HasPermission(requestContext, ServiceEndpointSecurity.Collection, endpoint.Id.ToString("D"), 2, true);
        requestContext.TraceConditionally(34001211, TraceLevel.Verbose, "ServiceEndpoints", nameof (PlatformServiceEndpointService), (Func<string>) (() => string.Format("Endpoint {0}: {1} = {2}, ", (object) endpoint.Id, (object) "hasEndpointPermissions", (object) hasEndpointPermissions) + string.Format("{0} = {1}, ", (object) "hasFrameworkReadPermissions", (object) hasFrameworkReadPermissions) + string.Format("{0} = {1}.", (object) "isUserCollectionLevelAdmin", (object) isUserCollectionLevelAdmin)));
        if (!hasEndpointPermissions && !isUserCollectionLevelAdmin)
        {
          if (hasFrameworkReadPermissions)
          {
            IEnumerable<Guid> source = requestedEndpointIds;
            if ((source != null ? (source.Contains<Guid>(endpoint.Id) ? 1 : 0) : 0) != 0 && actionFilter == ServiceEndpointActionFilter.None)
            {
              requestContext.TraceConditionally(34001209, TraceLevel.Verbose, "ServiceEndpoints", nameof (PlatformServiceEndpointService), (Func<string>) (() => string.Format("Using shallow endpoint {0}.", (object) endpoint.Id)));
              return new ServiceEndpoint()
              {
                Id = endpoint.Id,
                Name = endpoint.Name
              };
            }
          }
          return (ServiceEndpoint) null;
        }
        ServiceEndpointType serviceEndpointType;
        serviceEndpointTypeMap.TryGetValue(endpoint.Type, out serviceEndpointType);
        if (getDetails)
        {
          requestContext.TraceConditionally(34001210, TraceLevel.Verbose, "ServiceEndpoints", nameof (PlatformServiceEndpointService), (Func<string>) (() => string.Format("Loading endpoint {0} details.", (object) endpoint.Id)));
          try
          {
            IVssRequestContext requestContext1 = requestContext;
            Guid scopeIdentifier1 = scopeIdentifier;
            ServiceEndpoint endpoint1 = endpoint;
            ServiceEndpointType endpointType = serviceEndpointType;
            IEnumerable<RefreshAuthenticationParameters> source = refreshAuthPayload;
            RefreshAuthenticationParameters refreshAuthenticationParameters = source != null ? source.FirstOrDefault<RefreshAuthenticationParameters>((Func<RefreshAuthenticationParameters, bool>) (authData => authData.EndpointId == endpoint.Id)) : (RefreshAuthenticationParameters) null;
            return this.GetEndpointDetails(requestContext1, scopeIdentifier1, endpoint1, endpointType, refreshAuthenticationParameters).GetAdditionalServiceEndpointDetails(requestContext, serviceEndpointType);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
            return (ServiceEndpoint) null;
          }
        }
        else
        {
          requestContext.TraceConditionally(34001205, TraceLevel.Info, "ServiceEndpoints", nameof (PlatformServiceEndpointService), (Func<string>) (() => string.Format("Filtering endpoint {0} data without details", (object) endpoint.Id)));
          PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpoint).PostGetOperations(requestContext, endpoint, scopeIdentifier, serviceEndpointType);
          PlatformServiceEndpointService.ClearConfidentialParameters(endpoint, serviceEndpointType);
          return endpoint;
        }
      })).Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (endpoint => endpoint != null));
    }

    private static int GetRequiredEndpointPermissionBitForActionFilter(
      ServiceEndpointActionFilter actionFilter)
    {
      int bitForActionFilter = 1;
      if ((actionFilter & ServiceEndpointActionFilter.View) == ServiceEndpointActionFilter.View)
        bitForActionFilter = 16;
      if ((actionFilter & ServiceEndpointActionFilter.Use) == ServiceEndpointActionFilter.Use)
        bitForActionFilter |= 1;
      if ((actionFilter & ServiceEndpointActionFilter.Manage) == ServiceEndpointActionFilter.Manage)
        bitForActionFilter |= 2;
      return bitForActionFilter;
    }

    private static bool IsAzureBoardsGitHubIntegrationEndpointType(string endpointType) => VssStringComparer.ServiceEndpointTypeCompararer.Equals(endpointType, "GitHubBoards") || VssStringComparer.ServiceEndpointTypeCompararer.Equals(endpointType, "GitHubEnterpriseBoards");

    private static bool IsOutdatedConnection(
      string endpointType,
      EndpointAuthorization authorization)
    {
      return VssStringComparer.ServiceEndpointTypeCompararer.Equals(endpointType, "Bitbucket") && authorization.GetConfigurationId() == InternalAuthConfigurationConstants.BitbucketAzurePipelinesOAuthAppId;
    }

    public static IServiceEndpointOperationsExtension GetEndpointOperationsExtension(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      IDisposableReadOnlyList<IServiceEndpointOperationsExtension> extensions = requestContext.GetExtensions<IServiceEndpointOperationsExtension>(ExtensionLifetime.Service);
      string filter = endpoint.Type + ":" + endpoint.Authorization.Scheme;
      IServiceEndpointOperationsExtension operationsExtension1;
      if (extensions == null)
      {
        operationsExtension1 = (IServiceEndpointOperationsExtension) null;
      }
      else
      {
        IEnumerable<IServiceEndpointOperationsExtension> source = extensions.Where<IServiceEndpointOperationsExtension>((Func<IServiceEndpointOperationsExtension, bool>) (e => string.Equals(filter, e.SupportedEndpointType, StringComparison.OrdinalIgnoreCase)));
        operationsExtension1 = source != null ? source.SingleOrDefault<IServiceEndpointOperationsExtension>() : (IServiceEndpointOperationsExtension) null;
      }
      IServiceEndpointOperationsExtension operationsExtension2 = operationsExtension1;
      if (operationsExtension2 == null)
      {
        filter = endpoint.Type + ":*";
        IServiceEndpointOperationsExtension operationsExtension3;
        if (extensions == null)
        {
          operationsExtension3 = (IServiceEndpointOperationsExtension) null;
        }
        else
        {
          IEnumerable<IServiceEndpointOperationsExtension> source = extensions.Where<IServiceEndpointOperationsExtension>((Func<IServiceEndpointOperationsExtension, bool>) (e => string.Equals(filter, e.SupportedEndpointType, StringComparison.OrdinalIgnoreCase)));
          operationsExtension3 = source != null ? source.SingleOrDefault<IServiceEndpointOperationsExtension>() : (IServiceEndpointOperationsExtension) null;
        }
        operationsExtension2 = operationsExtension3;
      }
      if (operationsExtension2 == null)
      {
        filter = "*:" + endpoint.Authorization.Scheme;
        IServiceEndpointOperationsExtension operationsExtension4;
        if (extensions == null)
        {
          operationsExtension4 = (IServiceEndpointOperationsExtension) null;
        }
        else
        {
          IEnumerable<IServiceEndpointOperationsExtension> source = extensions.Where<IServiceEndpointOperationsExtension>((Func<IServiceEndpointOperationsExtension, bool>) (e => string.Equals(filter, e.SupportedEndpointType, StringComparison.OrdinalIgnoreCase)));
          operationsExtension4 = source != null ? source.SingleOrDefault<IServiceEndpointOperationsExtension>() : (IServiceEndpointOperationsExtension) null;
        }
        operationsExtension2 = operationsExtension4;
      }
      if (operationsExtension2 == null)
        operationsExtension2 = extensions.FirstOrDefault<IServiceEndpointOperationsExtension>((Func<IServiceEndpointOperationsExtension, bool>) (e => string.Equals("default", e.SupportedEndpointType, StringComparison.OrdinalIgnoreCase)));
      return operationsExtension2;
    }

    public virtual void DeleteTeamProject(IVssRequestContext systemRequestContext, Guid projectId)
    {
      systemRequestContext.CheckSystemRequestContext();
      foreach (ServiceEndpoint queryServiceEndpoint in this.QueryServiceEndpoints(systemRequestContext, projectId, "", (IEnumerable<string>) null, (IEnumerable<Guid>) null, (string) null, true, false, ServiceEndpointActionFilter.None, (IEnumerable<RefreshAuthenticationParameters>) null))
      {
        IVssRequestContext systemRequestContext1 = systemRequestContext;
        List<Guid> scopeIdentifiers = new List<Guid>();
        scopeIdentifiers.Add(projectId);
        ServiceEndpoint endpoint = queryServiceEndpoint;
        this.DeleteEndpointAndStrongBox(systemRequestContext1, scopeIdentifiers, endpoint);
      }
      using (ServiceEndpointComponent component = systemRequestContext.CreateComponent<ServiceEndpointComponent>())
        component.DeleteTeamProject(projectId);
    }

    public void HandleOAuthConfigurationDelete(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      IList<ServiceEndpointOAuthConfigurationReference> byConfigurationId = this.GetServiceEndpointsByConfigurationId(requestContext, configurationId);
      HashSet<Guid> guidSet = new HashSet<Guid>();
      foreach (ServiceEndpointOAuthConfigurationReference configurationReference in (IEnumerable<ServiceEndpointOAuthConfigurationReference>) byConfigurationId)
        guidSet.Add(configurationReference.ServiceEndpointProjectId);
      OAuthEndpointStatus oauthEndpointStatus = new OAuthEndpointStatus("OAuthConfigurationDeleted", string.Format("OAuth Configuration {0} associated with this endpoint is deleted.", (object) configurationId));
      foreach (Guid guid in guidSet)
      {
        Guid endpointProjectId = guid;
        List<Guid> list = byConfigurationId.Where<ServiceEndpointOAuthConfigurationReference>((Func<ServiceEndpointOAuthConfigurationReference, bool>) (endpointReference => endpointReference.ServiceEndpointProjectId == endpointProjectId)).Select<ServiceEndpointOAuthConfigurationReference, Guid>((Func<ServiceEndpointOAuthConfigurationReference, Guid>) (endpointReference => endpointReference.ServiceEndpointId)).ToList<Guid>();
        this.MarkOAuth2EndpointsAsDirty(requestContext, endpointProjectId, list, oauthEndpointStatus.Serialize<OAuthEndpointStatus>());
      }
    }

    private void MarkOAuth2EndpointsAsDirty(
      IVssRequestContext requestContext,
      Guid endpointDataspace,
      List<Guid> endpointIdsToBeMarkedDirty,
      string operationStatus)
    {
      using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
        component.MarkOAuth2EndpointsAsDirty(requestContext, endpointDataspace, (IList<Guid>) endpointIdsToBeMarkedDirty, operationStatus);
    }

    private IList<ServiceEndpointOAuthConfigurationReference> GetServiceEndpointsByConfigurationId(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
        return component.GetServiceEndpointsByConfigurationId(requestContext, configurationId);
    }

    private void ValidateOwner(ServiceEndpoint endpoint)
    {
      if (!endpoint.Owner.Equals(ServiceEndpointOwner.AgentCloud, StringComparison.OrdinalIgnoreCase) && !endpoint.Owner.Equals(ServiceEndpointOwner.Boards, StringComparison.OrdinalIgnoreCase) && !endpoint.Owner.Equals(ServiceEndpointOwner.Environment, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(ServiceEndpointResources.ServiceEndpointInvalidOwner());
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private void DeleteSecurityChecks(
      IVssRequestContext requestContext,
      Guid endpointId,
      IList<Guid> projectIdList)
    {
      if (this.ServiceEndpointSecurity.HasPermission(requestContext, ServiceEndpointSecurity.Collection, endpointId.ToString("D"), 2, true))
        return;
      if (projectIdList.Count == 0)
        throw new ArgumentException(ServiceEndpointResources.ServiceEndpointAccessDeniedForDeleteOperation());
      foreach (Guid projectId in (IEnumerable<Guid>) projectIdList)
        this.CheckDeleteEndpointPermission(requestContext, projectId, endpointId);
    }

    public ServiceEndpoint HandleOperationServiceEndpointForCollection(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string operation,
      out bool completed)
    {
      completed = true;
      if (string.Equals(operation, "ConvertAuthenticationScheme", StringComparison.OrdinalIgnoreCase))
      {
        IContributedFeatureService service = requestContext.GetService<IContributedFeatureService>();
        bool flag1 = requestContext.IsFeatureEnabled("ServiceEndpoints.EnableARMServiceConnectionUpgradeToWIF");
        IVssRequestContext requestContext1 = requestContext;
        bool flag2 = service.IsFeatureEnabled(requestContext1, "ms.vss-distributedtask-web.workload-identity-federation");
        bool flag3 = string.Equals(endpoint?.Authorization?.Scheme, "WorkloadIdentityFederation");
        if (!flag1 || flag3 && !flag2)
          throw new ArgumentException(ServiceEndpointResources.InvalidSpnOperation((object) "ConvertAuthenticationScheme"));
        return this.ConvertServiceEndpointForCollection(requestContext, endpoint, out completed);
      }
      return string.Equals(operation, "ChangeProperties", StringComparison.OrdinalIgnoreCase) && requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints") ? this.ChangeEndpointProperties(requestContext, endpoint) : this.UpdateServiceEndpointForCollection(requestContext, endpoint, operation);
    }

    private ServiceEndpoint ChangeEndpointProperties(
      IVssRequestContext requestContext,
      ServiceEndpoint endpointUpdates)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (ChangeEndpointProperties)))
      {
        ArgumentUtility.CheckForNull<ServiceEndpoint>(endpointUpdates, nameof (endpointUpdates));
        ServiceEndpoint endpointToUpdate = this.GetEndpointById(requestContext, endpointUpdates.Id);
        requestContext.RunSynchronously((Func<Task>) (() => this.PopulateServiceEndpointSharedStatus(requestContext, (IList<ServiceEndpoint>) new List<ServiceEndpoint>()
        {
          endpointToUpdate
        })));
        this.ValidateOwner(requestContext, endpointToUpdate);
        ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpointToUpdate);
        List<Guid> list = endpointToUpdate.ServiceEndpointProjectReferences.Select<ServiceEndpointProjectReference, Guid>((Func<ServiceEndpointProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>();
        this.UpdateSecurityChecks(requestContext, list, endpointToUpdate, false);
        ServiceEndpoint originalEndpoint = endpointToUpdate.Clone();
        endpointToUpdate.IsDisabled = endpointUpdates.IsDisabled;
        this.UpdateServiceEndpointInternal(requestContext, list, endpointToUpdate, serviceEndpointType);
        PlatformServiceEndpointService.AuditPropertyChanges(requestContext, endpointUpdates, originalEndpoint);
        return endpointToUpdate;
      }
    }

    private static void AuditPropertyChanges(
      IVssRequestContext requestContext,
      ServiceEndpoint updatedEndpoint,
      ServiceEndpoint originalEndpoint)
    {
      if (requestContext != null && !requestContext.ExecutionEnvironment.IsHostedDeployment || originalEndpoint.IsDisabled == updatedEndpoint.IsDisabled)
        return;
      Dictionary<string, object> data = new Dictionary<string, object>()
      {
        {
          "EndpointId",
          (object) updatedEndpoint.Id
        },
        {
          "IsDisabled",
          (object) updatedEndpoint.IsDisabled
        },
        {
          "Type",
          (object) updatedEndpoint.Type
        }
      };
      requestContext.LogAuditEvent(ServiceConnectionAuditConstants.ServiceConnectionPropertyChanged, data);
    }

    private ServiceEndpoint UpdateServiceEndpointForCollection(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string operation)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (UpdateServiceEndpointForCollection)))
      {
        ArgumentUtility.CheckForNull<ServiceEndpoint>(endpoint, nameof (endpoint));
        this.SetupDefaultValues(endpoint);
        this.ValidateOwner(requestContext, endpoint);
        List<Guid> projectReferences = PlatformServiceEndpointService.GetProjectReferences(endpoint);
        ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpoint);
        PlatformServiceEndpointService.EnsureOperationSupportedForEndpoint(endpoint, operation);
        ServiceEndpoint existingEndpoint = this.GetEndpointById(requestContext, endpoint.Id);
        requestContext.RunSynchronously((Func<Task>) (() => this.PopulateServiceEndpointSharedStatus(requestContext, (IList<ServiceEndpoint>) new List<ServiceEndpoint>()
        {
          existingEndpoint
        })));
        Guid scopeIdentifier = projectReferences.FirstOrDefault<Guid>();
        this.PreUpdateOperations(requestContext, scopeIdentifier, endpoint, existingEndpoint, serviceEndpointType, operation);
        bool isCollectionEndpointChanged = !endpoint.Equals((object) existingEndpoint);
        this.UpdateSecurityChecks(requestContext, projectReferences, endpoint, isCollectionEndpointChanged);
        this.UpdateServiceEndpointInternal(requestContext, projectReferences, endpoint, serviceEndpointType);
        this.PostUpdateOperations(requestContext, scopeIdentifier, endpoint, existingEndpoint, serviceEndpointType, operation);
        foreach (Guid projectId in projectReferences)
          CustomerIntelligenceHelper.PublishServiceEndpointCreatedOrUpdatedOrDeletedTelemetry(requestContext, projectId, endpoint, "UpdateServiceEndpoint");
        endpoint.ConvertGitHubEndpointsForMigrations(requestContext);
        return endpoint;
      }
    }

    private ServiceEndpoint ConvertServiceEndpointForCollection(
      IVssRequestContext requestContext,
      ServiceEndpoint endpointToConvert,
      out bool wasModified)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(endpointToConvert, nameof (endpointToConvert));
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (ConvertServiceEndpointForCollection)))
      {
        wasModified = false;
        List<Guid> projectReferences = PlatformServiceEndpointService.GetProjectReferences(endpointToConvert);
        Guid scopeIdentifier = projectReferences.FirstOrDefault<Guid>();
        if (scopeIdentifier == Guid.Empty)
          throw new ArgumentException(ServiceEndpointResources.ProjectReferenceMissing());
        ServiceEndpoint serviceEndpoint = this.GetServiceEndpoint(requestContext.Elevate(), scopeIdentifier, endpointToConvert.Id, ServiceEndpointActionFilter.None, (RefreshAuthenticationParameters) null) ?? throw new ServiceEndpointNotFoundException(ServiceEndpointResources.EndpointNotFound((object) endpointToConvert.Id));
        this.UpdateSecurityChecks(requestContext, projectReferences, serviceEndpoint, true);
        ServiceEndpointValidator.AssertEndpointNotModified(endpointToConvert, serviceEndpoint);
        PlatformServiceEndpointService.EnsureStatusNotInProgress(serviceEndpoint);
        ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpointToConvert);
        IServiceEndpointOperationsExtension operationsExtension = PlatformServiceEndpointService.GetEndpointOperationsExtension(requestContext, endpointToConvert);
        PlatformServiceEndpointService.ConversionData data = new PlatformServiceEndpointService.ConversionData()
        {
          EndpointToConvert = endpointToConvert,
          ProjectIds = projectReferences,
          ScopeIdentifier = scopeIdentifier,
          ExistingEndpoint = serviceEndpoint,
          EndpointType = serviceEndpointType,
          EndpointOperationsExtension = operationsExtension
        };
        ServiceEndpoint endpoint;
        if (operationsExtension != null && operationsExtension.SupportsUpgradeBetween(serviceEndpoint.Authorization.Scheme, endpointToConvert.Authorization.Scheme))
        {
          endpoint = this.UpgradeAuthenticationScheme(requestContext, data, out wasModified);
        }
        else
        {
          if (operationsExtension == null || !operationsExtension.SupportsDowngradeBetween(serviceEndpoint.Authorization.Scheme, endpointToConvert.Authorization.Scheme))
            throw new ArgumentException(ServiceEndpointResources.ConversionBetweenAuthSchemesNotSupported((object) endpointToConvert.Type, (object) serviceEndpoint.Authorization.Scheme, (object) endpointToConvert.Authorization.Scheme));
          endpoint = this.DowngradeAuthenticationScheme(requestContext, data, out wasModified);
        }
        PlatformServiceEndpointService.ClearConfidentialParameters(endpoint, serviceEndpointType);
        foreach (Guid projectId in projectReferences)
          CustomerIntelligenceHelper.PublishServiceEndpointCreatedOrUpdatedOrDeletedTelemetry(requestContext, projectId, endpoint, "ConvertServiceEndpoint");
        return endpoint;
      }
    }

    private ServiceEndpoint UpgradeAuthenticationScheme(
      IVssRequestContext requestContext,
      PlatformServiceEndpointService.ConversionData data,
      out bool wasModified)
    {
      wasModified = false;
      ServiceEndpoint upgradedEndpoint;
      if (this.TryUpgradeServiceEndpointScheme(requestContext, data.ExistingEndpoint, data.EndpointToConvert.Authorization.Scheme, UpgradeServiceEndpointSuccessCriteria.AtLeastOneSuccessful, out upgradedEndpoint))
      {
        data.EndpointOperationsExtension?.PostUpgradeOperations(requestContext, data.ScopeIdentifier, upgradedEndpoint, data.EndpointType);
        wasModified = true;
        return upgradedEndpoint;
      }
      ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, data.ExistingEndpoint);
      try
      {
        data.EndpointOperationsExtension?.PreUpgradeOperations(requestContext, upgradedEndpoint, data.EndpointType);
        data.ExistingEndpoint.OperationStatus = JObject.FromObject((object) new AzureSpnOperationStatus("InProgress", "converting_scheme"));
      }
      catch (Exception ex)
      {
        data.ExistingEndpoint.OperationStatus = JObject.FromObject((object) new AzureSpnOperationStatus("Failed", "converting_scheme_failed"));
        throw;
      }
      finally
      {
        this.UpdateServiceEndpointInternal(requestContext, data.ProjectIds, data.ExistingEndpoint, serviceEndpointType);
      }
      data.EndpointOperationsExtension?.ProcessUpgradeOperations(requestContext, data.ScopeIdentifier, upgradedEndpoint, data.EndpointType);
      return data.ExistingEndpoint;
    }

    private ServiceEndpoint DowngradeAuthenticationScheme(
      IVssRequestContext requestContext,
      PlatformServiceEndpointService.ConversionData data,
      out bool wasModified)
    {
      wasModified = true;
      ServiceEndpoint serviceEndpoint = data.ExistingEndpoint.Clone();
      if (!ServiceEndpointAuthorizationHelper.TryRecoverOldAuthorizationSchemeParameters(requestContext, serviceEndpoint))
      {
        requestContext.TraceWarning(34000866, nameof (PlatformServiceEndpointService), "Could not downgrade scheme for endpoint {0} from {1} to {2} as old authentication scheme parameters could not be recovered.", (object) data.ExistingEndpoint.Id, (object) data.ExistingEndpoint.Authorization.Scheme, (object) data.EndpointToConvert.Authorization.Scheme);
        throw new InvalidServiceEndpointRequestException(ServiceEndpointResources.CouldNotDowngradeMissingRecoveryData((object) data.ExistingEndpoint.Authorization.Scheme, (object) data.EndpointToConvert.Authorization.Scheme));
      }
      serviceEndpoint.ConvertAuthenticationScheme(requestContext, data.EndpointToConvert.Authorization.Scheme, out List<string> _);
      serviceEndpoint.OperationStatus = JObject.FromObject((object) new AzureSpnOperationStatus("Ready", ""));
      this.UpdateServiceEndpointInternal(requestContext, data.ProjectIds, serviceEndpoint, data.EndpointType);
      IdentityService service = requestContext.GetService<IdentityService>();
      serviceEndpoint.CreatedBy = service.GetIdentity(requestContext, serviceEndpoint.CreatedBy.Id).ToIdentityRef(requestContext);
      this.UpdateStrongBox(requestContext, serviceEndpoint, data.EndpointType, true);
      ServiceEndpointAuthorizationHelper.ClearAuthorizationSchemeInputsForRecovery(requestContext, serviceEndpoint);
      requestContext.TraceInfo(34000865, nameof (PlatformServiceEndpointService), "Downgraded scheme for endpoint {0} from {1} to {2}", (object) data.ExistingEndpoint.Id, (object) data.ExistingEndpoint.Authorization.Scheme, (object) serviceEndpoint.Authorization.Scheme);
      return serviceEndpoint;
    }

    private static List<Guid> GetProjectReferences(ServiceEndpoint endpoint) => endpoint.ServiceEndpointProjectReferences.Where<ServiceEndpointProjectReference>((Func<ServiceEndpointProjectReference, bool>) (x => x != null && x.ProjectReference != null)).Select<ServiceEndpointProjectReference, Guid>((Func<ServiceEndpointProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>();

    private ServiceEndpoint GetEndpointById(
      IVssRequestContext requestContext,
      Guid endpointId,
      Guid scopeIdentifier = default (Guid))
    {
      return this.QueryServiceEndpointsInternal(requestContext, scopeIdentifier, string.Empty, (IEnumerable<string>) new List<string>(), (IEnumerable<Guid>) new List<Guid>()
      {
        endpointId
      }, (string) null, true).FirstOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x.Id == endpointId));
    }

    private static void EnsureOperationSupportedForEndpoint(
      ServiceEndpoint endpoint,
      string operation)
    {
      if (!endpoint.Type.Equals("AzureRM", StringComparison.OrdinalIgnoreCase) && !operation.IsNullOrEmpty<char>())
        throw new ServiceEndpointException(ServiceEndpointResources.EndpointOperationNotSupported((object) operation, (object) endpoint.Type));
      if (endpoint.Type.Equals("AzureRM", StringComparison.OrdinalIgnoreCase) && !operation.IsNullOrEmpty<char>() && !Enum.TryParse<SpnOperation>(operation, out SpnOperation _))
        throw new ServiceEndpointException(ServiceEndpointResources.InvalidSpnOperation((object) operation));
    }

    public ServiceEndpoint CreateServiceEndpointForCollection(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (CreateServiceEndpointForCollection)))
      {
        if (endpoint.ServiceEndpointProjectReferences != null)
        {
          if (endpoint.ServiceEndpointProjectReferences.Any<ServiceEndpointProjectReference>())
          {
            try
            {
              endpoint.Id = Guid.NewGuid();
              this.SetupDefaultValues(endpoint);
              this.ValidateOwner(requestContext, endpoint);
              if (requestContext.IsFeatureEnabled("ServiceEndpoints.EnableProjectReferencesDataFilling"))
                this.ValidateAndFillProjectReferences(requestContext, endpoint);
              List<Guid> guidList = new List<Guid>();
              if (endpoint.ServiceEndpointProjectReferences != null)
                guidList = endpoint.ServiceEndpointProjectReferences.Where<ServiceEndpointProjectReference>((Func<ServiceEndpointProjectReference, bool>) (x => x != null && x.ProjectReference != null)).Select<ServiceEndpointProjectReference, Guid>((Func<ServiceEndpointProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>();
              this.CreateSecurityChecks(requestContext, guidList, endpoint.GetServiceEndpointCreator(requestContext), endpoint.Id);
              ServiceEndpointSecurity.AssignProjectLevelAdminAsCollectionAdminForCreate(requestContext, endpoint);
              ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, endpoint);
              this.PreCreateOperations(requestContext, endpoint, serviceEndpointType);
              this.AddServiceEndpointInternal(requestContext, guidList, endpoint, serviceEndpointType);
              this.PostCreateOperations(requestContext, guidList.First<Guid>(), endpoint, serviceEndpointType);
              endpoint.ConvertGitHubEndpointsForMigrations(requestContext);
              return endpoint;
            }
            catch (Exception ex)
            {
              requestContext.TraceException(nameof (PlatformServiceEndpointService), ex);
              throw;
            }
          }
        }
        throw new ArgumentException(ServiceEndpointResources.AtleastOneProjectReferenceRequired());
      }
    }

    private void ValidateAndFillProjectReferences(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      if (endpoint.ServiceEndpointProjectReferences == null || endpoint.ServiceEndpointProjectReferences.Count == 0)
        return;
      IProjectService service = requestContext.GetService<IProjectService>();
      foreach (ServiceEndpointProjectReference projectReference1 in (IEnumerable<ServiceEndpointProjectReference>) endpoint.ServiceEndpointProjectReferences)
      {
        ProjectReference projectReference2 = projectReference1.ProjectReference;
        if (projectReference2 == null)
          throw new ArgumentException(ServiceEndpointResources.ProjectReferenceMissing());
        if (projectReference2.Id == Guid.Empty && string.IsNullOrEmpty(projectReference2.Name))
          throw new ArgumentException(ServiceEndpointResources.ServiceEndpointProjectReferenceInformationNotProvided((object) endpoint.Id));
        if (projectReference2.Id == Guid.Empty)
          projectReference2.Id = service.GetProjectId(requestContext, projectReference2.Name);
        if (string.IsNullOrEmpty(projectReference2.Name))
          projectReference2.Name = service.GetProjectName(requestContext, projectReference2.Id);
      }
    }

    private void CreateSecurityChecks(
      IVssRequestContext requestContext,
      List<Guid> projectIds,
      Microsoft.VisualStudio.Services.Identity.Identity endpointCreator,
      Guid endpointId)
    {
      foreach (Guid projectId in projectIds)
        this.ProjectSecurityChecks(requestContext, projectId, endpointCreator, endpointId);
    }

    private void ValidateOwner(IVssRequestContext requestContext, ServiceEndpoint endpoint)
    {
      if (endpoint.Owner.Equals(ServiceEndpointOwner.Library, StringComparison.OrdinalIgnoreCase))
        return;
      this.ServiceEndpointSecurity.CheckCallerIsServicePrincipal(requestContext, Guid.Empty.ToString("D"));
      if (!endpoint.Owner.Equals(ServiceEndpointOwner.AgentCloud, StringComparison.OrdinalIgnoreCase) && !endpoint.Owner.Equals(ServiceEndpointOwner.Boards, StringComparison.OrdinalIgnoreCase) && !endpoint.Owner.Equals(ServiceEndpointOwner.Environment, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(ServiceEndpointResources.ServiceEndpointInvalidOwner());
    }

    private void SetupDefaultValues(ServiceEndpoint endpoint)
    {
      if (!endpoint.Owner.IsNullOrEmpty<char>())
        return;
      endpoint.Owner = ServiceEndpointOwner.Library;
    }

    public void ShareServiceEndpoint(
      IVssRequestContext requestContext,
      Guid endpointId,
      List<ServiceEndpointProjectReference> endpointProjectReferences)
    {
      ArgumentUtility.CheckForEmptyGuid(endpointId, nameof (endpointId), "ServiceEndpoints");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) endpointProjectReferences, nameof (endpointProjectReferences), "ServiceEndpoints");
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) endpointProjectReferences, nameof (endpointProjectReferences), "ServiceEndpoints");
      foreach (ServiceEndpointProjectReference projectReference in endpointProjectReferences)
        PlatformServiceEndpointService.CheckServiceEndpointProjectReference(projectReference);
      this.ServiceEndpointSecurity.CheckPermission(requestContext, ServiceEndpointSecurity.Collection, endpointId.ToString("D"), 2, false, (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.ServiceEndpointAccessDeniedForCollectionAdminOperation()));
      List<Guid> list = endpointProjectReferences.Where<ServiceEndpointProjectReference>((Func<ServiceEndpointProjectReference, bool>) (x => x.ProjectReference != null)).Select<ServiceEndpointProjectReference, Guid>((Func<ServiceEndpointProjectReference, Guid>) (x => x.ProjectReference.Id)).ToList<Guid>();
      this.CreateSecurityChecks(requestContext, list, requestContext.GetUserIdentity(), endpointId);
      using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
        component.ShareServiceEndpoint(endpointId, endpointProjectReferences);
    }

    private static void CheckServiceEndpointProjectReference(
      ServiceEndpointProjectReference endpointProjectReference)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(endpointProjectReference.Name, "Name", "ServiceEndpoints");
      if (endpointProjectReference.Description == null)
        return;
      endpointProjectReference.Description = endpointProjectReference.Description.Trim();
      ArgumentUtility.CheckStringLength(endpointProjectReference.Description, "Description", 2048, expectedServiceArea: "ServiceEndpoints");
    }

    public List<ServiceEndpoint> GetServiceEndpointsByConfigurationId(
      IVssRequestContext tfsRequestContext,
      Guid configurationId,
      bool includeDetails,
      string owner)
    {
      IList<ServiceEndpointOAuthConfigurationReference> byConfigurationId1;
      using (ServiceEndpointComponent component = tfsRequestContext.CreateComponent<ServiceEndpointComponent>())
        byConfigurationId1 = component.GetServiceEndpointsByConfigurationId(tfsRequestContext, configurationId);
      List<Guid> list = byConfigurationId1.Select<ServiceEndpointOAuthConfigurationReference, Guid>((Func<ServiceEndpointOAuthConfigurationReference, Guid>) (x => x.ServiceEndpointId)).ToList<Guid>();
      List<ServiceEndpoint> byConfigurationId2 = new List<ServiceEndpoint>();
      if (!list.IsNullOrEmpty<Guid>())
      {
        IVssRequestContext requestContext = tfsRequestContext;
        Guid empty = Guid.Empty;
        List<string> authSchemes = new List<string>();
        List<Guid> endpointIds = list;
        bool flag = includeDetails;
        string owner1 = owner;
        int num = flag ? 1 : 0;
        byConfigurationId2 = this.QueryServiceEndpoints(requestContext, empty, (string) null, (IEnumerable<string>) authSchemes, (IEnumerable<Guid>) endpointIds, owner1, true, num != 0, ServiceEndpointActionFilter.None, (IEnumerable<RefreshAuthenticationParameters>) null);
      }
      return byConfigurationId2;
    }

    public void UpdateServiceEndpointsDuplicateName(
      IVssRequestContext requestContext,
      Guid projectId,
      List<KeyValuePair<Guid, string>> serviceEndPointNames)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<List<KeyValuePair<Guid, string>>>(serviceEndPointNames, nameof (serviceEndPointNames));
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (UpdateServiceEndpointsDuplicateName)))
      {
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
        {
          int batchSize = 500;
          component.UpdateServiceEndpointDuplicateName(projectId, serviceEndPointNames, batchSize);
        }
      }
    }

    public IList<ServiceEndpointPartial> QueryAllServiceEndpoints(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      using (new MethodScope(requestContext, nameof (PlatformServiceEndpointService), nameof (QueryAllServiceEndpoints)))
      {
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
          return component.QueryAllServiceEndpoints(projectId);
      }
    }

    private class ConversionData
    {
      public ServiceEndpoint EndpointToConvert { get; set; }

      public List<Guid> ProjectIds { get; set; }

      public Guid ScopeIdentifier { get; set; }

      public ServiceEndpoint ExistingEndpoint { get; set; }

      public ServiceEndpointType EndpointType { get; set; }

      public IServiceEndpointOperationsExtension EndpointOperationsExtension { get; set; }
    }
  }
}
