// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion.NpmRevisionAndVersionProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion
{
  public class NpmRevisionAndVersionProviderBootstrapper : 
    RequireAggHandlerBootstrapper<PackageNameRequest<NpmPackageName>, RevisionAndVersions, IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmRevisionAndVersionProviderBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackageNameRequest<NpmPackageName>, RevisionAndVersions> Bootstrap(
      IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService)
    {
      return (IAsyncHandler<PackageNameRequest<NpmPackageName>, RevisionAndVersions>) new NpmRevisionAndVersionsProviderHandler(metadataService);
    }
  }
}
