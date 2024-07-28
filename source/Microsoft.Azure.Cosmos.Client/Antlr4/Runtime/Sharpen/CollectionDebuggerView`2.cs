// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.CollectionDebuggerView`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace Antlr4.Runtime.Sharpen
{
  internal sealed class CollectionDebuggerView<T, U>
  {
    private readonly ICollection<KeyValuePair<T, U>> c;

    public CollectionDebuggerView(ICollection<KeyValuePair<T, U>> col) => this.c = col;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<T, U>[] Items
    {
      get
      {
        KeyValuePair<T, U>[] array = new KeyValuePair<T, U>[this.c.Count];
        this.c.CopyTo(array, 0);
        return array;
      }
    }
  }
}
