// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenRestoreToFeedValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenRestoreToFeedValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData, IMavenMetadataAggregationAccessor>
  {
    protected override IAsyncHandler<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData> Bootstrap(
      IMavenMetadataAggregationAccessor metadataAccessor)
    {
      return UntilNonNullHandler.Create<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new RestoreToFeedValidatingOpGeneratingHandler<MavenPackageIdentity, IMavenMetadataEntry, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>) metadataAccessor.ToPointQueryHandler<MavenPackageIdentity, IMavenMetadataEntry>(), (IAsyncHandler<PackageRequest<MavenPackageIdentity>, IRestoreToFeedOperationData>) new MavenRestoreToFeedOpGeneratingHandler()), (IAsyncHandler<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>((IEnumerable<string>) new string[1]
      {
        "Deleted"
      }));
    }
  }
}
