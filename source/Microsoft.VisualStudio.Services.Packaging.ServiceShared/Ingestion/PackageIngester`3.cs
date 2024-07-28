// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.PackageIngester`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class PackageIngester<TPackageId, TContent, TStorable> : 
    IAsyncHandler<PackageIngestionRequest<TPackageId, TContent>>,
    IAsyncHandler<PackageIngestionRequest<TPackageId, TContent>, NullResult>,
    IHaveInputType<PackageIngestionRequest<TPackageId, TContent>>,
    IHaveOutputType<NullResult>
    where TPackageId : IPackageIdentity
    where TStorable : IPackageRequest<TPackageId>
  {
    private readonly IFeatureFlagService featureFlagService;
    private readonly IAsyncHandler<PackageIngestionRequest<TPackageId, TContent>, TStorable> storablePackageGenerator;
    private readonly IAsyncHandler<TStorable, NullResult> storablePackageStorer;
    private readonly ITracerService tracerService;
    private readonly IFeedPerms permsFacade;

    public PackageIngester(
      IFeatureFlagService featureFlagService,
      IAsyncHandler<PackageIngestionRequest<TPackageId, TContent>, TStorable> storablePackageGenerator,
      IAsyncHandler<TStorable, NullResult> storablePackageStorer,
      ITracerService tracerService,
      IFeedPerms permsFacade)
    {
      this.featureFlagService = featureFlagService;
      this.storablePackageGenerator = storablePackageGenerator;
      this.storablePackageStorer = storablePackageStorer;
      this.tracerService = tracerService;
      this.permsFacade = permsFacade;
    }

    public async Task<NullResult> Handle(
      PackageIngestionRequest<TPackageId, TContent> request)
    {
      PackageIngester<TPackageId, TContent, TStorable> sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(request.Feed);
        if (sendInTheThisObject.featureFlagService.IsEnabled(request.Protocol.ReadOnlyFeatureFlagName))
          throw new FeatureReadOnlyException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ServiceReadOnly());
        FeedPermissionConstants packagePermission = PackageIngestionUtils.GetRequiredAddPackagePermission(PackageIngestionUtils.GetIngestionDirection(request.SourceChain));
        sendInTheThisObject.permsFacade.Validate(request.Feed, packagePermission);
        TStorable request1 = await sendInTheThisObject.storablePackageGenerator.Handle(request);
        if ((object) request.ExpectedIdentity != null && !request1.PackageId.Equals((object) request.ExpectedIdentity))
          throw new UpstreamUnexpectedPackageDataException("todo: use a protocol agnostic string here. bad upstream package.");
        NullResult nullResult = await sendInTheThisObject.storablePackageStorer.Handle(request1);
      }
      NullResult nullResult1;
      return nullResult1;
    }
  }
}
