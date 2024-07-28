// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewRequestContextCacheUtil
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
  public class CodeReviewRequestContextCacheUtil
  {
    internal const string c_codeReviewKeyPrefix = "_CodeReview#";
    internal const string c_statusesKeyPrefix = "_Statuses#";
    private const string c_area = "CodeReviewRequestContextCacheUtil";

    public static Review GetCodeReview(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      ReviewScope reviewScope)
    {
      if (projectId == Guid.Empty || reviewId <= 0)
        return (Review) null;
      Review cachedCodeReview = CodeReviewRequestContextCacheUtil.GetCachedCodeReview(requestContext, projectId, reviewId, reviewScope);
      if (cachedCodeReview != null)
        return cachedCodeReview;
      Review review = requestContext.GetService<ICodeReviewService>().GetReview(requestContext, projectId, reviewId, CodeReviewExtendedProperties.None, reviewScope, new int?(0));
      if (review != null)
        CodeReviewRequestContextCacheUtil.CacheCodeReview(requestContext, review, reviewScope);
      return review;
    }

    public static IEnumerable<Status> GetStatusesOrDefault(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? iterationId = null,
      bool includeProperties = false)
    {
      if (projectId == Guid.Empty || reviewId <= 0)
        return (IEnumerable<Status>) null;
      IEnumerable<Status> cachedStatuses = CodeReviewRequestContextCacheUtil.GetCachedStatuses(requestContext, projectId, reviewId, iterationId, includeProperties);
      if (cachedStatuses != null)
        return cachedStatuses;
      IEnumerable<Status> statuses = requestContext.GetService<ICodeReviewStatusService>().GetStatuses(requestContext, projectId, reviewId, iterationId, includeProperties);
      if (statuses != null)
        CodeReviewRequestContextCacheUtil.CacheStatuses(requestContext, projectId, reviewId, iterationId, includeProperties, statuses);
      return statuses;
    }

    public static Status GetStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int statusId,
      int? iterationId = null)
    {
      if (projectId == Guid.Empty || reviewId <= 0)
        return (Status) null;
      IEnumerable<Status> cachedStatuses = CodeReviewRequestContextCacheUtil.GetCachedStatuses(requestContext, projectId, reviewId, iterationId, true);
      Status status1 = cachedStatuses != null ? cachedStatuses.FirstOrDefault<Status>((Func<Status, bool>) (s => s.Id == statusId)) : (Status) null;
      if (status1 != null)
        return status1;
      Status status2 = requestContext.GetService<ICodeReviewStatusService>().GetStatus(requestContext, projectId, reviewId, statusId, iterationId);
      if (status2 != null)
      {
        IEnumerable<Status> second = (IEnumerable<Status>) new Status[1]
        {
          status2
        };
        IEnumerable<Status> statuses = (cachedStatuses != null ? cachedStatuses.Concat<Status>(second) : (IEnumerable<Status>) null) ?? second;
        CodeReviewRequestContextCacheUtil.CacheStatuses(requestContext, projectId, reviewId, iterationId, true, statuses);
      }
      return status2;
    }

    internal static Review GetCachedCodeReview(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      ReviewScope reviewScope)
    {
      string codeReviewCacheKey1 = CodeReviewRequestContextCacheUtil.GetCodeReviewCacheKey(projectId, reviewId, ReviewScope.All);
      Review cachedValue1 = CodeReviewRequestContextCacheUtil.GetCachedValue<Review>(requestContext, codeReviewCacheKey1);
      if (cachedValue1 != null)
        return cachedValue1;
      if (reviewScope != ReviewScope.All)
      {
        string codeReviewCacheKey2 = CodeReviewRequestContextCacheUtil.GetCodeReviewCacheKey(projectId, reviewId, reviewScope);
        Review cachedValue2 = CodeReviewRequestContextCacheUtil.GetCachedValue<Review>(requestContext, codeReviewCacheKey2);
        if (cachedValue2 != null)
          return cachedValue2;
      }
      return (Review) null;
    }

    internal static IEnumerable<Status> GetCachedStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? iterationId,
      bool includeProperties)
    {
      string statusesCacheKey = CodeReviewRequestContextCacheUtil.GetStatusesCacheKey(projectId, reviewId, iterationId, includeProperties);
      return CodeReviewRequestContextCacheUtil.GetCachedValue<IEnumerable<Status>>(requestContext, statusesCacheKey);
    }

    internal static void InvalidateCachedStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? iterationId = null)
    {
      CodeReviewRequestContextCacheUtil.RemoveCachedValue(requestContext, CodeReviewRequestContextCacheUtil.GetStatusesCacheKey(projectId, reviewId, iterationId, true));
      CodeReviewRequestContextCacheUtil.RemoveCachedValue(requestContext, CodeReviewRequestContextCacheUtil.GetStatusesCacheKey(projectId, reviewId, iterationId, false));
      if (!iterationId.HasValue)
        return;
      CodeReviewRequestContextCacheUtil.RemoveCachedValue(requestContext, CodeReviewRequestContextCacheUtil.GetStatusesCacheKey(projectId, reviewId, new int?(), true));
      CodeReviewRequestContextCacheUtil.RemoveCachedValue(requestContext, CodeReviewRequestContextCacheUtil.GetStatusesCacheKey(projectId, reviewId, new int?(), false));
    }

    internal static void CacheCodeReview(
      IVssRequestContext requestContext,
      Review review,
      ReviewScope reviewScope)
    {
      string codeReviewCacheKey = CodeReviewRequestContextCacheUtil.GetCodeReviewCacheKey(review.ProjectId, review.Id, reviewScope);
      CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, codeReviewCacheKey, (object) review);
    }

    internal static void CacheStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? iterationId,
      bool includeProperties,
      IEnumerable<Status> statuses)
    {
      string statusesCacheKey = CodeReviewRequestContextCacheUtil.GetStatusesCacheKey(projectId, reviewId, iterationId, includeProperties);
      CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, statusesCacheKey, (object) statuses);
    }

    protected internal static T GetCachedValue<T>(
      IVssRequestContext requestContext,
      string cacheKey)
      where T : class
    {
      return CodeReviewRequestContextCacheUtil.IsCachePopulated(requestContext, cacheKey) ? requestContext.RootContext.Items[cacheKey] as T : default (T);
    }

    protected internal static bool SetCachedValue(
      IVssRequestContext requestContext,
      string cacheKey,
      object value)
    {
      if (!CodeReviewRequestContextCacheUtil.IsCacheAvailable(requestContext, cacheKey))
        return false;
      requestContext.RootContext.Items[cacheKey] = value;
      return true;
    }

    protected internal static bool RemoveCachedValue(
      IVssRequestContext requestContext,
      string cacheKey)
    {
      if (!CodeReviewRequestContextCacheUtil.IsCachePopulated(requestContext, cacheKey))
        return false;
      requestContext.RootContext.Items.Remove(cacheKey);
      return true;
    }

    protected internal static bool IsCachePopulated(
      IVssRequestContext requestContext,
      string cacheKey)
    {
      return CodeReviewRequestContextCacheUtil.IsCacheAvailable(requestContext, cacheKey) && requestContext.RootContext.Items.ContainsKey(cacheKey);
    }

    protected internal static bool IsCacheAvailable(
      IVssRequestContext requestContext,
      string cacheKey)
    {
      return requestContext != null && requestContext.IsFeatureEnabled("Git.Util.RequestContextCaching") && !string.IsNullOrEmpty(cacheKey) && requestContext?.RootContext?.Items != null;
    }

    internal static string GetCodeReviewCacheKey(
      Guid projectId,
      int reviewId,
      ReviewScope reviewScope)
    {
      if (projectId == Guid.Empty || reviewId <= 0)
        return (string) null;
      return string.Format("{0}{1}.{2}.{3}", (object) "_CodeReview#", (object) projectId, (object) reviewId, (object) reviewScope);
    }

    internal static string GetStatusesCacheKey(
      Guid projectId,
      int reviewId,
      int? iterationId,
      bool includeProperties)
    {
      if (projectId == Guid.Empty || reviewId <= 0)
        return (string) null;
      return string.Format("{0}{1}.{2}.{3}.{4}", (object) "_Statuses#", (object) projectId, (object) reviewId, (object) iterationId.GetValueOrDefault(), (object) includeProperties);
    }
  }
}
