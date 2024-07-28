// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackIndexTransaction
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitPackIndexTransaction : IDisposable
  {
    private readonly IVssRequestContext m_rc;
    private readonly OdbId m_odbId;
    private readonly IGitKnownFilesProvider m_knownPrv;
    private readonly ILeaseService m_leaseSvc;
    private readonly IGitPackIndexPointerProvider m_packIndexPtrPrv;
    private readonly Lazy<GitOdbSettings> m_odbSettings;
    private RenewableLease m_renewableLease;
    private bool m_disposed;
    private static readonly TimeSpan s_leaseTimeSpan = new TimeSpan(0, 0, 45);
    private static readonly TimeSpan s_renewLeaseTimeSpan = new TimeSpan(0, 0, 30);
    private static readonly TimeSpan s_leaseTimeout = new TimeSpan(0, 10, 0);
    private const string c_layer = "GitPackIndexTransaction";

    public GitPackIndexTransaction(
      IVssRequestContext rc,
      OdbId odbId,
      IGitKnownFilesProvider knownFilesProvider,
      ILeaseService leaseService,
      IGitPackIndexPointerProvider packIndexPointerProvider,
      Lazy<GitOdbSettings> odbSettings)
    {
      this.m_rc = rc;
      this.m_odbId = odbId;
      this.m_knownPrv = knownFilesProvider;
      this.m_leaseSvc = leaseService;
      this.m_packIndexPtrPrv = packIndexPointerProvider;
      this.m_odbSettings = odbSettings;
      this.KnownFilesBuilder = new GitKnownFilesBuilder();
    }

    public void TryExpirePendingExtantAndDispose()
    {
      using (this)
      {
        if (this.m_disposed)
          return;
        try
        {
          this.m_knownPrv.Update(this.KnownFilesBuilder.GetCreates());
        }
        catch (Exception ex)
        {
          this.m_rc.TraceException(1013882, GitServerUtils.TraceArea, nameof (GitPackIndexTransaction), ex);
        }
      }
    }

    public void EnsureIndexLease()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(nameof (GitPackIndexTransaction));
      if (this.m_renewableLease != null)
        return;
      this.m_renewableLease = new RenewableLease(this.m_rc, this.m_leaseSvc, "index-" + this.m_odbId.Value.ToString("N"), this.m_rc.IsServicingContext ? TimeSpan.FromMinutes(5.0) : GitPackIndexTransaction.s_leaseTimeSpan, GitPackIndexTransaction.s_renewLeaseTimeSpan, this.m_odbSettings.Value.IndexTransactionLeaseRenewCount, GitPackIndexTransaction.s_leaseTimeout, true);
    }

    public void CommitAndDispose(
      ConcatGitPackIndex oldIndex,
      ConcatGitPackIndex newIndex,
      bool alsoSetLastPacked = false,
      IEnumerable<Sha1Id> removedObjects = null)
    {
      using (this)
      {
        ArgumentUtility.CheckForNull<ConcatGitPackIndex>(oldIndex, nameof (oldIndex));
        ArgumentUtility.CheckForNull<ConcatGitPackIndex>(newIndex, nameof (newIndex));
        Sha1Id? realTipSubindexId1 = oldIndex.GetRealTipSubindexId(true);
        Sha1Id? realTipSubindexId2 = newIndex.GetRealTipSubindexId(false);
        if (this.m_disposed)
          throw new ObjectDisposedException(nameof (GitPackIndexTransaction));
        if (this.m_renewableLease == null)
        {
          this.TryExpirePendingExtantAndDispose();
          throw new InvalidOperationException(Resources.Get("CannotWriteWithoutExclusiveLock"));
        }
        GitPackIndexTransaction.IndexCheckResult indexCheckResult = GitPackIndexTransaction.CheckForDataLoss(oldIndex, newIndex, removedObjects);
        if (indexCheckResult.Exception != null)
        {
          this.TryExpirePendingExtantAndDispose();
          this.m_rc.Trace(1013715, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitPackIndexTransaction), string.Format("Data loss detected for ODB: {0}, {1}: {2}, {3}: {4}", (object) this.m_odbId, (object) "oldIndexId", (object) realTipSubindexId1, (object) "newIndexId", (object) realTipSubindexId2));
          throw indexCheckResult.Exception;
        }
        this.m_rc.TraceAlways(1013716, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitPackIndexTransaction), FormattableString.Invariant(FormattableStringFactory.Create("{0} succeeded:\r\n                    ODB: {1}\r\n                    {2}: {3}\r\n                    {4}: {5}\r\n                    {6}: {7}\r\n                    {8}: {9}\r\n                    {10}: {11}", (object) "CheckForDataLoss", (object) this.m_odbId, (object) "oldIndexId", (object) realTipSubindexId1, (object) "NumOldObjectIdsChecked", (object) indexCheckResult.NumOldObjectIdsChecked, (object) "newIndexId", (object) realTipSubindexId2, (object) "NumNewObjectIdsChecked", (object) indexCheckResult.NumNewObjectIdsChecked, (object) "NumActualNewObjectIds", (object) indexCheckResult.NumActualNewObjectIds)));
        Sha1Id? nullable1 = this.m_packIndexPtrPrv.TrySetIndex(realTipSubindexId1, realTipSubindexId2, alsoSetLastPacked);
        Sha1Id? nullable2 = realTipSubindexId2;
        if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        {
          this.TryExpirePendingExtantAndDispose();
          throw new InvalidOperationException(Resources.Get("IndexPointerUpdateConflict"));
        }
        this.m_knownPrv.Update(this.KnownFilesBuilder.GetCreates());
      }
    }

    internal static GitPackIndexTransaction.IndexCheckResult CheckForDataLoss(
      ConcatGitPackIndex oldIndex,
      ConcatGitPackIndex newIndex,
      IEnumerable<Sha1Id> removedObjects)
    {
      removedObjects = (IEnumerable<Sha1Id>) ((object) removedObjects ?? (object) Array.Empty<Sha1Id>());
      int num1;
      if (newIndex.StableObjectOrderEpoch.HasValue)
      {
        Sha1Id sha1Id = newIndex.StableObjectOrderEpoch.Value;
        Sha1Id? objectOrderEpoch = oldIndex.StableObjectOrderEpoch;
        num1 = objectOrderEpoch.HasValue ? (sha1Id == objectOrderEpoch.GetValueOrDefault() ? 1 : 0) : 0;
      }
      else
        num1 = 0;
      bool flag1 = num1 != 0;
      ISha1IdTwoWayReadOnlyList objectIds1 = oldIndex.GetRangeNotIn(newIndex).ObjectIds;
      ISha1IdTwoWayReadOnlyList objectIds2 = newIndex.GetRangeNotIn(oldIndex).ObjectIds;
      InvalidOperationException exception = (InvalidOperationException) null;
      HashSet<Sha1Id> sha1IdSet = new HashSet<Sha1Id>();
      int num2 = 0;
      bool flag2 = false;
      foreach (Sha1Id sha1Id in (IEnumerable<Sha1Id>) objectIds1)
      {
        int index;
        if (!objectIds2.TryGetIndex(sha1Id, out index))
          sha1IdSet.Add(sha1Id);
        else if (flag1 && index != num2)
          flag2 = true;
        ++num2;
      }
      int numActualNewObjectIds = objectIds2.Count - (objectIds1.Count - sha1IdSet.Count);
      sha1IdSet.ExceptWith(removedObjects);
      bool flag3 = removedObjects.Any<Sha1Id>();
      if (sha1IdSet.Count != 0)
        exception = new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} unexpected missing object(s)", (object) sha1IdSet.Count)));
      else if (flag2)
      {
        exception = new InvalidOperationException("Stable object order mismatch");
      }
      else
      {
        Sha1Id? objectOrderEpoch1 = oldIndex.StableObjectOrderEpoch;
        if (objectOrderEpoch1.HasValue)
        {
          objectOrderEpoch1 = newIndex.StableObjectOrderEpoch;
          if (!objectOrderEpoch1.HasValue)
          {
            exception = new InvalidOperationException(string.Format("Removed the stable object order epoch, previously {0}", (object) oldIndex.StableObjectOrderEpoch));
            goto label_24;
          }
        }
        objectOrderEpoch1 = oldIndex.StableObjectOrderEpoch;
        if (objectOrderEpoch1.HasValue && !flag3)
        {
          objectOrderEpoch1 = oldIndex.StableObjectOrderEpoch;
          Sha1Id? objectOrderEpoch2 = newIndex.StableObjectOrderEpoch;
          if ((objectOrderEpoch1.HasValue == objectOrderEpoch2.HasValue ? (objectOrderEpoch1.HasValue ? (objectOrderEpoch1.GetValueOrDefault() != objectOrderEpoch2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
            exception = new InvalidOperationException(string.Format("Stable object order epoch was changed. New value: {0}", (object) newIndex.StableObjectOrderEpoch));
        }
      }
label_24:
      return new GitPackIndexTransaction.IndexCheckResult(objectIds1.Count, objectIds2.Count, numActualNewObjectIds, exception);
    }

    public void Dispose()
    {
      this.m_disposed = true;
      this.m_renewableLease?.Dispose();
      this.m_renewableLease = (RenewableLease) null;
    }

    public GitKnownFilesBuilder KnownFilesBuilder { get; }

    internal sealed class IndexCheckResult
    {
      public IndexCheckResult(
        int numOldObjectIdsChecked,
        int numNewObjectIdsChecked,
        int numActualNewObjectIds,
        InvalidOperationException exception)
      {
        this.NumOldObjectIdsChecked = numOldObjectIdsChecked;
        this.NumNewObjectIdsChecked = numNewObjectIdsChecked;
        this.NumActualNewObjectIds = numActualNewObjectIds;
        this.Exception = exception;
      }

      public int NumOldObjectIdsChecked { get; }

      public int NumNewObjectIdsChecked { get; }

      public int NumActualNewObjectIds { get; }

      public InvalidOperationException Exception { get; }
    }
  }
}
