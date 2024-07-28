// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.S2SManagementService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class S2SManagementService : IVssFrameworkService
  {
    private GraphConnection m_graphConnection;
    private static readonly string s_area = "RoutingService";
    private static readonly string s_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckHostedDeployment();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<ServicePrincipal> GetServicePrincipals(
      IVssRequestContext requestContext,
      ITFLogger tfLogger,
      bool includeTest = false)
    {
      if (requestContext.IsFeatureEnabled(AadServiceConstants.FeatureFlags.EnableMicrosoftGraphByOperation("S2SManagementServiceGetServicePrincipals")))
      {
        tfLogger.Info("Calling GetServicePrincipals from MsGraph");
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return (IList<ServicePrincipal>) this.GetGraphServicePrincipalsWithMicrosoftGraph(requestContext, tfLogger, includeTest).Select<AadServicePrincipal, ServicePrincipal>(S2SManagementService.\u003C\u003EO.\u003C0\u003E__ConvertGraphPrincipal ?? (S2SManagementService.\u003C\u003EO.\u003C0\u003E__ConvertGraphPrincipal = new Func<AadServicePrincipal, ServicePrincipal>(S2SManagementService.ConvertGraphPrincipal))).ToList<ServicePrincipal>();
      }
      tfLogger.Info("Calling GetServicePrincipals from AADGraph");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IList<ServicePrincipal>) this.GetGraphServicePrincipals(requestContext, tfLogger, includeTest).Select<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal, ServicePrincipal>(S2SManagementService.\u003C\u003EO.\u003C1\u003E__ConvertGraphPrincipal ?? (S2SManagementService.\u003C\u003EO.\u003C1\u003E__ConvertGraphPrincipal = new Func<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal, ServicePrincipal>(S2SManagementService.ConvertGraphPrincipal))).ToList<ServicePrincipal>();
    }

    private IList<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal> GetGraphServicePrincipals(
      IVssRequestContext requestContext,
      ITFLogger tfLogger,
      bool includeTest)
    {
      return (IList<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>) this.InvokeGraph<List<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>>(requestContext, tfLogger, (Func<GraphConnection, List<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>>) (connection =>
      {
        List<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal> servicePrincipals = new List<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>();
        string pagingToken = (string) null;
        bool flag = false;
        FilterGenerator filter = new FilterGenerator()
        {
          Top = 200
        };
        PagedResults<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal> pagedResults;
        for (; !flag; flag = pagedResults.IsLastPage)
        {
          pagedResults = new RetryManager(3, TimeSpan.FromSeconds(2.0)).InvokeFunc<PagedResults<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>>((Func<PagedResults<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>>) (() => connection.List<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>(pagingToken, filter)));
          foreach (Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal result1 in (IEnumerable<Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal>) pagedResults.Results)
          {
            Guid result2;
            if (Guid.TryParse(result1.AppId, out result2) && S2SManagementService.IsVssServicePrincipal(result2, includeTest))
              servicePrincipals.Add(result1);
          }
          pagingToken = pagedResults.PageToken;
        }
        return servicePrincipals;
      }));
    }

    private IList<AadServicePrincipal> GetGraphServicePrincipalsWithMicrosoftGraph(
      IVssRequestContext requestContext,
      ITFLogger tfLogger,
      bool includeTest)
    {
      SharedAadService service = requestContext.GetService<SharedAadService>();
      IS2SAuthSettings s2SauthSettings = requestContext.GetService<IOAuth2SettingsService>().GetS2SAuthSettings(requestContext);
      List<AadServicePrincipal> withMicrosoftGraph = new List<AadServicePrincipal>();
      string str = (string) null;
      try
      {
        GetServicePrincipalsRequest principalsRequest = new GetServicePrincipalsRequest();
        principalsRequest.MaxResults = new int?(200);
        principalsRequest.PagingToken = str;
        principalsRequest.ToTenant = s2SauthSettings.TenantId;
        principalsRequest.AccessToken = this.GetAccessToken(requestContext, true);
        GetServicePrincipalsRequest request = principalsRequest;
        request.MicrosoftGraphBaseUrlOverride = "https://graph.microsoft.com/v1.0";
        GetServicePrincipalsResponse servicePrincipals;
        do
        {
          request.PagingToken = str;
          servicePrincipals = service.GetServicePrincipals(requestContext, request);
          foreach (AadServicePrincipal servicePrincipal in servicePrincipals.ServicePrincipals)
          {
            if (S2SManagementService.IsVssServicePrincipal(servicePrincipal.AppId, includeTest))
              withMicrosoftGraph.Add(servicePrincipal);
          }
          str = servicePrincipals.PagingToken;
        }
        while (servicePrincipals.HasMoreResults);
      }
      catch (Exception ex)
      {
        requestContext.Trace(12345, TraceLevel.Error, S2SManagementService.s_area, S2SManagementService.s_layer, "Handling error in 'GetGraphServicePrincipalsWithMicrosoftGraph'", (object) ex);
        tfLogger.Error(string.Format("MsGraph. Unexpected exception in graph operation. Correlation ID: {0}\r\nException: {1}\r\nStack Trace: {2}", (object) requestContext.ActivityId, (object) ex.Message, (object) ex.StackTrace));
        throw;
      }
      return (IList<AadServicePrincipal>) withMicrosoftGraph;
    }

    private static bool IsVssServicePrincipal(Guid principalId, bool includeTest)
    {
      if (!ServicePrincipals.IsInternalServicePrincipalId(principalId))
        return false;
      return !S2SManagementService.HasServicePrincipalTestFlag(principalId) || includeTest;
    }

    private static ServicePrincipal ConvertGraphPrincipal(Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal principal) => new ServicePrincipal()
    {
      Id = Guid.Parse(principal.AppId),
      DisplayName = principal.DisplayName
    };

    private static ServicePrincipal ConvertGraphPrincipal(AadServicePrincipal principal) => new ServicePrincipal()
    {
      Id = principal.AppId,
      DisplayName = principal.DisplayName
    };

    private T InvokeGraph<T>(
      IVssRequestContext requestContext,
      ITFLogger tfLogger,
      Func<GraphConnection, T> action)
    {
      GraphConnection graphConnection = this.GetGraphConnection(requestContext);
      int num = 3;
      while (true)
      {
        try
        {
          return action(graphConnection);
        }
        catch (GraphException ex) when (num-- > 0)
        {
          if (ex is ExpiredTokenException)
          {
            requestContext.Trace(12345, TraceLevel.Error, S2SManagementService.s_area, S2SManagementService.s_layer, "Handling expired token exception.");
            graphConnection = this.GetGraphConnection(requestContext, true);
          }
          else if (ex.ErrorMessage != null && ex.ErrorMessage.Contains("The operation has timed out"))
            requestContext.Trace(12345, TraceLevel.Error, S2SManagementService.s_area, S2SManagementService.s_layer, "Handling request timeout exception.", (object) ex);
          else if (ex.ErrorMessage != null && ex.ErrorMessage.Contains("The underlying connection was closed"))
          {
            requestContext.Trace(12345, TraceLevel.Error, S2SManagementService.s_area, S2SManagementService.s_layer, "Handling connection closed exception.", (object) ex);
          }
          else
          {
            Dictionary<string, string> source = new Dictionary<string, string>()
            {
              {
                "AadCorrelationId",
                requestContext.ActivityId.ToString()
              },
              {
                "HttpStatusCode",
                ex.HttpStatusCode.ToString()
              },
              {
                "Code",
                ex.Code
              },
              {
                "ErrorMessage",
                ex.ErrorMessage
              },
              {
                "ResponseHeaders",
                ex.ResponseHeaders?.ToString()
              },
              {
                "ExtendedErrors",
                ex.ExtendedErrors != null ? string.Join(";", ex.ExtendedErrors.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key + "=" + kvp.Value))) : (string) null
              },
              {
                "ErrorResponse",
                ex.ErrorResponse?.Error?.Message?.MessageValue
              },
              {
                "ResponseUri",
                ex.ResponseUri
              }
            };
            requestContext.TraceAlways(12345, TraceLevel.Error, S2SManagementService.s_area, S2SManagementService.s_layer, "Received Graph exception:\r\n" + string.Join("\r\n", source.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key + "=" + kvp.Value))), (object) ex);
            tfLogger.Error("AADGraph. Received Graph exception:\r\n" + string.Join("\r\n", source.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key + "=" + kvp.Value))) + "\r\nStack Trace: " + ex.StackTrace);
            throw;
          }
        }
        catch (Exception ex)
        {
          requestContext.Trace(12345, TraceLevel.Error, S2SManagementService.s_area, S2SManagementService.s_layer, string.Format("Unexpected exception in graph operation. AAD Correlation ID: {0}", (object) requestContext.ActivityId), (object) ex);
          tfLogger.Error(string.Format("AADGraph. Unexpected exception in graph operation. AAD Correlation ID: {0}\r\nException: {1}\r\nStack Trace: {2}", (object) requestContext.ActivityId, (object) ex.Message, (object) ex.StackTrace));
          throw;
        }
      }
    }

    private GraphConnection GetGraphConnection(IVssRequestContext requestContext, bool forceCreate = false)
    {
      if (this.m_graphConnection == null | forceCreate)
        this.m_graphConnection = new GraphConnection(this.GetAccessToken(requestContext, false), requestContext.ActivityId, new GraphSettings()
        {
          ApiVersion = "1.5",
          GraphDomainName = "graph.windows.net"
        });
      else
        this.m_graphConnection.ClientRequestId = requestContext.ActivityId;
      return this.m_graphConnection;
    }

    private string GetAccessToken(IVssRequestContext requestContext, bool isMicrosoftGraph)
    {
      string str1 = !isMicrosoftGraph ? "https://login.windows.net/" : "https://login.microsoftonline.com/";
      string str2 = !isMicrosoftGraph ? "https://graph.windows.net" : "https://graph.microsoft.com";
      IS2SAuthSettings s2SauthSettings = requestContext.GetService<IOAuth2SettingsService>().GetS2SAuthSettings(requestContext);
      X509Certificate2 signingCertificate = s2SauthSettings.GetSigningCertificate(requestContext);
      IConfidentialClientApplication clientApplication = ConfidentialClientApplicationBuilder.Create(s2SauthSettings.PrimaryServicePrincipal.ToString("D")).WithAuthority(str1 + s2SauthSettings.TenantDomain).WithCertificate(signingCertificate).Build();
      int num = 2;
      while (true)
      {
        try
        {
          return clientApplication.AcquireTokenForClient((IEnumerable<string>) new string[1]
          {
            str2 + "/.default"
          }).ExecuteAsync().SyncResultConfigured<AuthenticationResult>().AccessToken;
        }
        catch (HttpRequestException ex) when (num-- > 0)
        {
          requestContext.Trace(12345, TraceLevel.Error, S2SManagementService.s_area, S2SManagementService.s_layer, "Handling http request exception during acquiring access token.");
        }
      }
    }

    private static unsafe bool HasServicePrincipalTestFlag(Guid servicePrincipal) => ((int) *(uint*) &servicePrincipal & -268435456) == -268435456;
  }
}
