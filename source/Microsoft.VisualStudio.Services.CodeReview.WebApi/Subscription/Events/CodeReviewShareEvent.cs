// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events.CodeReviewShareEvent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events
{
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-code.codereview-shared-event")]
  public class CodeReviewShareEvent : CodeReviewEvent
  {
    public CodeReviewShareEvent()
    {
    }

    public CodeReviewShareEvent(ShareReviewNotification shareNotification, string reviewEventType)
      : base((CodeReviewEventNotification) shareNotification, shareNotification.Review, reviewEventType)
    {
      this.ShareContext = shareNotification.ShareMessage;
      this.Receivers = new List<Guid>();
      foreach (IdentityRef receiver in this.ShareContext.Receivers)
      {
        if (!shareNotification.Review.Author.Id.Equals(receiver.ToString(), StringComparison.OrdinalIgnoreCase))
          this.Receivers.Add(new Guid(receiver.Id));
      }
      this.ExternalToolContext = shareNotification.ExternalToolContext;
    }

    [DataMember]
    public List<Guid> Receivers { get; set; }

    [DataMember]
    public NotificationContext ShareContext { get; set; }

    public override void AddActors(VssNotificationEvent notificationEvent)
    {
      base.AddActors(notificationEvent);
      foreach (Guid receiver in this.Receivers)
        notificationEvent.AddActor(CodeReviewEvent.Roles.Receiver, receiver);
    }
  }
}
