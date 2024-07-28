// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildRepositoryCache
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public class BuildRepositoryCache
  {
    private Dictionary<Tuple<Guid, string, string, Uri>, BuildRepositoryNameAndUrl> repos;
    private IVssRequestContext context;

    public BuildRepositoryCache(IVssRequestContext requestContext)
    {
      this.context = requestContext;
      this.repos = new Dictionary<Tuple<Guid, string, string, Uri>, BuildRepositoryNameAndUrl>();
    }

    public void PrimeCache(
      IEnumerable<Tuple<Guid, BuildRepository>> buildRepositories)
    {
      foreach (Tuple<Guid, BuildRepository> buildRepository in buildRepositories)
      {
        this.repos[new Tuple<Guid, string, string, Uri>(buildRepository.Item1, buildRepository.Item2.Type, buildRepository.Item2.Id, buildRepository.Item2.Url)] = new BuildRepositoryNameAndUrl()
        {
          Name = buildRepository.Item2.Name,
          Url = buildRepository.Item2.Url
        };
        if (!string.IsNullOrEmpty(buildRepository.Item2.Id))
          this.repos[new Tuple<Guid, string, string, Uri>(buildRepository.Item1, buildRepository.Item2.Type, buildRepository.Item2.Id, (Uri) null)] = new BuildRepositoryNameAndUrl()
          {
            Name = buildRepository.Item2.Name,
            Url = buildRepository.Item2.Url
          };
      }
    }

    public BuildRepositoryNameAndUrl GetRepository(
      Guid projectId,
      string type,
      string repoId,
      Uri repoUrl,
      int? definitionId)
    {
      using (PerformanceTimer.StartMeasure(this.context, "BuildRepositoryCache.GetRepository"))
      {
        Tuple<Guid, string, string, Uri> key1 = new Tuple<Guid, string, string, Uri>(projectId, type, repoId, repoUrl);
        if (this.repos.ContainsKey(key1))
          return this.repos[key1];
        BuildRepository repository = (BuildRepository) null;
        if (!BuildRepositoryExtensions.TryGetDefaultRepository(this.context, projectId, type, repoId, repoUrl, definitionId, out repository))
          this.context.TraceInfo(0, "BuildRepositoryConverter", "Default information for {0} repository {1} was not found.", (object) type, (object) repoId);
        Dictionary<Tuple<Guid, string, string, Uri>, BuildRepositoryNameAndUrl> repos = this.repos;
        Tuple<Guid, string, string, Uri> key2 = key1;
        BuildRepositoryNameAndUrl repositoryNameAndUrl;
        if (repository != null)
          repositoryNameAndUrl = new BuildRepositoryNameAndUrl()
          {
            Name = repository.Name,
            Url = repository.Url
          };
        else
          repositoryNameAndUrl = (BuildRepositoryNameAndUrl) null;
        repos[key2] = repositoryNameAndUrl;
        return this.repos[key1];
      }
    }
  }
}
