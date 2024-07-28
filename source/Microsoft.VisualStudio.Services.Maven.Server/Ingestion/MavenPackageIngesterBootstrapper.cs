// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Ingestion.MavenPackageIngesterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Maven.Server.Constants;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Ingestion.Validation;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;

namespace Microsoft.VisualStudio.Services.Maven.Server.Ingestion
{
  public class MavenPackageIngesterBootstrapper : 
    IBootstrapper<IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenPackageIngesterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>> Bootstrap()
    {
      RegistryServiceFacade registryServiceFacade = new RegistryServiceFacade(this.requestContext);
      IAsyncHandler<MavenPackageFileInfo, NullResult> validatingHandler = new MavenPackageVersionValidatingHandler().ThenForwardOriginalRequestTo<MavenPackageFileInfo, NullResult>((IAsyncHandler<MavenPackageFileInfo, NullResult>) new PomFileValidatorHandler((IRegistryService) registryServiceFacade));
      IValidator<IPackageRequest> validator = new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext, BlockedIdentityContext.Upload).Bootstrap().AsThrowingValidator<IPackageRequest>();
      ICommitLog commitLogReader = new MavenCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>, MavenStreamStorablePackageInfo> storablePackageGenerator = new MavenStoredPackageGenerator((IAsyncHandler<long>) new PackageSizeValidatingHandler((IRegistryService) registryServiceFacade, Protocol.Maven.CorrectlyCasedName), validatingHandler, new TerrapinIngestionValidatorBootstrapper(this.requestContext, (ICommitLogWriter<ICommitLogEntry>) commitLogReader, new ChangeProcessingFeedJobQueuerBootstrapper(this.requestContext, MavenServerConstants.ChangeProcessingJobConstants.MavenChangeProcessingJobCreationInfo, (ICommitLogEndpointReader) commitLogReader).Bootstrap(), (IIdentityResolver) MavenIdentityResolver.Instance).Bootstrap()).ValidateResultWith<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>, MavenStreamStorablePackageInfo, IPackageRequest>(validator);
      IAsyncHandler<MavenStreamStorablePackageInfo, ICommitLogEntry> handler = new MavenWriteBootstrapper<MavenStreamStorablePackageInfo, MavenCommitOperationData>(this.requestContext, (IRequireAggHandlerBootstrapper<MavenStreamStorablePackageInfo, MavenCommitOperationData>) new MavenAddReqToOpHandlerBootstrapper(this.requestContext), (IAsyncHandler<(MavenStreamStorablePackageInfo, MavenCommitOperationData), ICiData>) new MavenUploadPackageCiDataFacadeHandler(this.requestContext), true).Bootstrap();
      return (IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>>) new PackageIngester<MavenPackageIdentity, MavenPackageFileInfo, MavenStreamStorablePackageInfo>(this.requestContext.GetFeatureFlagFacade(), storablePackageGenerator, (IAsyncHandler<MavenStreamStorablePackageInfo, NullResult>) handler.ThenReturnNullResult<MavenStreamStorablePackageInfo, ICommitLogEntry>(), this.requestContext.GetTracerFacade(), (IFeedPerms) new FeedPermsFacade(this.requestContext));
    }
  }
}
