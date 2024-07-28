// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  public static class KubernetesHelper
  {
    public static ServiceEndpointDetails GetAzureRmEndpointDetails(
      Dictionary<string, string> endpointData)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string str1;
      if (!endpointData.TryGetValue("azureSubscriptionId", out str1) || string.IsNullOrWhiteSpace(str1))
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesCannotFetchClusterCredentialsMissingSubscriptionId());
      string str2;
      if (!endpointData.TryGetValue("azureSubscriptionName", out str2) || string.IsNullOrWhiteSpace(str2))
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesCannotFetchClusterCredentialsMissingSubscriptionName());
      string str3;
      if (!endpointData.TryGetValue("azureEnvironment", out str3) || string.IsNullOrWhiteSpace(str3))
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesCannotFetchClusterCredentialsMissingEnvironment());
      string str4;
      if (!endpointData.TryGetValue("azureTenantId", out str4) || string.IsNullOrWhiteSpace(str4))
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesCannotFetchClusterCredentialsMissingTenantId());
      string str5;
      if (!endpointData.TryGetValue("spnCreationMethod", out str5) || string.IsNullOrWhiteSpace(str5))
        str5 = "Automatic";
      string uriString;
      if (!endpointData.TryGetValue("cloudUrl", out uriString) || string.IsNullOrWhiteSpace(uriString))
        uriString = "https://management.azure.com";
      if (str5.Equals("Manual"))
      {
        if (!endpointData.TryGetValue("spnId", out empty1) || string.IsNullOrWhiteSpace(empty1))
          throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesCannotFetchClusterCredentialsMissingAzureSpnId());
        if (!endpointData.TryGetValue("spnKey", out empty2) || string.IsNullOrWhiteSpace(empty2))
          throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesCannotFetchClusterCredentialsMissingAzureSpnKey());
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "subscriptionId",
          str1
        },
        {
          "subscriptionName",
          str2
        },
        {
          "environment",
          str3
        },
        {
          "creationMode",
          str5
        }
      };
      EndpointAuthorization endpointAuthorization;
      if (str5.Equals("Manual"))
        endpointAuthorization = new EndpointAuthorization()
        {
          Scheme = "ServicePrincipal",
          Parameters = {
            {
              "TenantId",
              str4
            },
            {
              "ServicePrincipalId",
              empty1
            },
            {
              "ServicePrincipalKey",
              empty2
            }
          }
        };
      else
        endpointAuthorization = new EndpointAuthorization()
        {
          Scheme = "ServicePrincipal",
          Parameters = {
            {
              "TenantId",
              str4
            },
            {
              "AccessTokenType",
              "AppToken"
            }
          }
        };
      string str6;
      if (endpointData.TryGetValue("azureAccessToken", out str6) && !string.IsNullOrEmpty(str6))
      {
        string str7;
        if (endpointData.TryGetValue("AccessTokenFetchingMethod", out str7) && !string.IsNullOrEmpty(str7))
        {
          endpointAuthorization.Parameters.Add("AccessTokenFetchingMethod", str7);
          endpointAuthorization.Parameters.Add("AccessToken", str6);
        }
        else
        {
          endpointAuthorization.Parameters.Add("AccessToken", str6);
          endpointAuthorization.Parameters.Add("AccessTokenFetchingMethod", "Oauth");
        }
      }
      return new ServiceEndpointDetails()
      {
        Type = "AzureRM",
        Url = new Uri(uriString),
        Authorization = endpointAuthorization,
        Data = (IDictionary<string, string>) dictionary
      };
    }

    public static void FetchCloudUrl(
      ServiceEndpointType endpointType,
      Dictionary<string, string> endpointData)
    {
      string str = "https://management.azure.com";
      if (endpointType != null && endpointType.EndpointUrl != null && endpointType.EndpointUrl.DependsOn != null)
      {
        List<DependencyBinding> map = endpointType.EndpointUrl.DependsOn.Map;
        string empty = string.Empty;
        endpointData.TryGetValue("azureEnvironment", out empty);
        foreach (DependencyBinding dependencyBinding in map)
        {
          if (dependencyBinding.Key.Equals(empty, StringComparison.OrdinalIgnoreCase))
          {
            str = dependencyBinding.Value.ToString();
            break;
          }
        }
      }
      endpointData["cloudUrl"] = str;
    }
  }
}
