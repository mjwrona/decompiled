// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api.PyPiLargeUploadController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Client.Internal;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "largeUpload", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class PyPiLargeUploadController : PyPiApiController
  {
    [HttpPost]
    public async Task<HttpResponseMessage> AddPackageFromBlobStoreAsync(
      string feedId,
      [FromBody] AddPackageFromBlobRequestInternal blobRequest)
    {
      PyPiLargeUploadController uploadController = this;
      if (uploadController.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_NotSupportedOnPrem());
      if (blobRequest == null)
        throw new InvalidPublishRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_PushRequestMissingBlob());
      IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feed = uploadController.GetFeedRequest(feedId, readOnlyValidator);
      IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, NullResult> ingester = new PyPiPackageIngesterBootstrapper(uploadController.TfsRequestContext).Bootstrap();
      IContentBlobStore contentBlobStore = new ContentBlobStoreFacadeBootstrapper(uploadController.TfsRequestContext).Bootstrap();
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      BlobIdentifierWithBlocks blobIdentifierWithBlocks = blobRequest.Blob.ToBlobIdentifierWithBlocks();
      BlobIdentifierWithBlocks blobId = blobIdentifierWithBlocks;
      BlobReference blobReference = new BlobReference(new KeepUntilBlobReference(defaultTimeProvider.Now + TimeSpan.FromHours(1.0)));
      if (!await contentBlobStore.TryReferenceWithBlocksAsync(blobId, blobReference))
        throw PackageContentBlobNotFoundException.Create(blobIdentifierWithBlocks.BlobId.ValueString);
      NullResult nullResult = await ingester.Handle(new PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>(feed, new PackageIngestionFormData((IReadOnlyDictionary<string, string[]>) blobRequest.PackageMetadata, new PackageFileStream(blobRequest.FileName, blobRequest.Length, (Stream) null, blobIdentifierWithBlocks.BlobId), (PackageFileStream) null), "1"));
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
      feed = (IFeedRequest) null;
      ingester = (IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, NullResult>) null;
      blobIdentifierWithBlocks = (BlobIdentifierWithBlocks) null;
      return httpResponseMessage;
    }
  }
}
