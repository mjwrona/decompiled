// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.AzureServicePrincipalHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Server.Helpers.SecretsExpiration;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public static class AzureServicePrincipalHelper
  {
    internal static string ServiceEndpointArmUri = string.Empty;
    internal static string ServiceEndpointGraphUri = string.Empty;
    internal static string ServiceEndpointAuthorityUri = string.Empty;
    internal static string ServiceEndpointGraphApiVersion = string.Empty;
    internal static string ServiceEndpointMicrosoftGraphUri = string.Empty;
    internal static string ServiceEndpointMicrosoftGraphApiVersion = string.Empty;
    internal const string ReadOnlyDisabledSubscription = "ReadOnlyDisabledSubscription";
    internal const string AuthorizationFailed = "AuthorizationFailed";
    internal const string ParentResourceNotFound = "ParentResourceNotFound";
    internal const string NonRetriable = "NonRetriable";
    internal const string CanIgnore = "CanIgnore";
    internal static HttpClientHandler HttpClientHandler;
    internal static GraphConnection GraphConnection;
    private static Lazy<string> ProdMicrosoftGraphUrl = new Lazy<string>((Func<string>) (() => AzureServicePrincipalHelper.ServiceEndpointMicrosoftGraphUri + AzureServicePrincipalHelper.ServiceEndpointMicrosoftGraphApiVersion));
    private const string c_layer = "AzureServicePrincipalHelper";
    private const string SArea = "DistributedTask";
    private const string SLayer = "EndpointService";
    private const int MaxSpnLengthAllowed = 92;

    internal static void InitArmAndGraphDefaults(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint = null)
    {
      if (!requestContext.ExecutionEnvironment.IsDevFabricDeployment || AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
      {
        AzureServicePrincipalHelper.ServiceEndpointArmUri = "https://management.azure.com";
        AzureServicePrincipalHelper.ServiceEndpointGraphUri = "https://graph.windows.net/";
        AzureServicePrincipalHelper.ServiceEndpointAuthorityUri = "https://login.windows.net/";
        AzureServicePrincipalHelper.ServiceEndpointGraphApiVersion = "1.6";
        AzureServicePrincipalHelper.ServiceEndpointMicrosoftGraphUri = "https://graph.microsoft.com/";
        AzureServicePrincipalHelper.ServiceEndpointMicrosoftGraphApiVersion = "v1.0";
      }
      else
      {
        AzureServicePrincipalHelper.ServiceEndpointArmUri = "https://api-dogfood.resources.windows-int.net/";
        AzureServicePrincipalHelper.ServiceEndpointGraphUri = "https://graph.ppe.windows.net/";
        AzureServicePrincipalHelper.ServiceEndpointAuthorityUri = "https://login.windows-ppe.net/";
        AzureServicePrincipalHelper.ServiceEndpointGraphApiVersion = "1.5-internal";
        AzureServicePrincipalHelper.ServiceEndpointMicrosoftGraphUri = "https://graph.microsoft-ppe.com/";
        AzureServicePrincipalHelper.ServiceEndpointMicrosoftGraphApiVersion = "v1.0";
      }
      if (endpoint == null || requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        return;
      ServiceEndpointType serviceEndpointType = requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(requestContext, endpoint.Type, endpoint.Authorization.Scheme).FirstOrDefault<ServiceEndpointType>();
      string str = endpoint.Data.TryGetValue("environment", out str) ? str : "AzureCloud";
      if (serviceEndpointType == null || serviceEndpointType.DependencyData == null)
        return;
      foreach (DependencyData dependencyData in serviceEndpointType.DependencyData)
      {
        if (!string.IsNullOrEmpty(dependencyData.Input) && dependencyData.Input.Equals("environment", StringComparison.InvariantCultureIgnoreCase))
        {
          foreach (KeyValuePair<string, List<KeyValuePair<string, string>>> keyValuePair1 in dependencyData.Map)
          {
            if (!string.IsNullOrEmpty(keyValuePair1.Key) && keyValuePair1.Key.Equals(str, StringComparison.InvariantCultureIgnoreCase))
            {
              foreach (KeyValuePair<string, string> keyValuePair2 in keyValuePair1.Value)
              {
                if (!string.IsNullOrEmpty(keyValuePair2.Key))
                {
                  if (keyValuePair2.Key.Equals("resourceManagerUrl", StringComparison.InvariantCultureIgnoreCase))
                  {
                    AzureServicePrincipalHelper.ServiceEndpointArmUri = keyValuePair2.Value;
                    requestContext.TraceInfo(0, "EndpointService", "Service endpoint Arm Uri updated to {0}", (object) AzureServicePrincipalHelper.ServiceEndpointArmUri);
                  }
                  else if (keyValuePair2.Key.Equals("graphUrl", StringComparison.InvariantCultureIgnoreCase))
                  {
                    AzureServicePrincipalHelper.ServiceEndpointGraphUri = keyValuePair2.Value;
                    requestContext.TraceInfo(0, "EndpointService", "Service endpoint Graph Uri updated to {0}", (object) AzureServicePrincipalHelper.ServiceEndpointGraphUri);
                  }
                  else if (keyValuePair2.Key.Equals("environmentAuthorityUrl", StringComparison.InvariantCultureIgnoreCase))
                  {
                    AzureServicePrincipalHelper.ServiceEndpointAuthorityUri = keyValuePair2.Value;
                    requestContext.TraceInfo(0, "EndpointService", "Service endpoint Authority Uri updated to {0}", (object) AzureServicePrincipalHelper.ServiceEndpointAuthorityUri);
                  }
                }
              }
            }
          }
        }
      }
    }

    public static void UpdateAzureRmEndpointCreationMode(ServiceEndpoint endpoint, bool isUpdate)
    {
      if (!endpoint.Type.Equals("AzureRM", StringComparison.OrdinalIgnoreCase) || endpoint.Data.ContainsKey("creationMode") && !string.IsNullOrEmpty(endpoint.Data["creationMode"]))
        return;
      string str1;
      bool flag1 = endpoint.Data.TryGetValue("appObjectId", out str1) && !string.IsNullOrEmpty(str1);
      string str2;
      bool flag2 = endpoint.Data.TryGetValue("spnObjectId", out str2) && !string.IsNullOrEmpty(str2);
      string str3;
      bool flag3 = endpoint.Authorization.Parameters.TryGetValue("serviceprincipalid", out str3) && !string.IsNullOrEmpty(str3);
      string str4;
      bool flag4 = endpoint.Authorization.Parameters.TryGetValue("servicePrincipalKey", out str4) && !string.IsNullOrEmpty(str4);
      if (isUpdate)
      {
        if (flag1 | flag2 || !(flag3 | flag4))
        {
          endpoint.Data["creationMode"] = "Automatic";
        }
        else
        {
          if (!flag3)
            throw new ArgumentException(ServiceEndpointResources.ExpectedValueForServicePrincipalIdOrSetCreationModeAutomatic());
          endpoint.Data["creationMode"] = "Manual";
        }
      }
      else if (flag3 & flag4)
      {
        endpoint.Data["creationMode"] = "Manual";
      }
      else
      {
        if (flag3 | flag4)
          throw new ArgumentException(flag3 ? ServiceEndpointResources.ServicePrincipalKeyRequiredForManualCreationMode() : ServiceEndpointResources.ServicePrincipalClientIDRequiredForManualCreationMode());
        endpoint.Data["creationMode"] = "Automatic";
      }
    }

    public static bool IsSpnAutoCreateEndpoint(ServiceEndpoint endpoint)
    {
      bool endpoint1 = false;
      string str;
      if (endpoint.Data.TryGetValue("creationMode", out str))
        endpoint1 = endpoint.Type.Equals("AzureRM", StringComparison.OrdinalIgnoreCase) && str.Equals("Automatic", StringComparison.OrdinalIgnoreCase);
      return endpoint1;
    }

    public static void QueueSpnOperationJob(
      IVssRequestContext requestContext,
      ServicePrincipalJobData createServicePrincipalJobData,
      TimeSpan startOffset = default (TimeSpan))
    {
      using (new MethodScope(requestContext, nameof (AzureServicePrincipalHelper), nameof (QueueSpnOperationJob)))
      {
        try
        {
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) createServicePrincipalJobData);
          requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(requestContext, "AzureServicePrincipalEndpointJob", "Microsoft.Azure.DevOps.ServiceEndpoints.Server.Extensions.AzureServicePrincipalEndpointJob", xml, JobPriorityLevel.Highest, startOffset);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, TraceLevel.Error, "DistributedTask", "EndpointService", ex);
        }
      }
    }

    internal static void CreateAadApplicationAndAssignServicePrincipalPermissions(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string vstsToken = null,
      string projectName = "")
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (CreateAadApplicationAndAssignServicePrincipalPermissions));
      string parameter = endpoint.Authorization.Parameters["tenantid"];
      string tenantId = AzureServicePrincipalHelper.ValidateAndGetTenantId(requestContext, parameter);
      AzureServicePrincipalHelper.InitArmAndGraphDefaults(requestContext, endpoint);
      string armAccessToken = AzureServicePrincipalHelper.GetArmAccessToken(requestContext, tenantId, vstsToken);
      string graphAccessToken;
      AzureServicePrincipalHelper.AadApplicationAndServicePrincipal servicePrincipal;
      try
      {
        if (requestContext.IsFeatureEnabled("ServiceEnpoints.Service.UseMicrosoftGraph"))
        {
          graphAccessToken = AzureServicePrincipalHelper.GetMicrosoftGraphAccessToken(requestContext, tenantId, vstsToken);
          servicePrincipal = AzureServicePrincipalHelper.CreateAadAppWithMicrosoftGraph(requestContext, graphAccessToken, endpoint, projectName);
        }
        else
        {
          graphAccessToken = AzureServicePrincipalHelper.GetGraphAccessToken(requestContext, tenantId, vstsToken);
          servicePrincipal = AzureServicePrincipalHelper.CreateAadApp(requestContext, graphAccessToken, endpoint, projectName);
        }
        requestContext.TraceInfo(34000807, "EndpointService", "AadApplication created for tenant {0} and endpoint {1}", (object) tenantId, (object) endpoint.Id);
      }
      catch (Exception ex) when (
      {
        // ISSUE: unable to correctly present filter
        int num;
        switch (ex)
        {
          case AuthorizationException _:
          case MicrosoftGraphRequestDeniedException _:
            num = 1;
            break;
          default:
            num = ex is MicrosoftGraphAuthenticationException ? 1 : 0;
            break;
        }
        if ((uint) num > 0U)
        {
          SuccessfulFiltering;
        }
        else
          throw;
      }
      )
      {
        throw new ApplicationException(ServiceEndpointResources.CouldNotCreateAADapp((object) ex.Message) + ServiceEndpointResources.CouldNotCreateAADappHelpMessage(), ex);
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ServiceEndpointResources.CouldNotCreateAADapp((object) ex.Message), ex);
      }
      endpoint.Authorization.Parameters["tenantid"] = tenantId;
      endpoint.Authorization.Parameters["serviceprincipalid"] = servicePrincipal.SpnClientId;
      endpoint.Data["appObjectId"] = servicePrincipal.AppObjectId;
      endpoint.Data["spnObjectId"] = servicePrincipal.SpnObjectId;
      if (!string.IsNullOrEmpty(servicePrincipal.SpnKey))
        endpoint.Authorization.Parameters["servicePrincipalKey"] = servicePrincipal.SpnKey;
      AzureServicePrincipalHelper.FillAzureRoleAssignmentPermission(endpoint);
      AzureServicePrincipalHelper.AssignServicePrincipalPermissions(requestContext, armAccessToken, graphAccessToken, endpoint);
      requestContext.TraceLeave(0, "EndpointService", nameof (CreateAadApplicationAndAssignServicePrincipalPermissions));
    }

    private static void FillAzureRoleAssignmentPermission(ServiceEndpoint endpoint)
    {
      List<AzurePermission> toSerialize = (List<AzurePermission>) null;
      string toDeserialize;
      if (endpoint.Data.TryGetValue("azureSpnPermissions", out toDeserialize))
        toSerialize = JsonUtility.FromString<List<AzurePermission>>(toDeserialize);
      if (toSerialize == null)
        toSerialize = new List<AzurePermission>();
      List<AzurePermission> azurePermissionList = toSerialize;
      AzureRoleAssignmentPermission assignmentPermission = new AzureRoleAssignmentPermission();
      assignmentPermission.RoleAssignmentId = Guid.NewGuid();
      assignmentPermission.Provisioned = false;
      azurePermissionList.Add((AzurePermission) assignmentPermission);
      endpoint.Data["azureSpnPermissions"] = JsonUtility.ToString<AzurePermission>((IList<AzurePermission>) toSerialize);
    }

    internal static void UnassignServicePrincipalPermissionsAndDeleteAadApplication(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string vstsToken = null)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (UnassignServicePrincipalPermissionsAndDeleteAadApplication));
      string tenantId;
      endpoint.Authorization.Parameters.TryGetValue("tenantid", out tenantId);
      string str1;
      endpoint.Authorization.Parameters.TryGetValue("serviceprincipalid", out str1);
      string spnObjectId;
      endpoint.Data.TryGetValue("spnObjectId", out spnObjectId);
      string appObjectId;
      endpoint.Data.TryGetValue("appObjectId", out appObjectId);
      if (string.IsNullOrEmpty(tenantId))
      {
        requestContext.TraceWarning(34000808, "EndpointService", "Tenant id is null or empty for endpoint {0}", (object) endpoint.Id);
      }
      else
      {
        AzureServicePrincipalHelper.InitArmAndGraphDefaults(requestContext, endpoint);
        string armAccessToken = AzureServicePrincipalHelper.GetArmAccessToken(requestContext, tenantId, vstsToken);
        bool flag = requestContext.IsFeatureEnabled("ServiceEnpoints.Service.UseMicrosoftGraph");
        string str2 = flag ? AzureServicePrincipalHelper.GetMicrosoftGraphAccessToken(requestContext, tenantId, vstsToken) : AzureServicePrincipalHelper.GetGraphAccessToken(requestContext, tenantId, vstsToken);
        if (!string.IsNullOrEmpty(str1))
          AzureServicePrincipalHelper.UnassignServicePrincipalPermissions(requestContext, armAccessToken, str2, endpoint);
        if (!string.IsNullOrEmpty(spnObjectId) && !string.IsNullOrEmpty(appObjectId))
        {
          if (flag)
            AzureServicePrincipalHelper.DeleteAadAppWithMicrosoftGraph(requestContext, str2, spnObjectId, appObjectId);
          else
            AzureServicePrincipalHelper.DeleteAadApp(requestContext, str2, spnObjectId, appObjectId);
          requestContext.TraceInfo(0, "EndpointService", "AadApplication deleted for tenant {0} and endpoint {1}", (object) tenantId, (object) endpoint.Id);
        }
        requestContext.TraceLeave(0, "EndpointService", nameof (UnassignServicePrincipalPermissionsAndDeleteAadApplication));
      }
    }

    internal static void UpdateAadApplicationCredsAndAssignAzurePermissions(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string accessToken = null,
      string endpointCurrentScope = null)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (UpdateAadApplicationCredsAndAssignAzurePermissions));
      AzureServicePrincipalHelper.AssignAzurePermissionsForArmEndpoint(requestContext, endpoint, accessToken, endpointCurrentScope);
      AzureServicePrincipalHelper.UpdateAadApplicationCredsForArmEndpointUpdate(requestContext, endpoint, accessToken);
      requestContext.TraceLeave(0, "EndpointService", nameof (UpdateAadApplicationCredsAndAssignAzurePermissions));
    }

    internal static void AssignAzurePermissionsForArmEndpoint(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string accessToken = null,
      string endpointCurrentScope = null)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (AssignAzurePermissionsForArmEndpoint));
      AzureServicePrincipalHelper.InitArmAndGraphDefaults(requestContext, endpoint);
      string parameter = endpoint.Authorization.Parameters["tenantid"];
      string graphAccessToken = requestContext.IsFeatureEnabled("ServiceEnpoints.Service.UseMicrosoftGraph") ? AzureServicePrincipalHelper.GetMicrosoftGraphAccessToken(requestContext, parameter, accessToken) : AzureServicePrincipalHelper.GetGraphAccessToken(requestContext, parameter, accessToken);
      string armAccessToken = AzureServicePrincipalHelper.GetArmAccessToken(requestContext, parameter, accessToken);
      if (AzureServicePrincipalHelper.IsServicePrincipalScopingChanged(endpoint, endpointCurrentScope))
      {
        AzureServicePrincipalHelper.UnassignServicePrincipalPermissions(requestContext, armAccessToken, graphAccessToken, endpoint, endpointCurrentScope);
        AzureServicePrincipalHelper.FillAzureRoleAssignmentPermission(endpoint);
      }
      AzureServicePrincipalHelper.AssignServicePrincipalPermissions(requestContext, armAccessToken, graphAccessToken, endpoint);
      requestContext.TraceLeave(0, "EndpointService", nameof (AssignAzurePermissionsForArmEndpoint));
    }

    internal static void UpdateAadApplicationCredsForArmEndpointUpdate(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string accessToken = null)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (UpdateAadApplicationCredsForArmEndpointUpdate));
      AzureServicePrincipalHelper.InitArmAndGraphDefaults(requestContext, endpoint);
      string parameter = endpoint.Authorization.Parameters["tenantid"];
      if (requestContext.IsFeatureEnabled("ServiceEnpoints.Service.UseMicrosoftGraph"))
      {
        string graphAccessToken = AzureServicePrincipalHelper.GetMicrosoftGraphAccessToken(requestContext, parameter, accessToken);
        AzureServicePrincipalHelper.UpdateAadApplicationCredsWithMicrosoftGraph(requestContext, endpoint, graphAccessToken);
      }
      else
      {
        string graphAccessToken = AzureServicePrincipalHelper.GetGraphAccessToken(requestContext, parameter, accessToken);
        AzureServicePrincipalHelper.UpdateAadApplicationCreds(requestContext, endpoint, graphAccessToken);
      }
      requestContext.TraceLeave(0, "EndpointService", nameof (UpdateAadApplicationCredsForArmEndpointUpdate));
    }

    internal static void UpdateAadApplicationCreds(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string graphAccessToken)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (UpdateAadApplicationCreds));
      string parameter = endpoint.Authorization.Parameters["tenantid"];
      string objectId = endpoint.Data["appObjectId"];
      string tenantId = AzureServicePrincipalHelper.ValidateAndGetTenantId(requestContext, parameter);
      GraphConnection graphConnection = AzureServicePrincipalHelper.GetGraphConnection(requestContext, graphAccessToken);
      Application entity = graphConnection.Get<Application>(objectId);
      PasswordCredential passwordCredential = new PasswordCredential()
      {
        StartDate = new DateTime?(DateTime.UtcNow),
        EndDate = new DateTime?(DateTime.UtcNow.AddYears(2)),
        KeyId = new Guid?(Guid.NewGuid()),
        Value = AzureServicePrincipalHelper.GenerateRandomPassword(requestContext)
      };
      entity.PasswordCredentials.Clear();
      entity.PasswordCredentials.Add(passwordCredential);
      requestContext.TraceInfo(34000809, "EndpointService", "Updating password for AadApplication for tenant {0} and endpoint {1}", (object) tenantId, (object) endpoint.Id);
      graphConnection.Update<Application>(entity);
      requestContext.TraceInfo(0, "EndpointService", "AadApplication password for tenant {0} and endpoint {1} is updated.", (object) tenantId, (object) endpoint.Id);
      endpoint.Authorization.Parameters["servicePrincipalKey"] = passwordCredential.Value;
      requestContext.TraceLeave(0, "EndpointService", nameof (UpdateAadApplicationCreds));
    }

    internal static void UpdateAadApplicationCredsWithMicrosoftGraph(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string msGraphAccessToken)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (UpdateAadApplicationCredsWithMicrosoftGraph));
      string tenantId = AzureServicePrincipalHelper.ValidateAndGetTenantId(requestContext, endpoint.Authorization.Parameters["tenantid"]);
      string input = endpoint.Data["appObjectId"];
      AadService service = requestContext.GetService<AadService>();
      Guid appObjectId = Guid.Parse(input);
      GetApplicationByIdRequest applicationByIdRequest1 = new GetApplicationByIdRequest();
      applicationByIdRequest1.AccessToken = msGraphAccessToken;
      applicationByIdRequest1.AppObjectId = appObjectId;
      GetApplicationByIdRequest applicationByIdRequest2 = applicationByIdRequest1;
      applicationByIdRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
      GetApplicationByIdResponse applicationById = service.GetApplicationById(requestContext, applicationByIdRequest2);
      if (endpoint.HasAuthorizationScheme("WorkloadIdentityFederation"))
      {
        RemoveApplicationFederatedCredentialsRequest credentialsRequest1 = AzureServicePrincipalHelper.CreateRemoveFederatedCredentialsRequest(requestContext, endpoint, msGraphAccessToken, appObjectId);
        service.RemoveApplicationFederatedCredentials(requestContext, credentialsRequest1);
        AddApplicationFederatedCredentialsRequest credentialsRequest2 = AzureServicePrincipalHelper.CreateAddFederatedCredentialsRequest(requestContext, endpoint, msGraphAccessToken, appObjectId);
        service.AddApplicationFederatedCredentials(requestContext, credentialsRequest2);
      }
      else
      {
        IList<AadPasswordCredential> passwordCredentials = applicationById.Application.PasswordCredentials;
        IEnumerable<Guid> guids = passwordCredentials != null ? passwordCredentials.Where<AadPasswordCredential>((Func<AadPasswordCredential, bool>) (x => x.KeyId.HasValue)).Select<AadPasswordCredential, Guid>((Func<AadPasswordCredential, Guid>) (x => x.KeyId.Value)) : (IEnumerable<Guid>) null;
        requestContext.TraceInfo(34000857, "EndpointService", "Adding password for AadApplication for tenant {0} and endpoint {1}", (object) tenantId, (object) endpoint.Id);
        AddApplicationPasswordRequest applicationPasswordRequest1 = new AddApplicationPasswordRequest();
        applicationPasswordRequest1.AccessToken = msGraphAccessToken;
        applicationPasswordRequest1.AppObjectId = Guid.Parse(input);
        applicationPasswordRequest1.SecretExpirationDate = SecretsExpirationHelper.GetSecretEndDate(requestContext);
        AddApplicationPasswordRequest applicationPasswordRequest2 = applicationPasswordRequest1;
        applicationPasswordRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
        AddApplicationPasswordResponse passwordResponse = service.AddApplicationPassword(requestContext, applicationPasswordRequest2);
        endpoint.Authorization.Parameters["servicePrincipalKey"] = passwordResponse.PasswordCredentialCreated.Value;
        requestContext.TraceInfo(34000858, "EndpointService", "Removing password for AadApplication for tenant {0} and endpoint {1}", (object) tenantId, (object) endpoint.Id);
        foreach (Guid guid in guids)
        {
          RemoveApplicationPasswordRequest applicationPasswordRequest3 = new RemoveApplicationPasswordRequest();
          applicationPasswordRequest3.AccessToken = msGraphAccessToken;
          applicationPasswordRequest3.AppObjectId = Guid.Parse(input);
          applicationPasswordRequest3.PasswordKeyId = guid;
          RemoveApplicationPasswordRequest applicationPasswordRequest4 = applicationPasswordRequest3;
          applicationPasswordRequest4.SetGraphUriOverrideOnDevFabric(requestContext);
          service.RemoveApplicationPassword(requestContext, applicationPasswordRequest4);
        }
      }
      requestContext.TraceInfo(0, "EndpointService", "AadApplication password for tenant {0} and endpoint {1} is updated.", (object) tenantId, (object) endpoint.Id);
      requestContext.TraceLeave(0, "EndpointService", nameof (UpdateAadApplicationCredsWithMicrosoftGraph));
    }

    private static AddApplicationFederatedCredentialsRequest CreateAddFederatedCredentialsRequest(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string msGraphAccessToken,
      Guid appObjectId)
    {
      requestContext.TraceInfo(34000861, "EndpointService", string.Format("Creating the federation entry for application objectId '{0:D}", (object) appObjectId));
      OidcFederationClaims federationClaims = OidcFederationClaims.CreateOidcFederationClaims(requestContext, endpoint);
      string name = endpoint.ServiceEndpointProjectReferences.FirstOrDefault<ServiceEndpointProjectReference>().ProjectReference.Name;
      ILocationService service = requestContext.GetService<ILocationService>();
      string accessMappingMoniker1 = AccessMappingConstants.PublicAccessMappingMoniker;
      IVssRequestContext requestContext1 = requestContext;
      Guid referenceIdentifier = Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier;
      string accessMappingMoniker2 = accessMappingMoniker1;
      string str = service.GetLocationServiceUrl(requestContext1, referenceIdentifier, accessMappingMoniker2);
      if (string.IsNullOrEmpty(str))
        str = "https://dev.azure.com/" + requestContext.ServiceHost.Name + "/";
      if (!str.EndsWith("/"))
        str += "/";
      AddApplicationFederatedCredentialsRequest credentialsRequest = new AddApplicationFederatedCredentialsRequest();
      credentialsRequest.AccessToken = msGraphAccessToken;
      credentialsRequest.FederationName = endpoint.Id.ToString("D");
      credentialsRequest.FederationDescription = string.Format("Federation for Service Connection {0} in {1}{2}/_settings/adminservices?resourceId={3}", (object) endpoint.Name, (object) str, (object) name, (object) endpoint.Id);
      credentialsRequest.AppObjectId = appObjectId;
      credentialsRequest.FederationSubject = federationClaims.Subject;
      credentialsRequest.FederationAudience = federationClaims.Audience;
      credentialsRequest.FederationIssuer = federationClaims.Issuer;
      AddApplicationFederatedCredentialsRequest aadServiceRequest = credentialsRequest;
      aadServiceRequest.SetGraphUriOverrideOnDevFabric(requestContext);
      return aadServiceRequest;
    }

    private static RemoveApplicationFederatedCredentialsRequest CreateRemoveFederatedCredentialsRequest(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string msGraphAccessToken,
      Guid appObjectId)
    {
      requestContext.TraceInfo(34000862, "EndpointService", string.Format("Removing the federation entry for application objectId '{0:D}", (object) appObjectId));
      RemoveApplicationFederatedCredentialsRequest aadServiceRequest = new RemoveApplicationFederatedCredentialsRequest();
      aadServiceRequest.AccessToken = msGraphAccessToken;
      aadServiceRequest.FederationName = endpoint.Id.ToString("D");
      aadServiceRequest.AppObjectId = appObjectId;
      aadServiceRequest.SetGraphUriOverrideOnDevFabric(requestContext);
      return aadServiceRequest;
    }

    internal static Uri GetBaseUri(
      IVssRequestContext requestContext,
      string registryPath,
      string defaultValue)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (GetBaseUri));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string uriString = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) registryPath, defaultValue);
      if (string.IsNullOrEmpty(uriString) && requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        uriString = defaultValue;
      requestContext.TraceLeave(0, "EndpointService", nameof (GetBaseUri));
      return new Uri(uriString);
    }

    internal static string HandleHttpResponse(
      IVssRequestContext requestContext,
      HttpResponseMessage response,
      string errorMessage,
      string helpMessage = null,
      ServiceEndpoint endpoint = null)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (HandleHttpResponse));
      string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
      if (!response.IsSuccessStatusCode)
      {
        requestContext.TraceError(34000810, "EndpointService", "Error: {0}", (object) result);
        AzureServicePrincipalHelper.ArmErrorContent armErrorContent = JsonConvert.DeserializeObject<AzureServicePrincipalHelper.ArmErrorContent>(result);
        if (endpoint != null)
          requestContext.TraceError(34000810, "EndpointService", "Endpoint ID: {0}, ErrorCode: {1}", (object) endpoint.Id, (object) armErrorContent.Error.ErrorCode);
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, " {0}: error code: {1}, inner error code: {2}, inner error message {3}", (object) errorMessage, (object) response.StatusCode.ToString(), (object) armErrorContent.Error.ErrorCode, (object) armErrorContent.Error.ErrorMessage);
        if (!string.IsNullOrEmpty(helpMessage))
          message = message + " " + helpMessage;
        ApplicationException applicationException = new ApplicationException(message);
        if (response.StatusCode == HttpStatusCode.Forbidden && armErrorContent.Error?.ErrorCode == "AuthorizationFailed" || response.StatusCode == HttpStatusCode.Conflict && armErrorContent.Error?.ErrorCode == "ReadOnlyDisabledSubscription")
          applicationException.Data.Add((object) "NonRetriable", (object) true);
        if (response.StatusCode == HttpStatusCode.NotFound && armErrorContent.Error?.ErrorCode == "ParentResourceNotFound")
          applicationException.Data.Add((object) "CanIgnore", (object) true);
        throw applicationException;
      }
      requestContext.TraceLeave(0, "EndpointService", nameof (HandleHttpResponse));
      return result;
    }

    internal static HttpResponseMessage ExecuteHttpRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage request)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (ExecuteHttpRequest));
      List<DelegatingHandler> delegatingHandlers = ClientProviderHelper.GetMinimalDelegatingHandlers(requestContext, typeof (AzureResourceManagerService), ClientProviderHelper.Options.CreateDefault(requestContext), "AzureSP");
      using (HttpMessageHandler handler = (HttpMessageHandler) AzureServicePrincipalHelper.HttpClientHandler ?? HttpClientFactory.CreatePipeline((HttpMessageHandler) new HttpClientHandler(), (IEnumerable<DelegatingHandler>) delegatingHandlers))
      {
        using (HttpClient httpClient = new HttpClient(handler))
        {
          httpClient.Timeout = new TimeSpan(0, 0, 60);
          HttpResponseMessage result = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
          requestContext.TraceLeave(0, "EndpointService", nameof (ExecuteHttpRequest));
          return result;
        }
      }
    }

    internal static string GetArmAccessToken(
      IVssRequestContext requestContext,
      string tenantId,
      string vstsToken = null)
    {
      return MsalAzureAccessTokenHelper.GetArmAccessToken(requestContext, tenantId, vstsToken);
    }

    internal static string GetGraphAccessToken(
      IVssRequestContext requestContext,
      string tenantId,
      string vstsToken = null)
    {
      return MsalAzureAccessTokenHelper.GetResourceAccessToken(requestContext, tenantId, AzureServicePrincipalHelper.ServiceEndpointGraphUri, vstsToken);
    }

    internal static string GetMicrosoftGraphAccessToken(
      IVssRequestContext requestContext,
      string tenantId,
      string vstsToken = null)
    {
      return MsalAzureAccessTokenHelper.GetResourceAccessToken(requestContext, tenantId, AzureServicePrincipalHelper.ServiceEndpointMicrosoftGraphUri, vstsToken);
    }

    private static string ValidateAndGetTenantId(IVssRequestContext requestContext, string tenantId)
    {
      if (string.IsNullOrEmpty(tenantId))
      {
        requestContext.TraceInfo(0, "EndpointService", "TenantId is null or empty. Retrieving it from the request context");
        tenantId = AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext).ToString();
      }
      return tenantId;
    }

    private static AzureServicePrincipalHelper.AadApplicationAndServicePrincipal CreateAadApp(
      IVssRequestContext requestContext,
      string graphAccessToken,
      ServiceEndpoint endpoint,
      string projectName)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (CreateAadApp));
      int spnLengthAllowed = 92;
      GraphConnection graphConnection = AzureServicePrincipalHelper.GetGraphConnection(requestContext, graphAccessToken);
      Guid.NewGuid();
      Application appObject = new Application()
      {
        DisplayName = AzureServicePrincipalHelper.GetAutomaticSpnName(requestContext, endpoint, projectName, spnLengthAllowed)
      };
      appObject.ReplyUrls.Add("https://VisualStudio/SPN");
      appObject.Homepage = "https://VisualStudio/SPN";
      PasswordCredential passwordCredential = new PasswordCredential()
      {
        StartDate = new DateTime?(DateTime.UtcNow),
        EndDate = new DateTime?(SecretsExpirationHelper.GetSecretEndDate(requestContext)),
        KeyId = new Guid?(Guid.NewGuid()),
        Value = AzureServicePrincipalHelper.GenerateRandomPassword(requestContext)
      };
      appObject.PasswordCredentials.Add(passwordCredential);
      requestContext.TraceInfo(34000812, "EndpointService", "Creating the application using the app id '{0}', home page {1} and display name {2}.", (object) appObject.AppId, (object) appObject.Homepage, (object) appObject.DisplayName);
      AzureServicePrincipalHelper.InvokeActionWithRetry(5, (Action) (() =>
      {
        requestContext.TraceInfo(0, "EndpointService", "Creating new AAD application with display name '" + appObject.DisplayName + "'");
        appObject = graphConnection.Add<Application>(appObject);
      }), (Action<Exception>) (ex =>
      {
        if (ex is AuthorizationException)
          throw ex;
      }));
      requestContext.TraceInfo(34000813, "EndpointService", "Creating Service principal using appid {0}", (object) appObject.AppId);
      Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal servicePrincipal = new Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal()
      {
        DisplayName = appObject.DisplayName,
        AccountEnabled = new bool?(true),
        AppId = appObject.AppId
      };
      AzureServicePrincipalHelper.InvokeActionWithRetry(20, (Action) (() =>
      {
        requestContext.TraceInfo(0, "EndpointService", "Creating new AAD service principal with display name '" + servicePrincipal.DisplayName + "' for app id '" + servicePrincipal.AppId + "'");
        servicePrincipal = graphConnection.Add<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>(servicePrincipal);
      }), (Action<Exception>) (ex =>
      {
        if (ex is AuthorizationException)
          throw ex;
      }));
      requestContext.TraceLeave(0, "EndpointService", nameof (CreateAadApp));
      return new AzureServicePrincipalHelper.AadApplicationAndServicePrincipal()
      {
        SpnClientId = appObject.AppId,
        SpnKey = passwordCredential.Value,
        SpnObjectId = servicePrincipal.ObjectId,
        AppObjectId = appObject.ObjectId
      };
    }

    private static AzureServicePrincipalHelper.AadApplicationAndServicePrincipal CreateAadAppWithMicrosoftGraph(
      IVssRequestContext requestContext,
      string graphAccessToken,
      ServiceEndpoint endpoint,
      string projectName)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (CreateAadAppWithMicrosoftGraph));
      AadService service = requestContext.GetService<AadService>();
      AadApplication aadApplication = new AadApplication.Factory()
      {
        DisplayName = AzureServicePrincipalHelper.GetAutomaticSpnName(requestContext, endpoint, projectName, 92),
        ReplyUrls = ((IList<string>) new List<string>(1)
        {
          "https://VisualStudio/SPN"
        }),
        HomePage = "https://VisualStudio/SPN"
      }.Create();
      CreateApplicationRequest applicationRequest1 = new CreateApplicationRequest();
      applicationRequest1.AccessToken = graphAccessToken;
      applicationRequest1.ApplicationToCreate = aadApplication;
      CreateApplicationRequest applicationRequest2 = applicationRequest1;
      applicationRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
      requestContext.TraceInfo(34000812, "EndpointService", "Creating the application using home page " + aadApplication.HomePage + " and display name " + aadApplication.DisplayName + ".");
      AadApplication applicationCreated = service.CreateApplication(requestContext, applicationRequest2).ApplicationCreated;
      string str = (string) null;
      if (endpoint.HasAuthorizationScheme("WorkloadIdentityFederation"))
      {
        AddApplicationFederatedCredentialsRequest credentialsRequest = AzureServicePrincipalHelper.CreateAddFederatedCredentialsRequest(requestContext, endpoint, graphAccessToken, applicationCreated.ObjectId);
        service.AddApplicationFederatedCredentials(requestContext, credentialsRequest);
      }
      else
      {
        AddApplicationPasswordRequest applicationPasswordRequest1 = new AddApplicationPasswordRequest();
        applicationPasswordRequest1.AccessToken = graphAccessToken;
        applicationPasswordRequest1.AppObjectId = applicationCreated.ObjectId;
        applicationPasswordRequest1.SecretExpirationDate = SecretsExpirationHelper.GetSecretEndDate(requestContext);
        AddApplicationPasswordRequest applicationPasswordRequest2 = applicationPasswordRequest1;
        applicationPasswordRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
        requestContext.TraceInfo(34000857, "EndpointService", string.Format("Creating the application password with application objectId '{0}", (object) applicationCreated.ObjectId));
        str = service.AddApplicationPassword(requestContext, applicationPasswordRequest2).PasswordCredentialCreated.Value;
      }
      AadServicePrincipal servicePrincipal = new AadServicePrincipal.Factory()
      {
        DisplayName = applicationCreated.DisplayName,
        AccountEnabled = true,
        AppId = applicationCreated.AppId
      }.Create();
      CreateServicePrincipalRequest principalRequest1 = new CreateServicePrincipalRequest();
      principalRequest1.AccessToken = graphAccessToken;
      principalRequest1.ServicePrincipalToCreate = servicePrincipal;
      CreateServicePrincipalRequest principalRequest2 = principalRequest1;
      principalRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
      requestContext.TraceInfo(34000813, "EndpointService", string.Format("Creating new AAD service principal with display name '{0}' for app id '{1}'", (object) servicePrincipal.DisplayName, (object) servicePrincipal.AppId));
      AadServicePrincipal principalCreated = service.CreateServicePrincipal(requestContext, principalRequest2).ServicePrincipalCreated;
      requestContext.TraceLeave(0, "EndpointService", nameof (CreateAadAppWithMicrosoftGraph));
      return new AzureServicePrincipalHelper.AadApplicationAndServicePrincipal()
      {
        SpnClientId = principalCreated.AppId.ToString(),
        SpnKey = str,
        SpnObjectId = principalCreated.ObjectId.ToString(),
        AppObjectId = applicationCreated.ObjectId.ToString()
      };
    }

    private static void SetGraphUriOverrideOnDevFabric(
      this AadServiceRequest aadServiceRequest,
      IVssRequestContext requestContext)
    {
      if (!AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
        return;
      aadServiceRequest.MicrosoftGraphBaseUrlOverride = AzureServicePrincipalHelper.ProdMicrosoftGraphUrl.Value;
    }

    private static string GetServicePrincipalId(
      IVssRequestContext requestContext,
      string graphAccessToken,
      ServiceEndpoint endpoint)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (GetServicePrincipalId));
      GraphConnection graphConnection = AzureServicePrincipalHelper.GetGraphConnection(requestContext, graphAccessToken);
      string spnClientId = endpoint.Authorization.Parameters["serviceprincipalid"];
      Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal servicePrincipal = (Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal) null;
      AzureServicePrincipalHelper.InvokeActionWithRetry(5, (Action) (() =>
      {
        requestContext.TraceInfo(34000814, "EndpointService", "Getting the application using the spnClient id '{0}'", (object) spnClientId);
        servicePrincipal = graphConnection.GetServicePrincipalsByAppIds((IList<string>) new List<string>()
        {
          spnClientId
        }).FirstOrDefault<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>();
      }), (Action<Exception>) (ex =>
      {
        switch (ex)
        {
          case ObjectNotFoundException _:
            throw new AzureKeyVaultManualSpnObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServiceEndpointResources.AzureKeyVaultSpnObjectNotFoundException((object) "AzureKeyVaultManualSpnObjectNotFoundException"), (object) ex));
          case AuthorizationException _:
            throw ex;
        }
      }));
      if (servicePrincipal == null)
        throw new ApplicationException(ServiceEndpointResources.CouldNotFindServicePrincipal((object) spnClientId));
      requestContext.TraceLeave(0, "EndpointService", nameof (GetServicePrincipalId));
      return servicePrincipal.ObjectId;
    }

    private static string GetServicePrincipalIdWithMicrosoftGraph(
      IVssRequestContext requestContext,
      string msGraphAccessToken,
      ServiceEndpoint endpoint)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (GetServicePrincipalIdWithMicrosoftGraph));
      string parameter = endpoint.Authorization.Parameters["serviceprincipalid"];
      AadServicePrincipal servicePrincipal = (AadServicePrincipal) null;
      AadService service = requestContext.GetService<AadService>();
      try
      {
        GetServicePrincipalsRequest principalsRequest1 = new GetServicePrincipalsRequest();
        principalsRequest1.AccessToken = msGraphAccessToken;
        principalsRequest1.AppIds = (IEnumerable<string>) new List<string>(1)
        {
          parameter
        };
        GetServicePrincipalsRequest principalsRequest2 = principalsRequest1;
        principalsRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
        servicePrincipal = service.GetServicePrincipals(requestContext, principalsRequest2).ServicePrincipals.FirstOrDefault<AadServicePrincipal>();
      }
      catch (ObjectNotFoundException ex)
      {
        throw new AzureKeyVaultManualSpnObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServiceEndpointResources.AzureKeyVaultSpnObjectNotFoundException((object) "AzureKeyVaultManualSpnObjectNotFoundException"), (object) ex));
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34000860, "EndpointService", string.Format("Unable to get service principal for AppId '{0}', error : {1}", (object) parameter, (object) ex));
      }
      if (servicePrincipal == null)
        throw new ApplicationException(ServiceEndpointResources.CouldNotFindServicePrincipal((object) parameter));
      requestContext.TraceLeave(0, "EndpointService", nameof (GetServicePrincipalIdWithMicrosoftGraph));
      return servicePrincipal.ObjectId.ToString();
    }

    private static string GetAutomaticSpnName(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string projectName,
      int spnLengthAllowed)
    {
      if (projectName == null)
        projectName = "";
      string str = requestContext.ServiceHost.Name;
      string empty = string.Empty;
      if (AzureServicePrincipalHelper.IsEndpointScopedToSubscription(endpoint))
      {
        if (!endpoint.Data.TryGetValue("subscriptionId", out empty))
          empty = string.Empty;
      }
      else if (AzureServicePrincipalHelper.IsEndpointScopedToWorkspace(endpoint))
      {
        if (!endpoint.Data.TryGetValue("mlWorkspaceName", out empty))
          empty = string.Empty;
      }
      else if (!endpoint.Data.TryGetValue("managementGroupId", out empty))
        empty = string.Empty;
      string automaticSpnName = str + "-" + projectName + "-" + empty;
      if (automaticSpnName.Length <= spnLengthAllowed)
        return automaticSpnName;
      spnLengthAllowed = spnLengthAllowed - empty.Length - 2;
      if (str.Length > spnLengthAllowed / 2 && projectName.Length > spnLengthAllowed / 2)
      {
        str = str.Substring(0, spnLengthAllowed / 2);
        projectName = projectName.Substring(0, spnLengthAllowed - str.Length);
      }
      else if (str.Length > spnLengthAllowed / 2 && str.Length + projectName.Length > spnLengthAllowed)
        str = str.Substring(0, spnLengthAllowed - projectName.Length);
      else if (projectName.Length > spnLengthAllowed / 2 && str.Length + projectName.Length > spnLengthAllowed)
        projectName = projectName.Substring(0, spnLengthAllowed - str.Length);
      return str + "-" + projectName + "-" + empty;
    }

    private static void DeleteAadApp(
      IVssRequestContext requestContext,
      string graphAccessToken,
      string spnObjectId,
      string appObjectId)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (DeleteAadApp));
      GraphConnection graphConnection = AzureServicePrincipalHelper.GetGraphConnection(requestContext, graphAccessToken);
      Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal servicePrincipal = (Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal) null;
      try
      {
        servicePrincipal = graphConnection.Get<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>(spnObjectId);
      }
      catch (Exception ex)
      {
        if (!(ex is ObjectNotFoundException) && (!(ex.GetType() == typeof (AggregateException)) || !(ex.InnerException is ObjectNotFoundException)))
        {
          throw;
        }
        else
        {
          string str = ex.GetType() == typeof (AggregateException) ? ex.InnerException?.Message ?? ex.Message : ex.ToString();
          requestContext.TraceError(34000815, "EndpointService", "Unable to fetch spnObject for spnObjectId {0}, error : {1}", (object) spnObjectId, (object) str);
        }
      }
      Application application = (Application) null;
      try
      {
        application = graphConnection.Get<Application>(appObjectId);
      }
      catch (Exception ex)
      {
        if (!(ex is ObjectNotFoundException) && (!(ex.GetType() == typeof (AggregateException)) || !(ex.InnerException is ObjectNotFoundException)))
        {
          throw;
        }
        else
        {
          string str = ex.GetType() == typeof (AggregateException) ? ex.InnerException?.Message ?? ex.Message : ex.ToString();
          requestContext.TraceError(34000815, "EndpointService", "Unable to fetch appObject for appObjectId {0}, error : {1}", (object) appObjectId, (object) str);
        }
      }
      if (servicePrincipal != null)
        graphConnection.Delete((GraphObject) servicePrincipal);
      if (application != null)
        graphConnection.Delete((GraphObject) application);
      requestContext.TraceLeave(0, "EndpointService", nameof (DeleteAadApp));
    }

    private static void DeleteAadAppWithMicrosoftGraph(
      IVssRequestContext requestContext,
      string msGraphAccessToken,
      string spnObjectId,
      string appObjectId)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (DeleteAadAppWithMicrosoftGraph));
      AadService service = requestContext.GetService<AadService>();
      try
      {
        DeleteServicePrincipalRequest principalRequest1 = new DeleteServicePrincipalRequest();
        principalRequest1.AccessToken = msGraphAccessToken;
        principalRequest1.ServicePrincipalObjectId = Guid.Parse(spnObjectId);
        DeleteServicePrincipalRequest principalRequest2 = principalRequest1;
        principalRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
        service.DeleteServicePrincipal(requestContext, principalRequest2);
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34000815, "EndpointService", string.Format("Unable to delete AAD service principal for ObjectId '{0}', error : '{1}', continue.", (object) spnObjectId, (object) ex));
      }
      try
      {
        DeleteApplicationRequest applicationRequest1 = new DeleteApplicationRequest();
        applicationRequest1.AccessToken = msGraphAccessToken;
        applicationRequest1.AppObjectId = Guid.Parse(appObjectId);
        DeleteApplicationRequest applicationRequest2 = applicationRequest1;
        applicationRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
        service.DeleteApplication(requestContext, applicationRequest2);
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34000815, "EndpointService", string.Format("Unable to delete AAD application for ObjectId '{0}', error : '{1}', continue.", (object) appObjectId, (object) ex));
      }
      requestContext.TraceLeave(0, "EndpointService", nameof (DeleteAadAppWithMicrosoftGraph));
    }

    private static bool IsServicePrincipalScopingChanged(
      ServiceEndpoint endpoint,
      string endpointCurrentScope)
    {
      string scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifier(endpoint);
      return !string.Equals(endpointCurrentScope, scopeIdentifier, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsEndpointScopedToSubscription(ServiceEndpoint endpoint)
    {
      string a;
      endpoint.Data.TryGetValue("scopeLevel", out a);
      return a == null || string.Equals(a, "Subscription", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsEndpointScopedToManagementGroup(ServiceEndpoint endpoint)
    {
      string a;
      return endpoint.Data.TryGetValue("scopeLevel", out a) && string.Equals(a, "ManagementGroup", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsEndpointScopedToWorkspace(ServiceEndpoint endpoint)
    {
      string a;
      return endpoint.Data.TryGetValue("scopeLevel", out a) && string.Equals(a, "AzureMLWorkspace", StringComparison.OrdinalIgnoreCase);
    }

    public static bool TryGetReverseSchemeDeadlineData(
      this ServiceEndpoint endpoint,
      out DateTime revertSchemeDeadline)
    {
      revertSchemeDeadline = DateTime.MinValue;
      string s;
      return endpoint.Data.TryGetValue(nameof (revertSchemeDeadline), out s) && DateTime.TryParseExact(s, "o", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out revertSchemeDeadline);
    }

    public static string GetScopeIdentifier(ServiceEndpoint endpoint)
    {
      string scopeIdentifier;
      if (AzureServicePrincipalHelper.IsEndpointScopedToManagementGroup(endpoint))
      {
        string managementGroupId = endpoint.Data["managementGroupId"];
        scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifierForManagementGroup(endpoint, managementGroupId);
      }
      else if (AzureServicePrincipalHelper.IsEndpointScopedToWorkspace(endpoint))
      {
        string subscriptionId = endpoint.Data["subscriptionId"];
        string resourceGroupName = endpoint.Data["resourceGroupName"];
        string workspaceName = endpoint.Data["mlWorkspaceName"];
        scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifierForWorkspace(endpoint, subscriptionId, resourceGroupName, workspaceName);
      }
      else
      {
        string subscriptionId = endpoint.Data["subscriptionId"];
        scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifierForSubscription(endpoint, subscriptionId);
      }
      return scopeIdentifier;
    }

    public static void AssignServicePrincipalPermissions(
      IVssRequestContext requestContext,
      string armAccessToken,
      string graphAccessToken,
      ServiceEndpoint endpoint)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (AssignServicePrincipalPermissions));
      List<AzurePermission> azurePermissionList = AzureServicePrincipalHelper.DeserializeAzurePermissions(endpoint);
      if (azurePermissionList.IsNullOrEmpty<AzurePermission>())
      {
        requestContext.TraceInfo(34000816, "EndpointService", "No Azure spn permissions exists for endpoint {0} hence not setting any permissions", (object) endpoint.Id);
      }
      else
      {
        Uri baseUri = AzureServicePrincipalHelper.GetBaseUri(requestContext, "/Service/Commerce/AzureResourceManager/BaseUrl", AzureServicePrincipalHelper.ServiceEndpointArmUri);
        string tenantId = endpoint.Authorization.Parameters["tenantid"];
        string spnObjectId = AzureServicePrincipalHelper.GetSpnObjectId(requestContext, graphAccessToken, endpoint);
        foreach (AzurePermission azurePermission1 in azurePermissionList)
        {
          AzurePermission azurePermission = azurePermission1;
          try
          {
            AzureServicePrincipalHelper.InvokeActionWithRetry(60, (Action) (() => AzureServicePrincipalHelper.ProcessAzurePermission(requestContext, armAccessToken, endpoint, baseUri, tenantId, spnObjectId, azurePermission)), (Action<Exception>) (ex =>
            {
              if (ex.Data.Contains((object) "NonRetriable"))
                throw ex;
            }));
          }
          catch (Exception ex)
          {
            if (ex.Data.Contains((object) "CanIgnore"))
              requestContext.TraceException("EndpointService", ex);
            else
              throw;
          }
        }
        endpoint.Data["azureSpnPermissions"] = JsonUtility.ToString<AzurePermission>((IList<AzurePermission>) azurePermissionList);
        requestContext.TraceLeave(0, "EndpointService", nameof (AssignServicePrincipalPermissions));
      }
    }

    private static string GetAssignedPermissionsBySpnRelativeUri(
      ServiceEndpoint endpoint,
      string spnObjectId)
    {
      return string.Format("{0}/providers/Microsoft.Authorization/roleAssignments?api-version={1}&$filter=principalId eq '{2}'", (object) AzureServicePrincipalHelper.GetScopeIdentifier(endpoint).Trim('/'), (object) "2015-07-01", (object) spnObjectId);
    }

    private static bool CheckRoleAlreadyExists(
      IVssRequestContext requestContext,
      AzurePermission azurePermission,
      Uri baseUri,
      ServiceEndpoint endpoint,
      string spnObjectId,
      string armAccessToken)
    {
      try
      {
        if (azurePermission.ResourceProvider != "Microsoft.RoleAssignment")
          return false;
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUri, AzureServicePrincipalHelper.GetAssignedPermissionsBySpnRelativeUri(endpoint, spnObjectId)));
        request.Headers.Add("Authorization", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bearer {0}", (object) armAccessToken));
        HttpResponseMessage httpResponseMessage = AzureServicePrincipalHelper.ExecuteHttpRequest(requestContext, request);
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
          return false;
        JArray jarray = (JArray) JObject.Parse(httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult())["value"];
        string scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifier(endpoint);
        string str1 = AzureServicePrincipalHelper.GetAuthorizationParameter(endpoint, "Role") ?? AzureServicePrincipalHelper.GetRoleId(endpoint);
        foreach (JToken jtoken in jarray)
        {
          string a = (string) jtoken[(object) "properties"]?[(object) "scope"] ?? string.Empty;
          string str2 = (string) jtoken[(object) "properties"]?[(object) "roleDefinitionId"] ?? string.Empty;
          string existingRoleAssignmentId = (string) jtoken[(object) "name"] ?? string.Empty;
          if (string.Equals(a, scopeIdentifier, StringComparison.OrdinalIgnoreCase) && str2.Contains(str1))
            return AzureServicePrincipalHelper.UpdateEndpointData(endpoint, azurePermission, existingRoleAssignmentId);
        }
        return false;
      }
      catch (HttpRequestException ex)
      {
        requestContext.TraceError(34000839, "EndpointService", string.Format("HTTP request error for endpoint '{0}': {1}", (object) endpoint.Id, (object) ex.Message));
        return false;
      }
      catch (JsonReaderException ex)
      {
        requestContext.TraceError(34000839, "EndpointService", string.Format("JSON parsing error for endpoint '{0}': {1}", (object) endpoint.Id, (object) ex.Message));
        return false;
      }
    }

    private static bool UpdateEndpointData(
      ServiceEndpoint endpoint,
      AzurePermission azurePermission,
      string existingRoleAssignmentId)
    {
      if (!(azurePermission is AzureRoleAssignmentPermission assignmentPermission))
        return false;
      string oldValue = assignmentPermission.RoleAssignmentId.ToString();
      endpoint.Data["azureSpnRoleAssignmentId"] = endpoint.Data.ContainsKey("azureSpnRoleAssignmentId") ? endpoint.Data["azureSpnRoleAssignmentId"].Replace(oldValue, existingRoleAssignmentId) : existingRoleAssignmentId;
      if (endpoint.Data.ContainsKey("azureSpnPermissions"))
        endpoint.Data["azureSpnPermissions"] = endpoint.Data["azureSpnPermissions"].Replace(oldValue, existingRoleAssignmentId);
      return true;
    }

    private static void ProcessAzurePermission(
      IVssRequestContext requestContext,
      string armAccessToken,
      ServiceEndpoint endpoint,
      Uri baseUri,
      string tenantId,
      string spnObjectId,
      AzurePermission azurePermission)
    {
      string permissionDescription = azurePermission.GetPermissionDescription();
      requestContext.TraceInfo(0, "EndpointService", "Assigning azure permission '" + permissionDescription + "' for the service principal '" + spnObjectId + "'");
      if (requestContext.IsFeatureEnabled("ServiceEndpoints.EnableServiceEndpointsCheckRoleAlreadyExists") && AzureServicePrincipalHelper.CheckRoleAlreadyExists(requestContext, azurePermission, baseUri, endpoint, spnObjectId, armAccessToken))
      {
        azurePermission.Provisioned = true;
        requestContext.TraceInfo(0, "EndpointService", string.Format("Permission '{0}' was previously assigned for the service principal: '{1}', tenantId: '{2}', endpoint: '{3}'", (object) azurePermission.GetPermissionDescription(), (object) spnObjectId, (object) tenantId, (object) endpoint.Id));
      }
      else
      {
        Uri requestUri = new Uri(baseUri, azurePermission.GetAssignPermissionRelativeUri(endpoint));
        string permissionPayload = azurePermission.GetAssignPermissionPayload(endpoint, spnObjectId);
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, requestUri);
        request.Headers.Add("Authorization", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bearer {0}", (object) armAccessToken));
        request.Content = (HttpContent) new StringContent(permissionPayload, Encoding.UTF8, "application/json");
        HttpResponseMessage response = AzureServicePrincipalHelper.ExecuteHttpRequest(requestContext, request);
        string result;
        if (AzureServicePrincipalHelper.IsEndpointScopedToSubscription(endpoint))
        {
          string helpMessage = "";
          if (azurePermission.ResourceProvider == "Microsoft.RoleAssignment")
            helpMessage = ServiceEndpointResources.RoleAssignmentHelpMessageForSubscription();
          string str = endpoint.Data["subscriptionId"];
          string azurePermissionError = ServiceEndpointResources.FailedToSetAzurePermissionError((object) permissionDescription, (object) spnObjectId, (object) str);
          result = AzureServicePrincipalHelper.HandleHttpResponse(requestContext, response, azurePermissionError, helpMessage, endpoint);
        }
        else if (AzureServicePrincipalHelper.IsEndpointScopedToWorkspace(endpoint))
        {
          string str = endpoint.Data["mlWorkspaceName"];
          string errorOnMlWorkspace = ServiceEndpointResources.FailedToSetAzurePermissionErrorOnMLWorkspace((object) permissionDescription, (object) spnObjectId, (object) str);
          result = AzureServicePrincipalHelper.HandleHttpResponse(requestContext, response, errorOnMlWorkspace, ServiceEndpointResources.RoleAssignmentHelpMessageForMLWorkspace());
        }
        else
        {
          string str = endpoint.Data["managementGroupId"];
          string onManagementGroup = ServiceEndpointResources.FailedToSetAzurePermissionErrorOnManagementGroup((object) permissionDescription, (object) spnObjectId, (object) str);
          result = AzureServicePrincipalHelper.HandleHttpResponse(requestContext, response, onManagementGroup, ServiceEndpointResources.RoleAssignmentHelpMessageForManagementGroup());
        }
        azurePermission.OnAssignPermissionCompletion(endpoint, spnObjectId, result);
        requestContext.TraceInfo(0, "EndpointService", string.Format("Assigned azure permission '{0}' for the service principal: '{1}', tenantId: '{2}', endpoint: '{3}'", (object) azurePermission.GetPermissionDescription(), (object) spnObjectId, (object) tenantId, (object) endpoint.Id));
        azurePermission.Provisioned = true;
      }
    }

    public static string GetAzDevAccessToken(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      string vstsToken = (string) null;
      string errorMessage;
      if (endpoint.Authorization.Parameters.ContainsKey("AccessToken") && !VstsAccessTokenHelper.TryGetVstsAccessToken(requestContext, endpoint, out vstsToken, out string _, out string _, out string _, out errorMessage, false))
        throw new InvalidOperationException(errorMessage);
      return vstsToken;
    }

    public static void ValidateUserPermissions(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string endpointCurrentScope)
    {
      AzureServicePrincipalHelper.InitArmAndGraphDefaults(requestContext, endpoint);
      string parameter = endpoint.Authorization.Parameters["tenantid"];
      string azDevAccessToken = AzureServicePrincipalHelper.GetAzDevAccessToken(requestContext, endpoint);
      string armAccessToken = AzureServicePrincipalHelper.GetArmAccessToken(requestContext, parameter, azDevAccessToken);
      AzureServicePrincipalHelper.ValidateRoleAssignmentPermissions(requestContext, endpoint, endpointCurrentScope, armAccessToken);
      if (requestContext.IsFeatureEnabled("ServiceEnpoints.Service.UseMicrosoftGraph"))
      {
        string graphAccessToken = AzureServicePrincipalHelper.GetMicrosoftGraphAccessToken(requestContext, parameter, azDevAccessToken);
        AzureServicePrincipalHelper.ValidateUserPermissionsOnActiveDirectoryWithMicrosoftGraph(requestContext, endpoint, graphAccessToken);
      }
      else
      {
        string graphAccessToken = AzureServicePrincipalHelper.GetGraphAccessToken(requestContext, parameter, azDevAccessToken);
        AzureServicePrincipalHelper.ValidateUserPermissionsOnActiveDirectory(requestContext, endpoint, graphAccessToken);
      }
    }

    private static AzureServicePrincipalHelper.ARMApiQueryResult<AzureServicePrincipalHelper.RoleAssignmentPermission> GetRoleAssignmentPermissions(
      IVssRequestContext requestContext,
      string getPermissionsUri,
      string armAccessToken)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Regex.Replace(getPermissionsUri, "/+", "/").Replace("https:/", "https://").Replace("http:/", "http://"));
      request.Headers.Add("Authorization", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bearer {0}", (object) armAccessToken));
      HttpResponseMessage response = AzureServicePrincipalHelper.ExecuteHttpRequest(requestContext, request);
      string str = AzureServicePrincipalHelper.HandleHttpResponse(requestContext, response, ServiceEndpointResources.InsufficientPrivilegesOnSubscription());
      AzureServicePrincipalHelper.ARMApiQueryResult<AzureServicePrincipalHelper.RoleAssignmentPermission> assignmentPermissions = new AzureServicePrincipalHelper.ARMApiQueryResult<AzureServicePrincipalHelper.RoleAssignmentPermission>()
      {
        Value = (IList<AzureServicePrincipalHelper.RoleAssignmentPermission>) new List<AzureServicePrincipalHelper.RoleAssignmentPermission>()
      };
      if (!string.IsNullOrEmpty(str))
        assignmentPermissions = JsonConvert.DeserializeObject<AzureServicePrincipalHelper.ARMApiQueryResult<AzureServicePrincipalHelper.RoleAssignmentPermission>>(str);
      return assignmentPermissions;
    }

    private static void ValidateRoleAssignmentPermissions(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string endpointCurrentScope,
      string armAccessToken)
    {
      if (AzureServicePrincipalHelper.IsServicePrincipalScopingChanged(endpoint, endpointCurrentScope))
      {
        string getPermissionsUri = AzureServicePrincipalHelper.ServiceEndpointArmUri + endpointCurrentScope + "/providers/Microsoft.Authorization/permissions?api-version=2015-07-01";
        if (!AzureServicePrincipalHelper.ValidatePermissions(AzureServicePrincipalHelper.GetRoleAssignmentPermissions(requestContext, getPermissionsUri, armAccessToken).Value, "microsoft.authorization/roleassignments/delete"))
          throw new ServiceEndpointException(ServiceEndpointResources.InsufficientPrivilegesOverScope((object) "Microsoft.Authorization/roleassignments/delete", (object) endpointCurrentScope));
      }
      string scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifier(endpoint);
      string getPermissionsUri1 = AzureServicePrincipalHelper.ServiceEndpointArmUri + scopeIdentifier + "/providers/Microsoft.Authorization/permissions?api-version=2015-07-01";
      if (!AzureServicePrincipalHelper.ValidatePermissions(AzureServicePrincipalHelper.GetRoleAssignmentPermissions(requestContext, getPermissionsUri1, armAccessToken).Value, "microsoft.authorization/roleassignments/write"))
        throw new ServiceEndpointException(ServiceEndpointResources.InsufficientPrivilegesOverScope((object) "Microsoft.Authorization/roleassignments/write", (object) scopeIdentifier));
    }

    public static void ValidateAssignPermissionsOnKeyVault(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      List<AzurePermission> azurePermissionList = AzureServicePrincipalHelper.DeserializeAzurePermissions(endpoint);
      if (azurePermissionList.IsNullOrEmpty<AzurePermission>())
      {
        requestContext.TraceInfo(34000816, "EndpointService", "No Azure spn permissions exists for endpoint {0} hence not validating any permisisons", (object) endpoint.Id);
      }
      else
      {
        AzureServicePrincipalHelper.InitArmAndGraphDefaults(requestContext, endpoint);
        string parameter = endpoint.Authorization.Parameters["tenantid"];
        string azDevAccessToken = AzureServicePrincipalHelper.GetAzDevAccessToken(requestContext, endpoint);
        string armAccessToken = AzureServicePrincipalHelper.GetArmAccessToken(requestContext, parameter, azDevAccessToken);
        foreach (AzurePermission azurePermission in azurePermissionList.Where<AzurePermission>((Func<AzurePermission, bool>) (p => !p.Provisioned && p.ResourceProvider == "Microsoft.KeyVault")))
        {
          string str1 = endpoint.Data["subscriptionId"];
          AzureKeyVaultPermission keyVaultPermission = (AzureKeyVaultPermission) azurePermission;
          string str2 = "/subscriptions/" + str1 + "/resourceGroups/" + keyVaultPermission.ResourceGroup + "/providers/Microsoft.KeyVault/vaults/" + keyVaultPermission.Vault;
          string getPermissionsUri = AzureServicePrincipalHelper.ServiceEndpointArmUri + str2 + "/providers/Microsoft.Authorization/permissions?api-version=2015-07-01";
          if (!AzureServicePrincipalHelper.ValidatePermissions(AzureServicePrincipalHelper.GetRoleAssignmentPermissions(requestContext, getPermissionsUri, armAccessToken).Value, "microsoft.keyvault/vaults/accesspolicies/write"))
            throw new ServiceEndpointException(ServiceEndpointResources.InsufficientPrivilegesOverKeyVault((object) str2));
        }
      }
    }

    internal static bool ValidatePermissions(
      IList<AzureServicePrincipalHelper.RoleAssignmentPermission> permissions,
      string permissionToMatch)
    {
      try
      {
        TimeSpan matchTimeout = TimeSpan.FromSeconds(10.0);
        foreach (AzureServicePrincipalHelper.RoleAssignmentPermission permission in (IEnumerable<AzureServicePrincipalHelper.RoleAssignmentPermission>) permissions)
        {
          bool flag = false;
          foreach (string action in (IEnumerable<string>) permission.Actions)
          {
            string pattern = "^" + Regex.Escape(action).Replace("\\*", ".*") + "$";
            if (Regex.IsMatch(permissionToMatch, pattern, RegexOptions.IgnoreCase, matchTimeout))
            {
              flag = true;
              break;
            }
          }
          if (flag)
          {
            foreach (string notAction in (IEnumerable<string>) permission.NotActions)
            {
              string pattern = "^" + Regex.Escape(notAction).Replace("\\*", ".*") + "$";
              if (Regex.IsMatch(permissionToMatch, pattern, RegexOptions.IgnoreCase, matchTimeout))
              {
                flag = false;
                break;
              }
            }
          }
          if (flag)
            return true;
        }
      }
      catch (RegexMatchTimeoutException ex)
      {
        throw new RegexMatchTimeoutException("Failed to validate '" + permissionToMatch + "' permission.", (Exception) ex);
      }
      return false;
    }

    private static void ValidateUserPermissionsOnActiveDirectory(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string graphAccessToken)
    {
      string objectId = endpoint.Data["appObjectId"];
      GraphConnection graphConnection = AzureServicePrincipalHelper.GetGraphConnection(requestContext, graphAccessToken);
      Application entity = graphConnection.Get<Application>(objectId);
      try
      {
        entity.DisplayName = entity.DisplayName;
        graphConnection.Update<Application>(entity);
      }
      catch (AuthorizationException ex)
      {
        throw new ServiceEndpointException(ServiceEndpointResources.InsufficientPrivilegesOnAAD((object) objectId), (Exception) ex);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    private static void ValidateUserPermissionsOnActiveDirectoryWithMicrosoftGraph(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string msGraphAccessToken)
    {
      string input = endpoint.Data["appObjectId"];
      AadService service = requestContext.GetService<AadService>();
      GetApplicationByIdRequest applicationByIdRequest1 = new GetApplicationByIdRequest();
      applicationByIdRequest1.AccessToken = msGraphAccessToken;
      applicationByIdRequest1.AppObjectId = Guid.Parse(input);
      GetApplicationByIdRequest applicationByIdRequest2 = applicationByIdRequest1;
      applicationByIdRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
      AadApplication application = service.GetApplicationById(requestContext, applicationByIdRequest2).Application;
      try
      {
        UpdateApplicationRequest applicationRequest1 = new UpdateApplicationRequest();
        applicationRequest1.AccessToken = msGraphAccessToken;
        applicationRequest1.ApplicationToUpdate = application;
        UpdateApplicationRequest applicationRequest2 = applicationRequest1;
        applicationRequest2.SetGraphUriOverrideOnDevFabric(requestContext);
        service.UpdateApplication(requestContext, applicationRequest2);
      }
      catch (MicrosoftGraphRequestDeniedException ex)
      {
        throw new ServiceEndpointException(ServiceEndpointResources.InsufficientPrivilegesOnAAD((object) input), (Exception) ex);
      }
    }

    private static string GetSpnObjectId(
      IVssRequestContext requestContext,
      string graphAccessToken,
      ServiceEndpoint endpoint)
    {
      string enumerable;
      endpoint.Data.TryGetValue("spnObjectId", out enumerable);
      if (enumerable.IsNullOrEmpty<char>())
        enumerable = requestContext.IsFeatureEnabled("ServiceEnpoints.Service.UseMicrosoftGraph") ? AzureServicePrincipalHelper.GetServicePrincipalIdWithMicrosoftGraph(requestContext, graphAccessToken, endpoint) : AzureServicePrincipalHelper.GetServicePrincipalId(requestContext, graphAccessToken, endpoint);
      return enumerable;
    }

    private static List<AzurePermission> DeserializeAzurePermissions(ServiceEndpoint endpoint)
    {
      List<AzurePermission> source = (List<AzurePermission>) null;
      if (endpoint.Data.ContainsKey("azureSpnPermissions"))
        source = JsonUtility.FromString<List<AzurePermission>>(endpoint.Data["azureSpnPermissions"]);
      if (source == null)
        source = new List<AzurePermission>();
      string input;
      Guid roleAssignmentId;
      if (endpoint.Data.TryGetValue("azureSpnRoleAssignmentId", out input) && Guid.TryParse(input, out roleAssignmentId) && !source.Any<AzurePermission>((Func<AzurePermission, bool>) (a => a.GetType() == typeof (AzureRoleAssignmentPermission) && ((AzureRoleAssignmentPermission) a).RoleAssignmentId == roleAssignmentId)))
      {
        List<AzurePermission> azurePermissionList = source;
        AzureRoleAssignmentPermission assignmentPermission = new AzureRoleAssignmentPermission();
        assignmentPermission.RoleAssignmentId = roleAssignmentId;
        assignmentPermission.Provisioned = true;
        azurePermissionList.Add((AzurePermission) assignmentPermission);
      }
      return source;
    }

    private static void UnassignServicePrincipalPermissions(
      IVssRequestContext requestContext,
      string armAccessToken,
      string graphAccessToken,
      ServiceEndpoint endpoint,
      string endpointCurrentScope = null)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (UnassignServicePrincipalPermissions));
      List<AzurePermission> azurePermissionList = AzureServicePrincipalHelper.DeserializeAzurePermissions(endpoint);
      if (azurePermissionList.IsNullOrEmpty<AzurePermission>())
      {
        requestContext.TraceInfo(34000817, "EndpointService", "No Azure spn permissions exists for endpoint {0} hence not removing any permisisons", (object) endpoint.Id);
      }
      else
      {
        string tenantId = endpoint.Authorization.Parameters["tenantid"];
        Uri baseUri = AzureServicePrincipalHelper.GetBaseUri(requestContext, "/Service/Commerce/AzureResourceManager/BaseUrl", AzureServicePrincipalHelper.ServiceEndpointArmUri);
        string spnObjectId = AzureServicePrincipalHelper.GetSpnObjectId(requestContext, graphAccessToken, endpoint);
        foreach (AzurePermission azurePermission1 in azurePermissionList.Where<AzurePermission>((Func<AzurePermission, bool>) (p => p.Provisioned && p.UnAssignPermissionSupported())))
        {
          AzurePermission azurePermission = azurePermission1;
          AzureServicePrincipalHelper.InvokeActionWithRetry(20, (Action) (() =>
          {
            requestContext.TraceInfo(0, "EndpointService", "Unassigning azure permission '" + azurePermission.GetPermissionDescription() + "' for the service principal '" + spnObjectId + "'");
            HttpResponseMessage response = AzureServicePrincipalHelper.ExecuteHttpRequest(requestContext, new HttpRequestMessage(HttpMethod.Delete, new Uri(baseUri, azurePermission.GetUnassignPermissionRelativeUri(endpoint, endpointCurrentScope)))
            {
              Headers = {
                {
                  "Authorization",
                  string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bearer {0}", (object) armAccessToken)
                }
              }
            });
            string result;
            if (AzureServicePrincipalHelper.IsEndpointScopedToSubscription(endpoint))
            {
              string str = endpoint.Data["subscriptionId"];
              result = AzureServicePrincipalHelper.HandleHttpResponse(requestContext, response, ServiceEndpointResources.FailedToRemoveAzurePermissionError((object) azurePermission.GetPermissionDescription(), (object) spnObjectId, (object) str));
            }
            else if (AzureServicePrincipalHelper.IsEndpointScopedToWorkspace(endpoint))
            {
              string str = endpoint.Data["mlWorkspaceName"];
              result = AzureServicePrincipalHelper.HandleHttpResponse(requestContext, response, ServiceEndpointResources.FailedToRemoveAzurePermissionErrorOnMLWorkspace((object) azurePermission.GetPermissionDescription(), (object) spnObjectId, (object) str));
            }
            else
            {
              string str = endpoint.Data["managementGroupId"];
              result = AzureServicePrincipalHelper.HandleHttpResponse(requestContext, response, ServiceEndpointResources.FailedToRemoveAzurePermissionErrorOnManagementGroup((object) azurePermission.GetPermissionDescription(), (object) spnObjectId, (object) str));
            }
            azurePermission.OnUnassignPermissionCompletion(endpoint, spnObjectId, result);
            azurePermission.Provisioned = false;
            requestContext.TraceInfo(0, "EndpointService", string.Format("Unassigned azure permission '{0}' for the service principal: '{1}', tenantId: '{2}', endpoint: '{3}'", (object) azurePermission.GetPermissionDescription(), (object) spnObjectId, (object) tenantId, (object) endpoint.Id));
          }), (Action<Exception>) (ex =>
          {
            if (ex.Data.Contains((object) "NonRetriable"))
              throw ex;
          }));
        }
        endpoint.Data["azureSpnPermissions"] = (string) null;
        requestContext.TraceLeave(0, "EndpointService", nameof (UnassignServicePrincipalPermissions));
      }
    }

    private static GraphConnection GetGraphConnection(
      IVssRequestContext requestContext,
      string accessToken)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (GetGraphConnection));
      GraphConnection graphConnection;
      if (AzureServicePrincipalHelper.GraphConnection != null)
      {
        graphConnection = AzureServicePrincipalHelper.GraphConnection;
      }
      else
      {
        string host = new Uri(AzureServicePrincipalHelper.ServiceEndpointGraphUri).Host;
        graphConnection = new GraphConnection(accessToken, new GraphSettings()
        {
          ApiVersion = AzureServicePrincipalHelper.ServiceEndpointGraphApiVersion,
          GraphDomainName = host
        });
      }
      requestContext.TraceLeave(0, "EndpointService", nameof (GetGraphConnection));
      return graphConnection;
    }

    private static string GenerateRandomPassword(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (GenerateRandomPassword));
      byte[] numArray = new byte[40];
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
        cryptoServiceProvider.GetNonZeroBytes(numArray);
      string base64String = Convert.ToBase64String(numArray);
      requestContext.TraceLeave(0, "EndpointService", nameof (GenerateRandomPassword));
      return base64String;
    }

    private static void InvokeActionWithRetry(
      int count,
      Action action,
      Action<Exception> onException)
    {
      new RetryManager(count, TimeSpan.FromSeconds(1.0), onException).Invoke(action);
    }

    private static string GetPermissionDescription(this AzurePermission permission)
    {
      switch (permission.ResourceProvider)
      {
        case "Microsoft.RoleAssignment":
          return string.Format("RoleAssignmentId: {0}", (object) ((AzureRoleAssignmentPermission) permission).RoleAssignmentId);
        case "Microsoft.KeyVault":
          AzureKeyVaultPermission keyVaultPermission = (AzureKeyVaultPermission) permission;
          return "ResourceGroup: " + keyVaultPermission.ResourceGroup + ", Vault: " + keyVaultPermission.Vault;
        default:
          throw new ServiceEndpointException(permission.ResourceProvider + " is not a supported resource provider for azure permission");
      }
    }

    private static string GetAssignPermissionRelativeUri(
      this AzurePermission permission,
      ServiceEndpoint endpoint)
    {
      switch (permission.ResourceProvider)
      {
        case "Microsoft.RoleAssignment":
          AzureRoleAssignmentPermission assignmentPermission = (AzureRoleAssignmentPermission) permission;
          return string.Format("{0}/providers/Microsoft.Authorization/roleAssignments/{1}?api-version={2}", (object) AzureServicePrincipalHelper.GetScopeIdentifier(endpoint).Trim('/'), (object) assignmentPermission.RoleAssignmentId, (object) "2015-07-01");
        case "Microsoft.KeyVault":
          string str = endpoint.Data["subscriptionId"];
          AzureKeyVaultPermission keyVaultPermission = (AzureKeyVaultPermission) permission;
          return string.Format("subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.KeyVault/vaults/{2}/accessPolicies/add?api-version=2016-10-01", (object) str, (object) keyVaultPermission.ResourceGroup, (object) keyVaultPermission.Vault);
        default:
          throw new ServiceEndpointException(permission.ResourceProvider + " is not a supported resource provider for azure permission");
      }
    }

    private static string GetUnassignPermissionRelativeUri(
      this AzurePermission permission,
      ServiceEndpoint endpoint,
      string endpointCurrentScope = null)
    {
      switch (permission.ResourceProvider)
      {
        case "Microsoft.RoleAssignment":
          AzureRoleAssignmentPermission assignmentPermission = (AzureRoleAssignmentPermission) permission;
          return string.Format("{0}/providers/Microsoft.Authorization/roleAssignments/{1}?api-version={2}", (object) (endpointCurrentScope ?? AzureServicePrincipalHelper.GetScopeIdentifier(endpoint)).Trim('/'), (object) assignmentPermission.RoleAssignmentId, (object) "2015-07-01");
        case "Microsoft.KeyVault":
          throw new NotImplementedException();
        default:
          throw new ServiceEndpointException(permission.ResourceProvider + " is not a supported resource provider for azure permission");
      }
    }

    private static string GetAssignPermissionPayload(
      this AzurePermission permission,
      ServiceEndpoint endpoint,
      string spnObjectId)
    {
      switch (permission.ResourceProvider)
      {
        case "Microsoft.RoleAssignment":
          string scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifier(endpoint);
          return string.Format("\r\n                                            {{\r\n                                              \"properties\": {{\r\n                                                \"roleDefinitionId\": \"{0}\",\r\n                                                \"principalId\": \"{1}\",\r\n                                                \"scope\": \"{2}\"\r\n                                                }}\r\n                                            }}", (object) AzureServicePrincipalHelper.GetRoleDefinitionIdentifier(endpoint, scopeIdentifier), (object) spnObjectId, (object) scopeIdentifier);
        case "Microsoft.KeyVault":
          return string.Format("\r\n                                    {{\r\n                                        \"properties\": {{\r\n                                            \"accessPolicies\": [\r\n                                                    {{\r\n                                                        \"tenantId\": \"{0}\",\r\n                                                        \"objectId\": \"{1}\",\r\n                                                        \"permissions\": {{\r\n                                                            \"keys\": [],\r\n                                                            \"secrets\": [\"Get\", \"List\"],\r\n                                                            \"certificates\": []\r\n                                                        }}\r\n                                                    }}\r\n                                            ]\r\n                                        }}\r\n                                    }}", (object) endpoint.Authorization.Parameters["tenantid"], (object) spnObjectId);
        default:
          throw new ServiceEndpointException(permission.ResourceProvider + " is not a supported resource provider for azure permission");
      }
    }

    private static void OnAssignPermissionCompletion(
      this AzurePermission permission,
      ServiceEndpoint endpoint,
      string spnObjectId,
      string result)
    {
      switch (permission.ResourceProvider)
      {
        case "Microsoft.RoleAssignment":
          AzureRoleAssignmentPermission assignmentPermission = (AzureRoleAssignmentPermission) permission;
          if (result.Equals("false", StringComparison.InvariantCultureIgnoreCase))
          {
            string scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifier(endpoint);
            string definitionIdentifier = AzureServicePrincipalHelper.GetRoleDefinitionIdentifier(endpoint, scopeIdentifier);
            if (AzureServicePrincipalHelper.IsEndpointScopedToSubscription(endpoint))
            {
              string str = endpoint.Data["subscriptionId"];
              throw new ServiceEndpointException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to assign role '{0}' to the service principal '{1}' on subscription id '{2}'.  Error details: '{3}'", (object) definitionIdentifier, (object) spnObjectId, (object) str, (object) result));
            }
            if (AzureServicePrincipalHelper.IsEndpointScopedToWorkspace(endpoint))
            {
              string str = endpoint.Data["mlWorkspaceName"];
              throw new ServiceEndpointException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to assign role '{0}' to the service principal '{1}' on machine learning workspace id '{2}'.  Error details: '{3}'", (object) definitionIdentifier, (object) spnObjectId, (object) str, (object) result));
            }
            string str1 = endpoint.Data["managementGroupId"];
            throw new ServiceEndpointException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to assign role '{0}' to the service principal '{1}' on management group id '{2}'.  Error details: '{3}'", (object) definitionIdentifier, (object) spnObjectId, (object) str1, (object) result));
          }
          endpoint.Data["azureSpnRoleAssignmentId"] = assignmentPermission.RoleAssignmentId.ToString();
          break;
        case "Microsoft.KeyVault":
          break;
        default:
          throw new ServiceEndpointException(permission.ResourceProvider + " is not a supported resource provider for azure permission");
      }
    }

    private static bool UnAssignPermissionSupported(this AzurePermission permission)
    {
      switch (permission.ResourceProvider)
      {
        case "Microsoft.RoleAssignment":
          return true;
        case "Microsoft.KeyVault":
          return false;
        default:
          throw new ServiceEndpointException(permission.ResourceProvider + " is not a supported resource provider for azure permission");
      }
    }

    private static void OnUnassignPermissionCompletion(
      this AzurePermission permission,
      ServiceEndpoint endpoint,
      string spnObjectId,
      string result)
    {
      switch (permission.ResourceProvider)
      {
        case "Microsoft.RoleAssignment":
          if (result.Equals("false", StringComparison.InvariantCultureIgnoreCase))
          {
            string scopeIdentifier = AzureServicePrincipalHelper.GetScopeIdentifier(endpoint);
            string definitionIdentifier = AzureServicePrincipalHelper.GetRoleDefinitionIdentifier(endpoint, scopeIdentifier);
            if (AzureServicePrincipalHelper.IsEndpointScopedToSubscription(endpoint))
            {
              string str = endpoint.Data["subscriptionId"];
              throw new ServiceEndpointException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to remove role '{0}' to the service principal '{1}' on subscription id '{2}'.  Error details: '{3}'", (object) definitionIdentifier, (object) spnObjectId, (object) str, (object) result));
            }
            if (AzureServicePrincipalHelper.IsEndpointScopedToWorkspace(endpoint))
            {
              string str = endpoint.Data["mlWorkspaceName"];
              throw new ServiceEndpointException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to remove role '{0}' to the service principal '{1}' on machine learning workspace id '{2}'.  Error details: '{3}'", (object) definitionIdentifier, (object) spnObjectId, (object) str, (object) result));
            }
            string str1 = endpoint.Data["managementGroupId"];
            throw new ServiceEndpointException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to remove role '{0}' to the service principal '{1}' on managementGroup id '{2}'.  Error details: '{3}'", (object) definitionIdentifier, (object) spnObjectId, (object) str1, (object) result));
          }
          endpoint.Data.Remove("azureSpnRoleAssignmentId");
          break;
        case "Microsoft.KeyVault":
          break;
        default:
          throw new ServiceEndpointException(permission.ResourceProvider + " is not a supported resource provider for azure permission");
      }
    }

    private static string GetRoleDefinitionIdentifier(
      ServiceEndpoint serviceEndpoint,
      string scopeIdentifier)
    {
      string str = AzureServicePrincipalHelper.GetAuthorizationParameter(serviceEndpoint, "Role") ?? AzureServicePrincipalHelper.GetRoleId(serviceEndpoint);
      return "/" + scopeIdentifier.Trim('/') + "/providers/Microsoft.Authorization/roleDefinitions/" + str;
    }

    private static string GetRoleId(ServiceEndpoint serviceEndpoint) => AzureServicePrincipalHelper.IsEndpointScopedToSubscription(serviceEndpoint) || AzureServicePrincipalHelper.IsEndpointScopedToWorkspace(serviceEndpoint) ? "b24988ac-6180-42a0-ab88-20f7382dd24c" : "8e3af657-a8ff-443c-a75c-2fe8c4bcb635";

    private static string GetScopeIdentifierForSubscription(
      ServiceEndpoint serviceEndpoint,
      string subscriptionId)
    {
      return AzureServicePrincipalHelper.GetAuthorizationParameter(serviceEndpoint, "Scope") ?? "/subscriptions/" + subscriptionId + "/";
    }

    private static string GetScopeIdentifierForManagementGroup(
      ServiceEndpoint serviceEndpoint,
      string managementGroupId)
    {
      return AzureServicePrincipalHelper.GetAuthorizationParameter(serviceEndpoint, "Scope") ?? "/providers/Microsoft.Management/managementGroups/" + managementGroupId + "/";
    }

    private static string GetScopeIdentifierForWorkspace(
      ServiceEndpoint serviceEndpoint,
      string subscriptionId,
      string resourceGroupName,
      string workspaceName)
    {
      string authorizationParameter = AzureServicePrincipalHelper.GetAuthorizationParameter(serviceEndpoint, "Scope");
      if (authorizationParameter != null)
        return authorizationParameter;
      return "/subscriptions/" + subscriptionId + "/resourceGroups/" + resourceGroupName + "/providers/Microsoft.MachineLearningServices/workspaces/" + workspaceName;
    }

    private static string GetAuthorizationParameter(
      ServiceEndpoint serviceEndpoint,
      string parameterName)
    {
      string str;
      return serviceEndpoint.Authorization?.Parameters == null || !serviceEndpoint.Authorization.Parameters.TryGetValue(parameterName, out str) || string.IsNullOrWhiteSpace(str) ? (string) null : str;
    }

    private class AadApplicationAndServicePrincipal
    {
      public string SpnClientId { get; set; }

      public string SpnKey { get; set; }

      public string SpnObjectId { get; set; }

      public string AppObjectId { get; set; }
    }

    internal class ARMApiQueryResult<T>
    {
      public IList<T> Value { get; set; }
    }

    internal class RoleAssignmentPermission
    {
      public IList<string> Actions { get; set; }

      public IList<string> NotActions { get; set; }
    }

    internal class ArmErrorContent
    {
      [JsonProperty(PropertyName = "error")]
      public AzureServicePrincipalHelper.ArmError Error { get; set; }
    }

    internal class ArmError
    {
      [JsonProperty(PropertyName = "code")]
      public string ErrorCode { get; set; }

      [JsonProperty(PropertyName = "message")]
      public string ErrorMessage { get; set; }
    }
  }
}
