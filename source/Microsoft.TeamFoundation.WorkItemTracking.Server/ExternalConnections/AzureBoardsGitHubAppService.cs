// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.AzureBoardsGitHubAppService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class AzureBoardsGitHubAppService : IAzureBoardsGitHubAppService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public GitHubData.V3.InstallationDetails VerifyAndGetInstallationDetails(
      IVssRequestContext requestContext,
      string installationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(installationId, nameof (installationId));
      IGitHubAppAccessTokenProvider accessTokenProvider = GitHubHttpClientFactory.GetAppAccessTokenProvider(requestContext, GitHubAppType.Boards);
      GitHubAuthentication appTokenAuthentication = new GitHubAuthentication(GitHubAuthScheme.ApplicationToken, accessTokenProvider.CreateEncodedAppAccessToken());
      GitHubResult<GitHubData.V3.InstallationDetails> installationDetails = GitHubHttpClientFactory.Create(requestContext, accessTokenProvider).GetInstallationDetails(appTokenAuthentication, installationId);
      if (installationDetails?.Result == null || string.IsNullOrEmpty(installationDetails?.Result.Account.Login))
        throw new GitHubAppCannotFetchInstallationException(installationId);
      return installationDetails?.Result;
    }
  }
}
