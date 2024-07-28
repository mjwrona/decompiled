// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.MinimalBuildRepositoryExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class MinimalBuildRepositoryExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildRepository ToWebApiBuildRepository(
      this MinimalBuildRepository srvBuildRepository,
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
      using (PerformanceTimer.StartMeasure(requestContext, "MinimalBuildRepositoryExtensions.ToWebApiBuildRepository"))
      {
        if (repositoryCache == null)
          repositoryCache = new BuildRepositoryCache(requestContext);
        string str = (string) null;
        Uri uri = (Uri) null;
        if (refreshNameAndUrl)
        {
          BuildRepositoryNameAndUrl repository = repositoryCache.GetRepository(projectId, srvBuildRepository.Type, srvBuildRepository.Id, (Uri) null, definitionId);
          if (repository != null)
          {
            if (!string.IsNullOrEmpty(repository.Name))
              str = repository.Name;
            if (repository.Url != (Uri) null)
              uri = repository.Url;
          }
        }
        return new Microsoft.TeamFoundation.Build.WebApi.BuildRepository(securedObject)
        {
          Id = srvBuildRepository.Id,
          Type = srvBuildRepository.Type,
          Name = str,
          Url = uri
        };
      }
    }
  }
}
