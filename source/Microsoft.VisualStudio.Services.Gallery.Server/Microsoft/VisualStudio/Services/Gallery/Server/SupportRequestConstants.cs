// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SupportRequestConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class SupportRequestConstants
  {
    public const string BEARER = "Bearer";
    public const string USER_AGENT = "User-Agent";
    public const string DMCA_CLAIM = "DMCA claim";
    public const string MALICIOUS_OR_VIRUS_EXTENSION = "Malicious/Virus extension";
    public const string TOP_PUBLISHER_REQUEST = "Top publisher request";
    public const string UPDATE_PUBLISHER_DISPLAY_NAME = "Update publisher display name";
    public const string LOCK_OR_REMOVE_EXTENSIONS = "Lock or remove an extension";
    public const string DELETE_PUBLISHER_ACCOUNT = "Delete publisher account";
    public const string APPEAL_REVIEW_OR_QANDA = "Appeal Review or Q and A";
    public const string AUTHETNICATION_ISSUE = "Authentication Issue";
    public const string WEBSITE_USABILITY_ISSUE = "Website usability Issue";
    public const string MISSING_PAGE = "Missing Page (404)";
    public const string OTHER = "Other";
    public const string UNABLE_TO_DOWNLOAD_EXTENSION = "Unable to download the extension";
    public const string UNABLE_TO_ADD_MEMBER = "Unable to add a member";
    public const string NON_SIGNED_IN_USER = "NonSignedIn User";
    public const string DMCA_CLAIM_Label = "DMCA";
    public const string MALICIOUS_OR_VIRUS_EXTENSION_Label = "malicious";
    public const string TOP_PUBLISHER_REQUEST_Label = "top-publisher";
    public const string UPDATE_PUBLISHER_DISPLAY_NAME_Label = "displayname-change";
    public const string LOCK_OR_REMOVE_EXTENSIONS_Label = "extension-management";
    public const string DELETE_PUBLISHER_ACCOUNT_Label = "delete-publisher";
    public const string APPEAL_REVIEW_OR_QANDA_Label = "appeal-review";
    public const string AUTHETNICATION_ISSUE_Label = "authentication";
    public const string WEBSITE_USABILITY_ISSUE_Label = "website";
    public const string MISSING_PAGE_Label = "missing-page";
    public const string OTHER_Label = "other";
    public const string UNABLE_TO_DOWNLOAD_EXTENSION_Label = "unable-to-download-extension";
    public const string UNABLE_TO_ADD_MEMBER_Label = "unable-to-add-member";
    public static Dictionary<string, string> labelToReasonMap = new Dictionary<string, string>()
    {
      {
        "DMCA",
        "DMCA claim"
      },
      {
        "malicious",
        "Malicious/Virus extension"
      },
      {
        "top-publisher",
        "Top publisher request"
      },
      {
        "displayname-change",
        "Update publisher display name"
      },
      {
        "extension-management",
        "Lock or remove an extension"
      },
      {
        "delete-publisher",
        "Delete publisher account"
      },
      {
        "appeal-review",
        "Appeal Review or Q and A"
      },
      {
        "authentication",
        "Authentication Issue"
      },
      {
        "website",
        "Website usability Issue"
      },
      {
        "missing-page",
        "Missing Page (404)"
      },
      {
        "other",
        "Other"
      },
      {
        "unable-to-download-extension",
        "Unable to download the extension"
      },
      {
        "unable-to-add-member",
        "Unable to add a member"
      }
    };
    public const string EXTENSION_DETAILS_PAGE = "extensionDetailsPage";
    public const string PUBLISHER_MANAGEMENT_PAGE = "publisherManagementPage";
    public const string APPEAL_REVIEW_PAGE = "appealReview";
    public const string FOOTER = "footer";
    public static List<string> PossibleExtensionDetailsPageReasons = new List<string>()
    {
      "unable-to-download-extension",
      "DMCA",
      "malicious",
      "other"
    };
    public static List<string> PossiblePublisherManagementPageReasons = new List<string>()
    {
      "top-publisher",
      "unable-to-add-member",
      "displayname-change",
      "extension-management",
      "delete-publisher",
      "other"
    };
    public static List<string> PossibleFooterReasons = new List<string>()
    {
      "authentication",
      "website",
      "missing-page",
      "other"
    };
    public static List<string> PossibleAppealReviewPageReasons = new List<string>()
    {
      "appeal-review"
    };
    public const string GALLERY_CSR_GITHUB_REPO_PAT_TOKEN = "GalleryCSRGitHubPATToken";
    public const string GALLERY_CSR_STORAGE_CONNECTION_STRING = "GalleryCSRStorageConnectionString";
    public const string CSR_BLOB_CONTAINER_NAME = "customersupportblobcontainer";
    public const string CREATE_SUPPORT_REQUEST_CREATE_ACTION = "Created";
    public const string CRS_TICKET_CREATION_FAILED = "Failed";
    public const string CRS_TICKET_CREATION_SUCCESS = "Success";
    public const string CSR_GITHUB_ISSUE_DESCRIPTION = "Please find the blob URL for the ticket: ";
  }
}
