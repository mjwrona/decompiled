// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexingPolicyTranslator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  internal static class IndexingPolicyTranslator
  {
    public static IndexingPolicy TranslateIndexingPolicyV1ToV2(IndexingPolicyOld indexingPolicyOld)
    {
      if (indexingPolicyOld == null)
        throw new ArgumentNullException(nameof (indexingPolicyOld));
      IndexingPolicy v2 = new IndexingPolicy();
      v2.Automatic = indexingPolicyOld.Automatic;
      v2.IndexingMode = indexingPolicyOld.IndexingMode;
      foreach (IndexingPath includedPath1 in indexingPolicyOld.IncludedPaths)
      {
        IncludedPath includedPath2 = new IncludedPath()
        {
          Path = includedPath1.Path,
          Indexes = new Collection<Index>()
        };
        int? nullable;
        if (includedPath1.IndexType == IndexType.Hash)
        {
          Collection<Index> indexes1 = includedPath2.Indexes;
          HashIndex hashIndex1 = new HashIndex(DataType.Number);
          nullable = includedPath1.NumericPrecision;
          hashIndex1.Precision = nullable.HasValue ? new short?((short) nullable.GetValueOrDefault()) : new short?();
          indexes1.Add((Index) hashIndex1);
          Collection<Index> indexes2 = includedPath2.Indexes;
          HashIndex hashIndex2 = new HashIndex(DataType.String);
          nullable = includedPath1.StringPrecision;
          hashIndex2.Precision = nullable.HasValue ? new short?((short) nullable.GetValueOrDefault()) : new short?();
          indexes2.Add((Index) hashIndex2);
        }
        else if (includedPath1.IndexType == IndexType.Range)
        {
          Collection<Index> indexes3 = includedPath2.Indexes;
          RangeIndex rangeIndex = new RangeIndex(DataType.Number);
          nullable = includedPath1.NumericPrecision;
          rangeIndex.Precision = nullable.HasValue ? new short?((short) nullable.GetValueOrDefault()) : new short?();
          indexes3.Add((Index) rangeIndex);
          Collection<Index> indexes4 = includedPath2.Indexes;
          HashIndex hashIndex = new HashIndex(DataType.String);
          nullable = includedPath1.StringPrecision;
          hashIndex.Precision = nullable.HasValue ? new short?((short) nullable.GetValueOrDefault()) : new short?();
          indexes4.Add((Index) hashIndex);
        }
        v2.IncludedPaths.Add(includedPath2);
      }
      foreach (string excludedPath1 in (IEnumerable<string>) indexingPolicyOld.ExcludedPaths)
      {
        ExcludedPath excludedPath2 = new ExcludedPath()
        {
          Path = excludedPath1
        };
        v2.ExcludedPaths.Add(excludedPath2);
      }
      return v2;
    }
  }
}
