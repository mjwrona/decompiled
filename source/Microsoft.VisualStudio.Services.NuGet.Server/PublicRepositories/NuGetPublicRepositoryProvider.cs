// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPublicRepositoryProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public static class NuGetPublicRepositoryProvider
  {
    public static IPublicRepositoryProvider<IUpstreamNuGetClient> Bootstrap(
      IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (IPublicRepositoryProvider<IUpstreamNuGetClient>) new PublicRepositoryProvider<IUpstreamNuGetClient>((IEnumerable<IPublicRepository<IUpstreamNuGetClient>>) ImmutableArray.Create<INuGetPublicRepository<IUpstreamNuGetClient>>((INuGetPublicRepository<IUpstreamNuGetClient>) NuGetPublicRepository.BootstrapNuGetOrg(deploymentRequestContext), NuGetPublicRepository.BootstrapPsGallery(deploymentRequestContext)));
    }
  }
}
