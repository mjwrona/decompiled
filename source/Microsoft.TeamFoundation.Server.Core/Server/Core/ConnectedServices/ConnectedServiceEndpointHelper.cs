// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ConnectedServices.ConnectedServiceEndpointHelper
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core.ConnectedServices
{
  public class ConnectedServiceEndpointHelper
  {
    public static readonly Uri ServiceManagementDefaultAzureUri = new Uri("https://management.core.windows.net");
    private const string AzureCertXmlFormat = "<?xml version='1.0' encoding='utf-8'?>\r\n                <PublishData>\r\n                    <PublishProfile\r\n                    SchemaVersion='2.0'\r\n                    PublishMethod='AzureServiceManagementAPI'>\r\n                    <Subscription\r\n                        ServiceManagementUrl='{0}'\r\n                        Id='{1}'\r\n                        Name='{2}'\r\n                        ManagementCertificate='{3}' />\r\n                    </PublishProfile>\r\n                </PublishData>";
    private const string AzureCredentialsXmlFormat = "<?xml version = '1.0' encoding='utf-8'?>\r\n                    <Subscription Id = '{0}' Name='{1}'>\r\n                            <Credentials>\r\n                                <Username>{2}</Username>\r\n                                <Password>{3}</Password>\r\n                            </Credentials>\r\n                    </Subscription>";
    private const string ChefOrGenericCredentialsXmlFormat = "<Credentials>\r\n                            <UserName>{0}</UserName>\r\n                            <PasswordKey>{1}</PasswordKey>\r\n                      </Credentials>";
    private const string GitHubCredentialsXmlFormat = "<GitHubAuthorization>\r\n                          <TokenType>{0}</TokenType>\r\n                          <Token>{1}</Token>\r\n                    </GitHubAuthorization>";

    public static ConnectedServiceKind ConvertToConnectedServiceKind(string endpointType)
    {
      ConnectedServiceKind connectedServiceKind = ConnectedServiceKind.Custom;
      if (endpointType.Equals("Azure", StringComparison.InvariantCultureIgnoreCase))
        connectedServiceKind = ConnectedServiceKind.AzureSubscription;
      else if (endpointType.Equals("Chef", StringComparison.InvariantCultureIgnoreCase))
        connectedServiceKind = ConnectedServiceKind.Chef;
      else if (endpointType.Equals("Generic", StringComparison.InvariantCultureIgnoreCase))
        connectedServiceKind = ConnectedServiceKind.Generic;
      else if (endpointType.Equals("GitHub", StringComparison.InvariantCultureIgnoreCase))
        connectedServiceKind = ConnectedServiceKind.GitHub;
      return connectedServiceKind;
    }

    public static string GenerateCredentialsXmlFromServiceEndpoint(ServiceEndpoint endpoint)
    {
      string fromServiceEndpoint = string.Empty;
      if (endpoint.Type.Equals("Azure", StringComparison.InvariantCultureIgnoreCase))
      {
        if (endpoint.Authorization.Scheme.Equals("Certificate", StringComparison.InvariantCultureIgnoreCase))
        {
          string str1;
          endpoint.Data.TryGetValue("SubscriptionId", out str1);
          string str2;
          endpoint.Authorization.Parameters.TryGetValue("Certificate", out str2);
          if (!string.IsNullOrEmpty(str1?.Trim()) && !string.IsNullOrEmpty(str2?.Trim()))
            fromServiceEndpoint = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<?xml version='1.0' encoding='utf-8'?>\r\n                <PublishData>\r\n                    <PublishProfile\r\n                    SchemaVersion='2.0'\r\n                    PublishMethod='AzureServiceManagementAPI'>\r\n                    <Subscription\r\n                        ServiceManagementUrl='{0}'\r\n                        Id='{1}'\r\n                        Name='{2}'\r\n                        ManagementCertificate='{3}' />\r\n                    </PublishProfile>\r\n                </PublishData>", (object) ConnectedServiceEndpointHelper.ServiceManagementDefaultAzureUri, (object) str1, (object) HttpUtility.HtmlEncode(endpoint.Name), (object) str2);
        }
        else if (endpoint.Authorization.Scheme.Equals("UsernamePassword", StringComparison.InvariantCultureIgnoreCase))
          fromServiceEndpoint = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<?xml version = '1.0' encoding='utf-8'?>\r\n                    <Subscription Id = '{0}' Name='{1}'>\r\n                            <Credentials>\r\n                                <Username>{2}</Username>\r\n                                <Password>{3}</Password>\r\n                            </Credentials>\r\n                    </Subscription>", (object) endpoint.Data["SubscriptionId"], (object) HttpUtility.HtmlEncode(endpoint.Name), (object) HttpUtility.HtmlEncode(endpoint.Authorization.Parameters["Username"]), (object) HttpUtility.HtmlEncode(endpoint.Authorization.Parameters["Password"]));
      }
      else if ((endpoint.Type.Equals("Chef", StringComparison.InvariantCultureIgnoreCase) || endpoint.Type.Equals("Generic", StringComparison.InvariantCultureIgnoreCase)) && endpoint.Authorization.Scheme.Equals("UsernamePassword", StringComparison.InvariantCultureIgnoreCase))
        fromServiceEndpoint = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<Credentials>\r\n                            <UserName>{0}</UserName>\r\n                            <PasswordKey>{1}</PasswordKey>\r\n                      </Credentials>", (object) endpoint.Authorization.Parameters["Username"], (object) endpoint.Authorization.Parameters["Password"]);
      else if (endpoint.Type.Equals("GitHub", StringComparison.InvariantCultureIgnoreCase))
        fromServiceEndpoint = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<GitHubAuthorization>\r\n                          <TokenType>{0}</TokenType>\r\n                          <Token>{1}</Token>\r\n                    </GitHubAuthorization>", endpoint.Authorization.Scheme == "OAuth" ? (object) "OAUTH" : (object) "Personal Access Token", (object) endpoint.Authorization.Parameters["AccessToken"]);
      return fromServiceEndpoint;
    }

    public static ConnectedServiceMetadata ToConnectedServiceMetadata(
      ServiceEndpoint endpoint,
      string teamProject)
    {
      string str = endpoint.Data.ContainsKey("ServiceUri") ? endpoint.Data["ServiceUri"] : string.Empty;
      return new ConnectedServiceMetadata()
      {
        Name = endpoint.Id.ToString("D"),
        FriendlyName = endpoint.Name,
        Kind = ConnectedServiceEndpointHelper.ConvertToConnectedServiceKind(endpoint.Type),
        Description = endpoint.Description,
        TeamProject = teamProject,
        ServiceUri = str,
        AuthenticatedBy = Guid.Parse(endpoint.CreatedBy.Id)
      };
    }
  }
}
