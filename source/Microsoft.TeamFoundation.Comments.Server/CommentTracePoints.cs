// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentTracePoints
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

namespace Microsoft.TeamFoundation.Comments.Server
{
  public static class CommentTracePoints
  {
    private const int Base = 140000;
    public const string CommentServiceArea = "CommentService";
    public const string CommentServiceLayer = "Service";
    public const string CommentDataProviderArea = "CommentDataProvider";
    public const string CommentDataProviderLayer = "DataProvider";
    public const int GetCommentsServiceStart = 140001;
    public const int GetCommentsServiceEnd = 140009;
    public const int AddCommentsServiceStart = 140011;
    public const int AddCommentsServiceEnd = 140019;
    public const int UpdateCommentsServiceStart = 140021;
    public const int UpdateCommentsServiceEnd = 140029;
    public const int DeleteCommentsServiceStart = 140031;
    public const int DeleteCommentsServiceEnd = 140039;
    public const int GetCommentVersionsServiceStart = 140041;
    public const int GetCommentVersionsServiceEnd = 140049;
    public const int GetCommentsByIdsServiceStart = 140051;
    public const int GetCommentsByIdsServiceEnd = 140059;
    public const int GetCommentServiceStart = 140061;
    public const int GetCommentServiceEnd = 140069;
    public const int GetCommentVersionServiceStart = 140071;
    public const int GetCommentVersionServiceEnd = 140072;
    public const int GetCommentsVersionsServiceStart = 140075;
    public const int GetCommentsVersionsServiceEnd = 140076;
    public const int GetMentionsByCommentIdsServiceStart = 140081;
    public const int GetMentionsByCommentIdsServiceEnd = 140089;
    public const int AddCommentServiceStart = 140091;
    public const int AddCommentServiceEnd = 140099;
    public const int DeleteCommentServiceStart = 140101;
    public const int DeleteCommentServiceEnd = 140109;
    public const int UpdateCommentServiceStart = 140121;
    public const int UpdateCommentServiceEnd = 140129;
    public const int DeleteCommentByIdServiceStart = 140101;
    public const int DeleteCommentByIdServiceEnd = 140109;
    public const int DownloadAttachmentStart = 140111;
    public const int GetCommentAttachmentStart = 140112;
    public const int GetCommentAttachmentEnd = 140113;
    public const int DownloadAttachmentEnd = 140119;
    public const int UploadAttachmentStart = 140121;
    public const int UploadAttachmentEnd = 140129;
    public const int DestroyCommentsServiceStart = 140131;
    public const int DestroyCommentsServiceEnd = 140139;
    public const int GetCommentsByIdsStart = 140201;
    public const int GetCommentsByIdsEnd = 140209;
    public const int AddCommentsStart = 140211;
    public const int AddCommentsEnd = 140219;
    public const int UpdateCommentsStart = 140221;
    public const int UpdateCommentsEnd = 140229;
    public const int DeleteCommentsStart = 140231;
    public const int DeleteCommentsEnd = 140239;
    public const int GetCommentVersionsStart = 140241;
    public const int GetCommentVersionsEnd = 140242;
    public const int GetCommentsVersionsStart = 140244;
    public const int GetCommentsVersionsEnd = 140245;
    public const int GetCommentsStart = 140251;
    public const int GetCommentsEnd = 140259;
    public const int GetCommentsWithChildrenStart = 140261;
    public const int GetCommentsWithChildrenEnd = 140269;
    public const int GetCommentsFromParentStart = 140271;
    public const int GetCommentsFromParentEnd = 140279;
    public const int GetAttachmentsStart = 140281;
    public const int GetAttachmentsEnd = 140289;
    public const int AddAttacmhentsStart = 140291;
    public const int AddAttacmhentsEnd = 140299;
    public const int TryScheduleCommentEventNotificationStart = 140281;
    public const int TryScheduleCommentEventNotificationEnd = 140282;
    public const int TryScheduleCommentEventNotificationError = 140283;
    public const int TryScheduleCommentEventNotificationFailedToQueueError = 140284;
    public const int CommentAddedEventCallBack = 140285;
    public const int CreateCommentReactionStart = 140301;
    public const int CreateCommentReactionInfo = 140302;
    public const int CreateCommentReactionEnd = 140309;
    public const int DeleteCommentReactionStart = 140311;
    public const int DeleteCommentReactionEnd = 140319;
    public const int GetEngagedUsersStart = 140321;
    public const int GetEngagedUsersEnd = 140329;
    public const int GetReactionsByCommentIdStart = 140331;
    public const int GetReactionsByCommentIdEnd = 140339;
    public const int FireCommentReactionChangedEventStart = 140343;
    public const int FireCommentReactionChangedEventEnd = 140344;
    public const int CommentsMarkDownRenderDataStart = 140351;
    public const int CommentsMarkDownRenderDataEnd = 140359;
  }
}
