// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.IdentityHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class IdentityHelper
  {
    internal static bool CompareRequesterIdentity(
      IVssRequestContext requestContext,
      string userIdentity)
    {
      return requestContext.GetUserIdentity().Id.ToString().Equals(userIdentity, StringComparison.OrdinalIgnoreCase);
    }

    internal static IdentityRef GetRequesterIdentityRef(IVssRequestContext requestContext) => requestContext.GetUserIdentity().ToCRIdentityRef(requestContext);

    internal static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetMappedIdentities(
      IVssRequestContext requestContext,
      IList<Guid> uniqueUserIdentities)
    {
      return Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.IdentityHelper.GetMappedIdentities(requestContext, uniqueUserIdentities);
    }

    internal static Dictionary<string, IdentityRef> GetVsoIdentities(
      IVssRequestContext requestContext,
      IList<Guid> userIdentities)
    {
      List<Guid> list = userIdentities.Distinct<Guid>().ToList<Guid>();
      Dictionary<string, IdentityRef> vsoIdentities = new Dictionary<string, IdentityRef>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in IdentityHelper.GetMappedIdentities(requestContext, (IList<Guid>) list).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (matchedIdentity => matchedIdentity != null)))
        vsoIdentities[identity.Id.ToString()] = identity.ToCRIdentityRef(requestContext);
      return vsoIdentities;
    }

    internal static IEnumerable<Review> FillIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Review> reviews)
    {
      List<Guid> userIdentities = new List<Guid>();
      foreach (Review review in reviews)
      {
        userIdentities.Add(Guid.Parse(review.Author.Id));
        if (review.Iterations != null)
        {
          foreach (Iteration iteration in (IEnumerable<Iteration>) review.Iterations)
          {
            userIdentities.Add(Guid.Parse(iteration.Author.Id));
            if (iteration.Statuses != null)
              userIdentities.AddRange(iteration.Statuses.Where<Status>((Func<Status, bool>) (status => status.Author != null)).Select<Status, Guid>((Func<Status, Guid>) (status => Guid.Parse(status.Author.Id))));
          }
        }
        if (review.Reviewers != null)
          userIdentities.AddRange(review.Reviewers.Select<Reviewer, Guid>((Func<Reviewer, Guid>) (reviewer => Guid.Parse(reviewer.Identity.Id))));
        if (review.Attachments != null)
          userIdentities.AddRange(review.Attachments.Select<Attachment, Guid>((Func<Attachment, Guid>) (attachment => Guid.Parse(attachment.Author.Id))));
        if (review.Statuses != null)
          userIdentities.AddRange(review.Statuses.Where<Status>((Func<Status, bool>) (status => status.Author != null)).Select<Status, Guid>((Func<Status, Guid>) (status => Guid.Parse(status.Author.Id))));
      }
      Dictionary<string, IdentityRef> matchedIdentities = IdentityHelper.GetVsoIdentities(requestContext, (IList<Guid>) userIdentities);
      foreach (Review review in reviews)
      {
        review.Author = IdentityHelper.GetMatchedIdentity(review.Author, matchedIdentities);
        if (review.Iterations != null)
          review.Iterations = (IList<Iteration>) IdentityHelper.FillAuthorIdentities(review.Iterations, matchedIdentities).ToList<Iteration>();
        if (review.Reviewers != null)
          review.Reviewers = (IList<Reviewer>) IdentityHelper.FillReviewerIdentities((IEnumerable<Reviewer>) review.Reviewers, matchedIdentities).ToList<Reviewer>();
        if (review.Attachments != null)
          review.Attachments = (IList<Attachment>) IdentityHelper.FillAuthorIdentities((IEnumerable<Attachment>) review.Attachments, matchedIdentities).ToList<Attachment>();
        if (review.Statuses != null)
          review.Statuses = (IList<Status>) IdentityHelper.FillAuthorIdentities(review.Statuses, matchedIdentities).ToList<Status>();
        yield return review;
      }
    }

    internal static IList<Iteration> FillAuthorIdentities(
      IVssRequestContext requestContext,
      IList<Iteration> iterations)
    {
      if (iterations == null)
        return (IList<Iteration>) new List<Iteration>();
      List<Guid> userIdentities = new List<Guid>();
      userIdentities.AddRange(iterations.Select<Iteration, Guid>((Func<Iteration, Guid>) (iteration => Guid.Parse(iteration.Author.Id))));
      Dictionary<string, IdentityRef> vsoIdentities = IdentityHelper.GetVsoIdentities(requestContext, (IList<Guid>) userIdentities);
      return IdentityHelper.FillAuthorIdentities(iterations, vsoIdentities);
    }

    private static IList<Iteration> FillAuthorIdentities(
      IList<Iteration> iterations,
      Dictionary<string, IdentityRef> matchedIdentities)
    {
      foreach (Iteration iteration in (IEnumerable<Iteration>) iterations)
      {
        iteration.Author = IdentityHelper.GetMatchedIdentity(iteration.Author, matchedIdentities);
        if (iteration.Statuses != null)
          iteration.Statuses = IdentityHelper.FillAuthorIdentities(iteration.Statuses, matchedIdentities);
      }
      return iterations;
    }

    internal static IEnumerable<Reviewer> FillReviewerIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Reviewer> reviewers)
    {
      if (reviewers == null || reviewers.Count<Reviewer>() == 0)
        return (IEnumerable<Reviewer>) new List<Reviewer>();
      List<Guid> userIdentities = new List<Guid>();
      userIdentities.AddRange(reviewers.Select<Reviewer, Guid>((Func<Reviewer, Guid>) (reviewer => Guid.Parse(reviewer.Identity.Id))));
      Dictionary<string, IdentityRef> vsoIdentities = IdentityHelper.GetVsoIdentities(requestContext, (IList<Guid>) userIdentities);
      return IdentityHelper.FillReviewerIdentities(reviewers, vsoIdentities);
    }

    private static IEnumerable<Reviewer> FillReviewerIdentities(
      IEnumerable<Reviewer> reviewers,
      Dictionary<string, IdentityRef> matchedIdentities)
    {
      foreach (Reviewer reviewer in reviewers)
        reviewer.Identity = IdentityHelper.GetMatchedIdentity(reviewer.Identity, matchedIdentities);
      return reviewers;
    }

    internal static IEnumerable<Attachment> FillAuthorIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Attachment> attachments)
    {
      if (attachments == null || attachments.Count<Attachment>() == 0)
        return (IEnumerable<Attachment>) new List<Attachment>();
      List<Guid> userIdentities = new List<Guid>();
      userIdentities.AddRange(attachments.Select<Attachment, Guid>((Func<Attachment, Guid>) (attachment => Guid.Parse(attachment.Author.Id))));
      Dictionary<string, IdentityRef> vsoIdentities = IdentityHelper.GetVsoIdentities(requestContext, (IList<Guid>) userIdentities);
      return IdentityHelper.FillAuthorIdentities(attachments, vsoIdentities);
    }

    private static IEnumerable<Attachment> FillAuthorIdentities(
      IEnumerable<Attachment> attachments,
      Dictionary<string, IdentityRef> matchedIdentities)
    {
      foreach (Attachment attachment in attachments)
        attachment.Author = IdentityHelper.GetMatchedIdentity(attachment.Author, matchedIdentities);
      return attachments;
    }

    internal static IList<Status> FillAuthorIdentities(
      IVssRequestContext requestContext,
      IList<Status> statuses)
    {
      if (statuses == null || statuses.Count<Status>() == 0)
        return (IList<Status>) Array.Empty<Status>();
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (Status statuse in (IEnumerable<Status>) statuses)
      {
        if (statuse.Author != null)
          source.Add(Guid.Parse(statuse.Author.Id));
      }
      Dictionary<string, IdentityRef> vsoIdentities = IdentityHelper.GetVsoIdentities(requestContext, (IList<Guid>) source.ToList<Guid>());
      return IdentityHelper.FillAuthorIdentities(statuses, vsoIdentities);
    }

    private static IList<Status> FillAuthorIdentities(
      IList<Status> statuses,
      Dictionary<string, IdentityRef> matchedIdentities)
    {
      foreach (Status statuse in (IEnumerable<Status>) statuses)
      {
        if (statuse.Author != null)
          statuse.Author = IdentityHelper.GetMatchedIdentity(statuse.Author, matchedIdentities);
      }
      return statuses;
    }

    internal static IdentityRef GetMatchedIdentity(
      IdentityRef identity,
      Dictionary<string, IdentityRef> matchedIdentities)
    {
      IdentityRef matchedIdentity;
      if (!matchedIdentities.TryGetValue(identity.Id, out matchedIdentity))
        matchedIdentity = identity;
      return matchedIdentity;
    }

    internal static bool IsUserProjectAdmin(IVssRequestContext requestContext, Guid projectId)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IdentityDescriptor descriptor = service.ReadIdentities(requestContext1, IdentitySearchFilter.AdministratorsGroup, projectId.ToString(), QueryMembership.None, (IEnumerable<string>) null, ReadIdentitiesOptions.None).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>()?.Descriptor;
      return descriptor != (IdentityDescriptor) null && service.IsMember(requestContext, descriptor, requestContext.UserContext);
    }
  }
}
