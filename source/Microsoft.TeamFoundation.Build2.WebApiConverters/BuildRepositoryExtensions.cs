// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildRepositoryExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildRepositoryExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildRepository ToWebApiBuildRepository(
      this Microsoft.TeamFoundation.Build2.Server.BuildRepository srvBuildRepository,
      IVssRequestContext requestContext,
      Guid projectId,
      ISecuredObject securedObject,
      BuildRepositoryCache repositoryCache = null,
      bool refreshNameAndUrl = true,
      int? definitionId = null)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildRepository == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildRepository) null;
      using (PerformanceTimer.StartMeasure(requestContext, "BuildRepositoryExtensions.ToWebApiBuildRepository"))
      {
        if (repositoryCache == null)
          repositoryCache = new BuildRepositoryCache(requestContext);
        string name = srvBuildRepository.Name;
        Uri url = srvBuildRepository.Url;
        if (refreshNameAndUrl)
        {
          BuildRepositoryNameAndUrl repository = repositoryCache.GetRepository(projectId, srvBuildRepository.Type, srvBuildRepository.Id, srvBuildRepository.Url, definitionId);
          if (repository != null)
          {
            if (!string.IsNullOrEmpty(repository.Name))
              name = repository.Name;
            if (repository.Url != (Uri) null)
              url = repository.Url;
          }
        }
        return new Microsoft.TeamFoundation.Build.WebApi.BuildRepository(securedObject)
        {
          Id = srvBuildRepository.Id,
          Type = srvBuildRepository.Type,
          Name = name,
          Url = url,
          DefaultBranch = srvBuildRepository.DefaultBranch,
          RootFolder = srvBuildRepository.RootFolder,
          Clean = srvBuildRepository.Clean,
          CheckoutSubmodules = srvBuildRepository.CheckoutSubmodules,
          Properties = srvBuildRepository.Properties
        };
      }
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildRepository ToBuildServerBuildRepository(
      this Microsoft.TeamFoundation.Build.WebApi.BuildRepository webApiBuildRepository)
    {
      if (webApiBuildRepository == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildRepository) null;
      Microsoft.TeamFoundation.Build2.Server.BuildRepository serverBuildRepository = new Microsoft.TeamFoundation.Build2.Server.BuildRepository();
      serverBuildRepository.Id = webApiBuildRepository.Id;
      serverBuildRepository.Type = webApiBuildRepository.Type;
      serverBuildRepository.Name = webApiBuildRepository.Name;
      serverBuildRepository.Url = webApiBuildRepository.Url;
      serverBuildRepository.DefaultBranch = webApiBuildRepository.DefaultBranch;
      serverBuildRepository.RootFolder = webApiBuildRepository.RootFolder;
      serverBuildRepository.Clean = webApiBuildRepository.Clean;
      serverBuildRepository.CheckoutSubmodules = webApiBuildRepository.CheckoutSubmodules;
      serverBuildRepository.Properties = webApiBuildRepository.Properties;
      return serverBuildRepository;
    }

    internal static bool TryGetDefaultRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repositoryId,
      Uri repositoryUrl,
      int? definitionId,
      out Microsoft.TeamFoundation.Build2.Server.BuildRepository repository)
    {
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repositoryType, false);
      if (sourceProvider != null)
      {
        try
        {
          string str = Guid.Empty.ToString();
          if (definitionId.HasValue && ("Git".Equals(repositoryType, StringComparison.OrdinalIgnoreCase) || "Svn".Equals(repositoryType, StringComparison.OrdinalIgnoreCase)))
          {
            IBuildDefinitionService definitionService = requestContext.GetService<IBuildDefinitionService>();
            requestContext.RunSynchronously<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>((Func<Task<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>>) (() => definitionService.GetDefinitionAsync(requestContext, projectId, definitionId.Value)))?.Repository.Properties.TryGetValue("connectedServiceId", out str);
          }
          ref Microsoft.TeamFoundation.Build2.Server.BuildRepository local = ref repository;
          Microsoft.TeamFoundation.Build2.Server.BuildRepository buildRepository = new Microsoft.TeamFoundation.Build2.Server.BuildRepository();
          buildRepository.Id = repositoryId;
          buildRepository.Type = repositoryType;
          buildRepository.Url = repositoryUrl;
          buildRepository.Properties["connectedServiceId"] = str;
          local = buildRepository;
          sourceProvider.SetRepositoryNameAndUrl(requestContext, projectId, repository);
          return true;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (BuildRepositoryExtensions), ex);
        }
      }
      repository = (Microsoft.TeamFoundation.Build2.Server.BuildRepository) null;
      return false;
    }
  }
}
