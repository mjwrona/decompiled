// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.Utilities.GitHub.Boards.GitHubBoardsServiceEndpointHelper
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.Utilities, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6309B6D0-0EEE-4299-AA79-F0B62882E0B1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.Utilities.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.ExternalIntegration.Utilities.GitHub.Boards
{
  public class GitHubBoardsServiceEndpointHelper
  {
    public static bool TryFindOrCreateGitHubAppServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      GitHubData.V3.InstallationDetails installationDetails,
      out ServiceEndpoint serviceEndpoint)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      serviceEndpoint = (ServiceEndpoint) null;
      if (installationDetails == null)
        return false;
      string installationId = installationDetails.Id;
      string login = installationDetails.Account?.Login;
      string avatarUrl = installationDetails.Account?.Avatar_url;
      string nodeId = installationDetails.Account?.Node_id;
      if (string.IsNullOrEmpty(login))
        return false;
      IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
      IServiceEndpointService2 endpointService2 = service;
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid scopeIdentifier = projectId;
      List<string> authSchemes = new List<string>();
      authSchemes.Add("InstallationToken");
      string boards = ServiceEndpointOwner.Boards;
      List<ServiceEndpoint> serviceEndpointList = endpointService2.QueryServiceEndpoints(requestContext1, scopeIdentifier, "GitHub", (IEnumerable<string>) authSchemes, (IEnumerable<Guid>) null, boards, false, true);
      GitHubBoardsServiceEndpointHelper.InvalidateServiceEndpointForSameOrgWithDifferentInstallationId(requestContext, (IEnumerable<ServiceEndpoint>) serviceEndpointList, projectId, installationId, nodeId);
      string endpointInstallationId = (string) null;
      string a;
      ServiceEndpoint serviceEndpoint1 = serviceEndpointList.FirstOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (endpoint => endpoint.Authorization?.Parameters?.TryGetValue("IdToken", out endpointInstallationId).GetValueOrDefault() && endpointInstallationId == installationId && endpoint.Data.TryGetValue("IsValid", out a) && string.Equals(a, "true", StringComparison.OrdinalIgnoreCase)));
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "IdToken",
          installationId
        }
      };
      if (serviceEndpoint1 != null)
      {
        IEnumerable<ServiceEndpointType> serviceEndpointTypes = requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(requestContext, serviceEndpoint1.Type, (string) null);
        serviceEndpoint1.ClearConfidentialAuthorizationParameters(serviceEndpointTypes.First<ServiceEndpointType>());
        GitHubBoardsServiceEndpointHelper.AddOrUpdateAuthorizationParameters(serviceEndpoint1, (IDictionary<string, string>) parameters);
        serviceEndpoint = serviceEndpoint1;
      }
      else
      {
        ServiceEndpoint endpoint = new ServiceEndpoint()
        {
          Id = Guid.Empty,
          Type = "GitHub",
          Owner = ServiceEndpointOwner.Boards,
          Name = Guid.NewGuid().ToString(),
          Url = new GitHubRoot().Uri,
          Authorization = GitHubBoardsServiceEndpointHelper.GenerateInstallationTokenAuthorization(requestContext, installationId),
          Data = {
            {
              "IsValid",
              "true"
            },
            {
              "GitHubHandle",
              login
            },
            {
              "orgIntId",
              installationDetails?.Account?.Id
            }
          }
        };
        if (!string.IsNullOrWhiteSpace(avatarUrl))
          endpoint.Data.Add("AvatarUrl", avatarUrl);
        if (!string.IsNullOrWhiteSpace(nodeId))
          endpoint.Data.Add("OrgNodeId", nodeId);
        try
        {
          serviceEndpoint = service.CreateServiceEndpoint(requestContext.Elevate(), projectId, endpoint);
        }
        catch (Exception ex)
        {
          requestContext.Trace(919900, TraceLevel.Error, "BoardsGitHub", nameof (TryFindOrCreateGitHubAppServiceEndpoint), ex.Message);
          return false;
        }
        GitHubBoardsServiceEndpointHelper.AddOrUpdateAuthorizationParameters(serviceEndpoint, (IDictionary<string, string>) parameters);
      }
      return true;
    }

    public static EndpointAuthorization GenerateInstallationTokenAuthorization(
      IVssRequestContext requestContext,
      string installationId)
    {
      return new EndpointAuthorization()
      {
        Scheme = "InstallationToken",
        Parameters = {
          {
            "IdToken",
            installationId
          },
          {
            "IdSignature",
            GitHubSecretManagementHelper.GetJwtSignature(requestContext, installationId)
          }
        }
      };
    }

    private static void AddOrUpdateAuthorizationParameters(
      ServiceEndpoint serviceEndpoint,
      IDictionary<string, string> parameters)
    {
      if (serviceEndpoint.Authorization.Parameters == null)
        serviceEndpoint.Authorization.Parameters = (IDictionary<string, string>) new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) parameters)
        serviceEndpoint.Authorization.Parameters[parameter.Key] = parameter.Value;
    }

    private static void InvalidateServiceEndpointForSameOrgWithDifferentInstallationId(
      IVssRequestContext requestContext,
      IEnumerable<ServiceEndpoint> boardsGitHubEndpoints,
      Guid projectId,
      string installationId,
      string orgNodeId)
    {
      string endpointInstallationId = (string) null;
      string str1;
      string str2;
      List<ServiceEndpoint> list = boardsGitHubEndpoints.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (endpoint => endpoint.Authorization?.Parameters?.TryGetValue("IdToken", out endpointInstallationId).GetValueOrDefault() && endpointInstallationId != installationId && endpoint.Data.TryGetValue("OrgNodeId", out str1) && str1 == orgNodeId && endpoint.Data.TryGetValue("IsValid", out str2) && str2 == "true")).ToList<ServiceEndpoint>();
      if (!list.Any<ServiceEndpoint>())
        return;
      IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
      list.ForEach((Action<ServiceEndpoint>) (x => x.Data["IsValid"] = "false"));
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid scopeIdentifier = projectId;
      List<ServiceEndpoint> endpoints = list;
      service.UpdateServiceEndpoints(requestContext1, scopeIdentifier, (IEnumerable<ServiceEndpoint>) endpoints);
    }
  }
}
