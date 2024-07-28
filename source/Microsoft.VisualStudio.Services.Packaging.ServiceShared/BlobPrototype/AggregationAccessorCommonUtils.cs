// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationAccessorCommonUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class AggregationAccessorCommonUtils
  {
    public static ICommitLogEntry GetCommitLogEntryForInternalOperation(
      ICommitLogEntry commitLogEntry,
      ICommitOperationData operationData)
    {
      return (ICommitLogEntry) new CommitLogEntry(operationData, PackagingCommitId.Empty, PackagingCommitId.Empty, commitLogEntry.CommitId, commitLogEntry.SequenceNumber, commitLogEntry.CreatedDate, commitLogEntry.ModifiedDate, commitLogEntry.UserId, commitLogEntry.CorruptEntry);
    }

    public static IReadOnlyList<ICommitLogEntry> FlattenCommitLogEntry(ICommitLogEntry outerCommit)
    {
      if (outerCommit.CommitOperationData is IBatchCommitOperationData commitOperationData)
        return (IReadOnlyList<ICommitLogEntry>) commitOperationData.Operations.Select<ICommitOperationData, ICommitLogEntry>((Func<ICommitOperationData, ICommitLogEntry>) (operationData => AggregationAccessorCommonUtils.GetCommitLogEntryForInternalOperation(outerCommit, operationData))).ToArray<ICommitLogEntry>();
      return (IReadOnlyList<ICommitLogEntry>) new ICommitLogEntry[1]
      {
        outerCommit
      };
    }

    public static IEnumerable<ICommitLogEntry> FlattenBatches(
      this IEnumerable<ICommitLogEntry> source)
    {
      foreach (ICommitLogEntry commitLogEntry in source)
      {
        if (commitLogEntry.CommitOperationData is IBatchCommitOperationData commitOperationData)
        {
          foreach (ICommitOperationData operation in commitOperationData.Operations)
            yield return AggregationAccessorCommonUtils.GetCommitLogEntryForInternalOperation(commitLogEntry, operation);
        }
        else
          yield return commitLogEntry;
      }
    }

    public static IEnumerable<CastedOpCommit<TOperationData>> FilterToOpType<TOperationData>(
      this IEnumerable<ICommitLogEntry> source)
      where TOperationData : class, ICommitOperationData
    {
      foreach (ICommitLogEntry Commit in source)
      {
        if (Commit.CommitOperationData is TOperationData commitOperationData)
          yield return new CastedOpCommit<TOperationData>(Commit, commitOperationData);
      }
    }

    public static bool TryGetAggregationAccessorOfType<TAgg>(
      this IEnumerable<IAggregationAccessor> accessors,
      out TAgg aggregation)
    {
      using (IEnumerator<TAgg> enumerator = accessors.OfType<TAgg>().GetEnumerator())
      {
        if (!enumerator.MoveNext())
        {
          aggregation = default (TAgg);
          return false;
        }
        aggregation = enumerator.Current;
        if (!enumerator.MoveNext())
          return true;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Ambiguous aggregation match. Looking for an aggregation of type " + typeof (TAgg).GetPrettyName() + ", but found multiple matches:");
        stringBuilder.AppendLine(DisplayAggregation(aggregation));
        do
        {
          stringBuilder.AppendLine(DisplayAggregation(enumerator.Current));
        }
        while (enumerator.MoveNext());
        throw new InvalidHandlerException(stringBuilder.ToString());
      }

      static string DisplayAggregation(TAgg agg)
      {
        ref TAgg local1 = ref agg;
        string str;
        if ((object) default (TAgg) == null)
        {
          TAgg agg1 = local1;
          ref TAgg local2 = ref agg1;
          if ((object) agg1 == null)
          {
            str = (string) null;
            goto label_4;
          }
          else
            local1 = ref local2;
        }
        str = local1.GetType().GetPrettyName();
label_4:
        if (str == null)
          str = "(null)";
        return "  - " + str;
      }
    }
  }
}
