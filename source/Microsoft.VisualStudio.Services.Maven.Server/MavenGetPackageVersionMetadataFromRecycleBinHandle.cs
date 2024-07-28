// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenGetPackageVersionMetadataFromRecycleBinHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenGetPackageVersionMetadataFromRecycleBinHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<MavenRawPackageRequest, IMavenMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenGetPackageVersionMetadataFromRecycleBinHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    public IAsyncHandler<MavenRawPackageRequest, IMavenMetadataEntry> Bootstrap() => new ByFuncConverter<MavenRawPackageRequest, MavenRawPackageRequest<ShowDeletedBool>>((Func<MavenRawPackageRequest, MavenRawPackageRequest<ShowDeletedBool>>) (rawPackageRequest => new MavenRawPackageRequest<ShowDeletedBool>(rawPackageRequest, (ShowDeletedBool) true))).ThenDelegateTo<MavenRawPackageRequest, MavenRawPackageRequest<ShowDeletedBool>, IMavenMetadataEntry>(new MavenGetPackageVersionMetadataHandlerBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<MavenRawPackageRequest, IMavenMetadataEntry, IMavenMetadataEntry>((IAsyncHandler<IMavenMetadataEntry, IMavenMetadataEntry>) new ThrowIfNotInRecycleBinHandler<IMavenMetadataEntry>());
  }
}
