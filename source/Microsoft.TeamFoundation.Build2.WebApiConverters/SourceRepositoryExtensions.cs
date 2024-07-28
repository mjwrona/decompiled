// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.SourceRepositoryExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class SourceRepositoryExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.SourceRepository ToWebApiSourceRepository(
      this Microsoft.TeamFoundation.Build2.Server.SourceRepository srvSourceRepository)
    {
      if (srvSourceRepository == null)
        return (Microsoft.TeamFoundation.Build.WebApi.SourceRepository) null;
      return new Microsoft.TeamFoundation.Build.WebApi.SourceRepository()
      {
        Id = srvSourceRepository.Id,
        SourceProviderName = srvSourceRepository.SourceProviderName,
        Name = srvSourceRepository.Name,
        FullName = srvSourceRepository.FullName,
        DefaultBranch = srvSourceRepository.DefaultBranch,
        Properties = srvSourceRepository.Properties,
        Url = srvSourceRepository.Url
      };
    }

    public static Microsoft.TeamFoundation.Build.WebApi.SourceRepositories ToWebApiSourceRepositories(
      this Microsoft.TeamFoundation.Build2.Server.SourceRepositories srvSourceRepositories)
    {
      if (srvSourceRepositories == null)
        return (Microsoft.TeamFoundation.Build.WebApi.SourceRepositories) null;
      return new Microsoft.TeamFoundation.Build.WebApi.SourceRepositories()
      {
        ContinuationToken = srvSourceRepositories.ContinuationToken,
        PageLength = srvSourceRepositories.PageLength,
        Repositories = srvSourceRepositories.Repositories.Select<Microsoft.TeamFoundation.Build2.Server.SourceRepository, Microsoft.TeamFoundation.Build.WebApi.SourceRepository>((Func<Microsoft.TeamFoundation.Build2.Server.SourceRepository, Microsoft.TeamFoundation.Build.WebApi.SourceRepository>) (x => x.ToWebApiSourceRepository())).ToList<Microsoft.TeamFoundation.Build.WebApi.SourceRepository>(),
        TotalPageCount = srvSourceRepositories.TotalPageCount
      };
    }
  }
}
