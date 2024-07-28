// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.IndexingPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos
{
  public sealed class IndexingPolicy
  {
    internal const string DefaultPath = "/*";

    public IndexingPolicy()
    {
      this.Automatic = true;
      this.IndexingMode = IndexingMode.Consistent;
    }

    [JsonProperty(PropertyName = "automatic")]
    public bool Automatic { get; set; }

    [JsonProperty(PropertyName = "indexingMode")]
    [JsonConverter(typeof (StringEnumConverter))]
    public IndexingMode IndexingMode { get; set; }

    [JsonProperty(PropertyName = "includedPaths")]
    public Collection<IncludedPath> IncludedPaths { get; internal set; } = new Collection<IncludedPath>();

    [JsonProperty(PropertyName = "excludedPaths")]
    public Collection<ExcludedPath> ExcludedPaths { get; internal set; } = new Collection<ExcludedPath>();

    [JsonProperty(PropertyName = "compositeIndexes")]
    public Collection<Collection<CompositePath>> CompositeIndexes { get; internal set; } = new Collection<Collection<CompositePath>>();

    [JsonProperty(PropertyName = "spatialIndexes")]
    public Collection<SpatialPath> SpatialIndexes { get; internal set; } = new Collection<SpatialPath>();

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    internal sealed class CompositePathEqualityComparer : IEqualityComparer<CompositePath>
    {
      public static readonly IndexingPolicy.CompositePathEqualityComparer Singleton = new IndexingPolicy.CompositePathEqualityComparer();

      public bool Equals(CompositePath compositePath1, CompositePath compositePath2) => compositePath1 == compositePath2 || compositePath1 != null && compositePath2 != null && compositePath1.Path == compositePath2.Path && compositePath1.Order == compositePath2.Order && compositePath1.AdditionalProperties.EqualsTo(compositePath2.AdditionalProperties);

      public int GetHashCode(CompositePath compositePath) => compositePath == null ? 0 : compositePath.Path.GetHashCode() ^ compositePath.Order.GetHashCode();
    }

    internal sealed class CompositePathsEqualityComparer : IEqualityComparer<HashSet<CompositePath>>
    {
      public static readonly IndexingPolicy.CompositePathsEqualityComparer Singleton = new IndexingPolicy.CompositePathsEqualityComparer();
      private static readonly IndexingPolicy.CompositePathEqualityComparer compositePathEqualityComparer = new IndexingPolicy.CompositePathEqualityComparer();

      public bool Equals(
        HashSet<CompositePath> compositePaths1,
        HashSet<CompositePath> compositePaths2)
      {
        if (compositePaths1 == compositePaths2)
          return true;
        return compositePaths1 != null && compositePaths2 != null && compositePaths1.SetEquals((IEnumerable<CompositePath>) compositePaths2);
      }

      public int GetHashCode(HashSet<CompositePath> obj)
      {
        if (obj == null)
          return 0;
        int hashCode = 0;
        foreach (CompositePath compositePath in obj)
          hashCode ^= IndexingPolicy.CompositePathsEqualityComparer.compositePathEqualityComparer.GetHashCode(compositePath);
        return hashCode;
      }
    }

    private sealed class CompositeIndexesEqualityComparer : 
      IEqualityComparer<Collection<Collection<CompositePath>>>
    {
      private static readonly IndexingPolicy.CompositePathEqualityComparer compositePathEqualityComparer = new IndexingPolicy.CompositePathEqualityComparer();
      private static readonly IndexingPolicy.CompositePathsEqualityComparer compositePathsEqualityComparer = new IndexingPolicy.CompositePathsEqualityComparer();

      public bool Equals(
        Collection<Collection<CompositePath>> compositeIndexes1,
        Collection<Collection<CompositePath>> compositeIndexes2)
      {
        if (compositeIndexes1 == compositeIndexes2)
          return true;
        if (compositeIndexes1 == null || compositeIndexes2 == null)
          return false;
        HashSet<HashSet<CompositePath>> compositePathSetSet = new HashSet<HashSet<CompositePath>>((IEqualityComparer<HashSet<CompositePath>>) IndexingPolicy.CompositeIndexesEqualityComparer.compositePathsEqualityComparer);
        HashSet<HashSet<CompositePath>> other = new HashSet<HashSet<CompositePath>>((IEqualityComparer<HashSet<CompositePath>>) IndexingPolicy.CompositeIndexesEqualityComparer.compositePathsEqualityComparer);
        foreach (IEnumerable<CompositePath> collection in compositeIndexes1)
        {
          HashSet<CompositePath> compositePathSet = new HashSet<CompositePath>(collection, (IEqualityComparer<CompositePath>) IndexingPolicy.CompositeIndexesEqualityComparer.compositePathEqualityComparer);
          compositePathSetSet.Add(compositePathSet);
        }
        foreach (IEnumerable<CompositePath> collection in compositeIndexes2)
        {
          HashSet<CompositePath> compositePathSet = new HashSet<CompositePath>(collection, (IEqualityComparer<CompositePath>) IndexingPolicy.CompositeIndexesEqualityComparer.compositePathEqualityComparer);
          other.Add(compositePathSet);
        }
        return compositePathSetSet.SetEquals((IEnumerable<HashSet<CompositePath>>) other);
      }

      public int GetHashCode(
        Collection<Collection<CompositePath>> compositeIndexes)
      {
        int hashCode = 0;
        foreach (IEnumerable<CompositePath> compositeIndex in compositeIndexes)
        {
          HashSet<CompositePath> compositePathSet = new HashSet<CompositePath>(compositeIndex, (IEqualityComparer<CompositePath>) IndexingPolicy.CompositeIndexesEqualityComparer.compositePathEqualityComparer);
          hashCode ^= IndexingPolicy.CompositeIndexesEqualityComparer.compositePathsEqualityComparer.GetHashCode(compositePathSet);
        }
        return hashCode;
      }
    }

    internal sealed class SpatialSpecEqualityComparer : IEqualityComparer<SpatialPath>
    {
      public static readonly IndexingPolicy.SpatialSpecEqualityComparer Singleton = new IndexingPolicy.SpatialSpecEqualityComparer();

      public bool Equals(SpatialPath spatialSpec1, SpatialPath spatialSpec2) => spatialSpec1 == spatialSpec2 || spatialSpec1 != null && spatialSpec2 != null && !(spatialSpec1.Path != spatialSpec2.Path) && new HashSet<SpatialType>((IEnumerable<SpatialType>) spatialSpec1.SpatialTypes).SetEquals((IEnumerable<SpatialType>) new HashSet<SpatialType>((IEnumerable<SpatialType>) spatialSpec2.SpatialTypes)) && spatialSpec1.AdditionalProperties.EqualsTo(spatialSpec2.AdditionalProperties);

      public int GetHashCode(SpatialPath spatialSpec)
      {
        int hashCode = 0 ^ spatialSpec.Path.GetHashCode();
        foreach (SpatialType spatialType in spatialSpec.SpatialTypes)
          hashCode ^= spatialType.GetHashCode();
        return hashCode;
      }
    }

    internal sealed class AdditionalSpatialIndexesEqualityComparer : 
      IEqualityComparer<Collection<SpatialPath>>
    {
      private static readonly IndexingPolicy.SpatialSpecEqualityComparer spatialSpecEqualityComparer = new IndexingPolicy.SpatialSpecEqualityComparer();

      public bool Equals(
        Collection<SpatialPath> additionalSpatialIndexes1,
        Collection<SpatialPath> additionalSpatialIndexes2)
      {
        if (additionalSpatialIndexes1 == additionalSpatialIndexes2)
          return true;
        return additionalSpatialIndexes1 != null && additionalSpatialIndexes2 != null && new HashSet<SpatialPath>((IEnumerable<SpatialPath>) additionalSpatialIndexes1, (IEqualityComparer<SpatialPath>) IndexingPolicy.AdditionalSpatialIndexesEqualityComparer.spatialSpecEqualityComparer).SetEquals((IEnumerable<SpatialPath>) new HashSet<SpatialPath>((IEnumerable<SpatialPath>) additionalSpatialIndexes2, (IEqualityComparer<SpatialPath>) IndexingPolicy.AdditionalSpatialIndexesEqualityComparer.spatialSpecEqualityComparer));
      }

      public int GetHashCode(Collection<SpatialPath> additionalSpatialIndexes)
      {
        int hashCode = 0;
        foreach (SpatialPath additionalSpatialIndex in additionalSpatialIndexes)
          hashCode ^= IndexingPolicy.AdditionalSpatialIndexesEqualityComparer.spatialSpecEqualityComparer.GetHashCode(additionalSpatialIndex);
        return hashCode;
      }
    }

    internal sealed class IndexEqualityComparer : IEqualityComparer<Index>
    {
      public static readonly IndexingPolicy.IndexEqualityComparer Comparer = new IndexingPolicy.IndexEqualityComparer();

      public bool Equals(Index index1, Index index2)
      {
        if (index1 == index2)
          return true;
        if (index1 == null || index2 == null || index1.Kind != index2.Kind || !index1.AdditionalProperties.EqualsTo(index2.AdditionalProperties))
          return false;
        switch (index1.Kind)
        {
          case IndexKind.Hash:
            short? precision1 = ((HashIndex) index1).Precision;
            int? nullable1 = precision1.HasValue ? new int?((int) precision1.GetValueOrDefault()) : new int?();
            precision1 = ((HashIndex) index2).Precision;
            int? nullable2 = precision1.HasValue ? new int?((int) precision1.GetValueOrDefault()) : new int?();
            if (!(nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue) || ((HashIndex) index1).DataType != ((HashIndex) index2).DataType)
              return false;
            break;
          case IndexKind.Range:
            short? precision2 = ((RangeIndex) index1).Precision;
            int? nullable3 = precision2.HasValue ? new int?((int) precision2.GetValueOrDefault()) : new int?();
            precision2 = ((RangeIndex) index2).Precision;
            int? nullable4 = precision2.HasValue ? new int?((int) precision2.GetValueOrDefault()) : new int?();
            if (!(nullable3.GetValueOrDefault() == nullable4.GetValueOrDefault() & nullable3.HasValue == nullable4.HasValue) || ((RangeIndex) index1).DataType != ((RangeIndex) index2).DataType)
              return false;
            break;
          case IndexKind.Spatial:
            if (((SpatialIndex) index1).DataType != ((SpatialIndex) index2).DataType)
              return false;
            break;
          default:
            throw new ArgumentException(string.Format("Unexpected Kind: {0}", (object) index1.Kind));
        }
        return true;
      }

      public int GetHashCode(Index index)
      {
        int num1 = (int) (IndexKind.Hash ^ index.Kind);
        switch (index.Kind)
        {
          case IndexKind.Hash:
            int num2 = num1;
            short? precision1 = ((HashIndex) index).Precision;
            int? nullable1 = precision1.HasValue ? new int?((int) precision1.GetValueOrDefault()) : new int?();
            return (nullable1.HasValue ? new int?(num2 ^ nullable1.GetValueOrDefault()) : new int?()).GetValueOrDefault() ^ ((HashIndex) index).DataType.GetHashCode();
          case IndexKind.Range:
            int num3 = num1;
            short? precision2 = ((RangeIndex) index).Precision;
            int? nullable2 = precision2.HasValue ? new int?((int) precision2.GetValueOrDefault()) : new int?();
            return (nullable2.HasValue ? new int?(num3 ^ nullable2.GetValueOrDefault()) : new int?()).GetValueOrDefault() ^ ((RangeIndex) index).DataType.GetHashCode();
          case IndexKind.Spatial:
            return num1 ^ ((SpatialIndex) index).DataType.GetHashCode();
          default:
            throw new ArgumentException(string.Format("Unexpected Kind: {0}", (object) index.Kind));
        }
      }
    }

    internal sealed class IncludedPathEqualityComparer : IEqualityComparer<IncludedPath>
    {
      public static readonly IndexingPolicy.IncludedPathEqualityComparer Singleton = new IndexingPolicy.IncludedPathEqualityComparer();
      private static readonly IndexingPolicy.IndexEqualityComparer indexEqualityComparer = new IndexingPolicy.IndexEqualityComparer();

      public bool Equals(IncludedPath includedPath1, IncludedPath includedPath2)
      {
        if (includedPath1 == includedPath2)
          return true;
        return includedPath1 != null && includedPath2 != null && !(includedPath1.Path != includedPath2.Path) && includedPath1.AdditionalProperties.EqualsTo(includedPath2.AdditionalProperties) && new HashSet<Index>((IEnumerable<Index>) includedPath1.Indexes, (IEqualityComparer<Index>) IndexingPolicy.IncludedPathEqualityComparer.indexEqualityComparer).SetEquals((IEnumerable<Index>) new HashSet<Index>((IEnumerable<Index>) includedPath2.Indexes, (IEqualityComparer<Index>) IndexingPolicy.IncludedPathEqualityComparer.indexEqualityComparer));
      }

      public int GetHashCode(IncludedPath includedPath)
      {
        int hashCode = 0 ^ includedPath.Path.GetHashCode();
        foreach (Index index in includedPath.Indexes)
          hashCode ^= IndexingPolicy.IncludedPathEqualityComparer.indexEqualityComparer.GetHashCode(index);
        return hashCode;
      }
    }

    internal sealed class ExcludedPathEqualityComparer : IEqualityComparer<ExcludedPath>
    {
      public static readonly IndexingPolicy.ExcludedPathEqualityComparer Singleton = new IndexingPolicy.ExcludedPathEqualityComparer();

      public bool Equals(ExcludedPath excludedPath1, ExcludedPath excludedPath2)
      {
        if (excludedPath1 == excludedPath2)
          return true;
        return excludedPath1 != null && excludedPath2 != null && excludedPath1.Path == excludedPath2.Path && excludedPath1.AdditionalProperties.EqualsTo(excludedPath2.AdditionalProperties);
      }

      public int GetHashCode(ExcludedPath excludedPath1) => excludedPath1.Path.GetHashCode();
    }

    internal sealed class IndexingPolicyEqualityComparer : IEqualityComparer<IndexingPolicy>
    {
      public static readonly IndexingPolicy.IndexingPolicyEqualityComparer Singleton = new IndexingPolicy.IndexingPolicyEqualityComparer();
      private static readonly IndexingPolicy.IncludedPathEqualityComparer includedPathEqualityComparer = new IndexingPolicy.IncludedPathEqualityComparer();
      private static readonly IndexingPolicy.ExcludedPathEqualityComparer excludedPathEqualityComparer = new IndexingPolicy.ExcludedPathEqualityComparer();
      private static readonly IndexingPolicy.CompositeIndexesEqualityComparer compositeIndexesEqualityComparer = new IndexingPolicy.CompositeIndexesEqualityComparer();
      private static readonly IndexingPolicy.AdditionalSpatialIndexesEqualityComparer additionalSpatialIndexesEqualityComparer = new IndexingPolicy.AdditionalSpatialIndexesEqualityComparer();

      public bool Equals(IndexingPolicy indexingPolicy1, IndexingPolicy indexingPolicy2) => indexingPolicy1 == indexingPolicy2 || (1 & (indexingPolicy1 == null ? 0 : (indexingPolicy2 != null ? 1 : 0)) & (indexingPolicy1.Automatic == indexingPolicy2.Automatic ? 1 : 0) & (indexingPolicy1.IndexingMode == indexingPolicy2.IndexingMode ? 1 : 0) & (IndexingPolicy.IndexingPolicyEqualityComparer.compositeIndexesEqualityComparer.Equals(indexingPolicy1.CompositeIndexes, indexingPolicy2.CompositeIndexes) ? 1 : 0) & (IndexingPolicy.IndexingPolicyEqualityComparer.additionalSpatialIndexesEqualityComparer.Equals(indexingPolicy1.SpatialIndexes, indexingPolicy2.SpatialIndexes) ? 1 : 0) & (indexingPolicy1.AdditionalProperties.EqualsTo(indexingPolicy2.AdditionalProperties) ? 1 : 0) & (new HashSet<IncludedPath>((IEnumerable<IncludedPath>) indexingPolicy1.IncludedPaths, (IEqualityComparer<IncludedPath>) IndexingPolicy.IndexingPolicyEqualityComparer.includedPathEqualityComparer).SetEquals((IEnumerable<IncludedPath>) new HashSet<IncludedPath>((IEnumerable<IncludedPath>) indexingPolicy2.IncludedPaths, (IEqualityComparer<IncludedPath>) IndexingPolicy.IndexingPolicyEqualityComparer.includedPathEqualityComparer)) ? 1 : 0) & (new HashSet<ExcludedPath>((IEnumerable<ExcludedPath>) indexingPolicy1.ExcludedPaths, (IEqualityComparer<ExcludedPath>) IndexingPolicy.IndexingPolicyEqualityComparer.excludedPathEqualityComparer).SetEquals((IEnumerable<ExcludedPath>) new HashSet<ExcludedPath>((IEnumerable<ExcludedPath>) indexingPolicy2.ExcludedPaths, (IEqualityComparer<ExcludedPath>) IndexingPolicy.IndexingPolicyEqualityComparer.excludedPathEqualityComparer)) ? 1 : 0)) != 0;

      public int GetHashCode(IndexingPolicy indexingPolicy)
      {
        int hashCode = 0 ^ indexingPolicy.Automatic.GetHashCode() ^ indexingPolicy.IndexingMode.GetHashCode() ^ IndexingPolicy.IndexingPolicyEqualityComparer.compositeIndexesEqualityComparer.GetHashCode(indexingPolicy.CompositeIndexes) ^ IndexingPolicy.IndexingPolicyEqualityComparer.additionalSpatialIndexesEqualityComparer.GetHashCode(indexingPolicy.SpatialIndexes);
        foreach (IncludedPath includedPath in indexingPolicy.IncludedPaths)
          hashCode ^= IndexingPolicy.IndexingPolicyEqualityComparer.includedPathEqualityComparer.GetHashCode(includedPath);
        foreach (ExcludedPath excludedPath in indexingPolicy.ExcludedPaths)
          hashCode ^= IndexingPolicy.IndexingPolicyEqualityComparer.excludedPathEqualityComparer.GetHashCode(excludedPath);
        return hashCode;
      }
    }
  }
}
