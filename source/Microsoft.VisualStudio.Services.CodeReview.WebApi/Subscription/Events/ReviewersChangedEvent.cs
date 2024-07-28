// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events.ReviewersChangedEvent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events
{
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-code.codereview-reviewers-changed-event")]
  public class ReviewersChangedEvent : CodeReviewEvent
  {
    public ReviewersChangedEvent()
    {
    }

    public ReviewersChangedEvent(
      CodeReviewEventNotification codeReviewNotification,
      Review review,
      string reviewEventType)
      : base(codeReviewNotification, review, reviewEventType)
    {
      this.ChangedReviewers = ReviewersChangedEvent.GetChangedReviewers(codeReviewNotification).ToList<Guid>();
    }

    [DataMember]
    public List<Guid> ChangedReviewers { get; set; }

    public override void AddActors(VssNotificationEvent notificationEvent)
    {
      base.AddActors(notificationEvent);
      if (this.ChangedReviewers == null || this.ChangedReviewers.Count <= 0)
        return;
      foreach (Guid changedReviewer in this.ChangedReviewers)
        notificationEvent.AddActor(CodeReviewEvent.Roles.ChangedReviewers, changedReviewer);
    }

    private static IEnumerable<Guid> GetChangedReviewers(CodeReviewEventNotification notification)
    {
      if (!(notification is ReviewersNotification reviewersNotification))
        return Enumerable.Empty<Guid>();
      List<Guid> guidList = new List<Guid>();
      return (IEnumerable<Guid>) reviewersNotification.Reviewers.Select<Reviewer, Guid>((Func<Reviewer, Guid>) (x => new Guid(x.Identity.Id))).ToList<Guid>();
    }
  }
}
