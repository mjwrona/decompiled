// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.RepositoryFileProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  internal class RepositoryFileProvider : IFileProvider
  {
    private readonly Guid m_projectId;
    private readonly ServiceEndpoint m_endpoint;
    private readonly RepositoryResource m_repository;
    private readonly ISourceProvider m_sourceProvider;
    private readonly IVssRequestContext m_requestContext;
    private readonly IReadOnlyList<ConfigurationFile> m_overrideFiles;

    public RepositoryFileProvider(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      ServiceEndpoint endpoint,
      ISourceProvider sourceProvider,
      IReadOnlyList<ConfigurationFile> overrideFiles = null)
    {
      this.m_endpoint = endpoint;
      this.m_projectId = projectId;
      this.m_repository = repository;
      this.m_requestContext = requestContext;
      this.m_sourceProvider = sourceProvider;
      if (overrideFiles != null)
        this.m_overrideFiles = overrideFiles;
      else
        this.m_overrideFiles = (IReadOnlyList<ConfigurationFile>) new List<ConfigurationFile>();
    }

    public RepositoryResource Repository => this.m_repository;

    public string GetDirectoryName(string path) => this.m_sourceProvider.GetDirectoryName(path);

    public string GetFileContent(string path)
    {
      ConfigurationFile configurationFile = this.m_overrideFiles.FirstOrDefault<ConfigurationFile>((Func<ConfigurationFile, bool>) (f => string.Equals(this.ResolvePath(string.Empty, f.Filename), this.ResolvePath(string.Empty, path), StringComparison.OrdinalIgnoreCase)));
      return configurationFile != null ? configurationFile.Contents : this.m_sourceProvider.GetFileContent(this.m_requestContext, this.m_projectId, this.m_repository, this.m_endpoint, path);
    }

    public string GetFileContentByUuid(string uuid) => !(this.m_sourceProvider is ICachingSourceProvider sourceProvider) ? (string) null : sourceProvider.GetFileContentByUuid(this.m_requestContext, this.m_projectId, this.m_repository, this.m_endpoint, uuid);

    public string GetFileName(string path) => this.m_sourceProvider.GetFileName(path);

    public string GetFileUuid(string path)
    {
      if (this.m_overrideFiles.FirstOrDefault<ConfigurationFile>((Func<ConfigurationFile, bool>) (f => string.Equals(f.Filename, path, StringComparison.OrdinalIgnoreCase))) != null)
        return string.Empty;
      return !(this.m_sourceProvider is ICachingSourceProvider sourceProvider) ? (string) null : sourceProvider.GetFileUuid(this.m_requestContext, this.m_projectId, this.m_repository, this.m_endpoint, path);
    }

    public string ResolvePath(string defaultRoot, string path) => this.m_sourceProvider.ResolvePath(defaultRoot, path);
  }
}
