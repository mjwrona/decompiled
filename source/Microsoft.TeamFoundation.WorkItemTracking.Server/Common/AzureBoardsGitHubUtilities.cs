// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.AzureBoardsGitHubUtilities
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Threading;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class AzureBoardsGitHubUtilities
  {
    private static readonly GitHubRateLimitTraceOptions s_gitHubRateLimitTracerOptions = new GitHubRateLimitTraceOptions(30, 3, true);

    public static GitHubHttpClient CreateGitHubHttpClient(
      IVssRequestContext requestContext,
      string projectId = null,
      string endpointId = null)
    {
      IGitHubAppAccessTokenProvider accessTokenProvider = GitHubHttpClientFactory.GetAppAccessTokenProvider(requestContext, GitHubAppType.Boards, projectId, endpointId);
      IVssRequestContext requestContext1 = requestContext;
      IGitHubAppAccessTokenProvider tokenProvider = accessTokenProvider;
      GitHubRateLimitTraceOptions limitTracerOptions = AzureBoardsGitHubUtilities.s_gitHubRateLimitTracerOptions;
      TimeSpan? timeout = new TimeSpan?();
      GitHubRateLimitTraceOptions rateLimitTraceOptions = limitTracerOptions;
      CancellationToken? cancellationToken = new CancellationToken?();
      return GitHubHttpClientFactory.Create(requestContext1, tokenProvider, timeout, rateLimitTraceOptions, cancellationToken);
    }

    public static bool IsAzureBoardsGitHubType(this ServiceEndpoint serviceEndpoint)
    {
      if (VssStringComparer.ServiceEndpointTypeCompararer.Equals("GitHubBoards", serviceEndpoint?.Type))
        return true;
      return VssStringComparer.ServiceEndpointTypeCompararer.Equals("GitHub", serviceEndpoint?.Type) && VssStringComparer.ServiceEndpointTypeCompararer.Equals(ServiceEndpointOwner.Boards, serviceEndpoint?.Owner);
    }

    public static bool IsAzureBoardsGitHubEnterpriseType(this ServiceEndpoint serviceEndpoint) => VssStringComparer.ServiceEndpointTypeCompararer.Equals("GitHubEnterpriseBoards", serviceEndpoint?.Type);

    public static bool IsAzureBoardsGitHubFamilyType(this ServiceEndpoint serviceEndpoint) => serviceEndpoint.IsAzureBoardsGitHubType() || serviceEndpoint.IsAzureBoardsGitHubEnterpriseType();

    public static bool IsGitHubApp(this ServiceEndpoint endpoint) => endpoint.IsAzureBoardsGitHubFamilyType() && endpoint?.Authorization.Scheme == "InstallationToken";

    public static string GetEnterpriseUrl(this ServiceEndpoint serviceEndpoint) => !serviceEndpoint.IsAzureBoardsGitHubEnterpriseType() ? (string) null : serviceEndpoint.Url.ToString();

    public static string GetProviderKey(this ServiceEndpoint serviceEndpoint) => !serviceEndpoint.IsAzureBoardsGitHubType() ? serviceEndpoint.Url.Host : "github.com";
  }
}
