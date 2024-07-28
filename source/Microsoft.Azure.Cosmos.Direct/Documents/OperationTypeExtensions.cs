// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OperationTypeExtensions
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;

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

    public static bool IsWriteOperation(this OperationType type) => type == OperationType.Create || type == OperationType.Patch || type == OperationType.Delete || type == OperationType.Replace || type == OperationType.ExecuteJavaScript || type == OperationType.BatchApply || type == OperationType.Batch || type == OperationType.Upsert || type == OperationType.CompleteUserTransaction;

    public static bool IsPointOperation(this OperationType type) => type == OperationType.Create || type == OperationType.Delete || type == OperationType.Read || type == OperationType.Patch || type == OperationType.Upsert || type == OperationType.Replace;

    public static bool IsReadOperation(this OperationType type) => type == OperationType.Read || type == OperationType.ReadFeed || type == OperationType.Query || type == OperationType.SqlQuery || type == OperationType.Head || type == OperationType.HeadFeed || type == OperationType.QueryPlan;
  }
}
