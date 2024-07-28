// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.SpatialIndexDefinition`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public class SpatialIndexDefinition<T>
  {
    private readonly SpatialPath spatialSpec = new SpatialPath();
    private readonly T parent;
    private readonly Action<SpatialPath> attachCallback;

    internal SpatialIndexDefinition(T parent, Action<SpatialPath> attachCallback)
    {
      this.parent = parent;
      this.attachCallback = attachCallback;
    }

    public SpatialIndexDefinition<T> Path(string path)
    {
      this.spatialSpec.Path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentNullException(nameof (path));
      return this;
    }

    public SpatialIndexDefinition<T> Path(string path, params SpatialType[] spatialTypes)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentNullException(nameof (path));
      if (spatialTypes == null)
        throw new ArgumentNullException(nameof (spatialTypes));
      this.spatialSpec.Path = path;
      foreach (SpatialType spatialType in spatialTypes)
        this.spatialSpec.SpatialTypes.Add(spatialType);
      return this;
    }

    public T Attach()
    {
      this.attachCallback(this.spatialSpec);
      return this.parent;
    }
  }
}
