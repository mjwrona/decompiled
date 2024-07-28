// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ConnectedService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.ConnectedServices;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Private)]
  [RequiredClientService("TeamFoundationStrongBoxService", "StrongBox")]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Framework")]
  public class ConnectedService
  {
    private StrongBoxItemInfo m_endpointInfo;
    private StrongBoxItemInfo m_credentialsXmlInfo;
    private StrongBoxItemInfo m_oAuthTokenInfo;
    private static readonly string s_Area = "ConnectedServicesService";
    private static readonly string s_Layer = nameof (ConnectedService);

    protected ConnectedService()
    {
    }

    public ConnectedService(ConnectedServiceMetadata metadata, List<StrongBoxItemInfo> serviceInfo)
    {
      this.ServiceMetadata = new ConnectedServiceMetadata(metadata);
      this.InitializeProperties(serviceInfo);
    }

    public ConnectedService(
      ServiceEndpoint endpoint,
      string teamProject,
      List<StrongBoxItemInfo> serviceInfo)
    {
      this.ServiceMetadata = ConnectedServiceEndpointHelper.ToConnectedServiceMetadata(endpoint, teamProject);
      this.InitializeProperties(serviceInfo);
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public ConnectedServiceMetadata ServiceMetadata { get; set; }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public StrongBoxItemInfo EndpointInfo
    {
      get => this.m_endpointInfo;
      set => this.m_endpointInfo = value;
    }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public StrongBoxItemInfo CredentialsXmlInfo
    {
      get => this.m_credentialsXmlInfo;
      set => this.m_credentialsXmlInfo = value;
    }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public StrongBoxItemInfo OAuthTokenInfo
    {
      get => this.m_oAuthTokenInfo;
      set => this.m_oAuthTokenInfo = value;
    }

    public string GetEndpoint(IVssRequestContext requestContext) => this.FetchStrongBoxValue(requestContext, this.m_endpointInfo);

    public string GetCredentialsXml(IVssRequestContext requestContext) => this.FetchStrongBoxValue(requestContext, this.m_credentialsXmlInfo);

    public string GetOAuthToken(IVssRequestContext requestContext) => this.m_oAuthTokenInfo != null ? this.FetchStrongBoxValue(requestContext, this.m_oAuthTokenInfo) : (string) null;

    public ServiceEndpoint ToServiceEndpoint(IVssRequestContext requestContext)
    {
      int num = this.ServiceMetadata.Kind == ConnectedServiceKind.AzureSubscription ? 1 : 0;
      Guid result1;
      Guid.TryParse(this.ServiceMetadata.Name, out result1);
      Uri result2;
      Uri.TryCreate(this.GetEndpoint(requestContext), UriKind.Absolute, out result2);
      ServiceEndpoint serviceEndpoint = new ServiceEndpoint()
      {
        Id = result1,
        Name = this.ServiceMetadata.FriendlyName,
        Authorization = ConnectedService.GetAuthorization(requestContext, this.GetCredentialsXml(requestContext)),
        CreatedBy = new IdentityRef()
        {
          Id = this.ServiceMetadata.AuthenticatedBy.ToString("D")
        },
        Description = this.ServiceMetadata.Description,
        Type = ConnectedService.MapToServiceEndpointType(this.ServiceMetadata.Kind),
        Url = result2
      };
      if (num != 0)
      {
        serviceEndpoint.Data.Add("SubscriptionId", this.ServiceMetadata.Name);
        serviceEndpoint.Data.Add("SubscriptionName", this.ServiceMetadata.FriendlyName);
        serviceEndpoint.Data.Add("ServiceUri", this.ServiceMetadata.ServiceUri);
        serviceEndpoint.Data.Add("Environment", "AzureCloud");
      }
      return serviceEndpoint;
    }

    public static string MapToServiceEndpointType(ConnectedServiceKind kind)
    {
      switch (kind)
      {
        case ConnectedServiceKind.AzureSubscription:
          return "Azure";
        case ConnectedServiceKind.Chef:
          return "Chef";
        case ConnectedServiceKind.Generic:
          return "Generic";
        case ConnectedServiceKind.GitHub:
          return "GitHub";
        default:
          return (string) null;
      }
    }

    public static EndpointAuthorization GetAuthorization(
      IVssRequestContext requestContext,
      string credentialsXml)
    {
      EndpointAuthorization authorization = (EndpointAuthorization) null;
      ArgumentUtility.CheckStringForNullOrEmpty(credentialsXml, nameof (credentialsXml));
      try
      {
        XElement xelement1 = XElement.Parse(credentialsXml);
        if (xelement1.Name == (XName) "PublishData")
        {
          XElement xelement2 = xelement1.Descendants((XName) "Subscription").FirstOrDefault<XElement>();
          if (xelement2 != null)
          {
            XAttribute xattribute = xelement2.Attributes((XName) "ManagementCertificate").FirstOrDefault<XAttribute>();
            if (xattribute != null)
              authorization = new EndpointAuthorization()
              {
                Scheme = "Certificate",
                Parameters = {
                  {
                    "Certificate",
                    xattribute.Value
                  }
                }
              };
          }
        }
        else if (xelement1.Name == (XName) "Subscription")
        {
          if (xelement1.Attributes((XName) "Id").FirstOrDefault<XAttribute>() != null)
          {
            XElement xelement3 = xelement1.Elements((XName) "Credentials").FirstOrDefault<XElement>();
            if (xelement3 != null)
            {
              XElement xelement4 = xelement3.Elements((XName) "Username").FirstOrDefault<XElement>();
              XElement xelement5 = xelement3.Elements((XName) "Password").FirstOrDefault<XElement>();
              if (xelement4 != null)
              {
                if (xelement5 != null)
                  authorization = new EndpointAuthorization()
                  {
                    Scheme = "UsernamePassword",
                    Parameters = {
                      {
                        "Username",
                        xelement4.Value
                      },
                      {
                        "Password",
                        xelement5.Value
                      }
                    }
                  };
              }
            }
          }
        }
        else if (xelement1.Name == (XName) "Credentials")
        {
          XElement xelement6 = xelement1.Elements((XName) "UserName").FirstOrDefault<XElement>();
          XElement xelement7 = xelement1.Elements((XName) "PasswordKey").FirstOrDefault<XElement>();
          if (xelement6 != null)
          {
            if (xelement7 != null)
              authorization = new EndpointAuthorization()
              {
                Scheme = "UsernamePassword",
                Parameters = {
                  {
                    "Username",
                    xelement6.Value
                  },
                  {
                    "Password",
                    xelement7.Value
                  }
                }
              };
          }
        }
        else if (xelement1.Name == (XName) "GitHubAuthorization")
        {
          XElement xelement8 = xelement1.Element((XName) "TokenType");
          XElement xelement9 = xelement1.Element((XName) "Token");
          if (xelement8 != null)
          {
            if (xelement9 != null)
            {
              if (xelement8.Value.Equals("OAUTH"))
                authorization = new EndpointAuthorization()
                {
                  Scheme = "OAuth",
                  Parameters = {
                    {
                      "AccessToken",
                      xelement9.Value
                    }
                  }
                };
              else if (xelement8.Value.Equals("Personal Access Token"))
                authorization = new EndpointAuthorization()
                {
                  Scheme = "PersonalAccessToken",
                  Parameters = {
                    {
                      "AccessToken",
                      xelement9.Value
                    }
                  }
                };
            }
          }
        }
      }
      catch (XmlException ex)
      {
        requestContext.TraceException(30253, ConnectedService.s_Area, ConnectedService.s_Layer, (Exception) ex);
      }
      return authorization;
    }

    private void InitializeProperties(List<StrongBoxItemInfo> propsList)
    {
      foreach (StrongBoxItemInfo props in propsList)
      {
        if (this.m_endpointInfo == null && string.Compare(props.LookupKey, "Endpoint", StringComparison.OrdinalIgnoreCase) == 0)
          this.m_endpointInfo = props;
        else if (this.m_credentialsXmlInfo == null && string.Compare(props.LookupKey, "Credentials", StringComparison.OrdinalIgnoreCase) == 0)
          this.m_credentialsXmlInfo = props;
        else if (this.m_oAuthTokenInfo == null && string.Compare(props.LookupKey, "OAuthToken", StringComparison.OrdinalIgnoreCase) == 0)
          this.m_oAuthTokenInfo = props;
      }
      ArgumentUtility.CheckForNull<StrongBoxItemInfo>(this.m_endpointInfo, "m_endpointInfo");
      ArgumentUtility.CheckForNull<StrongBoxItemInfo>(this.m_credentialsXmlInfo, "m_credentialsXmlInfo");
    }

    private string FetchStrongBoxValue(IVssRequestContext requestContext, StrongBoxItemInfo info)
    {
      requestContext.TraceEnter(30250, ConnectedService.s_Area, ConnectedService.s_Layer, nameof (FetchStrongBoxValue));
      try
      {
        return requestContext.GetService<TeamFoundationStrongBoxService>().GetString(requestContext, info.DrawerId, info.LookupKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30251, ConnectedService.s_Area, ConnectedService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(30252, ConnectedService.s_Area, ConnectedService.s_Layer, nameof (FetchStrongBoxValue));
      }
    }
  }
}
