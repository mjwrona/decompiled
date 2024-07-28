// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.RepositoryFileProviderFactory
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  public class RepositoryFileProviderFactory : IFileProviderFactory
  {
    private readonly bool m_includeCheckoutOptions;
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;
    private readonly IServiceEndpointStore m_endpointStore;
    private readonly Dictionary<string, IFileProvider> m_providers;
    private readonly Dictionary<string, RepositoryResource> m_repositories;
    private readonly IReadOnlyList<ConfigurationFile> m_overrideFiles;
    private static readonly HashSet<string> s_coreProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      RepositoryPropertyNames.Id,
      RepositoryPropertyNames.DefaultBranch,
      RepositoryPropertyNames.ExternalId,
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
      RepositoryPropertyNames.Mappings,
      RepositoryPropertyNames.Name,
      RepositoryPropertyNames.Project,
      RepositoryPropertyNames.Ref,
      RepositoryPropertyNames.Type,
      RepositoryPropertyNames.Version,
      RepositoryPropertyNames.VersionSpec
    };

    public RepositoryFileProviderFactory(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource self,
      IServiceEndpointStore endpointStore,
      IReadOnlyList<ConfigurationFile> overrideFiles = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<RepositoryResource>(self, nameof (self));
      this.m_projectId = projectId;
      this.m_endpointStore = endpointStore;
      this.m_requestContext = requestContext;
      this.m_providers = new Dictionary<string, IFileProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_repositories = new Dictionary<string, RepositoryResource>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_overrideFiles = overrideFiles;
      self.Alias = PipelineConstants.SelfAlias;
      this.m_includeCheckoutOptions = requestContext.IsFeatureEnabled("DistributedTask.IncludeCheckoutOptions");
      if (this.m_includeCheckoutOptions)
        self.Properties.DeleteAllExcept((ISet<string>) RepositoryFileProviderFactory.s_coreProperties_withCheckoutOptions);
      else
        self.Properties.DeleteAllExcept((ISet<string>) RepositoryFileProviderFactory.s_coreProperties);
      this.ValidateRepository(self, true);
      this.m_repositories.Add(self.Alias, self);
    }

    public IReadOnlyDictionary<string, RepositoryResource> Repositories => (IReadOnlyDictionary<string, RepositoryResource>) this.m_repositories;

    public void AddRepository(RepositoryResource repository)
    {
      ArgumentUtility.CheckForNull<RepositoryResource>(repository, nameof (repository));
      if (string.IsNullOrEmpty(repository.Alias))
        throw new ResourceValidationException(TaskResources.SpecifyRepoAlias());
      RepositoryResource repository1;
      if (this.m_repositories.TryGetValue(repository.Alias, out repository1))
      {
        if (!repository.Alias.Equals(PipelineConstants.SelfAlias, StringComparison.OrdinalIgnoreCase))
          throw new ResourceValidationException(TaskResources.RepoAlreadyExists((object) repository.Alias));
        if (this.m_includeCheckoutOptions && repository.Properties.Items.ContainsKey(RepositoryPropertyNames.CheckoutOptions) && repository1.Properties.Items.ContainsKey(RepositoryPropertyNames.CheckoutOptions))
          repository1.Properties.Items.Remove(RepositoryPropertyNames.CheckoutOptions);
        repository1.Properties.UnionWith(repository.Properties);
        this.ValidateRepository(repository1);
      }
      else
      {
        this.ValidateRepository(repository);
        this.m_repositories.Add(repository.Alias, repository);
      }
    }

    public IFileProvider GetProvider(string name)
    {
      name = string.IsNullOrEmpty(name) ? PipelineConstants.SelfAlias : name;
      IFileProvider provider;
      if (this.m_providers.TryGetValue(name, out provider))
        return provider;
      RepositoryResource repository;
      if (!this.m_repositories.TryGetValue(name, out repository))
        throw new ResourceNotFoundException(TaskResources.NoRepoFoundByName((object) name));
      ServiceEndpoint endpoint = this.m_endpointStore?.Get(repository.Endpoint);
      ISourceProvider sourceProvider = this.m_requestContext.GetService<ISourceProviderService>().GetSourceProvider(this.m_requestContext, repository.Type);
      RepositoryFileProviderFactory.VerifyYamlSupport(sourceProvider, repository);
      sourceProvider.ResolveVersion(this.m_requestContext, this.m_projectId, repository, endpoint);
      provider = (IFileProvider) new RepositoryFileProvider(this.m_requestContext, this.m_projectId, repository, endpoint, sourceProvider, this.m_overrideFiles);
      this.m_providers[name] = provider;
      return provider;
    }

    public IEnumerable<RepositoryResource> GetRepositories() => (IEnumerable<RepositoryResource>) this.m_repositories.Values.ToList<RepositoryResource>();

    public RepositoryResource GetRepository(string name)
    {
      RepositoryResource repositoryResource;
      return this.m_repositories.TryGetValue(name, out repositoryResource) ? repositoryResource : (RepositoryResource) null;
    }

    private void ValidateRepository(RepositoryResource repository, bool verifyYamlSupport = false)
    {
      ServiceEndpoint endpoint = (ServiceEndpoint) null;
      if (repository.Endpoint != null)
      {
        endpoint = this.m_endpointStore?.Get(repository.Endpoint);
        if (endpoint == null)
          throw new ResourceNotFoundException(TaskResources.EndpointNotFoundForRepo((object) repository.Alias, (object) repository.Endpoint));
        if (repository.Endpoint.Id == Guid.Empty)
          repository.Endpoint.Id = endpoint.Id;
      }
      ISourceProvider sourceProvider = this.m_requestContext.GetService<ISourceProviderService>().GetSourceProvider(this.m_requestContext, repository.Type);
      sourceProvider.Validate(this.m_requestContext, this.m_projectId, repository, endpoint);
      if (!verifyYamlSupport)
        return;
      RepositoryFileProviderFactory.VerifyYamlSupport(sourceProvider, repository);
    }

    private static void VerifyYamlSupport(
      ISourceProvider sourceProvider,
      RepositoryResource repository)
    {
      if (!sourceProvider.SupportsCapability("yamlDefinition"))
        throw new NotSupportedException(TaskResources.NoYamlSupport((object) repository.Type));
    }
  }
}
