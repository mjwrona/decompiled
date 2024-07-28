// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.IndexUtilizationInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class IndexUtilizationInfo
  {
    public static readonly IndexUtilizationInfo Empty = new IndexUtilizationInfo((IReadOnlyList<SingleIndexUtilizationEntity>) new List<SingleIndexUtilizationEntity>(), (IReadOnlyList<SingleIndexUtilizationEntity>) new List<SingleIndexUtilizationEntity>(), (IReadOnlyList<CompositeIndexUtilizationEntity>) new List<CompositeIndexUtilizationEntity>(), (IReadOnlyList<CompositeIndexUtilizationEntity>) new List<CompositeIndexUtilizationEntity>());

    [JsonConstructor]
    public IndexUtilizationInfo(
      IReadOnlyList<SingleIndexUtilizationEntity> utilizedSingleIndexes,
      IReadOnlyList<SingleIndexUtilizationEntity> potentialSingleIndexes,
      IReadOnlyList<CompositeIndexUtilizationEntity> utilizedCompositeIndexes,
      IReadOnlyList<CompositeIndexUtilizationEntity> potentialCompositeIndexes)
    {
      this.UtilizedSingleIndexes = (IReadOnlyList<SingleIndexUtilizationEntity>) ((IEnumerable<SingleIndexUtilizationEntity>) utilizedSingleIndexes ?? Enumerable.Empty<SingleIndexUtilizationEntity>()).Where<SingleIndexUtilizationEntity>((Func<SingleIndexUtilizationEntity, bool>) (item => item != null)).ToList<SingleIndexUtilizationEntity>();
      this.PotentialSingleIndexes = (IReadOnlyList<SingleIndexUtilizationEntity>) ((IEnumerable<SingleIndexUtilizationEntity>) potentialSingleIndexes ?? Enumerable.Empty<SingleIndexUtilizationEntity>()).Where<SingleIndexUtilizationEntity>((Func<SingleIndexUtilizationEntity, bool>) (item => item != null)).ToList<SingleIndexUtilizationEntity>();
      this.UtilizedCompositeIndexes = (IReadOnlyList<CompositeIndexUtilizationEntity>) ((IEnumerable<CompositeIndexUtilizationEntity>) utilizedCompositeIndexes ?? Enumerable.Empty<CompositeIndexUtilizationEntity>()).Where<CompositeIndexUtilizationEntity>((Func<CompositeIndexUtilizationEntity, bool>) (item => item != null)).ToList<CompositeIndexUtilizationEntity>();
      this.PotentialCompositeIndexes = (IReadOnlyList<CompositeIndexUtilizationEntity>) ((IEnumerable<CompositeIndexUtilizationEntity>) potentialCompositeIndexes ?? Enumerable.Empty<CompositeIndexUtilizationEntity>()).Where<CompositeIndexUtilizationEntity>((Func<CompositeIndexUtilizationEntity, bool>) (item => item != null)).ToList<CompositeIndexUtilizationEntity>();
    }

    public IReadOnlyList<SingleIndexUtilizationEntity> UtilizedSingleIndexes { get; }

    public IReadOnlyList<SingleIndexUtilizationEntity> PotentialSingleIndexes { get; }

    public IReadOnlyList<CompositeIndexUtilizationEntity> UtilizedCompositeIndexes { get; }

    public IReadOnlyList<CompositeIndexUtilizationEntity> PotentialCompositeIndexes { get; }

    internal static bool TryCreateFromDelimitedBase64String(
      string delimitedString,
      out IndexUtilizationInfo result)
    {
      if (delimitedString != null)
        return IndexUtilizationInfo.TryCreateFromDelimitedString(Encoding.UTF8.GetString(Convert.FromBase64String(delimitedString)), out result);
      result = IndexUtilizationInfo.Empty;
      return true;
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
        result = JsonConvert.DeserializeObject<IndexUtilizationInfo>(delimitedString, new JsonSerializerSettings()
        {
          MissingMemberHandling = MissingMemberHandling.Ignore,
          NullValueHandling = NullValueHandling.Ignore,
          Error = (EventHandler<ErrorEventArgs>) ((sender, parsingErrorEvent) => parsingErrorEvent.ErrorContext.Handled = true)
        }) ?? IndexUtilizationInfo.Empty;
        return true;
      }
      catch (JsonException ex)
      {
        result = IndexUtilizationInfo.Empty;
        return false;
      }
    }

    public static IndexUtilizationInfo CreateFromString(string delimitedString, bool isBse64Encoded)
    {
      IndexUtilizationInfo result;
      if (isBse64Encoded)
        IndexUtilizationInfo.TryCreateFromDelimitedBase64String(delimitedString, out result);
      else
        IndexUtilizationInfo.TryCreateFromDelimitedString(delimitedString, out result);
      return result;
    }

    public ref struct Accumulator
    {
      public Accumulator(
        IEnumerable<SingleIndexUtilizationEntity> utilizedSingleIndexes,
        IEnumerable<SingleIndexUtilizationEntity> potentialSingleIndexes,
        IEnumerable<CompositeIndexUtilizationEntity> utilizedCompositeIndexes,
        IEnumerable<CompositeIndexUtilizationEntity> potentialCompositeIndexes)
      {
        this.UtilizedSingleIndexes = utilizedSingleIndexes;
        this.PotentialSingleIndexes = potentialSingleIndexes;
        this.UtilizedCompositeIndexes = utilizedCompositeIndexes;
        this.PotentialCompositeIndexes = potentialCompositeIndexes;
      }

      public IEnumerable<SingleIndexUtilizationEntity> UtilizedSingleIndexes { get; }

      public IEnumerable<SingleIndexUtilizationEntity> PotentialSingleIndexes { get; }

      public IEnumerable<CompositeIndexUtilizationEntity> UtilizedCompositeIndexes { get; }

      public IEnumerable<CompositeIndexUtilizationEntity> PotentialCompositeIndexes { get; }

      public IndexUtilizationInfo.Accumulator Accumulate(IndexUtilizationInfo indexUtilizationInfo) => new IndexUtilizationInfo.Accumulator((this.UtilizedSingleIndexes ?? Enumerable.Empty<SingleIndexUtilizationEntity>()).Concat<SingleIndexUtilizationEntity>((IEnumerable<SingleIndexUtilizationEntity>) indexUtilizationInfo.UtilizedSingleIndexes), (this.PotentialSingleIndexes ?? Enumerable.Empty<SingleIndexUtilizationEntity>()).Concat<SingleIndexUtilizationEntity>((IEnumerable<SingleIndexUtilizationEntity>) indexUtilizationInfo.PotentialSingleIndexes), (this.UtilizedCompositeIndexes ?? Enumerable.Empty<CompositeIndexUtilizationEntity>()).Concat<CompositeIndexUtilizationEntity>((IEnumerable<CompositeIndexUtilizationEntity>) indexUtilizationInfo.UtilizedCompositeIndexes), (this.PotentialCompositeIndexes ?? Enumerable.Empty<CompositeIndexUtilizationEntity>()).Concat<CompositeIndexUtilizationEntity>((IEnumerable<CompositeIndexUtilizationEntity>) indexUtilizationInfo.PotentialCompositeIndexes));

      public static IndexUtilizationInfo ToIndexUtilizationInfo(
        IndexUtilizationInfo.Accumulator accumulator)
      {
        return new IndexUtilizationInfo((IReadOnlyList<SingleIndexUtilizationEntity>) accumulator.UtilizedSingleIndexes.ToList<SingleIndexUtilizationEntity>(), (IReadOnlyList<SingleIndexUtilizationEntity>) accumulator.PotentialSingleIndexes.ToList<SingleIndexUtilizationEntity>(), (IReadOnlyList<CompositeIndexUtilizationEntity>) accumulator.UtilizedCompositeIndexes.ToList<CompositeIndexUtilizationEntity>(), (IReadOnlyList<CompositeIndexUtilizationEntity>) accumulator.PotentialCompositeIndexes.ToList<CompositeIndexUtilizationEntity>());
      }
    }
  }
}
