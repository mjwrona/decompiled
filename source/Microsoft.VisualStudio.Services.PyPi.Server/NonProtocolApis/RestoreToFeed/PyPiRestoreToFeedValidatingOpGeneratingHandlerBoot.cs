// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.RestoreToFeed.PyPiRestoreToFeedValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.RestoreToFeed
{
  internal class PyPiRestoreToFeedValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData, IPyPiMetadataAggregationAccessor>
  {
    protected override IAsyncHandler<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData> Bootstrap(
      IPyPiMetadataAggregationAccessor metadataAccessor)
    {
      return UntilNonNullHandler.Create<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new RestoreToFeedValidatingOpGeneratingHandler<PyPiPackageIdentity, IPyPiMetadataEntry, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry>) metadataAccessor.ToPointQueryHandler<PyPiPackageIdentity, IPyPiMetadataEntry>(), (IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IRestoreToFeedOperationData>) new RestoreToFeedOpGeneratingHandler<PyPiPackageIdentity>()), (IAsyncHandler<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>((IEnumerable<string>) new string[1]
      {
        "Deleted"
      }));
    }
  }
}
