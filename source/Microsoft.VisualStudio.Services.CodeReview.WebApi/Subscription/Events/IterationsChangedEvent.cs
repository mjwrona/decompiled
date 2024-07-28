// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events.IterationsChangedEvent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events
{
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-code.codereview-iteration-changed-event")]
  public class IterationsChangedEvent : CodeReviewEvent
  {
    public IterationsChangedEvent()
    {
    }

    public IterationsChangedEvent(
      CodeReviewEventNotification codeReviewNotification,
      Review review,
      string reviewEventType)
      : base(codeReviewNotification, review, reviewEventType)
    {
      this.ChangedFiles = this.GetSortedChangeListByModifiedPath();
      if (this.ChangedFiles.Count <= 25)
        return;
      this.MoreChangesExist = true;
    }

    public override void UpdateEventTriggeredBy()
    {
      if (this.CodeReview.Iterations != null && this.CodeReview.Iterations.Count > 0)
        this.EventTriggeredBy = this.CodeReview.Iterations.Last<Iteration>().Author;
      else
        base.UpdateEventTriggeredBy();
    }

    [DataMember]
    public List<ChangeEntry> ChangedFiles { get; set; }

    [DataMember]
    public bool MoreChangesExist { get; set; }
  }
}
