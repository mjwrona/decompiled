// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.PullRequestsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "pullRequests")]
  [ClientGroupByResource("sourceProviders")]
  public class PullRequestsController : BuildApiController
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public PullRequest GetPullRequest(
      string providerName,
      string pullRequestId,
      [ClientQueryParameter] string repositoryId = null,
      [ClientQueryParameter] Guid? serviceEndpointId = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(providerName, nameof (providerName));
      ArgumentUtility.CheckStringForNullOrEmpty(repositoryId, nameof (repositoryId));
      ArgumentUtility.CheckStringForNullOrEmpty(pullRequestId, nameof (pullRequestId));
      return this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, providerName).GetPullRequest(this.TfsRequestContext, this.ProjectId, repositoryId, pullRequestId, serviceEndpointId);
    }
  }
}
