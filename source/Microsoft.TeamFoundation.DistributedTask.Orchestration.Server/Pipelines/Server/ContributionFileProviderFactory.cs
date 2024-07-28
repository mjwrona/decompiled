// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.ContributionFileProviderFactory
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class ContributionFileProviderFactory : IFileProviderFactory
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly string m_contributionId;

    public ContributionFileProviderFactory(IVssRequestContext requestContext, string contributionId)
    {
      this.m_requestContext = requestContext;
      this.m_contributionId = contributionId;
    }

    public void AddRepository(RepositoryResource repository)
    {
    }

    public IFileProvider GetProvider(string repositoryAlias) => (IFileProvider) new ContributionFileProvider(this.m_requestContext, this.m_contributionId);

    public IEnumerable<RepositoryResource> GetRepositories() => Enumerable.Empty<RepositoryResource>();

    public RepositoryResource GetRepository(string alias) => (RepositoryResource) null;
  }
}
