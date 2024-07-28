// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CustomerSupportRequestExtensions
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class CustomerSupportRequestExtensions
  {
    public static void ValidateSpecificRequestFields(
      this CustomerSupportRequest request,
      IVssRequestContext requestContext)
    {
      requestContext.GetService<PublishedExtensionService>();
      switch (request.SourceLink)
      {
        case "extensionDetailsPage":
          ArgumentUtility.CheckStringForNullOrWhiteSpace(request.ExtensionName, "ExtensionName");
          ArgumentUtility.CheckStringForNullOrWhiteSpace(request.PublisherName, "PublisherName");
          ArgumentUtility.CheckForNull<Uri>(request.ExtensionURL, "ExtensionURL");
          if (!SupportRequestConstants.PossibleExtensionDetailsPageReasons.Any<string>((Func<string, bool>) (reason => string.Equals(reason, request.Reason, StringComparison.Ordinal))))
            throw new ArgumentException(GalleryResources.InvalidCustomerSupportRequestReason());
          PublishedExtension andExtensionName1 = PublishedExtensionService.GetExtensionByPublisherNameAndExtensionName(requestContext, request.PublisherName, request.ExtensionName);
          if (andExtensionName1 == null)
            throw new ArgumentException(GalleryResources.ItemDoesNotExistError((object) request.PublisherName, (object) request.ExtensionName));
          if (andExtensionName1.IsVsOrVsCodeOrVsForMacExtension())
            break;
          throw new ArgumentException(GalleryResources.ExtensionTypeNotVsOrVsCodeOrVsForMac());
        case "publisherManagementPage":
          if (requestContext.GetUserIdentity() == null)
            throw new AnonymousCustomerSupportRequestException(GalleryResources.CannotCreateSupportRequestAnonymously());
          ArgumentUtility.CheckStringForNullOrWhiteSpace(request.PublisherName, "PublisherName");
          if (SupportRequestConstants.PossiblePublisherManagementPageReasons.Any<string>((Func<string, bool>) (reason => string.Equals(reason, request.Reason, StringComparison.Ordinal))))
            break;
          throw new ArgumentException(GalleryResources.InvalidCustomerSupportRequestReason());
        case "appealReview":
          if (requestContext.GetUserIdentity() == null)
            throw new AnonymousCustomerSupportRequestException(GalleryResources.CannotCreateSupportRequestAnonymously());
          ArgumentUtility.CheckForEmptyGuid(request.Review.UserId, "UserId");
          ArgumentUtility.CheckStringForNullOrWhiteSpace(request.Review.Text, "Text");
          ArgumentUtility.CheckForNull<Uri>(request.ExtensionURL, "ExtensionURL");
          ArgumentUtility.CheckStringForNullOrWhiteSpace(request.PublisherName, "PublisherName");
          if (!SupportRequestConstants.PossibleAppealReviewPageReasons.Any<string>((Func<string, bool>) (reason => string.Equals(reason, request.Reason, StringComparison.Ordinal))))
            throw new ArgumentException(GalleryResources.InvalidCustomerSupportRequestReason());
          PublishedExtension andExtensionName2 = PublishedExtensionService.GetExtensionByPublisherNameAndExtensionName(requestContext, request.PublisherName, request.ExtensionName);
          if (andExtensionName2 == null)
            throw new ArgumentException(GalleryResources.ItemDoesNotExistError((object) request.PublisherName, (object) request.ExtensionName));
          if (!andExtensionName2.IsVsOrVsCodeOrVsForMacExtension())
            throw new ArgumentException(GalleryResources.ExtensionTypeNotVsOrVsCodeOrVsForMac());
          GallerySecurity.CheckExtensionPermission(requestContext, andExtensionName2, (string) null, PublisherPermissions.UpdateExtension, false);
          break;
        case "footer":
          if (SupportRequestConstants.PossibleFooterReasons.Any<string>((Func<string, bool>) (reason => string.Equals(reason, request.Reason, StringComparison.Ordinal))))
            break;
          throw new ArgumentException(GalleryResources.InvalidCustomerSupportRequestReason());
        default:
          throw new ArgumentException(GalleryResources.InvalidCustomerSupportRequestSource());
      }
    }
  }
}
