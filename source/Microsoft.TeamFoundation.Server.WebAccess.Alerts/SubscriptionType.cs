// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.SubscriptionType
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  public enum SubscriptionType
  {
    Unknown,
    WorkItemChangedEvent,
    CheckinEvent,
    BuildCompletionEvent,
    BuildCompletionEvent2,
    BuildStatusChangeEvent,
    BuildResourceChangedEvent,
    CodeReviewChangedEvent,
    BuildCompletedEvent,
    GitPushEvent,
    GitPullRequestEvent,
    GitPullRequestCommentEvent,
  }
}
