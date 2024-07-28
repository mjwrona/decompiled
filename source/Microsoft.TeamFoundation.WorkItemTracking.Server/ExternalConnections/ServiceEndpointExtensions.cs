// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ServiceEndpointExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public static class ServiceEndpointExtensions
  {
    public static string GetGitHubHandle(this ServiceEndpoint endpoint)
    {
      if (endpoint == null)
        return (string) null;
      IDictionary<string, string> data = endpoint.Data;
      return data == null ? (string) null : data.GetValueOrDefault<string, string>("GitHubHandle");
    }

    public static string GetOrgNodeId(this ServiceEndpoint endpoint)
    {
      if (endpoint == null)
        return (string) null;
      IDictionary<string, string> data = endpoint.Data;
      return data == null ? (string) null : data.GetValueOrDefault<string, string>("OrgNodeId");
    }

    public static string GetInstallationId(this ServiceEndpoint endpoint)
    {
      if (endpoint == null)
        return (string) null;
      EndpointAuthorization authorization = endpoint.Authorization;
      if (authorization == null)
        return (string) null;
      IDictionary<string, string> parameters = authorization.Parameters;
      return parameters == null ? (string) null : parameters.GetValueOrDefault<string, string>("IdToken");
    }

    public static bool IsOauthConfigurationDeleted(this ServiceEndpoint endpoint)
    {
      string str = (string) null;
      if (endpoint != null && !endpoint.IsReady)
      {
        JObject operationStatus = endpoint.OperationStatus;
        if ((operationStatus != null ? (operationStatus.TryGetValue<string>("state", out str) ? 1 : 0) : 0) != 0 && str == "OAuthConfigurationDeleted")
          return true;
      }
      return false;
    }
  }
}
