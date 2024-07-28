// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraphClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Cloud.Aad.Throttling;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph.Extensions;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  internal class MicrosoftGraphClient : IMicrosoftGraphClient
  {
    private readonly IGraphServiceClientFactory graphServiceClientFactory;
    private static readonly CommandPropertiesSetter commandPropertiesSetterDefault = (CommandPropertiesSetter) new AadGraphClient.CommandPropertiesSetterDefault();
    private static readonly CommandPropertiesSetter commandPropertiesSetterLong = (CommandPropertiesSetter) new AadGraphClient.CommandPropertiesSetterLongExecutionTimeout();
    public static readonly IReadOnlyDictionary<string, CommandPropertiesSetter> CommandPropertiesSetterMap = (IReadOnlyDictionary<string, CommandPropertiesSetter>) new Dictionary<string, CommandPropertiesSetter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "GetUserStatusWithIdRequest",
        MicrosoftGraphClient.commandPropertiesSetterDefault
      },
      {
        "GetDescendantIdsRequest",
        MicrosoftGraphClient.commandPropertiesSetterLong
      }
    };
    protected const string Layer = "MicrosoftGraphClient";

    public MicrosoftGraphClient(
      IGraphServiceClientFactory graphServiceClientFactory = null)
    {
      this.graphServiceClientFactory = graphServiceClientFactory ?? (IGraphServiceClientFactory) new GraphServiceClientFactory();
    }

    public GetUserStatusWithIdResponse GetUserStatusWithId(
      IVssRequestContext context,
      GetUserStatusWithIdRequest request)
    {
      return this.ProcessRequest<GetUserStatusWithIdResponse>(context, (MicrosoftGraphClientRequest<GetUserStatusWithIdResponse>) request, MsGraphClientCounters.GetUserStatusWithId, nameof (GetUserStatusWithId));
    }

    public Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsResponse GetServicePrincipalsByIds(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsRequest request)
    {
      return this.ProcessRequest<Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsResponse>(context, (MicrosoftGraphClientRequest<Microsoft.VisualStudio.Services.Aad.Graph.GetServicePrincipalsByIdsResponse>) request, MsGraphClientCounters.GetServicePrincipalsByIds, nameof (GetServicePrincipalsByIds));
    }

    public MsGraphGetServicePrincipalsResponse GetServicePrincipals(
      IVssRequestContext context,
      MsGraphGetServicePrincipalsRequest request)
    {
      return this.ProcessRequest<MsGraphGetServicePrincipalsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetServicePrincipalsResponse>) request, MsGraphClientCounters.GetServicePrincipals, nameof (GetServicePrincipals));
    }

    public MsGraphGetUsersResponse GetUsers(
      IVssRequestContext context,
      MsGraphGetUsersRequest request)
    {
      return this.ProcessRequest<MsGraphGetUsersResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetUsersResponse>) request, MsGraphClientCounters.GetUsers, nameof (GetUsers));
    }

    public MsGraphGetGroupsResponse GetGroups(
      IVssRequestContext context,
      MsGraphGetGroupsRequest request)
    {
      return this.ProcessRequest<MsGraphGetGroupsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetGroupsResponse>) request, MsGraphClientCounters.GetGroups, nameof (GetGroups));
    }

    public MsGraphGetUsersWithIdsResponse GetUsersWithIds(
      IVssRequestContext context,
      MsGraphGetUsersWithIdsRequest request)
    {
      return this.ProcessRequest<MsGraphGetUsersWithIdsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetUsersWithIdsResponse>) request, MsGraphClientCounters.GetUsersWithIds, nameof (GetUsersWithIds));
    }

    public MsGraphGetUserRolesAndGroupsResponse GetUserRolesAndGroups(
      IVssRequestContext context,
      MsGraphGetUserRolesAndGroupsRequest request)
    {
      return this.ProcessRequest<MsGraphGetUserRolesAndGroupsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetUserRolesAndGroupsResponse>) request, MsGraphClientCounters.GetUserRolesAndGroups, nameof (GetUserRolesAndGroups));
    }

    public MsGraphGetTenantReponse GetTenant(
      IVssRequestContext context,
      MsGraphGetTenantRequest request)
    {
      return this.ProcessRequest<MsGraphGetTenantReponse>(context, (MicrosoftGraphClientRequest<MsGraphGetTenantReponse>) request, MsGraphClientCounters.GetTenant, nameof (GetTenant));
    }

    public MsGraphGetUserThumbnailReponse GetUserThumbnail(
      IVssRequestContext context,
      MsGraphGetUserThumbnailRequest request)
    {
      return this.ProcessRequest<MsGraphGetUserThumbnailReponse>(context, (MicrosoftGraphClientRequest<MsGraphGetUserThumbnailReponse>) request, MsGraphClientCounters.GetUserThumbnail, nameof (GetUserThumbnail));
    }

    public MsGraphGetDirectoryRoleMembersResponse GetDirectoryRoleMembers(
      IVssRequestContext context,
      MsGraphGetDirectoryRoleMembersRequest request)
    {
      return this.ProcessRequest<MsGraphGetDirectoryRoleMembersResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetDirectoryRoleMembersResponse>) request, MsGraphClientCounters.GetDirectoryRoleMembers, nameof (GetDirectoryRoleMembers));
    }

    public MsGraphGetDirectoryRolesWithIdsResponse GetDirectoryRolesWithIds(
      IVssRequestContext context,
      MsGraphGetDirectoryRolesWithIdsRequest request)
    {
      return this.ProcessRequest<MsGraphGetDirectoryRolesWithIdsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetDirectoryRolesWithIdsResponse>) request, MsGraphClientCounters.GetDirectoryRolesWithIds, nameof (GetDirectoryRolesWithIds));
    }

    public MsGraphGetDescendantIdsResponse GetDescendantIds(
      IVssRequestContext context,
      MsGraphGetDescendantIdsRequest request)
    {
      return this.ProcessRequest<MsGraphGetDescendantIdsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetDescendantIdsResponse>) request, MsGraphClientCounters.GetDescendantIds, nameof (GetDescendantIds));
    }

    public MsGraphGetDescendantsResponse GetDescendants(
      IVssRequestContext context,
      MsGraphGetDescendantsRequest request)
    {
      return this.ProcessRequest<MsGraphGetDescendantsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetDescendantsResponse>) request, MsGraphClientCounters.GetDescendants, nameof (GetDescendants));
    }

    public MsGraphGetSoftDeletedObjectsReponse<T> GetSoftDeletedObjects<T>(
      IVssRequestContext context,
      MsGraphGetSoftDeletedObjectsRequest<T> request)
      where T : AadObject
    {
      return this.ProcessRequest<MsGraphGetSoftDeletedObjectsReponse<T>>(context, (MicrosoftGraphClientRequest<MsGraphGetSoftDeletedObjectsReponse<T>>) request, MsGraphClientCounters.GetDirectoryRoles, nameof (GetSoftDeletedObjects));
    }

    public MsGraphGetAncestorIdsResponse GetAncestorIds(
      IVssRequestContext context,
      MsGraphGetAncestorIdsRequest request)
    {
      return this.ProcessRequest<MsGraphGetAncestorIdsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetAncestorIdsResponse>) request, MsGraphClientCounters.GetAncestorIds, nameof (GetAncestorIds));
    }

    public MsGraphGetDirectoryRolesResponse GetDirectoryRoles(
      IVssRequestContext context,
      MsGraphGetDirectoryRolesRequest request)
    {
      return this.ProcessRequest<MsGraphGetDirectoryRolesResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetDirectoryRolesResponse>) request, MsGraphClientCounters.GetSoftDeletedObjects, nameof (GetDirectoryRoles));
    }

    public MsGraphGetGroupsWithIdsResponse GetGroupsWithIds(
      IVssRequestContext context,
      MsGraphGetGroupsWithIdsRequest request)
    {
      return this.ProcessRequest<MsGraphGetGroupsWithIdsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetGroupsWithIdsResponse>) request, MsGraphClientCounters.GetGroupsWithIds, nameof (GetGroupsWithIds));
    }

    public MsGraphGetApplicationByIdResponse GetApplicationById(
      IVssRequestContext context,
      MsGraphGetApplicationByIdRequest request)
    {
      return this.ProcessRequest<MsGraphGetApplicationByIdResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetApplicationByIdResponse>) request, MsGraphClientCounters.GetApplicationById, nameof (GetApplicationById));
    }

    public MsGraphCreateApplicationResponse CreateApplication(
      IVssRequestContext context,
      MsGraphCreateApplicationRequest request)
    {
      return this.ProcessRequest<MsGraphCreateApplicationResponse>(context, (MicrosoftGraphClientRequest<MsGraphCreateApplicationResponse>) request, MsGraphClientCounters.CreateApplication, nameof (CreateApplication));
    }

    public MsGraphUpdateApplicationResponse UpdateApplication(
      IVssRequestContext context,
      MsGraphUpdateApplicationRequest request)
    {
      return this.ProcessRequest<MsGraphUpdateApplicationResponse>(context, (MicrosoftGraphClientRequest<MsGraphUpdateApplicationResponse>) request, MsGraphClientCounters.UpdateApplication, nameof (UpdateApplication));
    }

    public MsGraphDeleteApplicationResponse DeleteApplication(
      IVssRequestContext context,
      MsGraphDeleteApplicationRequest request)
    {
      return this.ProcessRequest<MsGraphDeleteApplicationResponse>(context, (MicrosoftGraphClientRequest<MsGraphDeleteApplicationResponse>) request, MsGraphClientCounters.DeleteApplication, nameof (DeleteApplication));
    }

    public MsGraphCreateServicePrincipalResponse CreateServicePrincipal(
      IVssRequestContext context,
      MsGraphCreateServicePrincipalRequest request)
    {
      return this.ProcessRequest<MsGraphCreateServicePrincipalResponse>(context, (MicrosoftGraphClientRequest<MsGraphCreateServicePrincipalResponse>) request, MsGraphClientCounters.CreateServicePrincipal, nameof (CreateServicePrincipal));
    }

    public MsGraphDeleteServicePrincipalResponse DeleteServicePrincipal(
      IVssRequestContext context,
      MsGraphDeleteServicePrincipalRequest request)
    {
      return this.ProcessRequest<MsGraphDeleteServicePrincipalResponse>(context, (MicrosoftGraphClientRequest<MsGraphDeleteServicePrincipalResponse>) request, MsGraphClientCounters.DeleteServicePrincipal, nameof (DeleteServicePrincipal));
    }

    public MsGraphAddApplicationPasswordResponse AddApplicationPassword(
      IVssRequestContext context,
      MsGraphAddApplicationPasswordRequest request)
    {
      return this.ProcessRequest<MsGraphAddApplicationPasswordResponse>(context, (MicrosoftGraphClientRequest<MsGraphAddApplicationPasswordResponse>) request, MsGraphClientCounters.AddApplicationPassword, nameof (AddApplicationPassword));
    }

    public MsGraphRemoveApplicationPasswordResponse RemoveApplicationPassword(
      IVssRequestContext context,
      MsGraphRemoveApplicationPasswordRequest request)
    {
      return this.ProcessRequest<MsGraphRemoveApplicationPasswordResponse>(context, (MicrosoftGraphClientRequest<MsGraphRemoveApplicationPasswordResponse>) request, MsGraphClientCounters.RemoveApplicationPassword, nameof (RemoveApplicationPassword));
    }

    public MsGraphAddApplicationFederatedCredentialsResponse AddApplicationFederatedCredentials(
      IVssRequestContext context,
      MsGraphAddApplicationFederatedCredentialsRequest request)
    {
      return this.ProcessRequest<MsGraphAddApplicationFederatedCredentialsResponse>(context, (MicrosoftGraphClientRequest<MsGraphAddApplicationFederatedCredentialsResponse>) request, MsGraphClientCounters.AddApplicationFederatedCredentials, nameof (AddApplicationFederatedCredentials));
    }

    public MsGraphRemoveApplicationFederatedCredentialsResponse RemoveApplicationFederatedCredentials(
      IVssRequestContext context,
      MsGraphRemoveApplicationFederatedCredentialsRequest request)
    {
      return this.ProcessRequest<MsGraphRemoveApplicationFederatedCredentialsResponse>(context, (MicrosoftGraphClientRequest<MsGraphRemoveApplicationFederatedCredentialsResponse>) request, MsGraphClientCounters.RemoveApplicationFederatedCredentials, nameof (RemoveApplicationFederatedCredentials));
    }

    public MsGraphGetResourceTenantsResponse GetResourceTenants(
      IVssRequestContext context,
      MsGraphGetResourceTenantsRequest request)
    {
      return this.ProcessRequest<MsGraphGetResourceTenantsResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetResourceTenantsResponse>) request, MsGraphClientCounters.GetResourceTenants, nameof (GetResourceTenants));
    }

    public MsGraphGetOrganizationDataBoundaryResponse GetOrganizationDataBoundary(
      IVssRequestContext context,
      MsGraphGetOrganizationDataBoundaryRequest request)
    {
      return this.ProcessRequest<MsGraphGetOrganizationDataBoundaryResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetOrganizationDataBoundaryResponse>) request, MsGraphClientCounters.GetOrganizationDataBoundary, nameof (GetOrganizationDataBoundary));
    }

    public MsGraphGetProfileDataResponse GetProfileData(
      IVssRequestContext context,
      MsGraphGetProfileDataRequest request)
    {
      return this.ProcessRequest<MsGraphGetProfileDataResponse>(context, (MicrosoftGraphClientRequest<MsGraphGetProfileDataResponse>) request, MsGraphClientCounters.GetProfileData, nameof (GetProfileData));
    }

    private T ProcessRequest<T>(
      IVssRequestContext context,
      MicrosoftGraphClientRequest<T> request,
      IAadPerfCounter counter = null,
      [CallerMemberName] string operation = "")
      where T : MicrosoftGraphClientResponse
    {
      try
      {
        context.TraceConditionally(44750001, TraceLevel.Info, "MicrosoftGraph", operation, (Func<string>) (() => string.Format("Request: {0}", (object) request)));
        using (MsGraphMetric msGraphMetric = new MsGraphMetric(context, operation))
        {
          MsGraphClientCounters.All.IncrementEntryCounters();
          counter?.IncrementEntryCounters();
          try
          {
            request.Validate();
          }
          catch (Exception ex)
          {
            msGraphMetric.Result = "Error";
            msGraphMetric.ExceptionType = ex.GetType().Name;
            MsGraphClientCounters.All.IncrementErrorCounters();
            counter?.IncrementErrorCounters();
            context.TraceException(44750008, "MicrosoftGraph", nameof (MicrosoftGraphClient), ex);
            throw;
          }
          T obj;
          try
          {
            Guid organizationAadTenantId = context.GetOrganizationAadTenantId();
            using (PerformanceTimer.StartMeasure(context, "AadGraph"))
            {
              if (organizationAadTenantId != new Guid())
              {
                IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                obj = vssRequestContext.GetService<AadThrottlingService>().Execute<T>(vssRequestContext, organizationAadTenantId.ToString(), AadServiceType.MicrosoftGraph, (Func<T>) (() => this.ExecuteRequest<T>(context, request, operation)), MicrosoftGraphClient.\u003C\u003EO.\u003C0\u003E__IsGraphRequestThrottled ?? (MicrosoftGraphClient.\u003C\u003EO.\u003C0\u003E__IsGraphRequestThrottled = new Func<Exception, AadThrottleInfo>(MicrosoftGraphClient.IsGraphRequestThrottled)));
              }
              else
                obj = this.ExecuteRequest<T>(context, request, operation);
            }
          }
          catch (Exception ex)
          {
            string format = (string) null;
            int tracepoint;
            Exception exception;
            switch (ex)
            {
              case ClientException _:
                tracepoint = 44750002;
                exception = (Exception) new MicrosoftGraphException(ex.Message, ex);
                break;
              case ServiceException serviceException:
                if (serviceException.IsResourceNotFoundError())
                {
                  tracepoint = 44750003;
                  exception = (Exception) new MicrosoftGraphObjectNotFoundException((Exception) serviceException);
                }
                else if (serviceException.IsUnauthorizedError())
                {
                  tracepoint = 44750004;
                  exception = (Exception) new MicrosoftGraphAuthenticationException((Exception) serviceException);
                }
                else if (serviceException.IsRequestDeniedError())
                {
                  tracepoint = 44750009;
                  exception = request.IsHttpResponseDisplayingForAppsEnabled<T>(context) ? (Exception) new MicrosoftGraphRequestDeniedExtendedException(serviceException) : (Exception) new MicrosoftGraphRequestDeniedException((Exception) serviceException);
                }
                else
                {
                  tracepoint = 44750005;
                  exception = (Exception) new MicrosoftGraphException(((Exception) serviceException).Message, (Exception) serviceException);
                }
                format = request.IsHttpResponseLoggingForAppsEnabled<T>(context) ? string.Format("Raw exception message: '{0}'. Request: '{1}'. Raw response body: '{2}'", (object) ((Exception) serviceException).Message, (object) request, (object) serviceException.RawResponseBody) : string.Format("Raw exception message: '{0}'. Request: '{1}'", (object) ((Exception) serviceException).Message, (object) request);
                break;
              default:
                tracepoint = 44750006;
                exception = ex;
                break;
            }
            msGraphMetric.Result = "Error";
            msGraphMetric.ExceptionType = exception.GetType().Name;
            MsGraphClientCounters.All.IncrementErrorCounters();
            counter?.IncrementErrorCounters();
            if (string.IsNullOrEmpty(format))
              context.TraceException(tracepoint, "MicrosoftGraph", nameof (MicrosoftGraphClient), ex);
            else
              context.TraceException(tracepoint, TraceLevel.Error, "MicrosoftGraph", nameof (MicrosoftGraphClient), ex, format);
            throw exception;
          }
          msGraphMetric.Result = "Success";
          MsGraphClientCounters.All.IncrementSuccessCounters();
          counter?.IncrementSuccessCounters();
          return obj;
        }
      }
      finally
      {
        context.TraceLeave(44750007, "MicrosoftGraph", nameof (MicrosoftGraphClient), operation);
      }
    }

    private T ExecuteRequest<T>(
      IVssRequestContext context,
      MicrosoftGraphClientRequest<T> request,
      string operation)
      where T : MicrosoftGraphClientResponse
    {
      CommandPropertiesSetter propertiesSetterDefault;
      MicrosoftGraphClient.CommandPropertiesSetterMap.TryGetValue(request.GetType().Name, out propertiesSetterDefault);
      if (propertiesSetterDefault == null)
        propertiesSetterDefault = MicrosoftGraphClient.commandPropertiesSetterDefault;
      return new CommandService<T>(context, CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) operation).AndCommandPropertiesDefaults(propertiesSetterDefault), (Func<T>) (() => this.ExecuteRequestInCircuitBreaker<T>(context, request)), (Func<T>) (() =>
      {
        throw new AadServiceNotAvailableException("Aad service not responding to requests.");
      })).Execute();
    }

    private T ExecuteRequestInCircuitBreaker<T>(
      IVssRequestContext context,
      MicrosoftGraphClientRequest<T> request)
      where T : MicrosoftGraphClientResponse
    {
      try
      {
        return request.Execute(context, this.graphServiceClientFactory.CreateGraphServiceClient(context, request.AccessToken.RawData));
      }
      catch (ServiceException ex)
      {
        if (ex.IsResourceNotFoundError() || ex.IsUnauthorizedError() || ex.IsBadRequest() || ex.IsRequestDeniedError())
          ((Exception) ex).Data[(object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}"] = (object) true;
        throw;
      }
    }

    private static AadThrottleInfo IsGraphRequestThrottled(Exception exception)
    {
      for (int index = 0; exception != null && index < 100; exception = exception.InnerException)
      {
        if (exception is ServiceException serviceException && serviceException.StatusCode == (HttpStatusCode) 429)
          return new AadThrottleInfo()
          {
            IsThrottled = true,
            RetryAfter = (TimeSpan?) serviceException.ResponseHeaders?.RetryAfter?.Delta
          };
        ++index;
      }
      return new AadThrottleInfo() { IsThrottled = false };
    }
  }
}
