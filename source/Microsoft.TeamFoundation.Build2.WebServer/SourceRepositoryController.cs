// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.SourceRepositoryController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "repositories")]
  [ClientGroupByResource("sourceProviders")]
  public class SourceRepositoryController : BuildApiController
  {
    [HttpGet]
    public Microsoft.TeamFoundation.Build.WebApi.SourceRepositories ListRepositories(
      string providerName,
      [ClientQueryParameter] Guid? serviceEndpointId = null,
      [ClientQueryParameter] string repository = null,
      [ClientQueryParameter] ResultSet resultSet = ResultSet.All,
      [ClientQueryParameter] bool pageResults = false,
      [ClientQueryParameter] string continuationToken = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(providerName, nameof (providerName));
      IBuildSourceProvider sourceProvider = this.GetSourceProvider(providerName);
      if (string.IsNullOrEmpty(repository))
        return resultSet == ResultSet.Top ? sourceProvider.GetTopUserRepositories(this.TfsRequestContext, this.ProjectId, serviceEndpointId).ToWebApiSourceRepositories() : sourceProvider.GetUserRepositories(this.TfsRequestContext, this.ProjectId, serviceEndpointId, pageResults, continuationToken).ToWebApiSourceRepositories();
      Microsoft.TeamFoundation.Build2.Server.SourceRepository userRepository = sourceProvider.GetUserRepository(this.TfsRequestContext, this.ProjectId, serviceEndpointId, repository);
      if (userRepository != null)
        return new Microsoft.TeamFoundation.Build.WebApi.SourceRepositories()
        {
          ContinuationToken = (string) null,
          PageLength = 1,
          Repositories = new List<Microsoft.TeamFoundation.Build.WebApi.SourceRepository>()
          {
            userRepository.ToWebApiSourceRepository()
          },
          TotalPageCount = 1
        };
      return new Microsoft.TeamFoundation.Build.WebApi.SourceRepositories()
      {
        ContinuationToken = (string) null,
        PageLength = 0,
        Repositories = new List<Microsoft.TeamFoundation.Build.WebApi.SourceRepository>(),
        TotalPageCount = 0
      };
    }

    private IBuildSourceProvider GetSourceProvider(string repositoryType) => this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, repositoryType);
  }
}
