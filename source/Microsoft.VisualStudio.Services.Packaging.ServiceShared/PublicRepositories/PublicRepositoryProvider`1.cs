// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepositoryProvider`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class PublicRepositoryProvider<TClient> : 
    IPublicRepositoryProvider<TClient>,
    IPublicRepositoryProvider
  {
    public PublicRepositoryProvider(
      IEnumerable<IPublicRepository<TClient>> publicRepositories)
    {
      this.Repositories = (IImmutableList<IPublicRepository<TClient>>) publicRepositories.ToImmutableArray<IPublicRepository<TClient>>();
    }

    IReadOnlyList<IPublicRepository> IPublicRepositoryProvider.Repositories => (IReadOnlyList<IPublicRepository>) this.Repositories;

    IReadOnlyList<IPublicRepository<TClient>> IPublicRepositoryProvider<TClient>.Repositories => (IReadOnlyList<IPublicRepository<TClient>>) this.Repositories;

    public IImmutableList<IPublicRepository<TClient>> Repositories { get; }

    IPublicRepository? IPublicRepositoryProvider.GetRepositoryForSourceOrDefault(
      WellKnownUpstreamSource wellKnownSource)
    {
      return (IPublicRepository) this.GetRepositoryForSourceOrDefault(wellKnownSource);
    }

    public IPublicRepository<TClient>? GetRepositoryForSourceOrDefault(
      WellKnownUpstreamSource wellKnownSource)
    {
      return this.Repositories.FirstOrDefault<IPublicRepository<TClient>>((Func<IPublicRepository<TClient>, bool>) (x => wellKnownSource.Equals(x.WellKnownUpstreamSource)));
    }
  }
}
