// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspacePartitionMap
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public class DataspacePartitionMap
  {
    private DataspaceHashRange[] m_ranges;
    private DataspacePartitionMapOverride[] m_overrides;

    public DataspacePartitionMap()
    {
    }

    public DataspacePartitionMap(
      string category,
      DataspaceHashRange[] ranges,
      DataspacePartitionMapOverride[] overrides)
    {
      this.Category = category;
      this.m_ranges = ranges;
      this.m_overrides = overrides;
    }

    [DataMember]
    public string Category { get; set; }

    [DataMember]
    public DataspaceHashRange[] Ranges
    {
      get => this.m_ranges;
      set
      {
        this.m_ranges = value;
        this.DataspaceIdentifiers = (Guid[]) null;
        this.ComputedRanges = (DataspaceHashRange[]) null;
      }
    }

    [DataMember]
    public DataspacePartitionMapOverride[] Overrides
    {
      get => this.m_overrides;
      set
      {
        this.m_overrides = value;
        this.DataspaceIdentifiers = (Guid[]) null;
        this.ComputedRanges = (DataspaceHashRange[]) null;
      }
    }

    [IgnoreDataMember]
    internal Guid[] DataspaceIdentifiers { get; private set; }

    [IgnoreDataMember]
    internal DataspaceHashRange[] ComputedRanges { get; private set; }

    public Guid GetDataspaceIdentifierForHash(int hashCode)
    {
      if (this.ComputedRanges == null || this.ComputedRanges.Length == 0)
        throw new InvalidOperationException("ValidateAndInitialize must first be called before using this method.");
      return this.ComputedRanges[DataspacePartitionMap.BinarySearch((IList<DataspaceHashRange>) this.ComputedRanges, hashCode)].DataspaceIdentifier;
    }

    public void ValidateAndInitialize()
    {
      ArgumentUtility.CheckStringForNullOrEmpty(this.Category, "Category");
      if (this.Ranges == null || this.Ranges.Length == 0)
        throw new ArgumentException("Cannot have an empty DataspacePartitionMap.");
      int num1 = int.MinValue;
      int num2 = int.MinValue;
      foreach (DataspaceHashRange range in this.Ranges)
      {
        if (num1 > num2)
          throw new ArgumentException("The entire hash space was already mapped but there are additional DataspaceHashRanges.");
        if (range.HashStart > range.HashEnd)
          throw new ArgumentException(string.Format("HashRange has a HashStart value greater than HashEnd. HashStart: {0}. HashEnd: {1}", (object) range.HashStart, (object) range.HashEnd));
        if (range.HashStart != num2)
          throw new ArgumentException("DataspacePartitionMap does not map the entire hash space or the provided map was not sorted.");
        num1 = range.HashEnd;
        num2 = range.HashEnd + 1;
      }
      if (num2 != int.MinValue)
        throw new ArgumentException("DataspacePartitionMap does not map the entire hash space.");
      this.Initialize();
    }

    private void Initialize()
    {
      if (this.DataspaceIdentifiers == null)
      {
        HashSet<Guid> source = new HashSet<Guid>();
        foreach (DataspaceHashRange range in this.Ranges)
          source.Add(range.DataspaceIdentifier);
        if (this.Overrides != null)
        {
          foreach (DataspacePartitionMapOverride partitionMapOverride in this.Overrides)
            source.Add(partitionMapOverride.DataspaceIdentifier);
        }
        this.DataspaceIdentifiers = source.ToArray<Guid>();
      }
      if (this.ComputedRanges != null)
        return;
      if (this.Overrides == null)
      {
        this.ComputedRanges = this.Ranges;
      }
      else
      {
        List<DataspaceHashRange> ranges = new List<DataspaceHashRange>((IEnumerable<DataspaceHashRange>) this.Ranges);
        foreach (DataspacePartitionMapOverride partitionMapOverride in this.Overrides)
        {
          Guid result;
          int stableHashCode = (!Guid.TryParse(partitionMapOverride.PartitionKey, out result) ? (IStableHashCode) new StringStableHashCode(partitionMapOverride.PartitionKey) : (IStableHashCode) new GuidStableHashCode(result)).GetStableHashCode();
          int index = DataspacePartitionMap.BinarySearch((IList<DataspaceHashRange>) ranges, stableHashCode);
          DataspaceHashRange dataspaceHashRange = ranges[index];
          ranges.RemoveAt(index);
          ranges.InsertRange(index, (IEnumerable<DataspaceHashRange>) new DataspaceHashRange[3]
          {
            new DataspaceHashRange(dataspaceHashRange.HashStart, stableHashCode - 1, dataspaceHashRange.DataspaceIdentifier),
            new DataspaceHashRange(stableHashCode, stableHashCode, partitionMapOverride.DataspaceIdentifier),
            new DataspaceHashRange(stableHashCode + 1, dataspaceHashRange.HashEnd, dataspaceHashRange.DataspaceIdentifier)
          });
        }
        this.ComputedRanges = ranges.ToArray();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int BinarySearch(IList<DataspaceHashRange> ranges, int hashCode)
    {
      int num1 = 0;
      int num2 = ranges.Count - 1;
      while (num1 <= num2)
      {
        int index = num1 + (num2 - num1) / 2;
        if (hashCode >= ranges[index].HashStart)
        {
          if (hashCode <= ranges[index].HashEnd)
            return index;
          num1 = index + 1;
        }
        else
          num2 = index - 1;
      }
      throw new InvalidOperationException("Unable to map hash to a dataspace identifier.");
    }
  }
}
