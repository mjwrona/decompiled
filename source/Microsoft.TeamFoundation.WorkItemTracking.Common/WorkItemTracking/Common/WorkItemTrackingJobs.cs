// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.WorkItemTrackingJobs
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WorkItemTrackingJobs
  {
    public static readonly Guid WorkItemReclassification = new Guid("D3FD3F27-A199-49BF-AE54-F1BC1F6DD781");
    public static readonly Guid IdentitySync = new Guid("69AD5827-6346-4B08-B29D-2EE8BE361F85");
    public static readonly Guid RemoteLinking = new Guid("990C015A-B965-4100-B37B-2F16B99905F2");
    public static readonly Guid RemoteLinkingProjectDelete = new Guid("DFF02F4D-C946-4419-AE57-3E7FB772B8E6");
    public static readonly Guid GitHubMentionProcessing = new Guid("B53FA2CE-C5F9-4D4A-A3C6-07A31606C91C");
    public static readonly Guid GitHubArtifactStorage = new Guid("2E8D0CF4-E004-4A2A-9CC7-33E8601C0E42");
    public static readonly Guid UpdateViolations = new Guid("760C015B-B965-4100-B37B-2F16B99905F2");
    public static readonly Guid TemporaryQueryCleanup = new Guid("3A521818-EC38-4351-9B9D-1AA2CDFDDC8D");
  }
}
