// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OperationTypeExtensions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal static class OperationTypeExtensions
  {
    private static readonly Dictionary<int, string> OperationTypeNames = new Dictionary<int, string>();

    static OperationTypeExtensions()
    {
      foreach (OperationType key in Enum.GetValues(typeof (OperationType)))
        OperationTypeExtensions.OperationTypeNames[(int) key] = key.ToString();
    }

    public static string ToOperationTypeString(this OperationType type) => OperationTypeExtensions.OperationTypeNames[(int) type];

    public static bool IsWriteOperation(this OperationType type) => type == OperationType.Create || type == OperationType.Delete || type == OperationType.Replace || type == OperationType.ExecuteJavaScript || type == OperationType.BatchApply || type == OperationType.Batch || type == OperationType.Upsert || type == OperationType.Recreate || type == OperationType.GetSplitPoint || type == OperationType.AbortSplit || type == OperationType.CompleteSplit || type == OperationType.PreReplaceValidation || type == OperationType.ReportThroughputUtilization || type == OperationType.BatchReportThroughputUtilization || type == OperationType.OfferUpdateOperation || type == OperationType.CompletePartitionMigration || type == OperationType.AbortPartitionMigration || type == OperationType.MigratePartition || type == OperationType.ForceConfigRefresh || type == OperationType.MasterReplaceOfferOperation || type == OperationType.InitiateDatabaseOfferPartitionShrink || type == OperationType.CompleteDatabaseOfferPartitionShrink || type == OperationType.EnsureSnapshotOperation || type == OperationType.GetSplitPoints;

    public static bool IsPointOperation(this OperationType type) => type == OperationType.Create || type == OperationType.Delete || type == OperationType.Read || type == OperationType.Patch || type == OperationType.Upsert || type == OperationType.Replace;

    public static bool IsReadOperation(this OperationType type) => type == OperationType.Read || type == OperationType.ReadFeed || type == OperationType.Query || type == OperationType.SqlQuery || type == OperationType.Head || type == OperationType.HeadFeed || type == OperationType.QueryPlan;

    public static string GetHttpMethod(this OperationType operationType)
    {
      switch (operationType)
      {
        case OperationType.ReadReplicaFromMasterPartition:
          return "GET";
        case OperationType.GetUnwrappedDek:
        case OperationType.ServiceReservation:
        case OperationType.ForceConfigRefresh:
        case OperationType.Pause:
        case OperationType.Resume:
        case OperationType.Stop:
        case OperationType.Recycle:
        case OperationType.Crash:
        case OperationType.Recreate:
        case OperationType.Throttle:
        case OperationType.GetSplitPoint:
        case OperationType.PreCreateValidation:
        case OperationType.AbortSplit:
        case OperationType.CompleteSplit:
        case OperationType.CompletePartitionMigration:
        case OperationType.AbortPartitionMigration:
        case OperationType.OfferUpdateOperation:
        case OperationType.OfferPreGrowValidation:
        case OperationType.BatchReportThroughputUtilization:
        case OperationType.PreReplaceValidation:
        case OperationType.MigratePartition:
        case OperationType.MasterReplaceOfferOperation:
        case OperationType.InitiateDatabaseOfferPartitionShrink:
        case OperationType.CompleteDatabaseOfferPartitionShrink:
        case OperationType.GetSplitPoints:
          return "POST";
        case OperationType.ExecuteJavaScript:
        case OperationType.Create:
        case OperationType.BatchApply:
        case OperationType.SqlQuery:
        case OperationType.Query:
        case OperationType.Upsert:
        case OperationType.Batch:
        case OperationType.QueryPlan:
          return "POST";
        case OperationType.Patch:
          return "PATCH";
        case OperationType.Read:
        case OperationType.ReadFeed:
          return "GET";
        case OperationType.Delete:
          return "DELETE";
        case OperationType.Replace:
          return "PUT";
        case OperationType.Head:
        case OperationType.HeadFeed:
          return "HEAD";
        case OperationType.EnsureSnapshotOperation:
          return "PUT";
        default:
          throw new NotImplementedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unsupported operation type: {0}.", (object) operationType));
      }
    }
  }
}
