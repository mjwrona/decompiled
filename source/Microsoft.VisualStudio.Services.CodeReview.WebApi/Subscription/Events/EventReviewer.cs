// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events.EventReviewer
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events
{
  [DataContract]
  public class EventReviewer : Reviewer
  {
    public EventReviewer()
    {
    }

    public EventReviewer(Reviewer reviewer)
    {
      this.Identity = reviewer.Identity;
      this.Kind = reviewer.Kind;
      this.CreatedDate = reviewer.CreatedDate;
      this.ModifiedDate = reviewer.ModifiedDate;
      this.IterationId = reviewer.IterationId;
      this.ReviewerStateId = reviewer.ReviewerStateId;
      this.VotedFor = reviewer.VotedFor;
      this.VotedForGroups = reviewer.VotedForGroups;
      this.VoteDescription = !ReviewerExtensions.IsInitialState(reviewer.ReviewerStateId.Value) ? ReviewerExtensions.GetDefaultState(reviewer.ReviewerStateId.Value).Description : string.Empty;
      this.IsRequiredKind = reviewer.Kind.GetValueOrDefault() == ReviewerKind.Required;
    }

    [DataMember]
    public string VoteDescription { get; set; }

    [DataMember]
    public bool IsRequiredKind { get; set; }
  }
}
