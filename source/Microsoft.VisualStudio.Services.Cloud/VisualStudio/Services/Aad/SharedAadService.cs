// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.SharedAadService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Aad
{
  internal class SharedAadService : AadService
  {
    private readonly SharedAadService.Settings settings = new SharedAadService.Settings();
    private readonly GraphClientsFactory graphClientFactory;
    private readonly AadTokenServiceFactory tokenServiceFactory;
    private bool serviceStartedWithDeploymentContext;
    private readonly IConfigQueryable<string> microsoftGraphApiDomainNameConfig;
    private static readonly JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    private static int microsoftGraphDataComparePercentage;
    private static long eligleForDataCompareRequestCount = 0;

    public SharedAadService()
      : this((GraphClientsFactory) null, (AadTokenServiceFactory) null)
    {
    }

    public SharedAadService(
      GraphClientsFactory graphClientFactory,
      AadTokenServiceFactory tokenServiceFactory)
      : this(graphClientFactory, tokenServiceFactory, ConfigProxy.Create<string>(AadServiceConstants.MicrosoftGraphConfigPrototypes.MicrosoftGraphApiDomainName))
    {
    }

    public SharedAadService(
      GraphClientsFactory graphClientFactory,
      AadTokenServiceFactory tokenServiceFactory,
      IConfigQueryable<string> config)
    {
      this.graphClientFactory = graphClientFactory ?? new GraphClientsFactory();
      this.tokenServiceFactory = tokenServiceFactory ?? new AadTokenServiceFactory();
      this.microsoftGraphApiDomainNameConfig = config;
    }

    public override void ServiceStart(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Aad/...");
      SharedAadService.UpdateSettings(context, this.settings);
      this.serviceStartedWithDeploymentContext = context.ServiceHost.Is(TeamFoundationHostType.Deployment);
    }

    public override void ServiceEnd(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      SharedAadService.UpdateSettings(context, this.settings);
    }

    private static void UpdateSettings(
      IVssRequestContext context,
      SharedAadService.Settings settings)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      settings.AuthAuthority = SharedAadService.GetRegistryValue(vssRequestContext, service, "/Service/Aad/AuthAuthority");
      settings.AuthClientId = SharedAadService.GetRegistryValue(vssRequestContext, service, "/Service/Aad/AuthClientId");
      settings.GraphApiDomainName = SharedAadService.GetRegistryValue(vssRequestContext, service, "/Service/Aad/GraphApiDomainName", "graph.windows.net");
      settings.GraphApiResource = SharedAadService.GetRegistryValue(vssRequestContext, service, "/Service/Aad/GraphApiResource");
      settings.GraphApiVersion = SharedAadService.GetRegistryValue(vssRequestContext, service, "/Service/Aad/GraphApiVersion", "1.5-internal");
      settings.MicrosoftServicesTenant = SharedAadService.GetRegistryValue(vssRequestContext, service, "/Service/Aad/MicrosoftServicesTenant", "microsoftservices.onmicrosoft.com");
      IDictionary<string, object> operationSettings = settings.OperationSettings;
      operationSettings["/Service/Aad/MaxRequestsPerBatch"] = (object) service.GetValue<int>(context, (RegistryQuery) "/Service/Aad/MaxRequestsPerBatch", 5);
      operationSettings["/Service/Aad/MicrosoftServicesTenantForGetTenantsByKey"] = (object) service.GetValue(context, (RegistryQuery) "/Service/Aad/MicrosoftServicesTenantForGetTenantsByKey", settings.MicrosoftServicesTenant);
      operationSettings["/Service/Aad/GetDescendantIds/MaxResults"] = (object) service.GetValue<int>(context, (RegistryQuery) "/Service/Aad/GetDescendantIds/MaxResults", 9990);
      operationSettings["/Service/Aad/GetDescendantIds/MaxPages"] = (object) service.GetValue<int>(context, (RegistryQuery) "/Service/Aad/GetDescendantIds/MaxPages", 10);
      operationSettings["/Service/Aad/GetDescendantIds/GraphApiVersion"] = (object) service.GetValue(context, (RegistryQuery) "/Service/Aad/GetDescendantIds/GraphApiVersion", "1.6-internal");
      operationSettings["/Service/Aad/GetDescendantIds/MaxPageSize"] = (object) SharedAadService.GetDescendantIdsMaxPageSize(context, service);
      operationSettings["VisualStudio.Services.Aad.EnableV2EndpointForGetDescendantIds"] = (object) context.IsFeatureEnabled("VisualStudio.Services.Aad.EnableV2EndpointForGetDescendantIds");
      SharedAadService.microsoftGraphDataComparePercentage = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Aad/MicrosoftGraphDataComparePercentage", 10);
    }

    private static int GetDescendantIdsMaxPageSize(
      IVssRequestContext context,
      IVssRegistryService registryService)
    {
      int descendantIdsMaxPageSize = registryService.GetValue<int>(context, (RegistryQuery) "/Service/Aad/GetDescendantIds/MaxPageSize", 999);
      if (descendantIdsMaxPageSize > 999)
      {
        descendantIdsMaxPageSize = 999;
        context.TraceAlways(1035049, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", string.Format("The configured value of Maximum Page Size {0} exceeds the Know Max Page Size Threshold {1}", (object) descendantIdsMaxPageSize, (object) 999));
      }
      return descendantIdsMaxPageSize;
    }

    private static string GetRegistryValue(
      IVssRequestContext context,
      IVssRegistryService registry,
      string key,
      string @default = null)
    {
      string registryValue = registry.GetValue<string>(context, (RegistryQuery) key, (string) null);
      if (string.IsNullOrWhiteSpace(registryValue))
        registryValue = !string.IsNullOrEmpty(@default) ? @default : throw new AadException(string.Format("No registry value for key: {0}", (object) key));
      return registryValue;
    }

    public override bool IsRequestAllowed(IVssRequestContext context, AadServiceRequest request) => this.IsRequestAllowed(context, request, out string _, out bool _);

    public override IDictionary<Guid, bool> AreObjectsVirtuallyInScope(
      IVssRequestContext requestContext,
      AadObjectType objectType,
      IEnumerable<Guid> objectIds)
    {
      return AadServiceUtils.AreObjectsVirtuallyInScope(requestContext, objectType, objectIds);
    }

    public override GetAncestorIdsResponse<T> GetAncestorIds<T>(
      IVssRequestContext context,
      GetAncestorIdsRequest<T> request)
    {
      return this.ProcessRequest<GetAncestorIdsResponse<T>>(context, (AadServiceRequest) request, AadServiceCounters.GetAncestorIds, nameof (GetAncestorIds));
    }

    public override GetAncestorsResponse GetAncestors<T>(
      IVssRequestContext context,
      GetAncestorsRequest<T> request)
    {
      return this.ProcessRequest<GetAncestorsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetAncestors, nameof (GetAncestors));
    }

    public override GetDescendantIdsResponse GetDescendantIds<T>(
      IVssRequestContext context,
      GetDescendantIdsRequest<T> request)
    {
      return this.ProcessRequest<GetDescendantIdsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetDescendantIds, nameof (GetDescendantIds));
    }

    public override GetDescendantsResponse GetDescendants<T>(
      IVssRequestContext context,
      GetDescendantsRequest<T> request)
    {
      return this.ProcessRequest<GetDescendantsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetDescendants, nameof (GetDescendants));
    }

    public override GetDirectoryRolesResponse GetDirectoryRoles(
      IVssRequestContext context,
      GetDirectoryRolesRequest request)
    {
      return this.ProcessRequest<GetDirectoryRolesResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetDirectoryRoles, nameof (GetDirectoryRoles));
    }

    public override GetDirectoryRoleMembersResponse GetDirectoryRoleMembers(
      IVssRequestContext context,
      GetDirectoryRoleMembersRequest request)
    {
      return this.ProcessRequest<GetDirectoryRoleMembersResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetDirectoryRoleMembers, nameof (GetDirectoryRoleMembers));
    }

    public override GetDirectoryRolesWithIdsResponse GetDirectoryRolesWithIds(
      IVssRequestContext context,
      GetDirectoryRolesWithIdsRequest request)
    {
      return this.ProcessRequest<GetDirectoryRolesWithIdsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetDirectoryRolesWithIds, nameof (GetDirectoryRolesWithIds));
    }

    public override GetGroupsResponse GetGroups(
      IVssRequestContext context,
      GetGroupsRequest request)
    {
      return this.ProcessRequest<GetGroupsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetGroups, nameof (GetGroups));
    }

    public override GetGroupsWithIdsResponse<T> GetGroupsWithIds<T>(
      IVssRequestContext context,
      GetGroupsWithIdsRequest<T> request)
    {
      return this.ProcessRequest<GetGroupsWithIdsResponse<T>>(context, (AadServiceRequest) request, AadServiceCounters.GetGroupsWithIds, nameof (GetGroupsWithIds));
    }

    public override GetTenantResponse GetTenant(
      IVssRequestContext context,
      GetTenantRequest request)
    {
      return this.ProcessRequest<GetTenantResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetTenant, nameof (GetTenant));
    }

    public override GetTenantsResponse GetTenants(
      IVssRequestContext context,
      GetTenantsRequest request)
    {
      return this.ProcessRequest<GetTenantsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetTenants, nameof (GetTenants));
    }

    public override GetUsersResponse GetUsers(IVssRequestContext context, GetUsersRequest request) => this.ProcessRequest<GetUsersResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetUsers, nameof (GetUsers));

    public override GetUsersWithIdsResponse<T> GetUsersWithIds<T>(
      IVssRequestContext context,
      GetUsersWithIdsRequest<T> request)
    {
      return this.ProcessRequest<GetUsersWithIdsResponse<T>>(context, (AadServiceRequest) request, AadServiceCounters.GetUsersWithIds, nameof (GetUsersWithIds));
    }

    public override Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.GetUserStatusWithIdResponse GetUserStatusWithId(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.GetUserStatusWithIdRequest request)
    {
      return this.ProcessRequest<Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.GetUserStatusWithIdResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetUserStatusWithId, nameof (GetUserStatusWithId));
    }

    public override GetUserThumbnailResponse GetUserThumbnail<T>(
      IVssRequestContext context,
      GetUserThumbnailRequest<T> request)
    {
      return this.ProcessRequest<GetUserThumbnailResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetUserThumbnail, nameof (GetUserThumbnail));
    }

    public override GetUserRolesAndGroupsResponse GetUserRolesAndGroups(
      IVssRequestContext context,
      GetUserRolesAndGroupsRequest request)
    {
      return this.ProcessRequest<GetUserRolesAndGroupsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetUserRolesAndGroups, nameof (GetUserRolesAndGroups));
    }

    public override GetServicePrincipalsByIdsResponse GetServicePrincipalsByIds(
      IVssRequestContext context,
      GetServicePrincipalsByIdsRequest request)
    {
      return this.ProcessRequest<GetServicePrincipalsByIdsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetServicePrincipalsByIds, nameof (GetServicePrincipalsByIds));
    }

    public override GetServicePrincipalsResponse GetServicePrincipals(
      IVssRequestContext context,
      GetServicePrincipalsRequest request)
    {
      return this.ProcessRequest<GetServicePrincipalsResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetServicePrincipals, nameof (GetServicePrincipals));
    }

    internal override InvalidateStaleAncestorIdsCacheResponse InvalidateStaleAncestorIdsCache<T>(
      IVssRequestContext context,
      InvalidateStaleAncestorIdsCacheRequest<T> request)
    {
      return this.ProcessRequest<InvalidateStaleAncestorIdsCacheResponse>(context, (AadServiceRequest) request, operation: nameof (InvalidateStaleAncestorIdsCache));
    }

    public override GetSoftDeletedObjectsResponse<TIdentifier, TType> GetSoftDeletedObjects<TIdentifier, TType>(
      IVssRequestContext context,
      GetSoftDeletedObjectsRequest<TIdentifier, TType> request)
    {
      return this.ProcessRequest<GetSoftDeletedObjectsResponse<TIdentifier, TType>>(context, (AadServiceRequest) request, AadServiceCounters.GetSoftDeletedObjects, nameof (GetSoftDeletedObjects));
    }

    public override GetApplicationByIdResponse GetApplicationById(
      IVssRequestContext context,
      GetApplicationByIdRequest request)
    {
      return this.ProcessRequest<GetApplicationByIdResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetApplicationById, nameof (GetApplicationById));
    }

    public override CreateApplicationResponse CreateApplication(
      IVssRequestContext context,
      CreateApplicationRequest request)
    {
      return this.ProcessRequest<CreateApplicationResponse>(context, (AadServiceRequest) request, AadServiceCounters.CreateApplication, nameof (CreateApplication));
    }

    public override DeleteApplicationResponse DeleteApplication(
      IVssRequestContext context,
      DeleteApplicationRequest request)
    {
      return this.ProcessRequest<DeleteApplicationResponse>(context, (AadServiceRequest) request, AadServiceCounters.DeleteApplication, nameof (DeleteApplication));
    }

    public override UpdateApplicationResponse UpdateApplication(
      IVssRequestContext context,
      UpdateApplicationRequest request)
    {
      return this.ProcessRequest<UpdateApplicationResponse>(context, (AadServiceRequest) request, AadServiceCounters.UpdateApplication, nameof (UpdateApplication));
    }

    public override CreateServicePrincipalResponse CreateServicePrincipal(
      IVssRequestContext context,
      CreateServicePrincipalRequest request)
    {
      return this.ProcessRequest<CreateServicePrincipalResponse>(context, (AadServiceRequest) request, AadServiceCounters.CreateServicePrincipal, nameof (CreateServicePrincipal));
    }

    public override DeleteServicePrincipalResponse DeleteServicePrincipal(
      IVssRequestContext context,
      DeleteServicePrincipalRequest request)
    {
      return this.ProcessRequest<DeleteServicePrincipalResponse>(context, (AadServiceRequest) request, AadServiceCounters.DeleteServicePrincipal, nameof (DeleteServicePrincipal));
    }

    public override AddApplicationPasswordResponse AddApplicationPassword(
      IVssRequestContext context,
      AddApplicationPasswordRequest request)
    {
      return this.ProcessRequest<AddApplicationPasswordResponse>(context, (AadServiceRequest) request, AadServiceCounters.AddApplicationPassword, nameof (AddApplicationPassword));
    }

    public override RemoveApplicationPasswordResponse RemoveApplicationPassword(
      IVssRequestContext context,
      RemoveApplicationPasswordRequest request)
    {
      return this.ProcessRequest<RemoveApplicationPasswordResponse>(context, (AadServiceRequest) request, AadServiceCounters.RemoveApplicationPassword, nameof (RemoveApplicationPassword));
    }

    public override AddApplicationFederatedCredentialsResponse AddApplicationFederatedCredentials(
      IVssRequestContext context,
      AddApplicationFederatedCredentialsRequest request)
    {
      return this.ProcessRequest<AddApplicationFederatedCredentialsResponse>(context, (AadServiceRequest) request, AadServiceCounters.AddApplicationFederatedCredentials, nameof (AddApplicationFederatedCredentials));
    }

    public override RemoveApplicationFederatedCredentialsResponse RemoveApplicationFederatedCredentials(
      IVssRequestContext context,
      RemoveApplicationFederatedCredentialsRequest request)
    {
      return this.ProcessRequest<RemoveApplicationFederatedCredentialsResponse>(context, (AadServiceRequest) request, AadServiceCounters.RemoveApplicationFederatedCredentials, nameof (RemoveApplicationFederatedCredentials));
    }

    public override GetOrganizationDataBoundaryResponse GetOrganizationDataBoundary(
      IVssRequestContext context,
      GetOrganizationDataBoundaryRequest request)
    {
      return this.ProcessRequest<GetOrganizationDataBoundaryResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetOrganizationDataBoundary, nameof (GetOrganizationDataBoundary));
    }

    public override GetProfileDataResponse GetProfileData(
      IVssRequestContext context,
      GetProfileDataRequest request)
    {
      return this.ProcessRequest<GetProfileDataResponse>(context, (AadServiceRequest) request, AadServiceCounters.GetProfileData, nameof (GetProfileData));
    }

    private T ProcessRequest<T>(
      IVssRequestContext context,
      AadServiceRequest request,
      IAadPerfCounter counter = null,
      [CallerMemberName] string operation = "")
      where T : AadServiceResponse
    {
      this.ValidateContext(context);
      try
      {
        context.TraceEnter(5256001, "VisualStudio.Services.Aad", "Service", operation);
        AadServiceCounters.All.IncrementEntryCounters();
        counter?.IncrementEntryCounters();
        try
        {
          this.ValidateRequest(request);
        }
        catch (Exception ex)
        {
          AadServiceCounters.All.IncrementErrorCounters();
          counter?.IncrementErrorCounters();
          throw;
        }
        string tenantId = (string) null;
        bool application = false;
        try
        {
          this.AuthorizeRequest(context, request, out tenantId, out application);
        }
        catch (Exception ex)
        {
          AadServiceCounters.All.IncrementErrorCounters();
          counter?.IncrementErrorCounters();
          context.TraceException(5256002, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", ex);
          throw;
        }
        T obj1 = default (T);
        T obj2 = !this.IsDualReadEligible(context, operation, request) ? this.ExecuteGraphRequest<T>(context, request, operation, tenantId, application, this.DetermineUseMicrosoftGraph(context, operation, request), counter) : this.DualReadDataCompare<T>(context, request, counter, operation, tenantId, application);
        AadServiceCounters.All.IncrementSuccessCounters();
        counter?.IncrementSuccessCounters();
        return obj2;
      }
      finally
      {
        context.TraceLeave(5256009, "VisualStudio.Services.Aad", "Service", operation);
      }
    }

    internal T DualReadDataCompare<T>(
      IVssRequestContext context,
      AadServiceRequest request,
      IAadPerfCounter counter,
      string operation,
      string tenantId,
      bool application)
      where T : AadServiceResponse
    {
      context.TraceConditionally(5256040, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => "Running Microsoft graph experiment for '" + operation + "'"));
      T anotherResponse = default (T);
      T obj;
      try
      {
        obj = this.ExecuteGraphRequest<T>(context, request, operation, tenantId, application, false, counter);
      }
      catch (Exception ex)
      {
        SharedAadService.LogExceptionWithStackTraceHelper(context, 5256041, ex, "Aad Graph", operation);
        throw;
      }
      if (((bool?) obj?.FromCache).GetValueOrDefault())
      {
        context.Trace(5256044, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", "Skip Microsoft graph experiment as data was retrieved from cache");
      }
      else
      {
        try
        {
          anotherResponse = this.ExecuteGraphRequest<T>(context, request, operation, tenantId, application, true, counter, true);
        }
        catch (Exception ex)
        {
          SharedAadService.LogExceptionWithStackTraceHelper(context, 5256042, ex, "Microsft Graph", operation);
        }
        if ((object) obj != null && (object) anotherResponse != null)
        {
          context.TraceSerializedConditionally(5256050, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", false, "Aad response:'{0}' - MsGraph response:'{1}'", (object) obj, (object) anotherResponse);
          string empty = string.Empty;
          try
          {
            string difference = obj.CompareAndGetDifference((AadServiceResponse) anotherResponse);
            if (!string.IsNullOrEmpty(difference))
            {
              context.Trace(5256043, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", "Response from '" + operation + "' are different between Aad and MsGraph result, see details.\r\n" + difference);
              context.TraceSerializedConditionally(5256052, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", false, "Aad response:'{0}' - MsGraph response:'{1}'", (object) obj, (object) anotherResponse);
            }
            else
              context.TraceAlways(5256048, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", "Response from '" + operation + "' are Equal between Aad and MsGraph result");
          }
          catch (Exception ex)
          {
            SharedAadService.LogExceptionWithStackTraceHelper(context, 5256045, ex, "Data Compare", operation);
          }
        }
        else
          context.Trace(5256046, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", "Microsoft graph experiment got null responses: '" + ((object) obj == null ? "Aad response is null" : "") + "' : '" + ((object) anotherResponse == null ? "MsGraph response is null" : "") + "'");
      }
      return obj;
    }

    internal bool IsDualReadEligible(
      IVssRequestContext context,
      string operation,
      AadServiceRequest request)
    {
      return !context.IsFeatureEnabled("VisualStudio.Services.Aad.DisableMicrosoftGraph") && !context.IsFeatureEnabled(AadServiceConstants.FeatureFlags.DisableMicrosoftGraphByOperation(operation)) && !context.IsFeatureEnabled(AadServiceConstants.FeatureFlags.EnableMicrosoftGraphByOperation(operation)) && context.IsFeatureEnabled("VisualStudio.Services.Aad.EnableGraphDataCompare") && (!(request is AadServicePagedRequest) || string.IsNullOrEmpty((request as AadServicePagedRequest).PagingToken)) && request.GraphApiSupportLevel == GraphApiSupportLevel.BothAadAndMicrosoftGraph && SharedAadService.ShouldExecuteDualReadHelper();
    }

    internal bool DetermineUseMicrosoftGraph(
      IVssRequestContext context,
      string operation,
      AadServiceRequest request)
    {
      switch (MicrosoftGraphUtils.GetRequestPagingTokenType(request))
      {
        case PagingTokenType.None:
        case PagingTokenType.Invalid:
          return !context.IsFeatureEnabled("VisualStudio.Services.Aad.DisableMicrosoftGraph") && request.GraphApiSupportLevel == GraphApiSupportLevel.BothAadAndMicrosoftGraph && !context.IsFeatureEnabled(AadServiceConstants.FeatureFlags.DisableMicrosoftGraphByOperation(operation)) && context.IsFeatureEnabled(AadServiceConstants.FeatureFlags.EnableMicrosoftGraphByOperation(operation));
        case PagingTokenType.AadGraph:
          context.TraceConditionally(5256047, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => "PagingTokenType.AadGraph type has been received during operation : '" + operation + "'"));
          return false;
        case PagingTokenType.MicrosoftGraph:
          context.TraceAlways(5256049, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", "PagingTokenType.MicrosoftGraph type has been received during operation : '" + operation + "'");
          return true;
        default:
          return false;
      }
    }

    private T ExecuteGraphRequest<T>(
      IVssRequestContext context1,
      AadServiceRequest request,
      string operation,
      string tenantId1,
      bool application1,
      bool useMicrosoftGraph,
      IAadPerfCounter counter,
      bool bypassCache = false)
      where T : AadServiceResponse
    {
      T obj = default (T);
      try
      {
        Func<IMicrosoftGraphClient> msGraphClient = (Func<IMicrosoftGraphClient>) (() => !request.UseBetaGraphVersion ? this.GetMicrosoftGraphClient(request.MicrosoftGraphBaseUrlOverride) : this.GetMicrosoftGraphClient(microsoftGraphVersionOverride: "beta"));
        Func<IVssRequestContext, string, bool, bool, JwtSecurityToken> accessToken = (Func<IVssRequestContext, string, bool, bool, JwtSecurityToken>) ((context2, tenantId2, application2, isMicrosoftGraphApi) => !string.IsNullOrEmpty(request.AccessToken) ? SharedAadService.jwtSecurityTokenHandler.ReadJwtToken(request.AccessToken) : this.GetAccessToken(context2, tenantId2, application2, isMicrosoftGraphApi));
        AadServiceRequestContext context = request is GetDescendantIdsRequest<Guid> || request is GetDescendantIdsRequest<IdentityDescriptor> ? new AadServiceRequestContext(context1, tenantId1, application1, this.settings.OperationSettings, new Func<IAadGraphClient>(this.GetGraphClientForDescendantIdsOperation), msGraphClient, accessToken) : new AadServiceRequestContext(context1, tenantId1, application1, this.settings.OperationSettings, new Func<IAadGraphClient>(this.GetGraphClient), msGraphClient, accessToken);
        switch (request.GraphApiSupportLevel)
        {
          case GraphApiSupportLevel.AadGraphOnly:
            obj = (T) request.Execute(context);
            break;
          case GraphApiSupportLevel.BothAadAndMicrosoftGraph:
            if (useMicrosoftGraph)
            {
              context1.TraceConditionally(5256051, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => "Invoking Microsoft Graph request '" + operation + "'"));
              obj = (T) request.ExecuteWithMicrosoftGraph(context, bypassCache);
              break;
            }
            obj = (T) request.Execute(context);
            break;
          case GraphApiSupportLevel.MicrosoftGraphOnly:
            obj = (T) request.ExecuteWithMicrosoftGraph(context);
            break;
        }
      }
      catch (AadException ex) when (ex is AadGraphAuthenticationException || ex is MicrosoftGraphAuthenticationException)
      {
        AadServiceCounters.All.IncrementErrorCounters();
        counter?.IncrementErrorCounters();
        context1.TraceException(5256006, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", (Exception) ex);
        throw;
      }
      catch (AadException ex) when (ex is AadGraphObjectNotFoundException || ex is MicrosoftGraphObjectNotFoundException)
      {
        AadServiceCounters.All.IncrementErrorCounters();
        counter?.IncrementErrorCounters();
        throw;
      }
      catch (AadCredentialsNotFoundException ex)
      {
        AadServiceCounters.All.IncrementErrorCounters();
        counter?.IncrementErrorCounters();
        throw;
      }
      catch (AadAccessSilentException ex)
      {
        AadServiceCounters.All.IncrementErrorCounters();
        counter?.IncrementErrorCounters();
        context1.TraceException(5256007, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", (Exception) ex);
        SharedAadService.ForceSignOutUser(context1);
        throw;
      }
      catch (Exception ex)
      {
        AadServiceCounters.All.IncrementFailureCounters();
        counter?.IncrementFailureCounters();
        context1.TraceException(5256003, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", ex);
        throw;
      }
      return obj;
    }

    private static void ForceSignOutUser(IVssRequestContext context)
    {
      if (context.IsFeatureEnabled("VisualStudio.Services.Aad.ProcessRequest.ForceSignOutUser.Disable"))
        return;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = context.GetUserIdentity();
      if (!userIdentity.IsExternalUser)
        return;
      bool flag = false;
      try
      {
        Guid guid = userIdentity.StorageKey(context, TeamFoundationHostType.Deployment);
        IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
        IdentityService service = vssRequestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(vssRequestContext, (IList<Guid>) new Guid[1]
        {
          guid
        }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity == null)
          return;
        identity.SetProperty("AuthenticationCredentialValidFrom", (object) (DateTimeOffset.UtcNow - IdentityConstants.AuthenticationCredentialValidFromBackDate).Ticks.ToString());
        flag = service.UpdateIdentities(vssRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
        {
          identity
        });
      }
      catch (Exception ex)
      {
        context.TraceException(5256017, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", ex);
      }
      finally
      {
        context.TraceAlways(5256017, TraceLevel.Warning, "VisualStudio.Services.Aad", "Service", string.Format("Force sign out result for user identity [{0}] after AadAccessSilentException: [{1}]", (object) userIdentity.Id, (object) flag));
      }
    }

    private void ValidateContext(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.IsSystemContext)
        ArgumentUtility.CheckForNull<IdentityDescriptor>(context.UserContext, "context.UserContext");
      if (this.serviceStartedWithDeploymentContext != context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException(string.Format("This service {0} started with deployment context but a {1} context was passed.", this.serviceStartedWithDeploymentContext ? (object) "was" : (object) "was not", (object) context.ServiceHost.HostType));
    }

    private void ValidateRequest(AadServiceRequest request)
    {
      ArgumentUtility.CheckForNull<AadServiceRequest>(request, nameof (request));
      bool flag = !string.IsNullOrWhiteSpace(request.ToTenant);
      bool toUserTenant = request.ToUserTenant;
      bool microsoftServicesTenant = request.ToMicrosoftServicesTenant;
      if (this.serviceStartedWithDeploymentContext)
      {
        if (!(flag ^ toUserTenant ^ microsoftServicesTenant))
          throw new ArgumentException("A deployment request must specify exactly one of the following: ToTenant, ToUserTenant, or ToMicrosoftServicesTenant.");
      }
      else if (flag | toUserTenant | microsoftServicesTenant)
        throw new ArgumentException("A non-deployment request must not specify any of the following: ToTenant, ToUserTenant, or ToMicrosoftServicesTenant.");
      request.Validate();
    }

    private void AuthorizeRequest(
      IVssRequestContext context,
      AadServiceRequest request,
      out string tenantId,
      out bool application)
    {
      if (context == null || request == null)
        throw new AadAccessException(string.Format("Invalid parameters to authorize request. Context is null: {0}. Request is null: {1}.", (object) (context == null), (object) (request == null)));
      if (!string.IsNullOrEmpty(request.AccessToken))
      {
        tenantId = (string) null;
        application = false;
      }
      else if (!this.IsRequestAllowed(context, request, out tenantId, out application))
        throw new AadAccessException(string.Format("The requesting identity (CUID {0}) does not have permission to access the tenant ({1}).", (object) context.GetUserCuid(), (object) tenantId));
    }

    private bool IsRequestAllowed(
      IVssRequestContext context,
      AadServiceRequest request,
      out string requestTenantId,
      out bool application)
    {
      application = false;
      string reason;
      (requestTenantId, reason) = this.IdentifyTenant(context, request);
      if (string.IsNullOrEmpty(requestTenantId))
      {
        context.TraceAlways(5256011, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", "Request tenant not found. Returning false. Reason: " + reason + ".");
        return false;
      }
      if (context.IsSystemContext)
      {
        context.Trace(5256012, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", "Request is at system context. Returning true.");
        application = true;
        return true;
      }
      string a = AadIdentityHelper.GetIdentityTenantId(context.UserContext).ToString();
      if (string.Equals(a, requestTenantId, StringComparison.InvariantCultureIgnoreCase))
      {
        context.Trace(5256015, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", string.Format("User '{0}' in tenant '{1}' is requesting operation within tenant. Returning true.", (object) context.UserContext, (object) a));
        application = context.UserContext.IsAadServicePrincipalType();
        return true;
      }
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, AadSecurity.NamespaceId);
      if (securityNamespace != null)
      {
        string token = AadSecurity.TenantsToken + AadSecurity.PathSeparator.ToString() + requestTenantId;
        if (securityNamespace.HasPermission(vssRequestContext, token, 1))
        {
          context.Trace(5256013, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", string.Format("Permission check succeeded for user '{0}'. Returning true.", (object) vssRequestContext.UserContext));
          application = true;
          return true;
        }
        context.Trace(5256014, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", string.Format("Permission check failed for user '{0}'. Continuing evaluation...", (object) vssRequestContext.UserContext));
      }
      context.Trace(5256016, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", string.Format("All authorization checks failed for user '{0}' requesting operation in tenant '{1}'. Returning false.", (object) context.UserContext, (object) requestTenantId));
      return false;
    }

    private (string tenantId, string reason) IdentifyTenant(
      IVssRequestContext context,
      AadServiceRequest request)
    {
      if (!string.IsNullOrWhiteSpace(request.ToTenant))
        return (request.ToTenant, "Request to be routed to a specific tenant " + request.ToTenant + ".");
      if (request.ToUserTenant)
      {
        Guid identityTenantId = AadIdentityHelper.GetIdentityTenantId(context.UserContext);
        string str = identityTenantId.ToString();
        return identityTenantId.Equals(Guid.Empty) ? ((string) null, "No valid user's home tenant found to route the request to.") : (str, "Request to be routed to user's home tenant " + str + ".");
      }
      if (request.ToMicrosoftServicesTenant)
      {
        object input;
        Guid result;
        return this.settings.OperationSettings.TryGetValue("/Service/Aad/MicrosoftServicesTenantForGetTenantsByKey", out input) && Guid.TryParse(input as string, out result) && !context.IsFeatureEnabled("VisualStudio.Services.Aad.DisableUseGuidMicrosoftServiceTenant") ? (result.ToString(), string.Format("Request to be routed to Microsoft Service tenant {0}.", (object) result)) : (this.settings.MicrosoftServicesTenant, "Request to be routed to Microsoft tenant " + this.settings.MicrosoftServicesTenant + ".");
      }
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return ((string) null, "Request context is deployment.");
      Guid organizationAadTenantId = context.GetOrganizationAadTenantId();
      if (organizationAadTenantId.Equals(Guid.Empty))
        return ((string) null, "No valid organization tenant found to route the request to.");
      string str1 = organizationAadTenantId.ToString();
      return (str1, "Request to be routed to organization tenant " + str1 + ".");
    }

    private IAadGraphClient GetGraphClient()
    {
      GraphClientsFactory graphClientFactory = this.graphClientFactory;
      string graphApiDomainName1 = this.settings.GraphApiDomainName;
      string graphApiVersion = this.settings.GraphApiVersion;
      string graphApiDomainName2 = graphApiDomainName1;
      return graphClientFactory.CreateGraphClient(graphApiVersion, graphApiDomainName2);
    }

    private IMicrosoftGraphClient GetMicrosoftGraphClient(
      string microsoftGraphRootUrlOverride = null,
      string microsoftGraphVersionOverride = null)
    {
      return this.graphClientFactory.CreateMicrosoftGraphClient(microsoftGraphRootUrlOverride, microsoftGraphVersionOverride);
    }

    private IAadGraphClient GetGraphClientForDescendantIdsOperation()
    {
      string paramName;
      if (!this.settings.OperationSettings.TryGetValue<string>("/Service/Aad/GetDescendantIds/GraphApiVersion", out paramName))
        throw new ArgumentException("No Graph Api Version is set for Get Descendant Ids Operation", paramName);
      bool flag;
      if (this.settings.OperationSettings.TryGetValue<bool>("VisualStudio.Services.Aad.EnableV2EndpointForGetDescendantIds", out flag) & flag)
      {
        GraphClientsFactory graphClientFactory = this.graphClientFactory;
        string str = this.settings.GraphApiDomainName + "/v2";
        string graphApiVersion = paramName;
        string graphApiDomainName = str;
        return graphClientFactory.CreateGraphClient(graphApiVersion, graphApiDomainName);
      }
      GraphClientsFactory graphClientFactory1 = this.graphClientFactory;
      string graphApiDomainName1 = this.settings.GraphApiDomainName;
      string graphApiVersion1 = paramName;
      string graphApiDomainName2 = graphApiDomainName1;
      return graphClientFactory1.CreateGraphClient(graphApiVersion1, graphApiDomainName2);
    }

    private JwtSecurityToken GetAccessToken(
      IVssRequestContext context,
      string tenantId,
      bool application,
      bool isMicrosoftGraphApi = false)
    {
      string resource = !isMicrosoftGraphApi ? "https://" + this.settings.GraphApiDomainName : this.microsoftGraphApiDomainNameConfig.QueryByCtx<string>(context);
      IAadTokenService aadTokenService = this.tokenServiceFactory.GetAadTokenService(context);
      if (application)
      {
        context.TraceConditionally(5256004, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("GetAccessTokenForApplication: {0}", (object) context.UserContext)));
        return aadTokenService.AcquireAppToken(context, resource, tenantId);
      }
      context.TraceConditionally(5256005, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("GetAccessTokenForUser: {0}", (object) context.UserContext)));
      return aadTokenService.AcquireToken(context, resource, tenantId);
    }

    private static bool ShouldExecuteDualReadHelper() => Interlocked.Increment(ref SharedAadService.eligleForDataCompareRequestCount) % 100L < (long) SharedAadService.microsoftGraphDataComparePercentage;

    private static void LogExceptionWithStackTraceHelper(
      IVssRequestContext context,
      int tracePoint,
      Exception ex,
      string step,
      string operation)
    {
      context.TraceException(tracePoint, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", ex, string.Format("Abandon Microsoft graph experiment, call to '{0}' ran into exception. Operation: '{1}' - Exception:'{2}'", (object) step, (object) operation, (object) ex));
    }

    private class Settings
    {
      private readonly IDictionary<string, object> operationSettings = (IDictionary<string, object>) new ConcurrentDictionary<string, object>();

      internal string AuthAuthority { get; set; }

      internal string AuthClientId { get; set; }

      internal string GraphApiDomainName { get; set; }

      internal string GraphApiResource { get; set; }

      internal string GraphApiVersion { get; set; }

      [Obsolete]
      internal string MicrosoftServicesTenant { get; set; }

      internal IDictionary<string, object> OperationSettings => this.operationSettings;
    }
  }
}
