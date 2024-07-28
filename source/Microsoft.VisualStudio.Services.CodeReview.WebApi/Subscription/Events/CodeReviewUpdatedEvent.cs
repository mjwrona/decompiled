// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events.CodeReviewUpdatedEvent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events
{
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-code.codereview-updated-event")]
  public class CodeReviewUpdatedEvent : CodeReviewEvent
  {
    public CodeReviewUpdatedEvent()
    {
    }

    public CodeReviewUpdatedEvent(
      CodeReviewEventNotification codeReviewNotification,
      Review review,
      string reviewEventType)
      : base(codeReviewNotification, review, reviewEventType)
    {
    }
  }
}
