// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.FileChangeUtilities
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal static class FileChangeUtilities
  {
    internal static void ValidateAndNormalizeInputs(
      this ChangeEntry entry,
      int? iterationId,
      HashSet<int> uniqueChangeTrackingIds,
      bool checkChangeTrackingId)
    {
      if (entry.IterationId.HasValue && iterationId.HasValue)
      {
        int? nullable = iterationId;
        int? iterationId1 = entry.IterationId;
        if (!(nullable.GetValueOrDefault() == iterationId1.GetValueOrDefault() & nullable.HasValue == iterationId1.HasValue))
          throw new ArgumentException(CodeReviewResources.MismatchedIterationIds((object) iterationId, (object) entry.IterationId));
      }
      if (iterationId.HasValue)
        entry.IterationId = iterationId;
      if (entry.ChangeId.HasValue)
        throw new ArgumentException(CodeReviewResources.ChangeIdCannotBeSet(), "ChangeId");
      if (entry.Base != null && entry.Base.Path != null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(entry.Base.SHA1Hash, "ContentHash");
        entry.Base.Path = DBPath.UserToDatabasePath(entry.Base.Path, true, true);
      }
      if (entry.Modified != null && entry.Modified.Path != null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(entry.Modified.SHA1Hash, "ContentHash");
        entry.Modified.Path = DBPath.UserToDatabasePath(entry.Modified.Path, true, true);
      }
      if (!checkChangeTrackingId)
        return;
      ArgumentUtility.CheckForOutOfRange<int>(entry.ChangeTrackingId, "ChangeTrackingId", 1);
      if (!uniqueChangeTrackingIds.Add(entry.ChangeTrackingId))
        throw new ArgumentException(CodeReviewResources.DuplicateChangeTrackingIdInChangeList((object) entry.ChangeTrackingId), "ChangeTrackingId");
    }

    internal static bool ShouldComputeChangeTrackingId(
      int iterationId,
      IEnumerable<ChangeEntry> changeList)
    {
      if (changeList == null)
        return false;
      bool flag = false;
      bool changeTrackingId = false;
      foreach (ChangeEntry change in changeList)
      {
        if (change.ChangeTrackingId < 1)
          changeTrackingId = true;
        else
          flag = true;
        if (changeTrackingId & flag)
          throw new ChangeEntriesWithBothSetAndUnSetChangeTrackingIdsException(iterationId);
      }
      return changeTrackingId;
    }

    internal static ChangeEntry PopulateReferenceLinksAndNormalizeOutputs(
      this ChangeEntry entry,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      entry.ConvertDatabasePathToUserPath();
      entry.PopulateReferenceLinks(requestContext, projectId, reviewId);
      return entry;
    }

    internal static ChangeEntry PopulateReferenceLinks(
      this ChangeEntry entry,
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      if (entry.Base != null && entry.Base.Path != null)
        entry.Base.AddReferenceLinks(requestContext, projectId, reviewId, entry.Base.SHA1Hash);
      if (entry.Modified != null && entry.Modified.Path != null)
        entry.Modified.AddReferenceLinks(requestContext, projectId, reviewId, entry.Modified.SHA1Hash);
      return entry;
    }

    internal static ChangeEntry ConvertDatabasePathToUserPath(this ChangeEntry entry)
    {
      if (entry.Base != null && entry.Base.Path != null)
        entry.Base.Path = DBPath.DatabaseToUserPath(entry.Base.Path, true, true);
      if (entry.Modified != null && entry.Modified.Path != null)
        entry.Modified.Path = DBPath.DatabaseToUserPath(entry.Modified.Path, true, true);
      return entry;
    }

    internal static string GetDownloadableFileName(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      string str = path.Replace('\\', '/').TrimEnd('/');
      if (str.Length == 0)
        throw new ArgumentException(CodeReviewResources.FileNotFoundException((object) path));
      int num = -1;
      if (str.Contains("/"))
        num = str.LastIndexOf('/');
      return num >= 0 ? str.Substring(num + 1) : str;
    }
  }
}
