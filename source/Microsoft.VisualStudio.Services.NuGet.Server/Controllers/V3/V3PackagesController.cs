// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3.V3PackagesController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "packages", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class V3PackagesController : NuGetApiController
  {
    [HttpPost]
    [ControllerMethodTraceFilter(5721000)]
    public async Task<HttpResponseMessage> AddPackageFromBlobStoreAsync(
      string feedId,
      [FromBody] AddPackageFromBlobRequestInternal blobRequest,
      bool? commitLogWrite = null,
      bool? noLegacyCatalog = null)
    {
      V3PackagesController packagesController = this;
      if (packagesController.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_NotSupportedOnPrem());
      if (blobRequest == null)
        throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PushRequestMissingBlob());
      IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = packagesController.GetFeedRequest(feedId, readOnlyValidator);
      IPackageIngestionService service = packagesController.TfsRequestContext.GetService<IPackageIngestionService>();
      if (!string.IsNullOrWhiteSpace(blobRequest.DropName))
      {
        FeatureEnabledConstants.EnableNuGetLargePackages.Bootstrap(packagesController.TfsRequestContext).RequireFeatureEnabled();
        if (!string.IsNullOrEmpty(blobRequest.BlobId) || blobRequest.Blob != (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob) null)
          throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PushCantUseBothBlobAndDrop());
        await service.AddPackageFromDropAsync(packagesController.TfsRequestContext, feedRequest, blobRequest.DropName, "vsopush");
      }
      else
      {
        BlobIdentifierWithOrWithoutBlocks blobIdentifier = packagesController.GetBlobIdentifier(blobRequest);
        await service.AddPackageFromBlobAsync(packagesController.TfsRequestContext, feedRequest, blobIdentifier, "vsopush", (IEnumerable<UpstreamSourceInfo>) Array.Empty<UpstreamSourceInfo>(), false);
      }
      return packagesController.Request.CreateResponse(HttpStatusCode.Accepted);
    }

    private BlobIdentifierWithOrWithoutBlocks GetBlobIdentifier(
      AddPackageFromBlobRequestInternal blobRequest)
    {
      if (string.IsNullOrWhiteSpace(blobRequest.BlobId) && blobRequest.Blob == (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob) null)
        throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PushRequestMissingBlob());
      if (!string.IsNullOrWhiteSpace(blobRequest.BlobId) && blobRequest.Blob != (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob) null)
        throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PushCantUseBothBlobAndBlobId());
      return !(blobRequest.Blob != (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob) null) ? new BlobIdentifierWithOrWithoutBlocks(this.DeserializeBlobId(blobRequest.BlobId)) : new BlobIdentifierWithOrWithoutBlocks(this.DeserializeBlob(blobRequest.Blob));
    }

    private Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks DeserializeBlob(
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob blob)
    {
      try
      {
        return blob.ToBlobIdentifierWithBlocks();
      }
      catch (ArgumentException ex)
      {
        throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PushValidBlobIdAndBlockHashesAreRequired(), (Exception) ex);
      }
      catch (InvalidDataException ex)
      {
        throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_BlockListDoesntMatchBlobId(), (Exception) ex);
      }
    }

    private BlobIdentifier DeserializeBlobId(string blobId)
    {
      try
      {
        return BlobIdentifier.Deserialize(blobId);
      }
      catch (ArgumentException ex)
      {
        throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_InvalidBlobId(), (Exception) ex);
      }
    }
  }
}
