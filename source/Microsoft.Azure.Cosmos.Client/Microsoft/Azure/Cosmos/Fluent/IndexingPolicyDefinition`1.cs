// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.IndexingPolicyDefinition`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public class IndexingPolicyDefinition<T>
  {
    private readonly IndexingPolicy indexingPolicy = new IndexingPolicy();
    private readonly T parent;
    private readonly Action<IndexingPolicy> attachCallback;
    private PathsDefinition<IndexingPolicyDefinition<T>> includedPathsBuilder;
    private PathsDefinition<IndexingPolicyDefinition<T>> excludedPathsBuilder;

    public IndexingPolicyDefinition()
    {
    }

    internal IndexingPolicyDefinition(T parent, Action<IndexingPolicy> attachCallback)
    {
      this.parent = parent;
      this.attachCallback = attachCallback;
    }

    public IndexingPolicyDefinition<T> WithIndexingMode(IndexingMode indexingMode)
    {
      this.indexingPolicy.IndexingMode = indexingMode;
      return this;
    }

    public IndexingPolicyDefinition<T> WithAutomaticIndexing(bool enabled)
    {
      this.indexingPolicy.Automatic = enabled;
      return this;
    }

    public PathsDefinition<IndexingPolicyDefinition<T>> WithIncludedPaths()
    {
      if (this.includedPathsBuilder == null)
        this.includedPathsBuilder = new PathsDefinition<IndexingPolicyDefinition<T>>(this, (Action<IEnumerable<string>>) (paths => this.AddIncludedPaths(paths)));
      return this.includedPathsBuilder;
    }

    public PathsDefinition<IndexingPolicyDefinition<T>> WithExcludedPaths()
    {
      if (this.excludedPathsBuilder == null)
        this.excludedPathsBuilder = new PathsDefinition<IndexingPolicyDefinition<T>>(this, (Action<IEnumerable<string>>) (paths => this.AddExcludedPaths(paths)));
      return this.excludedPathsBuilder;
    }

    public CompositeIndexDefinition<IndexingPolicyDefinition<T>> WithCompositeIndex() => new CompositeIndexDefinition<IndexingPolicyDefinition<T>>(this, (Action<Collection<CompositePath>>) (compositePaths => this.AddCompositePaths(compositePaths)));

    public SpatialIndexDefinition<IndexingPolicyDefinition<T>> WithSpatialIndex() => new SpatialIndexDefinition<IndexingPolicyDefinition<T>>(this, (Action<SpatialPath>) (spatialIndex => this.AddSpatialPath(spatialIndex)));

    public T Attach()
    {
      this.attachCallback(this.indexingPolicy);
      return this.parent;
    }

    private void AddCompositePaths(Collection<CompositePath> compositePaths) => this.indexingPolicy.CompositeIndexes.Add(compositePaths);

    private void AddSpatialPath(SpatialPath spatialSpec) => this.indexingPolicy.SpatialIndexes.Add(spatialSpec);

    private void AddIncludedPaths(IEnumerable<string> paths)
    {
      foreach (string path in paths)
        this.indexingPolicy.IncludedPaths.Add(new IncludedPath()
        {
          Path = path
        });
    }

    private void AddExcludedPaths(IEnumerable<string> paths)
    {
      foreach (string path in paths)
        this.indexingPolicy.ExcludedPaths.Add(new ExcludedPath()
        {
          Path = path
        });
    }
  }
}
