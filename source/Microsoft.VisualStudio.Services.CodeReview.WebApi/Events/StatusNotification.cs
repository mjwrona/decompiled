// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Events.StatusNotification
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Events
{
  [DataContract]
  public class StatusNotification : CodeReviewEventNotification
  {
    public StatusNotification(
      Guid projectId,
      int reviewId,
      int? iterationId,
      string sourceArtifactId,
      Status status,
      DateTime? priorReviewUpdatedTimestamp)
      : base(projectId, reviewId, sourceArtifactId, priorReviewUpdatedTimestamp, status.UpdatedDate)
    {
      this.Status = status;
      this.IterationId = iterationId;
    }

    [DataMember]
    public Status Status { get; internal set; }

    [DataMember]
    public int? IterationId { get; internal set; }
  }
}
