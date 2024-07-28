// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewSecurityEvaluator
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class ReviewSecurityEvaluator
  {
    public static void CheckReviewAccess(
      IVssRequestContext requestContext,
      IDisposableReadOnlyList<ICodeReviewSecurityExtension> securityExtensions,
      Guid projectId,
      int reviewId,
      string artifactUri)
    {
      if (!ReviewSecurityEvaluator.HasReviewAccess(requestContext, securityExtensions, projectId, reviewId, artifactUri))
        throw new CodeReviewNotFoundException(reviewId);
    }

    public static bool HasReviewAccess(
      IVssRequestContext requestContext,
      IDisposableReadOnlyList<ICodeReviewSecurityExtension> securityExtensions,
      Guid projectId,
      int reviewId,
      string artifactUri)
    {
      ICodeReviewSecurityExtension securityExtension = ReviewSecurityEvaluator.GetSecurityExtension(securityExtensions, artifactUri);
      return securityExtension != null ? securityExtension.HasReadReviewAccess(requestContext, projectId, reviewId, artifactUri) : string.IsNullOrEmpty(artifactUri);
    }

    public static void CheckWriteReviewAccess(
      IVssRequestContext requestContext,
      IDisposableReadOnlyList<ICodeReviewSecurityExtension> securityExtensions,
      Guid projectId,
      int reviewId,
      string artifactUri)
    {
      ReviewSecurityEvaluator.GetSecurityExtension(securityExtensions, artifactUri)?.CheckWriteReviewAccess(requestContext, projectId, reviewId, artifactUri);
    }

    public static IList<Review> FetchFilteredReviews(
      IVssRequestContext requestContext,
      IDisposableReadOnlyList<ICodeReviewSecurityExtension> securityExtensions,
      IEnumerable<Review> sourceReviews)
    {
      Dictionary<int, bool> reviewsChecked = new Dictionary<int, bool>();
      Dictionary<string, List<Review>> dictionary = new Dictionary<string, List<Review>>();
      List<Review> reviewList1 = new List<Review>();
      if (sourceReviews.Any<Review>())
      {
        foreach (Review sourceReview in sourceReviews)
        {
          ICodeReviewSecurityExtension securityExtension = ReviewSecurityEvaluator.GetSecurityExtension(securityExtensions, sourceReview.SourceArtifactId);
          if (securityExtension != null)
          {
            string name = securityExtension.GetType().Name;
            if (!dictionary.ContainsKey(name))
              dictionary.Add(name, new List<Review>());
            dictionary[name].Add(sourceReview);
          }
          else
            reviewList1.Add(sourceReview);
        }
        foreach (KeyValuePair<string, List<Review>> keyValuePair in dictionary)
        {
          foreach (ICodeReviewSecurityExtension securityExtension in (IEnumerable<ICodeReviewSecurityExtension>) securityExtensions)
          {
            if (securityExtension.GetType().Name.Equals(keyValuePair.Key))
            {
              securityExtension.CheckReadPermissionBatch(requestContext, keyValuePair.Value, reviewsChecked);
              break;
            }
          }
        }
        foreach (Review review in reviewList1)
        {
          if (!reviewsChecked.ContainsKey(review.Id))
            reviewsChecked.Add(review.Id, true);
        }
      }
      List<Review> reviewList2 = new List<Review>();
      foreach (Review sourceReview in sourceReviews)
      {
        if (reviewsChecked.ContainsKey(sourceReview.Id) && reviewsChecked[sourceReview.Id])
          reviewList2.Add(sourceReview);
      }
      return (IList<Review>) reviewList2;
    }

    public static ICodeReviewSecurityExtension GetSecurityExtension(
      IDisposableReadOnlyList<ICodeReviewSecurityExtension> securityExtensions,
      string artifactUri)
    {
      foreach (ICodeReviewSecurityExtension securityExtension in (IEnumerable<ICodeReviewSecurityExtension>) securityExtensions)
      {
        if (securityExtension.CanResolve(artifactUri))
          return securityExtension;
      }
      return (ICodeReviewSecurityExtension) null;
    }
  }
}
