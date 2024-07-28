// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.CompositeIndexDefinition`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public class CompositeIndexDefinition<T>
  {
    private readonly Collection<CompositePath> compositePaths = new Collection<CompositePath>();
    private readonly T parent;
    private readonly Action<Collection<CompositePath>> attachCallback;

    internal CompositeIndexDefinition(T parent, Action<Collection<CompositePath>> attachCallback)
    {
      this.parent = parent;
      this.attachCallback = attachCallback;
    }

    public CompositeIndexDefinition<T> Path(string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentNullException(nameof (path));
      this.compositePaths.Add(new CompositePath()
      {
        Path = path
      });
      return this;
    }

    public CompositeIndexDefinition<T> Path(string path, CompositePathSortOrder sortOrder)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentNullException(nameof (path));
      this.compositePaths.Add(new CompositePath()
      {
        Path = path,
        Order = sortOrder
      });
      return this;
    }

    public T Attach()
    {
      this.attachCallback(this.compositePaths);
      return this.parent;
    }
  }
}
