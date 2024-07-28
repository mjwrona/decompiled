// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiPublicRepositoryProvider
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  public static class PyPiPublicRepositoryProvider
  {
    public static IPublicRepositoryProvider<IUpstreamPyPiClient> Bootstrap(
      IVssRequestContext requestContext)
    {
      return (IPublicRepositoryProvider<IUpstreamPyPiClient>) new PublicRepositoryProvider<IUpstreamPyPiClient>((IEnumerable<IPublicRepository<IUpstreamPyPiClient>>) ImmutableArray.Create<IPyPiPublicRepository>(PyPiPublicRepository.BootstrapPyPi(requestContext.To(TeamFoundationHostType.Deployment))));
    }
  }
}
