// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexingPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class IndexingPolicy : JsonSerializable, ICloneable
  {
    private static readonly string DefaultPath = "/*";
    private Collection<IncludedPath> includedPaths;
    private Collection<ExcludedPath> excludedPaths;
    private Collection<Collection<CompositePath>> compositeIndexes;
    private Collection<SpatialSpec> spatialIndexes;

    public IndexingPolicy()
    {
      this.Automatic = true;
      this.IndexingMode = IndexingMode.Consistent;
    }

    public IndexingPolicy(params Index[] defaultIndexOverrides)
      : this()
    {
      this.IncludedPaths = defaultIndexOverrides != null ? new Collection<IncludedPath>()
      {
        new IncludedPath()
        {
          Path = IndexingPolicy.DefaultPath,
          Indexes = new Collection<Index>((IList<Index>) defaultIndexOverrides)
        }
      } : throw new ArgumentNullException(nameof (defaultIndexOverrides));
    }

    [JsonProperty(PropertyName = "automatic")]
    public bool Automatic
    {
      get => this.GetValue<bool>("automatic");
      set => this.SetValue("automatic", (object) value);
    }

    [JsonProperty(PropertyName = "indexingMode")]
    [JsonConverter(typeof (StringEnumConverter))]
    public IndexingMode IndexingMode
    {
      get
      {
        IndexingMode indexingMode = IndexingMode.Lazy;
        string str = this.GetValue<string>("indexingMode");
        if (!string.IsNullOrEmpty(str))
          indexingMode = (IndexingMode) Enum.Parse(typeof (IndexingMode), str, true);
        return indexingMode;
      }
      set => this.SetValue("indexingMode", (object) value.ToString());
    }

    [JsonProperty(PropertyName = "includedPaths")]
    public Collection<IncludedPath> IncludedPaths
    {
      get
      {
        if (this.includedPaths == null)
        {
          this.includedPaths = this.GetValue<Collection<IncludedPath>>("includedPaths");
          if (this.includedPaths == null)
            this.includedPaths = new Collection<IncludedPath>();
        }
        return this.includedPaths;
      }
      set
      {
        this.includedPaths = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (IncludedPaths)));
        this.SetValue("includedPaths", (object) this.includedPaths);
      }
    }

    [JsonProperty(PropertyName = "excludedPaths")]
    public Collection<ExcludedPath> ExcludedPaths
    {
      get
      {
        if (this.excludedPaths == null)
        {
          this.excludedPaths = this.GetValue<Collection<ExcludedPath>>("excludedPaths");
          if (this.excludedPaths == null)
            this.excludedPaths = new Collection<ExcludedPath>();
        }
        return this.excludedPaths;
      }
      set
      {
        this.excludedPaths = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (ExcludedPaths)));
        this.SetValue("excludedPaths", (object) this.excludedPaths);
      }
    }

    [JsonProperty(PropertyName = "compositeIndexes")]
    public Collection<Collection<CompositePath>> CompositeIndexes
    {
      get
      {
        if (this.compositeIndexes == null)
        {
          this.compositeIndexes = this.GetValue<Collection<Collection<CompositePath>>>("compositeIndexes");
          if (this.compositeIndexes == null)
            this.compositeIndexes = new Collection<Collection<CompositePath>>();
        }
        return this.compositeIndexes;
      }
      set
      {
        this.compositeIndexes = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (CompositeIndexes)));
        this.SetValue("compositeIndexes", (object) this.compositeIndexes);
      }
    }

    [JsonProperty(PropertyName = "spatialIndexes")]
    public Collection<SpatialSpec> SpatialIndexes
    {
      get
      {
        if (this.spatialIndexes == null)
        {
          this.spatialIndexes = this.GetValue<Collection<SpatialSpec>>("spatialIndexes");
          if (this.spatialIndexes == null)
            this.spatialIndexes = new Collection<SpatialSpec>();
        }
        return this.spatialIndexes;
      }
      set
      {
        this.spatialIndexes = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) "spatialIndexes"));
        this.SetValue("spatialIndexes", (object) this.spatialIndexes);
      }
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<bool>("automatic");
      Helpers.ValidateEnumProperties<IndexingMode>(this.IndexingMode);
      foreach (JsonSerializable includedPath in this.IncludedPaths)
        includedPath.Validate();
      foreach (JsonSerializable excludedPath in this.ExcludedPaths)
        excludedPath.Validate();
      foreach (Collection<CompositePath> compositeIndex in this.CompositeIndexes)
      {
        foreach (JsonSerializable jsonSerializable in compositeIndex)
          jsonSerializable.Validate();
      }
      foreach (JsonSerializable spatialIndex in this.SpatialIndexes)
        spatialIndex.Validate();
    }

    internal override void OnSave()
    {
      if (this.IndexingMode != IndexingMode.None && this.IncludedPaths.Count == 0 && this.ExcludedPaths.Count == 0)
        this.IncludedPaths.Add(new IncludedPath()
        {
          Path = IndexingPolicy.DefaultPath
        });
      foreach (JsonSerializable includedPath in this.IncludedPaths)
        includedPath.OnSave();
      this.SetValue("includedPaths", (object) this.IncludedPaths);
      foreach (JsonSerializable excludedPath in this.ExcludedPaths)
        excludedPath.OnSave();
      this.SetValue("excludedPaths", (object) this.ExcludedPaths);
      foreach (Collection<CompositePath> compositeIndex in this.CompositeIndexes)
      {
        foreach (JsonSerializable jsonSerializable in compositeIndex)
          jsonSerializable.OnSave();
      }
      this.SetValue("compositeIndexes", (object) this.CompositeIndexes);
      foreach (JsonSerializable spatialIndex in this.SpatialIndexes)
        spatialIndex.OnSave();
      this.SetValue("spatialIndexes", (object) this.spatialIndexes);
    }

    public object Clone()
    {
      IndexingPolicy indexingPolicy = new IndexingPolicy()
      {
        Automatic = this.Automatic,
        IndexingMode = this.IndexingMode
      };
      foreach (IncludedPath includedPath in this.IncludedPaths)
        indexingPolicy.IncludedPaths.Add((IncludedPath) includedPath.Clone());
      foreach (ExcludedPath excludedPath in this.ExcludedPaths)
        indexingPolicy.ExcludedPaths.Add((ExcludedPath) excludedPath.Clone());
      foreach (Collection<CompositePath> compositeIndex in this.CompositeIndexes)
      {
        Collection<CompositePath> collection = new Collection<CompositePath>();
        foreach (CompositePath compositePath1 in compositeIndex)
        {
          CompositePath compositePath2 = (CompositePath) compositePath1.Clone();
          collection.Add(compositePath2);
        }
        indexingPolicy.CompositeIndexes.Add(collection);
      }
      Collection<SpatialSpec> collection1 = new Collection<SpatialSpec>();
      foreach (SpatialSpec spatialIndex in this.SpatialIndexes)
      {
        SpatialSpec spatialSpec = (SpatialSpec) spatialIndex.Clone();
        collection1.Add(spatialSpec);
      }
      indexingPolicy.SpatialIndexes = collection1;
      return (object) indexingPolicy;
    }

    internal sealed class CompositePathEqualityComparer : IEqualityComparer<CompositePath>
    {
      public static readonly IndexingPolicy.CompositePathEqualityComparer Singleton = new IndexingPolicy.CompositePathEqualityComparer();

      public bool Equals(CompositePath compositePath1, CompositePath compositePath2) => compositePath1 == compositePath2 || compositePath1 != null && compositePath2 != null && compositePath1.Path == compositePath2.Path && compositePath2.Order == compositePath2.Order;

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

    internal sealed class SpatialSpecEqualityComparer : IEqualityComparer<SpatialSpec>
    {
      public static readonly IndexingPolicy.SpatialSpecEqualityComparer Singleton = new IndexingPolicy.SpatialSpecEqualityComparer();

      public bool Equals(SpatialSpec spatialSpec1, SpatialSpec spatialSpec2) => spatialSpec1 == spatialSpec2 || spatialSpec1 != null && spatialSpec2 != null && !(spatialSpec1.Path != spatialSpec2.Path) && new HashSet<SpatialType>((IEnumerable<SpatialType>) spatialSpec1.SpatialTypes).SetEquals((IEnumerable<SpatialType>) new HashSet<SpatialType>((IEnumerable<SpatialType>) spatialSpec2.SpatialTypes));

      public int GetHashCode(SpatialSpec spatialSpec)
      {
        int hashCode = 0 ^ spatialSpec.Path.GetHashCode();
        foreach (SpatialType spatialType in spatialSpec.SpatialTypes)
          hashCode ^= spatialType.GetHashCode();
        return hashCode;
      }
    }

    internal sealed class AdditionalSpatialIndexesEqualityComparer : 
      IEqualityComparer<Collection<SpatialSpec>>
    {
      private static readonly IndexingPolicy.SpatialSpecEqualityComparer spatialSpecEqualityComparer = new IndexingPolicy.SpatialSpecEqualityComparer();

      public bool Equals(
        Collection<SpatialSpec> additionalSpatialIndexes1,
        Collection<SpatialSpec> additionalSpatialIndexes2)
      {
        if (additionalSpatialIndexes1 == additionalSpatialIndexes2)
          return true;
        if (additionalSpatialIndexes1 == null || additionalSpatialIndexes2 == null)
          return false;
        HashSet<SpatialSpec> spatialSpecSet1 = new HashSet<SpatialSpec>((IEnumerable<SpatialSpec>) additionalSpatialIndexes1, (IEqualityComparer<SpatialSpec>) IndexingPolicy.AdditionalSpatialIndexesEqualityComparer.spatialSpecEqualityComparer);
        HashSet<SpatialSpec> spatialSpecSet2 = new HashSet<SpatialSpec>((IEnumerable<SpatialSpec>) additionalSpatialIndexes2, (IEqualityComparer<SpatialSpec>) IndexingPolicy.AdditionalSpatialIndexesEqualityComparer.spatialSpecEqualityComparer);
        Collection<SpatialSpec> other = additionalSpatialIndexes2;
        return spatialSpecSet1.SetEquals((IEnumerable<SpatialSpec>) other);
      }

      public int GetHashCode(Collection<SpatialSpec> additionalSpatialIndexes)
      {
        int hashCode = 0;
        foreach (SpatialSpec additionalSpatialIndex in additionalSpatialIndexes)
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
        if (index1 == null || index2 == null || index1.Kind != index2.Kind)
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
        return includedPath1 != null && includedPath2 != null && !(includedPath1.Path != includedPath2.Path) && new HashSet<Index>((IEnumerable<Index>) includedPath1.Indexes, (IEqualityComparer<Index>) IndexingPolicy.IncludedPathEqualityComparer.indexEqualityComparer).SetEquals((IEnumerable<Index>) new HashSet<Index>((IEnumerable<Index>) includedPath2.Indexes, (IEqualityComparer<Index>) IndexingPolicy.IncludedPathEqualityComparer.indexEqualityComparer));
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
        return excludedPath1 != null && excludedPath2 != null && excludedPath1.Path == excludedPath2.Path;
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

      public bool Equals(IndexingPolicy indexingPolicy1, IndexingPolicy indexingPolicy2) => indexingPolicy1 == indexingPolicy2 || (1 & (indexingPolicy1 == null ? 0 : (indexingPolicy2 != null ? 1 : 0)) & (indexingPolicy1.Automatic == indexingPolicy2.Automatic ? 1 : 0) & (indexingPolicy1.IndexingMode == indexingPolicy2.IndexingMode ? 1 : 0) & (IndexingPolicy.IndexingPolicyEqualityComparer.compositeIndexesEqualityComparer.Equals(indexingPolicy1.CompositeIndexes, indexingPolicy2.CompositeIndexes) ? 1 : 0) & (IndexingPolicy.IndexingPolicyEqualityComparer.additionalSpatialIndexesEqualityComparer.Equals(indexingPolicy1.SpatialIndexes, indexingPolicy2.SpatialIndexes) ? 1 : 0) & (new HashSet<IncludedPath>((IEnumerable<IncludedPath>) indexingPolicy1.IncludedPaths, (IEqualityComparer<IncludedPath>) IndexingPolicy.IndexingPolicyEqualityComparer.includedPathEqualityComparer).SetEquals((IEnumerable<IncludedPath>) new HashSet<IncludedPath>((IEnumerable<IncludedPath>) indexingPolicy2.IncludedPaths, (IEqualityComparer<IncludedPath>) IndexingPolicy.IndexingPolicyEqualityComparer.includedPathEqualityComparer)) ? 1 : 0) & (new HashSet<ExcludedPath>((IEnumerable<ExcludedPath>) indexingPolicy1.ExcludedPaths, (IEqualityComparer<ExcludedPath>) IndexingPolicy.IndexingPolicyEqualityComparer.excludedPathEqualityComparer).SetEquals((IEnumerable<ExcludedPath>) new HashSet<ExcludedPath>((IEnumerable<ExcludedPath>) indexingPolicy2.ExcludedPaths, (IEqualityComparer<ExcludedPath>) IndexingPolicy.IndexingPolicyEqualityComparer.excludedPathEqualityComparer)) ? 1 : 0)) != 0;

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
