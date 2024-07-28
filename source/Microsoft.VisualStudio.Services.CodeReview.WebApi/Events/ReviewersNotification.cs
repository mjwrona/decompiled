// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Events.ReviewersNotification
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Events
{
  [DataContract]
  public class ReviewersNotification : CodeReviewEventNotification
  {
    public ReviewersNotification(
      Guid projectId,
      int reviewId,
      string sourceArtifactId,
      IEnumerable<Reviewer> reviewers,
      DateTime? priorReviewUpdatedTimestamp,
      DateTime? latestReviewUpdatedTimestamp,
      EventType changeAction)
      : base(projectId, reviewId, sourceArtifactId, priorReviewUpdatedTimestamp, !latestReviewUpdatedTimestamp.HasValue ? (reviewers == null || !reviewers.Any<Reviewer>() ? new DateTime?() : reviewers.First<Reviewer>().ModifiedDate) : latestReviewUpdatedTimestamp)
    {
      this.Reviewers = reviewers;
      this.ChangeAction = changeAction;
    }

    [DataMember]
    public IEnumerable<Reviewer> Reviewers { get; private set; }

    [DataMember]
    public EventType ChangeAction { get; private set; }
  }
}
