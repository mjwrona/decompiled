// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.PathsDefinition`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public class PathsDefinition<T>
  {
    private readonly List<string> paths = new List<string>();
    private readonly T parent;
    private readonly Action<IEnumerable<string>> attachCallback;

    internal PathsDefinition(T parent, Action<IEnumerable<string>> attachCallback)
    {
      this.parent = parent;
      this.attachCallback = attachCallback;
    }

    public PathsDefinition<T> Path(string path)
    {
      this.paths.Add(path);
      return this;
    }

    public T Attach()
    {
      this.attachCallback((IEnumerable<string>) this.paths);
      return this.parent;
    }
  }
}
