// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemCommentTracePoints
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class WorkItemCommentTracePoints
  {
    private const int Base = 120000;
    public const string WorkItemCommentServiceArea = "WorkItemCommentService";
    public const string WorkItemCommentServiceLayer = "Service";
    public const int WorkItemGetCommentsServiceStart = 120001;
    public const int WorkItemGetCommentsServiceEnd = 120009;
    public const int WorkItemGetCommentsServiceException = 120010;
    public const int WorkItemAddCommentsServiceStart = 120011;
    public const int WorkItemAddCommentsServiceEnd = 120019;
    public const int WorkItemAddCommentsServiceException = 120020;
    public const int WorkItemUpdateCommentsServiceStart = 120021;
    public const int WorkItemUpdateCommentsServiceEnd = 120029;
    public const int WorkItemUpdateCommentsServiceException = 120030;
    public const int WorkItemDeleteCommentsServiceStart = 120031;
    public const int WorkItemDeleteCommentsServiceEnd = 120039;
    public const int WorkItemDeleteCommentsServiceException = 120040;
    public const int WorkItemGetCommentVersionsServiceStart = 120041;
    public const int WorkItemGetCommentVersionsServiceEnd = 120049;
    public const int WorkItemGetCommentVersionsServiceException = 120050;
    public const int WorkItemCommentUpdatedIdentityMapError = 120051;
    public const int WorkItemGetCommentServiceStart = 120061;
    public const int WorkItemGetCommentServiceEnd = 120069;
    public const int WorkItemGetCommentServiceException = 120070;
  }
}
