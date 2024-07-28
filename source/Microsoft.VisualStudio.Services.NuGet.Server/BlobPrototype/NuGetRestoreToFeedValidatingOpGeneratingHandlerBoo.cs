// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRestoreToFeedValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRestoreToFeedValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData, INuGetMetadataService>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetRestoreToFeedValidatingOpGeneratingHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    protected override IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData> Bootstrap(
      INuGetMetadataService metadataAccessor)
    {
      IAsyncHandler<IPackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<VssNuGetPackageIdentity, INuGetMetadataEntry>();
      IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, NuGetRestoreToFeedOperationData> requestToOpHandler = new NuGetRestoreToFeedOperationDataGeneratingHandlerBootstrapper(this.requestContext, (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>) pointQueryHandler).Bootstrap();
      return UntilNonNullHandler.Create<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData>) new RestoreToFeedValidatingOpGeneratingHandler<VssNuGetPackageIdentity, INuGetMetadataEntry, NuGetRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>) pointQueryHandler, requestToOpHandler), (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData>((IEnumerable<string>) new string[1]
      {
        "Deleted"
      }));
    }
  }
}
