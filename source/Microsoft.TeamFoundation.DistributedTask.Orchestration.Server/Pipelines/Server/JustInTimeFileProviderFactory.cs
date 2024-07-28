// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.JustInTimeFileProviderFactory
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class JustInTimeFileProviderFactory : IFileProviderFactory
  {
    private readonly IReadOnlyList<ConfigurationFile> m_files;
    private readonly IVssRequestContext m_requestContext;
    private readonly RepositoryResource m_repository;
    private readonly bool m_includeCheckoutOptions;
    private static readonly HashSet<string> s_coreProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      RepositoryPropertyNames.Id,
      RepositoryPropertyNames.DefaultBranch,
      RepositoryPropertyNames.ExternalId,
      RepositoryPropertyNames.IsJustInTimeRepository,
      RepositoryPropertyNames.Mappings,
      RepositoryPropertyNames.Name,
      RepositoryPropertyNames.Project,
      RepositoryPropertyNames.Ref,
      RepositoryPropertyNames.Type,
      RepositoryPropertyNames.Version,
      RepositoryPropertyNames.VersionSpec
    };
    private static readonly HashSet<string> s_coreProperties_withCheckoutOptions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      RepositoryPropertyNames.Id,
      RepositoryPropertyNames.CheckoutOptions,
      RepositoryPropertyNames.DefaultBranch,
      RepositoryPropertyNames.ExternalId,
      RepositoryPropertyNames.IsJustInTimeRepository,
      RepositoryPropertyNames.Mappings,
      RepositoryPropertyNames.Name,
      RepositoryPropertyNames.Project,
      RepositoryPropertyNames.Ref,
      RepositoryPropertyNames.Type,
      RepositoryPropertyNames.Version,
      RepositoryPropertyNames.VersionSpec
    };

    public JustInTimeFileProviderFactory(
      IVssRequestContext requestContext,
      IReadOnlyList<ConfigurationFile> files,
      Guid projectId,
      RepositoryResource self,
      IServiceEndpointStore endpointStore)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyList<ConfigurationFile>>(files, nameof (files));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<RepositoryResource>(self, nameof (self));
      ArgumentUtility.CheckForNull<IServiceEndpointStore>(endpointStore, nameof (endpointStore));
      this.m_requestContext = requestContext;
      this.m_files = files;
      self.Alias = PipelineConstants.SelfAlias;
      this.m_includeCheckoutOptions = requestContext.IsFeatureEnabled("DistributedTask.IncludeCheckoutOptions");
      if (this.m_includeCheckoutOptions)
        self.Properties.DeleteAllExcept((ISet<string>) JustInTimeFileProviderFactory.s_coreProperties_withCheckoutOptions);
      else
        self.Properties.DeleteAllExcept((ISet<string>) JustInTimeFileProviderFactory.s_coreProperties);
      JustInTimeFileProviderFactory.ValidateCapabilities(requestContext, projectId, self);
      this.m_repository = self;
    }

    public void AddRepository(RepositoryResource repository)
    {
    }

    public IFileProvider GetProvider(string repositoryAlias) => (IFileProvider) new JustInTimeFileProvider(this.m_requestContext, this.m_files, this.m_repository);

    public IEnumerable<RepositoryResource> GetRepositories() => (IEnumerable<RepositoryResource>) new RepositoryResource[1]
    {
      this.m_repository
    };

    public RepositoryResource GetRepository(string alias) => string.Equals(alias, this.m_repository.Alias, StringComparison.Ordinal) ? this.m_repository : (RepositoryResource) null;

    private static void ValidateCapabilities(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository)
    {
      if (!requestContext.GetService<ISourceProviderService>().GetSourceProvider(requestContext, repository.Type).SupportsCapability("yamlDefinition"))
        throw new NotSupportedException("The repository type " + repository.Type + " does not support YAML templates");
    }
  }
}
