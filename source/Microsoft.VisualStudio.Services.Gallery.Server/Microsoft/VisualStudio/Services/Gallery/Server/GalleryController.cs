// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;
using System.Net;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public abstract class GalleryController : TfsApiController
  {
    public override string ActivityLogArea => "Gallery";

    public override string TraceArea => "Gallery";

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      if (this.AllowAssetDomain)
        return;
      PublisherAssetConfiguration configuration = this.TfsRequestContext.GetService<IPublisherAssetService>().GetConfiguration(this.TfsRequestContext);
      if (!string.IsNullOrEmpty(configuration.Host) && controllerContext.Request.RequestUri.Host.EndsWith(configuration.Host, StringComparison.OrdinalIgnoreCase))
        throw new NotSupportedException("Method not supported through the asset domain");
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<PublisherDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<PublisherExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<ExtensionDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ExtensionExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<ExtensionAssetNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<InvalidPackageFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CharacterLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<VersionMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FileFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UnauthorizedAccessException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<VersionNotIncrementedException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<InvalidExtensionQueryException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionSizeExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<MethodNotAvailableException>(HttpStatusCode.ServiceUnavailable);
      exceptionMap.AddStatusCode<VerificationLogNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ExtensionDraftNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ExtensionLinkTypeDisabledException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionDraftTooManyEditsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionAssetCountLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PublisherMetadataLengthExceededMaxLimitException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PublisherMetadataSerializationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PublisherMetadataDeserializationException>(HttpStatusCode.InternalServerError);
      exceptionMap.AddStatusCode<InvalidPackageException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PackageValidationWarningException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionScopesChangedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BlockedHostValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PublisherSpamValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PublisherLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UrlsNotAllowedinPublisherProfileException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidReCaptchaTokenException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionVersionAlreadyExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PublisherDomainDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TokenNotFoundInDnsTxtRecordsException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DuplicateExtensionNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionCreationLimitExceedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DuplicateExtensionDisplayNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SimilarExtensionDisplayNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SimilarExtensionNameException>(HttpStatusCode.BadRequest);
    }

    protected void ConfirmHostedDeployment(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new NotSupportedException(GalleryResources.OnPremisesNotSupported());
    }

    protected virtual bool AllowAssetDomain => false;
  }
}
