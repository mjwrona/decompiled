// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryFeatures
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryPlan
{
  [Flags]
  internal enum QueryFeatures : ulong
  {
    None = 0,
    Aggregate = 1,
    CompositeAggregate = 2,
    Distinct = 4,
    GroupBy = 8,
    MultipleAggregates = 16, // 0x0000000000000010
    MultipleOrderBy = 32, // 0x0000000000000020
    OffsetAndLimit = 64, // 0x0000000000000040
    OrderBy = 128, // 0x0000000000000080
    Top = 256, // 0x0000000000000100
    NonValueAggregate = 512, // 0x0000000000000200
    DCount = 1024, // 0x0000000000000400
  }
}
