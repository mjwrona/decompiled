// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexingPolicyBuilder
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  internal sealed class IndexingPolicyBuilder
  {
    [JsonProperty(PropertyName = "includedPaths")]
    private readonly HashSet<IncludedPath> includedPaths;
    [JsonProperty(PropertyName = "excludedPaths")]
    private readonly HashSet<ExcludedPath> excludedPaths;
    [JsonProperty(PropertyName = "compositeIndexes")]
    private readonly HashSet<HashSet<IndexingPolicyBuilder.CompositePath>> compositeIndexes;
    [JsonProperty(PropertyName = "spatialIndexes")]
    private readonly HashSet<IndexingPolicyBuilder.SpatialIndex> spatialIndexes;
    private static readonly Collection<Index> DefaultIndexes = new Collection<Index>()
    {
      (Index) Index.Range(DataType.String, (short) -1),
      (Index) Index.Range(DataType.Number, (short) -1)
    };

    [JsonProperty(PropertyName = "automatic")]
    public bool Automatic { get; set; }

    [JsonProperty(PropertyName = "indexingMode")]
    [JsonConverter(typeof (StringEnumConverter))]
    public IndexingMode IndexingMode { get; set; }

    public IndexingPolicyBuilder()
    {
      this.includedPaths = new HashSet<IncludedPath>((IEqualityComparer<IncludedPath>) IndexingPolicy.IncludedPathEqualityComparer.Singleton);
      this.excludedPaths = new HashSet<ExcludedPath>((IEqualityComparer<ExcludedPath>) IndexingPolicy.ExcludedPathEqualityComparer.Singleton);
      this.compositeIndexes = new HashSet<HashSet<IndexingPolicyBuilder.CompositePath>>();
      this.spatialIndexes = new HashSet<IndexingPolicyBuilder.SpatialIndex>((IEqualityComparer<IndexingPolicyBuilder.SpatialIndex>) IndexingPolicyBuilder.SpatialIndex.SpatialIndexEqualityComparer.Singleton);
      this.Automatic = true;
      this.IndexingMode = IndexingMode.Consistent;
    }

    public void AddIncludedPath(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path must not be null.");
      this.includedPaths.Add(new IncludedPath()
      {
        Path = path,
        Indexes = IndexingPolicyBuilder.DefaultIndexes
      });
    }

    public void AddExcludedPath(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path must not be null.");
      this.excludedPaths.Add(new ExcludedPath()
      {
        Path = path
      });
    }

    public void AddCompositeIndex(
      params IndexingPolicyBuilder.CompositePath[] compositePaths)
    {
      if (compositePaths == null)
        throw new ArgumentNullException("compositePaths must not be null.");
      HashSet<IndexingPolicyBuilder.CompositePath> compositePathSet = new HashSet<IndexingPolicyBuilder.CompositePath>((IEqualityComparer<IndexingPolicyBuilder.CompositePath>) IndexingPolicyBuilder.CompositePath.CompositePathEqualityComparer.Singleton);
      foreach (IndexingPolicyBuilder.CompositePath compositePath in compositePaths)
      {
        if (compositePath == null)
          throw new ArgumentException("compositePaths must not have null elements.");
        compositePathSet.Add(compositePath);
      }
      this.compositeIndexes.Add(compositePathSet);
    }

    public void AddSpatialIndex(IndexingPolicyBuilder.SpatialIndex spatialIndex)
    {
      if (spatialIndex == null)
        throw new ArgumentNullException("spatialIndex must not be null.");
      this.spatialIndexes.Add(spatialIndex);
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.Indented);

    public sealed class CompositePath
    {
      [JsonProperty(PropertyName = "path")]
      public string Path { get; }

      [JsonProperty(PropertyName = "order")]
      [JsonConverter(typeof (StringEnumConverter))]
      public CompositePathSortOrder CompositePathSortOrder { get; }

      public CompositePath(string path, CompositePathSortOrder compositePathSortOrder = CompositePathSortOrder.Ascending)
      {
        this.Path = path != null ? path : throw new ArgumentNullException("path must not be null.");
        this.CompositePathSortOrder = compositePathSortOrder;
      }

      public sealed class CompositePathEqualityComparer : 
        IEqualityComparer<IndexingPolicyBuilder.CompositePath>
      {
        public static readonly IndexingPolicyBuilder.CompositePath.CompositePathEqualityComparer Singleton = new IndexingPolicyBuilder.CompositePath.CompositePathEqualityComparer();

        public bool Equals(
          IndexingPolicyBuilder.CompositePath compositePath1,
          IndexingPolicyBuilder.CompositePath compositePath2)
        {
          return compositePath1 == compositePath2 || compositePath1 != null && compositePath2 != null && compositePath1.Path == compositePath2.Path && compositePath2.CompositePathSortOrder == compositePath2.CompositePathSortOrder;
        }

        public int GetHashCode(IndexingPolicyBuilder.CompositePath compositePath) => compositePath == null ? 0 : compositePath.Path.GetHashCode() ^ compositePath.CompositePathSortOrder.GetHashCode();
      }
    }

    private sealed class CompositePathsEqualityComparer : 
      IEqualityComparer<HashSet<IndexingPolicyBuilder.CompositePath>>
    {
      public static readonly IndexingPolicyBuilder.CompositePathsEqualityComparer Singleton = new IndexingPolicyBuilder.CompositePathsEqualityComparer();
      private static readonly IndexingPolicyBuilder.CompositePath.CompositePathEqualityComparer compositePathEqualityComparer = new IndexingPolicyBuilder.CompositePath.CompositePathEqualityComparer();

      public bool Equals(
        HashSet<IndexingPolicyBuilder.CompositePath> compositePaths1,
        HashSet<IndexingPolicyBuilder.CompositePath> compositePaths2)
      {
        if (compositePaths1 == compositePaths2)
          return true;
        return compositePaths1 != null && compositePaths2 != null && compositePaths1.SetEquals((IEnumerable<IndexingPolicyBuilder.CompositePath>) compositePaths2);
      }

      public int GetHashCode(HashSet<IndexingPolicyBuilder.CompositePath> obj)
      {
        if (obj == null)
          return 0;
        int hashCode = 0;
        foreach (IndexingPolicyBuilder.CompositePath compositePath in obj)
          hashCode ^= IndexingPolicyBuilder.CompositePathsEqualityComparer.compositePathEqualityComparer.GetHashCode(compositePath);
        return hashCode;
      }
    }

    public sealed class SpatialIndex
    {
      [JsonProperty(PropertyName = "path")]
      public string Path { get; }

      [JsonProperty(PropertyName = "types", ItemConverterType = typeof (StringEnumConverter))]
      public HashSet<SpatialType> SpatialTypes { get; }

      public SpatialIndex(string path, params SpatialType[] spatialTypes)
      {
        this.Path = path != null ? path : throw new ArgumentNullException("path must not be null.");
        this.SpatialTypes = new HashSet<SpatialType>();
        foreach (SpatialType spatialType in spatialTypes)
          this.SpatialTypes.Add(spatialType);
      }

      public sealed class SpatialIndexEqualityComparer : 
        IEqualityComparer<IndexingPolicyBuilder.SpatialIndex>
      {
        public static readonly IndexingPolicyBuilder.SpatialIndex.SpatialIndexEqualityComparer Singleton = new IndexingPolicyBuilder.SpatialIndex.SpatialIndexEqualityComparer();

        public bool Equals(
          IndexingPolicyBuilder.SpatialIndex x,
          IndexingPolicyBuilder.SpatialIndex y)
        {
          if (x == y)
            return true;
          return x != null && y != null && (1 & (x.Path.Equals(y.Path) ? 1 : 0) & (x.SpatialTypes.SetEquals((IEnumerable<SpatialType>) y.SpatialTypes) ? 1 : 0)) != 0;
        }

        public int GetHashCode(IndexingPolicyBuilder.SpatialIndex obj)
        {
          int hashCode = 0 ^ obj.Path.GetHashCode();
          foreach (int spatialType in obj.SpatialTypes)
            hashCode ^= obj.Path.GetHashCode();
          return hashCode;
        }
      }
    }
  }
}
