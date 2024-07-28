// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.UniqueKeyDefinition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public class UniqueKeyDefinition
  {
    private readonly Collection<string> paths = new Collection<string>();
    private readonly ContainerBuilder parent;
    private readonly Action<UniqueKey> attachCallback;

    internal UniqueKeyDefinition(ContainerBuilder parent, Action<UniqueKey> attachCallback)
    {
      this.parent = parent;
      this.attachCallback = attachCallback;
    }

    public UniqueKeyDefinition Path(string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentNullException(nameof (path));
      this.paths.Add(path);
      return this;
    }

    public ContainerBuilder Attach()
    {
      this.attachCallback(new UniqueKey()
      {
        Paths = this.paths
      });
      return this.parent;
    }
  }
}
