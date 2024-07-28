// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.ParseTreeProperty`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Concurrent;

namespace Antlr4.Runtime.Tree
{
  internal class ParseTreeProperty<V>
  {
    protected internal ConcurrentDictionary<IParseTree, V> annotations = new ConcurrentDictionary<IParseTree, V>();

    public virtual V Get(IParseTree node)
    {
      V v;
      return !this.annotations.TryGetValue(node, out v) ? default (V) : v;
    }

    public virtual void Put(IParseTree node, V value) => this.annotations[node] = value;

    public virtual V RemoveFrom(IParseTree node)
    {
      V v;
      return !this.annotations.TryRemove(node, out v) ? default (V) : v;
    }
  }
}
