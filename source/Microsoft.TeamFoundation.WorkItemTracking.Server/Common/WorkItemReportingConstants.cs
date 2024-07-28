// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemReportingConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WorkItemReportingConstants
  {
    public const string Root = "/Service/WorkItemTracking/Settings/Reporting/";
    public const string All = "/Service/WorkItemTracking/Settings/Reporting/*";
    public const string RevisionsApiBigLoopBatchSize = "/Service/WorkItemTracking/Settings/Reporting/RevisionsApiBigLoopBatchSize";
    public const string RevisionsApiSmallLoopBatchSize = "/Service/WorkItemTracking/Settings/Reporting/RevisionsApiSmallLoopBatchSize";
    public const string LinksApiBigLoopBatchSize = "/Service/WorkItemTracking/Settings/Reporting/LinksApiBigLoopBatchSize";
    public const string LinksApiSmallLoopBatchSize = "/Service/WorkItemTracking/Settings/Reporting/LinksApiSmallLoopBatchSize";
    public const int RevisionsApiBigLoopDefaultBatchSize = 1000;
    public const int RevisionsApiBigLoopMaxBatchSize = 1000;
    public const int RevisionsApiBigLoopMinBatchSize = 1;
    public const int RevisionsApiSmallLoopDefaultBatchSize = 200;
    public const int RevisionsApiSmallLoopMaxBatchSize = 200;
    public const int RevisionsApiSmallLoopMinBatchSize = 200;
    public const int LinksApiBigLoopDefaultBatchSize = 1000;
    public const int LinksApiSmallLoopDefaultBatchSize = 200;
  }
}
