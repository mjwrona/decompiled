// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.MavenMetadataHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations
{
  public class MavenMetadataHandlerBootstrapper : 
    IBootstrapper<
    #nullable disable
    IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenMetadataHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> Bootstrap() => MavenAggregationResolver.Bootstrap(this.requestContext).HandlerFor<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry, IMavenMetadataAggregationAccessor>((Func<IMavenMetadataAggregationAccessor, IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>>) (metadataAccessor => ByAsyncFuncAsyncHandler.For<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>((Func<PackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>>) (async request => await metadataAccessor.GetPackageVersionStateAsync((IPackageRequest<MavenPackageIdentity>) request)))));
  }
}
