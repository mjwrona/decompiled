// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ReviewExtensions
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public static class ReviewExtensions
  {
    public static Review ShallowClone(this Review toClone, bool topLevelOnly = true) => new Review()
    {
      Id = toClone.Id,
      Author = toClone.Author,
      CompletedDate = toClone.CompletedDate,
      CreatedDate = toClone.CreatedDate,
      Description = toClone.Description,
      Properties = toClone.Properties,
      Status = toClone.Status,
      Title = toClone.Title,
      UpdatedDate = toClone.UpdatedDate,
      SourceArtifactId = toClone.SourceArtifactId,
      IsDeleted = toClone.IsDeleted,
      Reviewers = topLevelOnly ? (IList<Reviewer>) null : toClone.Reviewers,
      Iterations = topLevelOnly ? (IList<Iteration>) null : toClone.Iterations
    };

    public static bool IsReadOnly(this Review review)
    {
      ReviewStatus? status1 = review.Status;
      ReviewStatus reviewStatus1 = ReviewStatus.Abandoned;
      if (status1.GetValueOrDefault() == reviewStatus1 & status1.HasValue)
        return true;
      ReviewStatus? status2 = review.Status;
      ReviewStatus reviewStatus2 = ReviewStatus.Completed;
      return status2.GetValueOrDefault() == reviewStatus2 & status2.HasValue;
    }

    public static bool CanUpdate(this Review review)
    {
      ReviewStatus? status1 = review.Status;
      ReviewStatus reviewStatus1 = ReviewStatus.Active;
      if (status1.GetValueOrDefault() == reviewStatus1 & status1.HasValue)
        return true;
      ReviewStatus? status2 = review.Status;
      ReviewStatus reviewStatus2 = ReviewStatus.Creating;
      return status2.GetValueOrDefault() == reviewStatus2 & status2.HasValue;
    }

    public static Iteration ShallowClone(this Iteration toClone) => new Iteration()
    {
      Id = toClone.Id,
      Author = toClone.Author,
      CreatedDate = toClone.CreatedDate,
      Description = toClone.Description,
      IsUnpublished = toClone.IsUnpublished,
      UpdatedDate = toClone.UpdatedDate,
      Properties = toClone.Properties
    };

    public static bool HasPublishedIteration(this Review review) => review.Iterations != null && review.Iterations.Any<Iteration>((Func<Iteration, bool>) (iteration => !iteration.IsUnpublished));

    public static Status ShallowClone(this Status status) => new Status()
    {
      Author = status.Author,
      Context = status.Context,
      CreatedDate = status.CreatedDate,
      Description = status.Description,
      Id = status.Id,
      IterationId = status.IterationId,
      Links = status.Links,
      Properties = status.Properties,
      PropertyId = status.PropertyId,
      ReviewId = status.ReviewId,
      State = status.State,
      TargetUrl = status.TargetUrl,
      UpdatedDate = status.UpdatedDate
    };
  }
}
