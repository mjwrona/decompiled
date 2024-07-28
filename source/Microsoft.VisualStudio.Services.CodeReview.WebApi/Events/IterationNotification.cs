// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Events.IterationNotification
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Events
{
  [DataContract]
  public class IterationNotification : CodeReviewEventNotification
  {
    public IterationNotification(
      Guid projectId,
      int reviewId,
      string sourceArtifactId,
      Iteration iteration,
      DateTime? priorReviewUpdatedTimestamp)
      : base(projectId, reviewId, sourceArtifactId, priorReviewUpdatedTimestamp, iteration.UpdatedDate)
    {
      this.Iteration = iteration;
    }

    [DataMember]
    public Iteration Iteration { get; internal set; }
  }
}
