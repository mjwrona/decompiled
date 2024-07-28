// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexUtilizationInfo
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class IndexUtilizationInfo
  {
    public static readonly IndexUtilizationInfo Empty = new IndexUtilizationInfo((IReadOnlyList<IndexUtilizationData>) new List<IndexUtilizationData>(), (IReadOnlyList<IndexUtilizationData>) new List<IndexUtilizationData>());

    public IReadOnlyList<IndexUtilizationData> UtilizedIndexes { get; }

    public IReadOnlyList<IndexUtilizationData> PotentialIndexes { get; }

    [JsonConstructor]
    public IndexUtilizationInfo(
      IReadOnlyList<IndexUtilizationData> utilizedIndexes,
      IReadOnlyList<IndexUtilizationData> potentialIndexes)
    {
      List<IndexUtilizationData> indexUtilizationDataList1 = new List<IndexUtilizationData>();
      List<IndexUtilizationData> indexUtilizationDataList2 = new List<IndexUtilizationData>();
      if (utilizedIndexes != null)
      {
        foreach (IndexUtilizationData utilizedIndex in (IEnumerable<IndexUtilizationData>) utilizedIndexes)
        {
          if (utilizedIndex != null)
            indexUtilizationDataList1.Add(utilizedIndex);
        }
      }
      if (potentialIndexes != null)
      {
        foreach (IndexUtilizationData potentialIndex in (IEnumerable<IndexUtilizationData>) potentialIndexes)
        {
          if (potentialIndex != null)
            indexUtilizationDataList2.Add(potentialIndex);
        }
      }
      this.UtilizedIndexes = (IReadOnlyList<IndexUtilizationData>) indexUtilizationDataList1;
      this.PotentialIndexes = (IReadOnlyList<IndexUtilizationData>) indexUtilizationDataList2;
    }

    internal static bool TryCreateFromDelimitedString(
      string delimitedString,
      out IndexUtilizationInfo result)
    {
      if (delimitedString == null)
      {
        result = IndexUtilizationInfo.Empty;
        return true;
      }
      try
      {
        string str = Encoding.UTF8.GetString(Convert.FromBase64String(delimitedString));
        result = JsonConvert.DeserializeObject<IndexUtilizationInfo>(str);
        if (result == null)
          result = IndexUtilizationInfo.Empty;
        return true;
      }
      catch
      {
        result = IndexUtilizationInfo.Empty;
        return false;
      }
    }

    internal static IndexUtilizationInfo CreateFromIEnumerable(
      IEnumerable<IndexUtilizationInfo> indexUtilizationInfoList)
    {
      if (indexUtilizationInfoList == null)
        throw new ArgumentNullException(nameof (indexUtilizationInfoList));
      List<IndexUtilizationData> utilizedIndexes = new List<IndexUtilizationData>();
      List<IndexUtilizationData> potentialIndexes = new List<IndexUtilizationData>();
      foreach (IndexUtilizationInfo indexUtilizationInfo in indexUtilizationInfoList)
      {
        if (indexUtilizationInfo == null)
          throw new ArgumentException("indexUtilizationInfoList can not have a null element");
        utilizedIndexes.AddRange((IEnumerable<IndexUtilizationData>) indexUtilizationInfo.UtilizedIndexes);
        potentialIndexes.AddRange((IEnumerable<IndexUtilizationData>) indexUtilizationInfo.PotentialIndexes);
      }
      return new IndexUtilizationInfo((IReadOnlyList<IndexUtilizationData>) utilizedIndexes, (IReadOnlyList<IndexUtilizationData>) potentialIndexes);
    }
  }
}
