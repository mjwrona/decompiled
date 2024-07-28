// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.RepositoryBranchController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "branches")]
  [ClientGroupByResource("sourceProviders")]
  public class RepositoryBranchController : BuildApiController
  {
    [HttpGet]
    public IEnumerable<string> ListBranches(
      string providerName,
      [ClientQueryParameter] Guid? serviceEndpointId = null,
      [ClientQueryParameter] string repository = null,
      [ClientQueryParameter] string branchName = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(providerName, nameof (providerName));
      IBuildSourceProvider sourceProvider = this.GetSourceProvider(providerName);
      if (branchName == null)
        return sourceProvider.GetRepositoryBranches(this.TfsRequestContext, this.ProjectId, serviceEndpointId, repository);
      if (!sourceProvider.CheckForBranch(this.TfsRequestContext, this.ProjectId, serviceEndpointId, repository, branchName))
        throw new BranchNotFoundException(Resources.BranchNotFound((object) branchName, (object) repository));
      return (IEnumerable<string>) new List<string>()
      {
        branchName
      };
    }

    private IBuildSourceProvider GetSourceProvider(string repositoryType) => this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, repositoryType);
  }
}
