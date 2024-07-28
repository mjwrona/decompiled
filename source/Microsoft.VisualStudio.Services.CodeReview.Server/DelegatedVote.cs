// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.DelegatedVote
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class DelegatedVote
  {
    public int ReviewId { get; private set; }

    public Guid ReviewerId { get; private set; }

    public Guid VotedForId { get; private set; }

    public DelegatedVote(int reviewId, Guid reviewerId, Guid votedForId)
    {
      this.ReviewId = reviewId;
      this.ReviewerId = reviewerId;
      this.VotedForId = votedForId;
    }
  }
}
