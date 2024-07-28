// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.RestoreToFeed.NpmRestoreToFeedValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.RestoreToFeed
{
  internal class NpmRestoreToFeedValidatingOpGeneratingHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmRestoreToFeedValidatingOpGeneratingHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    public IAsyncHandler<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData> Bootstrap() => UntilNonNullHandler.Create<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new RestoreToFeedValidatingOpGeneratingHandler<NpmPackageIdentity, INpmMetadataEntry, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) new NpmMetadataHandlerBootstrapper(this.requestContext).Bootstrap(), (IAsyncHandler<PackageRequest<NpmPackageIdentity>, IRestoreToFeedOperationData>) new RestoreToFeedOpGeneratingHandler<NpmPackageIdentity>()), (IAsyncHandler<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>((IEnumerable<string>) new string[1]
    {
      "Deleted"
    }));
  }
}
