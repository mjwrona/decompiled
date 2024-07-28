// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CodeReviewConstants
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [GenerateAllConstants(null)]
  public static class CodeReviewConstants
  {
    public static readonly Guid ReviewsResourceId = new Guid("{EAA8EC98-2B9C-4730-96A3-4845BE1558D6}");
    public const string ReviewsResourceIdString = "EAA8EC98-2B9C-4730-96A3-4845BE1558D6";
    public static readonly Guid AccountLevelReviewsResourceId = new Guid("{D17478C8-387D-4359-BA97-1414AE770B76}");
    public const string AccountLevelReviewsResourceIdString = "D17478C8-387D-4359-BA97-1414AE770B76";
    public const string ReviewsResourceName = "reviews";
    public static readonly Guid ReviewsBatchResourceId = new Guid("{16B3F95B-5BA6-4F64-A2DB-1A03DE11D3BC}");
    public const string ReviewsBatchResourceName = "reviewsBatch";
    public static readonly Guid ReviewersResourceId = new Guid("{9B1869EC-B17F-4EFD-8597-8C89362F2063}");
    public const string ReviewersResourceName = "reviewers";
    public static readonly Guid ReviewerStatesResourceId = new Guid("{B324A000-5032-466C-830D-BF336C2552B4}");
    public const string StatesResourceName = "reviewerStates";
    public static readonly Guid PingResourceId = new Guid("{0477B7BF-E201-4C59-9793-2814AAB2B6C9}");
    public const string PingResourceName = "ping";
    public static readonly Guid IterationsResourceId = new Guid("{D2E77B94-A8C8-45E6-A163-7F1B4AE20EB9}");
    public const string IterationsResourceName = "iterations";
    public static readonly Guid ChangesResourceId = new Guid("{A4C0C4D0-B0ED-4A6F-8751-F32C7444580E}");
    public const string ChangesResourceName = "changes";
    public static readonly Guid ChangesContentResourceId = new Guid("{38F9AD45-10BC-4C0A-99AD-BEAAA51CA027}");
    public const string ChangesContentResourceName = "contents";
    public static readonly Guid ReviewCommentThreadsResourceId = new Guid("{1E0BB4EC-0587-42D8-A005-3815555E766A}");
    public const string ReviewCommentThreadsResourceName = "threads";
    public static readonly Guid ReviewCommentsResourceId = new Guid("{FAC703B5-FB23-4ABF-8D90-09DE88CD1293}");
    public const string ReviewCommentsResourceName = "comments";
    public static readonly Guid LikesResourceId = new Guid("{BA6F5F68-A41C-44E7-BFA2-B1FADF1E6B91}");
    public const string LikesResourceName = "likes";
    public static readonly Guid ProviderPropertyKind = new Guid("{113D5F8B-1C3E-4FD8-ADAB-57651B239DB5}");
    public const string ProviderResourceName = "provider";
    public static readonly Guid ContentsBatchResourceId = new Guid("{4FCD8BD9-2B3C-482D-829A-592369F47277}");
    public const string ContentsBatchResourceName = "contentsBatch";
    public static readonly Guid AttachmentsResourceId = new Guid("{9D61AC01-EAD6-429F-BC4D-1C18882D27C4}");
    public const string AttachmentsResourceName = "attachments";
    public static readonly Guid ReviewStatusesResourceId = new Guid("{502D7933-25DE-42E3-BC82-8478B3796655}");
    public const string ReviewStatusesResourceIdString = "502D7933-25DE-42E3-BC82-8478B3796655";
    public static readonly Guid IterationStatusesResourceId = new Guid("{CB958C49-F702-483A-BB3B-3454570FB72A}");
    public const string IterationStatusesResourceIdString = "CB958C49-F702-483A-BB3B-3454570FB72A";
    public const string StatusesResourceName = "statuses";
    public static readonly Guid ReviewPropertiesResourceId = new Guid("{7CF0E9A4-CCD5-4D63-9C52-5241A213C3FE}");
    public const string ReviewPropertiesResourceIdString = "7CF0E9A4-CCD5-4D63-9C52-5241A213C3FE";
    public static readonly Guid IterationPropertiesResourceId = new Guid("1031EA92-06F3-4550-A310-8BB3059B92FF");
    public const string IterationPropertiesResourceIdString = "1031EA92-06F3-4550-A310-8BB3059B92FF";
    public const string PropertiesResourceName = "properties";
    public static readonly Guid ReviewSettingsResourceId = new Guid("6A11B750-D84C-4F84-B96D-23526F716576");
    public const string ReviewSettingsResourceName = "settings";
    public static readonly Guid ShareReviewResourceId = new Guid("{EB58030E-C39B-41B1-9E1F-72E23B032FB4}");
    public const string ShareReviewResourceIdString = "EB58030E-C39B-41B1-9E1F-72E23B032FB4";
    public const string ShareReviewResourceName = "share";
    public static readonly Guid ArtifactVisitsResourceId = new Guid("{C4BC78AB-8D09-4B62-98F2-EFB1AFFE50F8}");
    public const string ArtifactVisitsResourceIdString = "C4BC78AB-8D09-4B62-98F2-EFB1AFFE50F8";
    public const string ArtifactVisitsResourceName = "artifactVisits";
    public static readonly Guid ArtifactVisitsBatchResourceId = new Guid("{D1786677-7A19-445B-9A7A-25728F48D149}");
    public const string ArtifactVisitsBatchResourceIdString = "D1786677-7A19-445B-9A7A-25728F48D149";
    public const string ArtifactVisitsBatchResourceName = "artifactVisitsBatch";
    public static readonly Guid ArtifactStatsBatchResourceId = new Guid("{2D358C96-88CC-42BA-9B5D-A2CB26C64972}");
    public const string ArtifactStatsBatchResourceIdString = "2D358C96-88CC-42BA-9B5D-A2CB26C64972";
    public const string ArtifactStatsBatchResourceName = "artifactStatsBatch";
    public const string ProjectScopeRouteKey = "project";
    public const string ProjectScopeTemplate = "{project}";
    public const string AreaId = "997A4743-5B0E-424B-AAFA-37B62A3E1DBF";
    public const string AreaName = "CodeReview";
    public const string VisitsAreaId = "D69BCC31-8EB7-42A6-B1B8-B52E91062597";
    public const string VisitsAreaName = "visits";
    internal const string InstanceType = "00000023-0000-8888-8000-000000000000";
    public const string JsonPatchMediaType = "application/json-patch+json";
    public const int MinReviewId = 1;
    public const int MinIterationId = 1;
    public const int MinChangeTrackingId = 1;
    public const int MinChangeId = 1;
    public const int MinAttachmentId = 1;
    public const int MinStatusId = 1;
    public const string DefaultDownloadFileName = "ReviewFile";
    public const int MaxNumberOfChanges = 2000;
    internal const int MaxChangesForChangeTrackingComputation = 10000;
    public const int MaxChangesForListingInEmail = 25;
    internal const int MaxPriorIterationsForChangeTrackingComputation = 5;
    internal const int MaxCommentsPerDiscussionThread = 500;
  }
}
