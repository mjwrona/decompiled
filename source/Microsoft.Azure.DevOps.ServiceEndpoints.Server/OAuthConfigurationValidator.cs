// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.OAuthConfigurationValidator
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class OAuthConfigurationValidator
  {
    public static void ValidateConfiguration(
      IVssRequestContext requestContext,
      OAuthConfigurationParams configurationParams)
    {
      OAuthConfigurationValidator.ValidateConfigurationName(configurationParams.Name);
      OAuthConfigurationValidator.ValidateConfigurationDetails(configurationParams.ClientId, configurationParams.ClientSecret);
      OAuthConfigurationValidator.ValidateConfigurationEndpointType(requestContext, configurationParams.EndpointType);
      Uri result;
      if (!Uri.TryCreate(configurationParams.Url?.OriginalString?.Trim(), UriKind.Absolute, out result))
        throw new ArgumentException(ServiceEndpointResources.InvalidUrl((object) configurationParams.Url?.OriginalString), "configuration.Url");
      configurationParams.Url = result;
    }

    private static void ValidateConfigurationName(string configurationName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(configurationName, "configuration.Name");
      ArgumentUtility.CheckStringLength(configurationName, "configuration.Name", 512);
    }

    private static void ValidateConfigurationEndpointType(
      IVssRequestContext requestContext,
      string endpointType)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(endpointType, "configuration.EndpointType");
      IServiceEndpointTypesService2 service = requestContext.GetService<IServiceEndpointTypesService2>();
      string str = endpointType == "Jira" ? "JiraConnectApp" : "OAuth2";
      IVssRequestContext requestContext1 = requestContext;
      string type = endpointType;
      string scheme = str;
      ServiceEndpointType serviceEndpointType = service.GetServiceEndpointTypes(requestContext1, type, scheme).FirstOrDefault<ServiceEndpointType>();
      if (serviceEndpointType == null || !serviceEndpointType.Name.Equals(endpointType, StringComparison.OrdinalIgnoreCase))
        throw new OAuthConfigurationException(ServiceEndpointResources.OAuth2NotSupportedForEndpointType((object) endpointType));
    }

    public static void ValidateConfigurationDetails(string clientId, string clientSecret)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(clientId, "configuration.ClientId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(clientSecret, "configuration.ClientSecret");
      ArgumentUtility.CheckStringLength(clientId, "configuration.ClientId", 128);
      ArgumentUtility.CheckStringLength(clientSecret, "configuration.ClientSecret", 128);
    }

    public static void ValidateConfigurationForUpdate(
      IVssRequestContext requestContext,
      OAuthConfigurationParams configurationParams)
    {
      if (configurationParams.Url != (Uri) null || configurationParams.ClientId != null || configurationParams.ClientSecret != null)
        throw new OAuthConfigurationException(ServiceEndpointResources.OAuthConfigurationUrlClientDetailsUpdateForbidden());
    }
  }
}
