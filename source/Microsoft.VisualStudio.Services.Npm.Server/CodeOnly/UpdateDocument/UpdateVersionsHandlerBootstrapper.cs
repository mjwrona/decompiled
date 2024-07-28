// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.UpdateDocument.UpdateVersionsHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.Telemetry;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.UpdateDocument
{
  public class UpdateVersionsHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageMetadataRequest, NullResult, INpmMetadataService>
  {
    private IVssRequestContext requestContext;

    public UpdateVersionsHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackageMetadataRequest, NullResult> Bootstrap(
      INpmMetadataService agg1)
    {
      IConverter<IRawPackageNameRequest, IPackageNameRequest<NpmPackageName>> nameConverter = new NpmRawPackageNameRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap();
      IAsyncHandler<IRawPackageRequest, NullResult> ingestHandler = NpmAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IRawPackageRequest, NullResult>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new IngestRawPackageIfNotAlreadyIngestedBootstrapper(this.requestContext, BlockedIdentityContext.Update));
      return new NpmWriteBootstrapper<PackageMetadataRequest, ICommitOperationData, NullResult>(this.requestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackageMetadataRequest, ICommitOperationData>((IAsyncHandler<PackageMetadataRequest, ICommitOperationData>) new PackageMetadataRequestToOpValidatingHandler(agg1, ingestHandler, nameConverter, this.requestContext.GetTracerFacade())), (IAsyncHandler<ICommitLogEntry, NullResult>) new ByFuncAsyncHandler<ICommitLogEntry, NullResult>((Func<ICommitLogEntry, NullResult>) (c => (NullResult) null)), (IAsyncHandler<(PackageMetadataRequest, ICommitOperationData), ICiData>) new NpmDeprecateVersionsCiDataHandler(this.requestContext), false).Bootstrap();
    }
  }
}
