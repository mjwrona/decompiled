// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel.PartitionKeyRangeComparer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel
{
  internal sealed class PartitionKeyRangeComparer : IComparer<PartitionKeyRange>
  {
    public static readonly PartitionKeyRangeComparer Singleton = new PartitionKeyRangeComparer();

    private PartitionKeyRangeComparer()
    {
    }

    public int Compare(PartitionKeyRange x, PartitionKeyRange y)
    {
      if (x == null)
        throw new ArgumentNullException(nameof (x));
      if (y == null)
        throw new ArgumentNullException(nameof (y));
      if (x.MinInclusive.Length == 0)
        return -1;
      return y.MinInclusive.Length == 0 ? 1 : x.MinInclusive.CompareTo(y.MinInclusive);
    }
  }
}
