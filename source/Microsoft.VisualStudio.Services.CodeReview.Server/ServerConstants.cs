// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ServerConstants
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal static class ServerConstants
  {
    public static readonly Guid ProviderPropertyKind = new Guid("{113D5F8B-1C3E-4FD8-ADAB-57651B239DB5}");
    public static readonly Guid ReviewPropertyKind = new Guid("{1E0DD125-D16B-464D-A29F-077C79D2B17A}");
    public static readonly Guid IterationPropertyKind = new Guid("{5EFA40F5-1F37-4775-A729-B693C7ED42F1}");
    public static readonly Guid AttachmentPropertyKind = new Guid("{5B3AFE61-F95D-4412-B9F8-5241A9C6203F}");
    public static readonly Guid ReviewSettingPropertyKind = new Guid("{BCA4161E-BF9F-4BD6-8409-D90E37424854}");
    public static readonly Guid ReviewStatusPropertyKind = new Guid("{77ED146E-BB19-4F74-875C-01D160EEBB0B}");
    internal const string CommentTrackingKeyPath = "/CodeReview/CommentTracking/";
    internal const string CommentTrackingFileLimitKeyPath = "/CodeReview/CommentTracking/FileLimit";
    internal const string CommentTrackingJobFileLimitKeyPath = "/CodeReview/CommentTracking/JobFileLimit";
    internal const string CommentTrackingFileSizeLimitKeyPath = "/CodeReview/CommentTracking/FileSizeLimit";
    internal const string ShareReceiversLimitKeyPath = "/CodeReview/CommentTracking/ShareReceiversLimit";
    internal const string ShareMessageLengthLimitKeyPath = "/CodeReview/CommentTracking/ShareMessageLimit";
    internal const string ShareSubjectLengthLimitKeyPath = "/CodeReview/CommentTracking/ShareSubjectLimit";
    internal const int CommentTrackingDefaultFileLimit = 20;
    internal const int CommentTrackingDefaultJobFileLimit = 1000;
    internal const int CommentTrackingDefaultFileSizeLimit = 6000000;
    internal const string BatchUploadKeyPath = "/CodeReview/BatchUpload/";
    internal const string BatchUploadMaxIndividualFileSizeKeyPath = "/CodeReview/BatchUpload/MaxIndividualFileSize";
    internal const string BatchUploadMaxZipFileSizeKeyPath = "/CodeReview/BatchUpload/MaxZipFileSize";
    internal const string StatusesKeyPath = "/CodeReview/Statuses/";
    internal const string StatusesTopReviewsForGetLatest = "/CodeReview/Statuses/TopReviewsForGetLatest";
    internal const string StatusesTopStatusesForGetLatest = "/CodeReview/Statuses/TopStatusesForGetLatest";
    internal const string StatusesMaxCodeReviewStatusCountKeyPath = "/CodeReview/Statuses/MaxCodeReviewStatusCount";
    internal const int StatusesTopReviewsForGetLatestDefault = 1000;
    internal const int StatusesTopStatusesForGetLatestDefault = 2000;
    internal const int MaxIndividualFileSizeInBytesDefault = 5242880;
    internal const int MaxZipPackageFileSizeInBytesDefault = 6291456;
    internal const int DefaultBufferSize = 81920;
    public static readonly int maxChangeListForPushNotification = 100;
    public const string CodeReviewStatusPropertyIdCounter = "CodeReviewStatusPropertyId";
  }
}
