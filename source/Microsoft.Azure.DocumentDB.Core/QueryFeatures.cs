// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryFeatures
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents
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
  }
}
