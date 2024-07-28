// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.ServiceEndpointValidator
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class ServiceEndpointValidator
  {
    private const string DeleteMethod = "Delete";
    private readonly IVssRequestContext _requestContext;
    private static List<InputDescriptor> s_hiddenAuthorizationParameters;

    public ServiceEndpointValidator(IVssRequestContext requestContext) => this._requestContext = requestContext;

    public void ValidateServiceEndpoint(
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType,
      bool isUpdate,
      bool isDraft = false)
    {
      ArgumentUtility.CheckForNull<ServiceEndpoint>(endpoint, nameof (endpoint));
      ArgumentUtility.CheckForEmptyGuid(endpoint.Id, "endpoint.Id");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(endpoint.Name, "endpoint.Name");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(endpoint.Type, "endpoint.Type");
      ArgumentUtility.CheckForNull<EndpointAuthorization>(endpoint.Authorization, "endpoint.Authorization");
      ServiceEndpointValidator.ValidateEndpointUrl(endpoint, endpointType);
      if (endpoint.Description != null)
        ArgumentUtility.CheckStringLength(endpoint.Description, "endpoint.Description", 1024, expectedServiceArea: "ServiceEndpoints");
      if (endpoint.OperationStatus != null)
        ArgumentUtility.CheckStringLength(JsonUtility.ToString<JToken>((IList<JToken>) endpoint.OperationStatus), "endpoint.OperationStatus", 3072, expectedServiceArea: "ServiceEndpoints");
      if (endpoint.Data != null)
        ArgumentUtility.CheckStringLength(JsonUtility.ToString((object) endpoint.Data), "endpoint.Data", 20480, expectedServiceArea: "ServiceEndpoints");
      if (endpoint.Authorization?.Parameters != null)
        ArgumentUtility.CheckStringLength(JsonUtility.ToString((object) endpoint.Authorization.Parameters), "endpoint.Authorization.Parameters", 20480, expectedServiceArea: "ServiceEndpoints");
      if (endpoint.Type.Equals("AzureRM", StringComparison.OrdinalIgnoreCase))
      {
        if (isUpdate && endpoint != null)
          endpoint.Data?.Remove("credentialDeletionMode");
        this.ValidateAzureRmEndpointDetails(endpoint, endpointType, isUpdate, isDraft);
      }
      else if (endpoint.Type.Equals("kubernetes", StringComparison.OrdinalIgnoreCase))
        this.ValidateKubernetesEndpointDetails(endpoint, existingEndpoint, endpointType, isUpdate);
      else if (endpoint.Type.Equals("dockerregistry", StringComparison.OrdinalIgnoreCase))
        this.ValidateDockerRegistryEndpointDetails(endpoint, existingEndpoint, endpointType, isUpdate);
      else if (endpoint.Type.Equals("ServiceFabric", StringComparison.OrdinalIgnoreCase))
        this.ValidateServiceFabricEndPointDetails(endpoint, existingEndpoint, endpointType, isUpdate);
      else
        this.ValidateEndpointDetails(endpoint, endpointType, isUpdate);
    }

    private void ValidateServiceFabricEndPointDetails(
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      if (endpoint.Authorization.Parameters.ContainsKey("Unsecured") && endpoint.Authorization.Parameters.ContainsKey("UseWindowsSecurity"))
        endpoint.Authorization.Parameters.Remove("UseWindowsSecurity");
      this.ValidateEndpointDetails(endpoint, endpointType, isUpdate);
    }

    public static void ValidateUrlChange(
      ServiceEndpoint oldEndpoint,
      ServiceEndpointDetails endpointDetails,
      ServiceEndpointType endpointType)
    {
      ArgumentUtility.CheckForNull<ServiceEndpoint>(oldEndpoint, nameof (oldEndpoint));
      ArgumentUtility.CheckForNull<ServiceEndpointDetails>(endpointDetails, nameof (endpointDetails));
      List<InputDescriptor> inputDescriptors;
      endpointType.TryGetAuthInputDescriptors(endpointDetails.Authorization.Scheme, out inputDescriptors);
      ServiceEndpointValidator.ValidateUrlOrAuthSchemeChange(oldEndpoint.Url, endpointDetails.Url, oldEndpoint.Data, endpointDetails.Data, oldEndpoint.Authorization, endpointDetails.Authorization, endpointDetails.Type, endpointType?.InputDescriptors, inputDescriptors);
    }

    public static void ValidateUrlOrAuthSchemeChange(
      ServiceEndpoint oldEndpoint,
      ServiceEndpoint newEndpoint,
      ServiceEndpointType endpointType)
    {
      ArgumentUtility.CheckForNull<ServiceEndpoint>(oldEndpoint, nameof (oldEndpoint));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(newEndpoint, nameof (newEndpoint));
      List<InputDescriptor> inputDescriptors;
      endpointType.TryGetAuthInputDescriptors(newEndpoint.Authorization.Scheme, out inputDescriptors);
      ServiceEndpointValidator.ValidateUrlOrAuthSchemeChange(oldEndpoint.Url, newEndpoint.Url, oldEndpoint.Data, newEndpoint.Data, oldEndpoint.Authorization, newEndpoint.Authorization, newEndpoint.Type, endpointType?.InputDescriptors, inputDescriptors);
    }

    private static void ValidateUrlOrAuthSchemeChange(
      Uri oldUri,
      Uri newUri,
      IDictionary<string, string> oldEndpointData,
      IDictionary<string, string> newEndpointData,
      EndpointAuthorization oldEndpointAuthorization,
      EndpointAuthorization newEndpointAuthorization,
      string endpointType,
      List<InputDescriptor> endpointTypeInputDescriptors,
      List<InputDescriptor> endpointTypeAuthSchemeInputDescriptors)
    {
      bool flag1 = Uri.Compare(oldUri, newUri, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
      bool flag2 = oldEndpointAuthorization != null && newEndpointAuthorization != null && !oldEndpointAuthorization.Scheme.IsNullOrEmpty<char>() && oldEndpointAuthorization.Scheme.Equals(newEndpointAuthorization.Scheme, StringComparison.OrdinalIgnoreCase);
      if (flag1 & flag2)
        return;
      if (!flag1)
        ServiceEndpointValidator.EnsureEndpointTypeIsNotAzureStack(endpointType, newEndpointData);
      ServiceEndpointValidator.EnsureConfidentialEndpointDataIsReset(oldEndpointData, newEndpointData, (IList<InputDescriptor>) endpointTypeInputDescriptors);
      ServiceEndpointValidator.EnsureConfidentialAuthParametersAreReset(oldEndpointAuthorization, newEndpointAuthorization, endpointType, endpointTypeAuthSchemeInputDescriptors);
    }

    private static void EnsureConfidentialAuthParametersAreReset(
      EndpointAuthorization oldEndpointAuthorization,
      EndpointAuthorization newEndpointAuthorization,
      string endpointType,
      List<InputDescriptor> authSchemeInputDescriptors)
    {
      string str1 = string.Empty;
      bool flag1 = false;
      if (oldEndpointAuthorization?.Parameters == null)
        return;
      if (authSchemeInputDescriptors != null)
      {
        bool flag2 = !string.Equals(newEndpointAuthorization.Scheme, oldEndpointAuthorization.Scheme, StringComparison.OrdinalIgnoreCase);
        foreach (InputDescriptor schemeInputDescriptor in authSchemeInputDescriptors)
        {
          if ((!VisibilityHelper.IsInputVisible(schemeInputDescriptor, newEndpointAuthorization.Parameters) ? 0 : (schemeInputDescriptor.Validation == null ? 1 : (schemeInputDescriptor.Validation.IsRequired ? 1 : 0))) != 0 && schemeInputDescriptor.IsConfidential && !ServiceEndpointValidator.IsInputDescriptorDisabled(schemeInputDescriptor))
          {
            string str2 = (string) null;
            string str3 = (string) null;
            newEndpointAuthorization.Parameters.TryGetValue(schemeInputDescriptor.Id, out str2);
            oldEndpointAuthorization.Parameters.TryGetValue(schemeInputDescriptor.Id, out str3);
            if (str2 == null && (flag2 || !string.IsNullOrEmpty(str3)))
            {
              flag1 = true;
              str1 = schemeInputDescriptor.Name;
              break;
            }
          }
        }
      }
      else if (string.Equals(endpointType, "Git", StringComparison.OrdinalIgnoreCase) || string.Equals(endpointType, "Generic", StringComparison.OrdinalIgnoreCase) || string.Equals(endpointType, "Subversion", StringComparison.OrdinalIgnoreCase) || string.Equals(endpointType, "SSH", StringComparison.OrdinalIgnoreCase))
      {
        string str4;
        newEndpointAuthorization.Parameters.TryGetValue("Password", out str4);
        string str5;
        oldEndpointAuthorization.Parameters.TryGetValue("Password", out str5);
        if (str4 == null && !string.IsNullOrEmpty(str5))
        {
          flag1 = true;
          str1 = ServiceEndpointResources.PasswordText();
        }
      }
      if (flag1)
        throw new ServiceEndpointException(ServiceEndpointResources.ServiceEndpointUrlChangeNotAllowed((object) str1));
    }

    private static void EnsureConfidentialEndpointDataIsReset(
      IDictionary<string, string> oldEndpointData,
      IDictionary<string, string> newEndpointData,
      IList<InputDescriptor> inputDescriptors)
    {
      if (oldEndpointData == null)
        return;
      bool flag = false;
      string str1 = (string) null;
      if (inputDescriptors != null)
      {
        foreach (InputDescriptor inputDescriptor in (IEnumerable<InputDescriptor>) inputDescriptors)
        {
          if ((inputDescriptor.Validation == null ? 1 : (inputDescriptor.Validation.IsRequired ? 1 : 0)) != 0 && inputDescriptor.IsConfidential && !ServiceEndpointValidator.IsInputDescriptorDisabled(inputDescriptor))
          {
            string str2 = (string) null;
            newEndpointData?.TryGetValue(inputDescriptor.Id, out str2);
            string str3;
            oldEndpointData.TryGetValue(inputDescriptor.Id, out str3);
            if (str2 == null && !string.IsNullOrEmpty(str3))
            {
              flag = true;
              str1 = inputDescriptor.Name;
              break;
            }
          }
        }
      }
      if (flag)
        throw new ServiceEndpointException(ServiceEndpointResources.ServiceEndpointUrlChangeNotAllowed((object) str1));
    }

    private static bool IsInputDescriptorDisabled(InputDescriptor descriptor) => descriptor?.Values?.IsDisabled.GetValueOrDefault();

    private static void EnsureEndpointTypeIsNotAzureStack(
      string endpointType,
      IDictionary<string, string> newEndpointData)
    {
      if (string.Compare(endpointType, "AzureRM", StringComparison.OrdinalIgnoreCase) != 0 || newEndpointData == null)
        return;
      string str;
      if (!newEndpointData.TryGetValue("environment", out str))
        str = "AzureCloud";
      if ("AzureStack".Equals(str))
        throw new ServiceEndpointException(ServiceEndpointResources.ServiceEndpointUrlChangeNotAllowedForAzureStack());
    }

    public void ValidateEndpointFields(
      IDictionary<string, string> data,
      IList<InputDescriptor> inputDescriptors,
      string paramName,
      bool isUpdate,
      bool isDraft = false)
    {
      if (inputDescriptors == null)
        return;
      ServiceEndpointValidator.ValidateForExtraParametersInEndpoint(data, inputDescriptors);
      foreach (InputDescriptor inputDescriptor in (IEnumerable<InputDescriptor>) inputDescriptors)
      {
        if (inputDescriptor.Validation != null)
        {
          InputDescriptorValidator.BaseInputValidator validator = InputDescriptorValidator.GetValidator(inputDescriptor.Validation);
          string str;
          bool flag1 = data.TryGetValue(inputDescriptor.Id, out str);
          bool flag2 = VisibilityHelper.IsInputVisible(inputDescriptor, data);
          bool flag3 = inputDescriptor.Validation.IsRequired & flag2;
          bool flag4 = flag3 && !flag1;
          bool flag5 = flag1 && string.IsNullOrWhiteSpace(str);
          if (!isDraft || !(flag4 | flag5))
          {
            if (flag4)
              throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotSpecified(), InputDescriptorValidator.ParamName(paramName, inputDescriptor.Id));
            if (flag5)
            {
              if (!(inputDescriptor.IsConfidential & isUpdate) && flag3 && (inputDescriptor.Values == null || !inputDescriptor.Values.IsDisabled))
                throw new ArgumentException(CommonResources.EmptyStringNotAllowed(), InputDescriptorValidator.ParamName(paramName, inputDescriptor.Id));
            }
            else if (flag1)
              validator.Validate(str, InputDescriptorValidator.ParamName(paramName, inputDescriptor.Id));
          }
        }
      }
    }

    private void ValidateKubernetesEndpointDetails(
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      string a;
      if (endpoint.Data.TryGetValue("authorizationType", out a) && (string.Equals(a, "Kubeconfig", StringComparison.OrdinalIgnoreCase) && string.Equals(endpoint.Authorization.Scheme, "Token", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "ServiceAccount", StringComparison.OrdinalIgnoreCase) && string.Equals(endpoint.Authorization.Scheme, "Kubernetes", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "AzureSubscription", StringComparison.OrdinalIgnoreCase) && string.Equals(endpoint.Authorization.Scheme, "Token", StringComparison.OrdinalIgnoreCase)))
        throw new ArgumentException(ServiceEndpointResources.KubernetesAuthTypeDoesNotMatchWithAuthScheme((object) endpoint.Authorization.Scheme, (object) a));
      if (isUpdate)
        this.ValidateKubernetesEndpointUpdate(endpoint, existingEndpoint, endpointType);
      this.ValidateEndpointDetails(endpoint, endpointType, isUpdate);
    }

    private void ValidateKubernetesEndpointUpdate(
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType)
    {
      if (existingEndpoint == null)
        return;
      string a1 = (string) null;
      string a2 = (string) null;
      endpoint.Data?.TryGetValue("authorizationType", out a1);
      existingEndpoint.Data?.TryGetValue("authorizationType", out a2);
      if (!string.Equals(a1, "AzureSubscription", StringComparison.OrdinalIgnoreCase) || !string.Equals(a2, "AzureSubscription", StringComparison.OrdinalIgnoreCase))
        return;
      this.PatchAutoPopulatedKubernetesEndpointMetadata(endpoint, existingEndpoint);
      List<InputDescriptor> inputDescriptors1 = endpointType?.InputDescriptors;
      if (inputDescriptors1 != null)
      {
        foreach (InputDescriptor inputDescriptor in inputDescriptors1)
        {
          string b = (string) null;
          string a3 = (string) null;
          endpoint.Data?.TryGetValue(inputDescriptor.Id, out b);
          existingEndpoint.Data?.TryGetValue(inputDescriptor.Id, out a3);
          if (!this.IsKubernetesEndpointParameterWhitelistedForUpdate(inputDescriptor.Id) && !string.Equals(a3, b, StringComparison.Ordinal))
            throw new ServiceEndpointException(ServiceEndpointResources.CannotUpdateEndpointDataKubernetesAzureSubscriptionType((object) inputDescriptor.Id, (object) a3, (object) b));
        }
      }
      List<InputDescriptor> inputDescriptors2;
      if (!endpointType.TryGetAuthInputDescriptors(endpoint.Authorization.Scheme, out inputDescriptors2) || inputDescriptors2 == null)
        return;
      foreach (InputDescriptor inputDescriptor in inputDescriptors2)
      {
        string b = (string) null;
        string a4 = (string) null;
        endpoint.Authorization?.Parameters.TryGetValue(inputDescriptor.Id, out b);
        existingEndpoint.Authorization?.Parameters.TryGetValue(inputDescriptor.Id, out a4);
        if (inputDescriptor.IsConfidential)
        {
          if (!this.IsKubernetesEndpointParameterWhitelistedForUpdate(inputDescriptor.Id) && b != null)
            throw new ServiceEndpointException(ServiceEndpointResources.CannotUpdateConfidentialAuthDataKubernetesAzureSubscriptionType((object) inputDescriptor.Id));
        }
        else if (!this.IsKubernetesEndpointParameterWhitelistedForUpdate(inputDescriptor.Id) && !string.Equals(a4, b, StringComparison.Ordinal))
          throw new ServiceEndpointException(ServiceEndpointResources.CannotUpdateNonConfidentialAuthDataKubernetesAzureSubscriptionType((object) inputDescriptor.Id));
      }
    }

    private void ValidateDockerRegistryEndpointDetails(
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      if (!isUpdate || !endpoint.Authorization.Scheme.Equals("ServicePrincipal", StringComparison.OrdinalIgnoreCase) || !existingEndpoint.Authorization.Scheme.Equals("ServicePrincipal", StringComparison.OrdinalIgnoreCase))
        return;
      List<InputDescriptor> inputDescriptors1 = endpointType?.InputDescriptors;
      if (inputDescriptors1 != null)
      {
        foreach (InputDescriptor inputDescriptor in inputDescriptors1)
        {
          string b = (string) null;
          string a = (string) null;
          endpoint.Data?.TryGetValue(inputDescriptor.Id, out b);
          existingEndpoint.Data?.TryGetValue(inputDescriptor.Id, out a);
          if (!string.Equals(a, b, StringComparison.Ordinal))
            throw new ServiceEndpointException(ServiceEndpointResources.CannotUpdateEndpointDataDockerACRType((object) inputDescriptor.Id, (object) a, (object) b));
        }
      }
      List<InputDescriptor> inputDescriptors2;
      if (!endpointType.TryGetAuthInputDescriptors(endpoint.Authorization.Scheme, out inputDescriptors2) || inputDescriptors2 == null)
        return;
      foreach (InputDescriptor inputDescriptor in inputDescriptors2)
      {
        string b = (string) null;
        string a = (string) null;
        endpoint.Authorization?.Parameters.TryGetValue(inputDescriptor.Id, out b);
        existingEndpoint.Authorization?.Parameters.TryGetValue(inputDescriptor.Id, out a);
        if (inputDescriptor.IsConfidential)
        {
          if (b != null)
            throw new ServiceEndpointException(ServiceEndpointResources.CannotUpdateConfidentialAuthDataAzureContainerRegistry((object) inputDescriptor.Id));
        }
        else if (!string.Equals(a, b, StringComparison.Ordinal))
          throw new ServiceEndpointException(ServiceEndpointResources.CannotUpdateNonConfidentialAuthDataAzureContainerRegistry((object) inputDescriptor.Id));
      }
    }

    private void PatchAutoPopulatedKubernetesEndpointMetadata(
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint)
    {
      string str1;
      if (existingEndpoint.Authorization.Parameters.TryGetValue("roleBindingName", out str1))
        endpoint.Authorization.Parameters["roleBindingName"] = str1;
      string str2;
      if (existingEndpoint.Authorization.Parameters.TryGetValue("secretName", out str2))
        endpoint.Authorization.Parameters["secretName"] = str2;
      string str3;
      if (!existingEndpoint.Authorization.Parameters.TryGetValue("serviceAccountName", out str3))
        return;
      endpoint.Authorization.Parameters["serviceAccountName"] = str3;
    }

    private bool IsKubernetesEndpointParameterWhitelistedForUpdate(string descriptorId) => string.Equals(descriptorId, "operation.type", StringComparison.OrdinalIgnoreCase) || string.Equals(descriptorId, "azureAccessToken", StringComparison.OrdinalIgnoreCase);

    private void ValidateAzureRmEndpointDetails(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      bool isUpdate,
      bool isDraft)
    {
      if (!isDraft)
      {
        string stringVar;
        endpoint.Authorization.Parameters.TryGetValue("tenantid", out stringVar);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar, "tenantid");
      }
      List<InputDescriptor> inputDescriptors = endpointType.AuthenticationSchemes.FirstOrDefault<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (authenticationScheme => authenticationScheme.Scheme.Equals(endpoint.Authorization.Scheme, StringComparison.OrdinalIgnoreCase)))?.InputDescriptors;
      ServiceEndpointValidator.ValidateForExtraParametersInEndpoint(endpoint.Authorization.Parameters, (IList<InputDescriptor>) inputDescriptors);
      ServiceEndpointValidator.ValidateForExtraParametersInEndpoint(endpoint.Data, (IList<InputDescriptor>) endpointType.InputDescriptors);
      if (endpoint.Authorization.Scheme.Equals("PublishProfile", StringComparison.OrdinalIgnoreCase))
      {
        this.ValidateEndpointDetailsForPublishProfileEndpoint(endpoint, endpointType, isUpdate);
      }
      else
      {
        if (!isDraft)
          this.ValidateAzureEndpointScopeLevel(endpoint);
        if (endpoint.Authorization.Scheme.Equals("ServicePrincipal", StringComparison.OrdinalIgnoreCase))
        {
          AzureServicePrincipalHelper.UpdateAzureRmEndpointCreationMode(endpoint, isUpdate);
          if (AzureServicePrincipalHelper.IsSpnAutoCreateEndpoint(endpoint))
            ServiceEndpointValidator.ValidateEndpointDetailsForSpnAutoCreateEndpoint(endpoint, endpointType, isUpdate);
          else
            this.ValidateEndpointDetailsForSpnManualCreateEndpoint(endpoint, endpointType, isUpdate);
        }
        else if (endpoint.Authorization.Scheme.Equals("ManagedServiceIdentity", StringComparison.OrdinalIgnoreCase))
        {
          this.ValidateEndpointDetailsForMSIEndpoint(endpoint, endpointType, isUpdate);
        }
        else
        {
          if (!endpoint.Authorization.Scheme.Equals("WorkloadIdentityFederation", StringComparison.OrdinalIgnoreCase))
            return;
          if (!this._requestContext.IsFeatureEnabled("ServiceEnpoints.Service.UseMicrosoftGraph"))
            throw new NotSupportedException("OIDC Federation Endpoint can be created automatically only with 'ServiceEnpoints.Service.UseMicrosoftGraph' enabled");
          if (!this._requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(this._requestContext, "ms.vss-distributedtask-web.workload-identity-federation"))
            throw new NotSupportedException("OIDC Federation Endpoint is not supported in this organization");
          if (AzureServicePrincipalHelper.IsSpnAutoCreateEndpoint(endpoint))
            ServiceEndpointValidator.ValidateEndpointDetailsForOidcFederationAutoCreateEndpoint(endpoint, endpointType, isUpdate);
          else
            this.ValidateEndpointDetailsForOidcFederationManualCreateEndpoint(endpoint, endpointType, isUpdate, isDraft);
        }
      }
    }

    private void ValidateAzureEndpointScopeLevel(ServiceEndpoint endpoint)
    {
      string str;
      if (endpoint.Data.TryGetValue("scopeLevel", out str) && !string.IsNullOrEmpty(str))
      {
        if (str.Equals("Subscription", StringComparison.OrdinalIgnoreCase))
        {
          string stringVar1;
          endpoint.Data.TryGetValue("subscriptionId", out stringVar1);
          ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar1, "subscriptionId");
          string stringVar2;
          endpoint.Data.TryGetValue("subscriptionName", out stringVar2);
          ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar2, "subscriptionName");
        }
        else if (str.Equals("ManagementGroup", StringComparison.OrdinalIgnoreCase))
        {
          string stringVar3;
          endpoint.Data.TryGetValue("managementGroupId", out stringVar3);
          ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar3, "managementGroupId");
          string stringVar4;
          endpoint.Data.TryGetValue("managementGroupName", out stringVar4);
          ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar4, "managementGroupName");
        }
        else
        {
          if (!str.Equals("AzureMLWorkspace", StringComparison.OrdinalIgnoreCase))
            return;
          string stringVar5;
          endpoint.Data.TryGetValue("mlWorkspaceName", out stringVar5);
          ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar5, "mlWorkspaceName");
          string stringVar6;
          endpoint.Data.TryGetValue("mlWorkspaceLocation", out stringVar6);
          ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar6, "mlWorkspaceLocation");
          string stringVar7;
          endpoint.Data.TryGetValue("resourceGroupName", out stringVar7);
          ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar7, "resourceGroupName");
        }
      }
      else
      {
        endpoint.Data["scopeLevel"] = "Subscription";
        string stringVar8;
        endpoint.Data.TryGetValue("subscriptionId", out stringVar8);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar8, "subscriptionId");
        string stringVar9;
        endpoint.Data.TryGetValue("subscriptionName", out stringVar9);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar9, "subscriptionName");
      }
    }

    public static void ValidateRequestParameters(string requestVerb, string requestContent)
    {
      if (requestVerb.IsNullOrEmpty<char>())
        return;
      if (!string.Equals(requestVerb, "GET", StringComparison.OrdinalIgnoreCase) && !string.Equals(requestVerb, "POST", StringComparison.OrdinalIgnoreCase) && !string.Equals(requestVerb, "Delete", StringComparison.OrdinalIgnoreCase))
        throw new ServiceEndpointQueryFailedException(ServiceEndpointResources.ServiceEndpointRequestVerbUnsupported((object) requestVerb));
      if ((string.Equals(requestVerb, "GET", StringComparison.OrdinalIgnoreCase) || string.Equals(requestVerb, "Delete", StringComparison.OrdinalIgnoreCase)) && !requestContent.IsNullOrEmpty<char>())
        throw new InvalidOperationException(ServiceEndpointResources.BodyNotExpectedInRequest((object) requestContent));
      if (string.Equals(requestVerb, "POST", StringComparison.OrdinalIgnoreCase) && !requestContent.IsNullOrEmpty<char>() && requestContent.Length > BearerTokenArgument.MaxBodySizeSupported)
        throw new InvalidOperationException(ServiceEndpointResources.BodySizeLimitExceeded((object) BearerTokenArgument.MaxBodySizeSupported));
    }

    public static void ValidateEndpointUrl(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(endpoint.Authorization.Scheme, "endpoint.Authorization.Scheme");
      if (endpoint.Authorization.IsOauth2())
        return;
      ServiceEndpointValidator.ValidateUrl(endpoint.Url, "endpoint.Url");
      if (!"azurerm".Equals(endpoint.Type, StringComparison.OrdinalIgnoreCase))
        return;
      ServiceEndpointValidator.ValidateAzureRmUrl(endpoint, endpointType, endpoint.Url);
    }

    public static void ValidateUrl(Uri url, string paramName)
    {
      ArgumentUtility.CheckForNull<Uri>(url, paramName);
      if (!Uri.TryCreate(url.OriginalString, UriKind.Absolute, out Uri _))
        throw new ArgumentException(ServiceEndpointResources.InvalidUrl((object) url), paramName);
    }

    private static void ValidateEndpointDetailsForSpnAutoCreateEndpoint(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      foreach (InputDescriptor inputDescriptor in endpointType.AuthenticationSchemes.FirstOrDefault<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (authenticationScheme => authenticationScheme.Scheme.Equals(endpoint.Authorization.Scheme, StringComparison.OrdinalIgnoreCase)))?.InputDescriptors)
      {
        if (inputDescriptor.Id == "serviceprincipalid" || inputDescriptor.Id == "servicePrincipalKey")
        {
          string enumerable;
          endpoint.Authorization.Parameters.TryGetValue(inputDescriptor.Id, out enumerable);
          if (!isUpdate && !enumerable.IsNullOrEmpty<char>())
            throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotExpectedForSpnAutoCreateEndpoint((object) inputDescriptor.Id));
        }
      }
      string str;
      bool flag = endpoint.Authorization.Parameters.TryGetValue("serviceprincipalid", out str) && !string.IsNullOrEmpty(str);
      foreach (InputDescriptor inputDescriptor in endpointType.InputDescriptors)
      {
        if (inputDescriptor.Id == "appObjectId" || inputDescriptor.Id == "azureSpnRoleAssignmentId" || inputDescriptor.Id == "spnObjectId")
        {
          string enumerable;
          endpoint.Data.TryGetValue(inputDescriptor.Id, out enumerable);
          if (isUpdate)
          {
            if (flag && enumerable.IsNullOrEmpty<char>())
              throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotSpecified(), inputDescriptor.Id);
          }
          else if (!enumerable.IsNullOrEmpty<char>())
            throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotExpectedForSpnAutoCreateEndpoint((object) inputDescriptor.Id));
        }
      }
    }

    private static void ValidateEndpointDetailsForOidcFederationAutoCreateEndpoint(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      ServiceEndpointAuthenticationScheme authenticationScheme1 = endpointType.AuthenticationSchemes.FirstOrDefault<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (authenticationScheme => authenticationScheme.Scheme.Equals(endpoint.Authorization.Scheme, StringComparison.OrdinalIgnoreCase)));
      InputDescriptor inputDescriptor1;
      string enumerable1;
      if ((authenticationScheme1 != null ? authenticationScheme1.InputDescriptors.ToDictionary<InputDescriptor, string, InputDescriptor>((Func<InputDescriptor, string>) (x => x.Id), (Func<InputDescriptor, InputDescriptor>) (x => x)) : (Dictionary<string, InputDescriptor>) null).TryGetValue("serviceprincipalid", out inputDescriptor1) && !isUpdate && endpoint.Authorization.Parameters.TryGetValue("serviceprincipalid", out enumerable1) && !enumerable1.IsNullOrEmpty<char>())
        throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotExpectedForSpnAutoCreateEndpoint((object) inputDescriptor1.Id));
      string str;
      bool flag = endpoint.Authorization.Parameters.TryGetValue("serviceprincipalid", out str) && !string.IsNullOrEmpty(str);
      foreach (InputDescriptor inputDescriptor2 in endpointType.InputDescriptors)
      {
        if (inputDescriptor2.Id == "appObjectId" || inputDescriptor2.Id == "azureSpnRoleAssignmentId" || inputDescriptor2.Id == "spnObjectId")
        {
          string enumerable2;
          endpoint.Data.TryGetValue(inputDescriptor2.Id, out enumerable2);
          if (isUpdate)
          {
            if (flag && enumerable2.IsNullOrEmpty<char>())
              throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotSpecified(), inputDescriptor2.Id);
          }
          else if (!enumerable2.IsNullOrEmpty<char>())
            throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotExpectedForSpnAutoCreateEndpoint((object) inputDescriptor2.Id));
        }
      }
    }

    private void ValidateEndpointDetailsForOidcFederationManualCreateEndpoint(
      ServiceEndpoint endpoint,
      ServiceEndpointType serviceEndpointType,
      bool isUpdate,
      bool isDraft)
    {
      foreach (InputDescriptor inputDescriptor in serviceEndpointType.InputDescriptors)
      {
        string enumerable;
        if ((inputDescriptor.Id == "appObjectId" || inputDescriptor.Id == "spnObjectId") && endpoint.Data.TryGetValue(inputDescriptor.Id, out enumerable) && !enumerable.IsNullOrEmpty<char>())
          throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotExpectedForSpnManualCreateEndpoint((object) inputDescriptor.Id));
      }
      this.ValidateEndpointDetailsWithExtension(endpoint, serviceEndpointType, isUpdate, isDraft);
    }

    private void ValidateEndpointDetailsForSpnManualCreateEndpoint(
      ServiceEndpoint endpoint,
      ServiceEndpointType serviceEndpointType,
      bool isUpdate)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(endpoint.Authorization.Parameters["serviceprincipalid"], "serviceprincipalid");
      string str;
      if (!endpoint.Authorization.Parameters.TryGetValue("authenticationType", out str) || string.IsNullOrEmpty(str))
      {
        endpoint.Authorization.Parameters["authenticationType"] = "spnKey";
        str = "spnKey";
      }
      if (!isUpdate)
      {
        switch (str)
        {
          case "spnKey":
            ArgumentUtility.CheckStringForNullOrWhiteSpace(endpoint.Authorization.Parameters["servicePrincipalKey"], "servicePrincipalKey");
            break;
          case "spnCertificate":
            ArgumentUtility.CheckStringForNullOrWhiteSpace(endpoint.Authorization.Parameters["servicePrincipalCertificate"], "servicePrincipalCertificate");
            break;
          default:
            throw new ArgumentException(ServiceEndpointResources.InvalidAuthType());
        }
      }
      foreach (InputDescriptor inputDescriptor in serviceEndpointType.InputDescriptors)
      {
        if (inputDescriptor.Id == "appObjectId" || inputDescriptor.Id == "spnObjectId")
        {
          string enumerable;
          endpoint.Data.TryGetValue(inputDescriptor.Id, out enumerable);
          if (!enumerable.IsNullOrEmpty<char>())
            throw new ArgumentException(ServiceEndpointResources.EndpointFieldNotExpectedForSpnManualCreateEndpoint((object) inputDescriptor.Id));
        }
      }
      this.ValidateEndpointDetailsWithExtension(endpoint, serviceEndpointType, isUpdate);
    }

    private void ValidateEndpointDetailsForMSIEndpoint(
      ServiceEndpoint endpoint,
      ServiceEndpointType serviceEndpointType,
      bool isUpdate)
    {
      string str = (string) null;
      foreach (InputDescriptor inputDescriptor in serviceEndpointType.InputDescriptors)
      {
        if (string.Equals(inputDescriptor.Id, "scopeLevel"))
        {
          endpoint.Data.TryGetValue("scopeLevel", out str);
          if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentException(ServiceEndpointResources.MissingProperty((object) inputDescriptor.Id));
          if (!"Subscription".Equals(str, StringComparison.OrdinalIgnoreCase) && !"ManagementGroup".Equals(str, StringComparison.OrdinalIgnoreCase) && !"AzureMLWorkspace".Equals(str, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(ServiceEndpointResources.InvalidScopeLevel((object) str), "endpoint.Data[" + inputDescriptor.Id + "]");
        }
      }
      foreach (InputDescriptor inputDescriptor in serviceEndpointType.InputDescriptors)
      {
        string id = inputDescriptor.Id;
        if (id != null)
        {
          switch (id.Length)
          {
            case 10:
              if (id == "scopeLevel")
                continue;
              goto label_30;
            case 11:
              if (id == "environment")
                continue;
              goto label_30;
            case 14:
              if (id == "subscriptionId")
                break;
              goto label_30;
            case 15:
              if (id == "mlWorkspaceName")
                goto label_28;
              else
                goto label_30;
            case 16:
              if (id == "subscriptionName")
                break;
              goto label_30;
            case 17:
              switch (id[0])
              {
                case 'm':
                  if (id == "managementGroupId")
                    goto label_26;
                  else
                    goto label_30;
                case 'r':
                  if (id == "resourceGroupName")
                    goto label_28;
                  else
                    goto label_30;
                default:
                  goto label_30;
              }
            case 19:
              switch (id[1])
              {
                case 'a':
                  if (id == "managementGroupName")
                    goto label_26;
                  else
                    goto label_30;
                case 'l':
                  if (id == "mlWorkspaceLocation")
                    goto label_28;
                  else
                    goto label_30;
                default:
                  goto label_30;
              }
            default:
              goto label_30;
          }
          if (!"Subscription".Equals(str, StringComparison.OrdinalIgnoreCase) && !"AzureMLWorkspace".Equals(str, StringComparison.OrdinalIgnoreCase) && endpoint.Data.ContainsKey(inputDescriptor.Id))
            throw new ArgumentException(ServiceEndpointResources.InvalidMSIEndpointField((object) inputDescriptor.Id), "endpoint.Data[" + inputDescriptor.Id + "]");
          continue;
label_26:
          if (!"ManagementGroup".Equals(str, StringComparison.OrdinalIgnoreCase) && endpoint.Data.ContainsKey(inputDescriptor.Id))
            throw new ArgumentException(ServiceEndpointResources.InvalidMSIEndpointField((object) inputDescriptor.Id), "endpoint.Data[" + inputDescriptor.Id + "]");
          continue;
label_28:
          if (!"AzureMLWorkspace".Equals(str, StringComparison.OrdinalIgnoreCase) && endpoint.Data.ContainsKey(inputDescriptor.Id))
            throw new ArgumentException(ServiceEndpointResources.InvalidMSIEndpointField((object) inputDescriptor.Id), "endpoint.Data[" + inputDescriptor.Id + "]");
          continue;
        }
label_30:
        if (endpoint.Data.ContainsKey(inputDescriptor.Id))
          throw new ArgumentException(ServiceEndpointResources.InvalidMSIEndpointField((object) inputDescriptor.Id), "endpoint.Data[" + inputDescriptor.Id + "]");
      }
    }

    private void ValidateEndpointDetailsForPublishProfileEndpoint(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      string stringVar1;
      endpoint.Data.TryGetValue("subscriptionName", out stringVar1);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar1, "subscriptionName");
      string stringVar2;
      endpoint.Data.TryGetValue("resourceId", out stringVar2);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(stringVar2, "resourceId");
      if (!stringVar2.StartsWith("/"))
        stringVar2 = "/" + stringVar2;
      string[] strArray = stringVar2.Split('/');
      if (strArray.Length < 9 || !strArray[1].Equals("subscriptions", StringComparison.OrdinalIgnoreCase) || !strArray[3].Equals("resourceGroups", StringComparison.OrdinalIgnoreCase) || !ServiceEndpointValidator.IsResourceTypeSupportedForPublishProfileEndpoint(strArray[6] + "/" + strArray[7]))
        throw new ArgumentException(ServiceEndpointResources.InvalidEndpointInput((object) stringVar2));
      foreach (InputDescriptor inputDescriptor in endpointType.InputDescriptors)
      {
        if (!string.Equals(inputDescriptor.Id, "subscriptionName") && !string.Equals(inputDescriptor.Id, "resourceId") && !string.Equals(inputDescriptor.Id, "environment") && !string.Equals(inputDescriptor.Id, "scopeLevel"))
        {
          string enumerable;
          endpoint.Data.TryGetValue(inputDescriptor.Id, out enumerable);
          if (!enumerable.IsNullOrEmpty<char>())
            throw new ArgumentException(ServiceEndpointResources.InvalidPublishProfileEndpointField((object) inputDescriptor.Id), "endpoint.Data[" + inputDescriptor.Id + "]");
        }
      }
    }

    private static bool IsResourceTypeSupportedForPublishProfileEndpoint(string resourceType) => string.Equals(resourceType, "microsoft.web/sites", StringComparison.OrdinalIgnoreCase);

    private static void ValidateForExtraParametersInEndpoint(
      IDictionary<string, string> data,
      IList<InputDescriptor> inputDescriptors)
    {
      IList<string> parametersInEndpoint = ServiceEndpointValidator.FindExtraParametersInEndpoint(data, inputDescriptors);
      if (parametersInEndpoint.Count > 0)
        throw new ArgumentException(ServiceEndpointResources.InvalidEndpointInput((object) string.Join(",", (IEnumerable<string>) parametersInEndpoint)));
    }

    public static IList<string> FindExtraParametersInEndpoint(
      IDictionary<string, string> data,
      IList<InputDescriptor> inputDescriptors)
    {
      IEnumerable<string> second1 = inputDescriptors.Select<InputDescriptor, string>((Func<InputDescriptor, string>) (t => t.Id));
      IEnumerable<string> first = data.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key)).Except<string>(second1, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IEnumerable<string> second2 = (IEnumerable<string>) new string[3]
      {
        "mlWorkspaceName",
        "mlWorkspaceLocation",
        "resourceGroupName"
      };
      IEnumerable<string> second3 = (IEnumerable<string>) new string[1]
      {
        "ServiceUri"
      };
      return (IList<string>) first.Except<string>(second3, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Except<string>(second2, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
    }

    public static void AssertEndpointNotModified(
      ServiceEndpoint endpoint1,
      ServiceEndpoint referenceEndpoint)
    {
      ServiceEndpointValidator.AssertPropertyEqualIfExists<Guid>(endpoint1, referenceEndpoint, (Func<ServiceEndpoint, Guid>) (endpoint2 => endpoint2.Id), "Id");
      ServiceEndpointValidator.AssertPropertyEqualIfExists<string>(endpoint1, referenceEndpoint, (Func<ServiceEndpoint, string>) (endpoint3 => endpoint3.Name), "Name");
      ServiceEndpointValidator.AssertPropertyEqualIfExists<string>(endpoint1, referenceEndpoint, (Func<ServiceEndpoint, string>) (endpoint4 => endpoint4.Description), "Description");
      ServiceEndpointValidator.AssertPropertyEqualIfExists<string>(endpoint1, referenceEndpoint, (Func<ServiceEndpoint, string>) (endpoint5 => endpoint5?.CreatedBy?.Id), "CreatedById");
      ServiceEndpointValidator.AssertPropertyEqualIfExists<Guid>(endpoint1, referenceEndpoint, (Func<ServiceEndpoint, Guid>) (endpoint6 => endpoint6.GroupScopeId), "GroupScopeId");
      ServiceEndpointValidator.AssertPropertyEqualIfExists<string>(endpoint1, referenceEndpoint, (Func<ServiceEndpoint, string>) (endpoint7 => endpoint7.Owner), "Owner");
      ServiceEndpointValidator.AssertPropertyEqualIfExists<Uri>(endpoint1, referenceEndpoint, (Func<ServiceEndpoint, Uri>) (endpoint8 => endpoint8.Url), "Url");
      Dictionary<Guid, ServiceEndpointProjectReference> dictionary = referenceEndpoint.ServiceEndpointProjectReferences.ToDictionary<ServiceEndpointProjectReference, Guid>((Func<ServiceEndpointProjectReference, Guid>) (x => x.ProjectReference.Id));
      foreach (ServiceEndpointProjectReference projectReference1 in (IEnumerable<ServiceEndpointProjectReference>) endpoint1.ServiceEndpointProjectReferences)
      {
        if (!dictionary.ContainsKey(projectReference1.ProjectReference.Id))
          throw new ArgumentException(ServiceEndpointResources.UpgradeCannotChangeProperties((object) "ServiceEndpointProjectReferences"));
        ServiceEndpointProjectReference projectReference2 = dictionary[projectReference1.ProjectReference.Id];
        int num1 = string.IsNullOrEmpty(projectReference1.Name) ? 0 : (!string.Equals(projectReference2.Name, projectReference1.Name) ? 1 : 0);
        bool flag1 = !string.IsNullOrEmpty(projectReference1.Description) && !string.Equals(projectReference2.Description, projectReference1.Description);
        bool flag2 = !string.IsNullOrEmpty(projectReference1.ProjectReference.Name) && !string.Equals(projectReference2.ProjectReference.Name, projectReference1.ProjectReference.Name);
        int num2 = flag1 ? 1 : 0;
        if ((num1 | num2 | (flag2 ? 1 : 0)) != 0)
          throw new ArgumentException(ServiceEndpointResources.UpgradeCannotChangeProperties((object) "ServiceEndpointProjectReferences"));
      }
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) endpoint1.Authorization.Parameters)
      {
        string b;
        if (!referenceEndpoint.Authorization.Parameters.TryGetValue(parameter.Key, out b) || !string.Equals(parameter.Value, b))
          throw new ArgumentException(ServiceEndpointResources.UpgradeCannotChangeProperties((object) ("Authorization.Parameters['" + parameter.Key + "']").ToLowerInvariant()));
      }
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) endpoint1.Data)
      {
        string b;
        if (!referenceEndpoint.Data.TryGetValue(keyValuePair.Key, out b) || !string.Equals(keyValuePair.Value, b))
          throw new ArgumentException(ServiceEndpointResources.UpgradeCannotChangeProperties((object) ("Data['" + keyValuePair.Key + "']").ToLowerInvariant()));
      }
    }

    private static void AssertPropertyEqualIfExists<T>(
      ServiceEndpoint endpoint,
      ServiceEndpoint referenceEndpoint,
      Func<ServiceEndpoint, T> selector,
      string name)
    {
      bool flag = false;
      if ((object) selector(endpoint) != null)
        flag = !object.Equals((object) selector(endpoint), (object) selector(referenceEndpoint));
      if (flag)
        throw new ArgumentException(ServiceEndpointResources.UpgradeCannotChangeProperties((object) name.ToLowerInvariant()));
    }

    private static void ValidateAzureRmUrl(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      Uri uriResult)
    {
      if (endpoint.Data == null)
        return;
      string str = endpoint.Data.TryGetValue("environment", out str) ? str : "AzureCloud";
      if (endpointType == null || endpointType.EndpointUrl == null || endpointType.EndpointUrl.DependsOn == null || endpointType.EndpointUrl.DependsOn.Map == null)
        return;
      bool flag = false;
      if ("AzureStack".Equals(str))
      {
        flag = true;
      }
      else
      {
        foreach (DependencyBinding dependencyBinding in endpointType.EndpointUrl.DependsOn.Map)
        {
          if (dependencyBinding.Key != null && str.Equals(dependencyBinding.Key, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            if (dependencyBinding.Value != null)
            {
              if (!uriResult.Equals((object) new Uri(dependencyBinding.Value)))
                throw new ArgumentException(ServiceEndpointResources.InvalidUrlForProvidedEnvironment((object) uriResult.ToString(), (object) "endpoint.Url", (object) str, (object) dependencyBinding.Value), "endpoint.Url");
              break;
            }
            break;
          }
        }
      }
      if (!flag)
        throw new ArgumentException(ServiceEndpointResources.InvalidEnvironmentProvided((object) str), "endpoint.data['environment']");
    }

    private void ValidateEndpointDetails(
      ServiceEndpoint endpoint,
      ServiceEndpointType serviceEndpointType,
      bool isUpdate)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(endpoint.Type, "endpoint.Type");
      ArgumentUtility.CheckForNull<EndpointAuthorization>(endpoint.Authorization, "endpoint.Authorization");
      ArgumentUtility.CheckStringForNullOrEmpty(endpoint.Authorization.Scheme, "endpoint.Authorization.Scheme");
      if (endpoint.Type.Equals("Bitbucket", StringComparison.OrdinalIgnoreCase) && (string.Equals(endpoint.Authorization.Scheme, "OAuth", StringComparison.OrdinalIgnoreCase) || string.Equals(endpoint.Authorization.Scheme, "OAuth2", StringComparison.OrdinalIgnoreCase)))
        this.ValidateAuthorizationForBitbucketOAuth(endpoint, serviceEndpointType, isUpdate);
      else if (endpoint.IsCustomEndpointType())
        this.ValidateEndpointDetailsWithExtension(endpoint, serviceEndpointType, isUpdate);
      else
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) endpoint.Authorization.Parameters, "endpoint.Authorization.Parameters");
    }

    private void ValidateAuthorizationForBitbucketOAuth(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      this.ValidateEndpointFields(endpoint.Data, (IList<InputDescriptor>) endpointType?.InputDescriptors, "endpoint.Data", isUpdate);
      try
      {
        IDictionary<string, string> parameters = endpoint.Authorization.Parameters;
        List<InputDescriptor> inputDescriptors = new List<InputDescriptor>();
        inputDescriptors.Add(new InputDescriptor()
        {
          Id = "RefreshToken",
          Name = "i18n:Refresh Token",
          Description = "Refresh Token.",
          InputMode = InputMode.TextArea,
          IsConfidential = true,
          Validation = new InputValidation()
          {
            IsRequired = true,
            DataType = InputDataType.String
          }
        });
        inputDescriptors.Add(new InputDescriptor()
        {
          Id = "ConfigurationId",
          Name = "i18n:OAuth Configuration",
          Description = "Configuration for connecting to the endpoint",
          InputMode = InputMode.Combo,
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.Guid
          },
          HasDynamicValueInformation = true
        });
        inputDescriptors.Add(new InputDescriptor()
        {
          Id = "OAuthAccessTokenIsSupplied",
          Name = "i18n:OAuthAccessTokenIsSupplied",
          Description = "Is OAuth Token Is Supplied",
          InputMode = InputMode.TextArea,
          IsConfidential = true,
          Validation = new InputValidation()
          {
            IsRequired = true,
            DataType = InputDataType.String
          }
        });
        int num = isUpdate ? 1 : 0;
        this.ValidateEndpointFields(parameters, (IList<InputDescriptor>) inputDescriptors, "endpoint.Authorization.Parameters", num != 0);
      }
      catch (ArgumentException ex)
      {
        IDictionary<string, string> parameters = endpoint.Authorization.Parameters;
        List<InputDescriptor> inputDescriptors = new List<InputDescriptor>();
        inputDescriptors.Add(new InputDescriptor()
        {
          Id = "accessToken",
          Name = "i18n:Access Token",
          Description = "The strongbox id where the access token is stored.",
          InputMode = InputMode.TextArea,
          IsConfidential = true,
          Validation = new InputValidation()
          {
            IsRequired = true,
            DataType = InputDataType.Guid
          }
        });
        inputDescriptors.Add(new InputDescriptor()
        {
          Id = "ConfigurationId",
          Name = "i18n:OAuth Configuration",
          Description = "Configuration for connecting to the endpoint",
          InputMode = InputMode.Combo,
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.Guid
          },
          HasDynamicValueInformation = true
        });
        inputDescriptors.Add(new InputDescriptor()
        {
          Id = "RefreshToken",
          Name = "i18n:Refresh Token",
          Description = "Refresh Token.",
          InputMode = InputMode.TextArea,
          IsConfidential = true,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.String
          }
        });
        int num = isUpdate ? 1 : 0;
        this.ValidateEndpointFields(parameters, (IList<InputDescriptor>) inputDescriptors, "endpoint.Authorization.Parameters", num != 0);
      }
    }

    private void ValidateEndpointDetailsWithExtension(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      bool isUpdate,
      bool isDraft = false)
    {
      ArgumentUtility.CheckForNull<EndpointAuthorization>(endpoint.Authorization, "endpoint.Authorization");
      if (endpointType == null)
        throw new ArgumentException(ServiceEndpointResources.InvalidEndpointType((object) endpoint.Type), "endpoint.Type");
      this.ValidateEndpointFields(endpoint.Data, (IList<InputDescriptor>) endpointType.InputDescriptors, "endpoint.Data", isUpdate, isDraft);
      if (string.IsNullOrWhiteSpace(endpoint.Authorization.Scheme) || !endpointType.AuthenticationSchemes.Any<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (authenticationScheme => authenticationScheme.Scheme.Equals(endpoint.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))))
        throw new ArgumentException(ServiceEndpointResources.InvalidAuthenticationScheme((object) endpoint.Authorization.Scheme), "endpoint.Authorization.Scheme");
      this.AddAuthorizationParametersForOAuth2(endpoint, endpointType);
      this.ValidateEndpointFields(endpoint.Authorization.Parameters, (IList<InputDescriptor>) endpointType.AuthenticationSchemes.FirstOrDefault<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (authenticationScheme => authenticationScheme.Scheme.Equals(endpoint.Authorization.Scheme, StringComparison.OrdinalIgnoreCase)))?.InputDescriptors, "endpoint.Authorization.Parameters", isUpdate, isDraft);
    }

    private void AddAuthorizationParametersForOAuth2(
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType)
    {
      if (!endpoint.Authorization.Scheme.Equals("OAuth2", StringComparison.OrdinalIgnoreCase))
        return;
      List<InputDescriptor> inputDescriptors = endpointType.AuthenticationSchemes.FirstOrDefault<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (authenticationScheme => authenticationScheme.Scheme.Equals(endpoint.Authorization.Scheme, StringComparison.OrdinalIgnoreCase)))?.InputDescriptors;
      if (inputDescriptors == null)
        return;
      foreach (InputDescriptor authorizationParameter in ServiceEndpointValidator.GetHiddenAuthorizationParameters())
      {
        InputDescriptor hiddenDescriptor = authorizationParameter;
        if (!inputDescriptors.Exists((Predicate<InputDescriptor>) (inputDescriptor => inputDescriptor.Id.Equals(hiddenDescriptor.Id, StringComparison.OrdinalIgnoreCase))))
          endpointType.AuthenticationSchemes.FirstOrDefault<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (authenticationScheme => authenticationScheme.Scheme.Equals(endpoint.Authorization.Scheme, StringComparison.OrdinalIgnoreCase)))?.InputDescriptors.Add(hiddenDescriptor);
      }
    }

    private static List<InputDescriptor> GetHiddenAuthorizationParameters()
    {
      if (ServiceEndpointValidator.s_hiddenAuthorizationParameters == null)
      {
        InputDescriptor inputDescriptor1 = new InputDescriptor()
        {
          Id = "AccessToken",
          Name = "i18n:Access Token",
          Description = "The strongbox id where the access token will be stored.",
          InputMode = InputMode.TextArea,
          IsConfidential = true,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.Guid
          }
        };
        InputDescriptor inputDescriptor2 = new InputDescriptor()
        {
          Id = "IssuedAt",
          Name = "i18n:Issued At",
          Description = "Time of the issued token",
          InputMode = InputMode.TextArea,
          IsConfidential = true,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.String
          }
        };
        InputDescriptor inputDescriptor3 = new InputDescriptor()
        {
          Id = "RefreshToken",
          Name = "i18n:Refresh Token",
          Description = "Refresh Token",
          InputMode = InputMode.TextArea,
          IsConfidential = true,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.String
          }
        };
        InputDescriptor inputDescriptor4 = new InputDescriptor()
        {
          Id = "ExpiresIn",
          Name = "i18n:Expires In",
          Description = "Expires In (seconds)",
          InputMode = InputMode.TextArea,
          IsConfidential = true,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.String
          }
        };
        ServiceEndpointValidator.s_hiddenAuthorizationParameters = new List<InputDescriptor>(4)
        {
          inputDescriptor3,
          inputDescriptor1,
          inputDescriptor2,
          inputDescriptor4
        };
      }
      return ServiceEndpointValidator.s_hiddenAuthorizationParameters;
    }
  }
}
