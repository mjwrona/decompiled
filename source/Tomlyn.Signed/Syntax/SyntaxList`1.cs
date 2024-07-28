// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxList`1
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections;
using System.Collections.Generic;
using Tomlyn.Helpers;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class SyntaxList<TSyntaxNode> : SyntaxList, IEnumerable<TSyntaxNode>, IEnumerable where TSyntaxNode : SyntaxNode
  {
    public void Add(TSyntaxNode node)
    {
      if ((object) node == null)
        throw new ArgumentNullException(nameof (node));
      if (node.Parent != null)
        throw ThrowHelper.GetExpectingNoParentException();
      this.Children.Add((SyntaxNode) node);
      node.Parent = (SyntaxNode) this;
    }

    public TSyntaxNode? GetChild(int index) => (TSyntaxNode) base.GetChild(index);

    protected override SyntaxNode GetChildImpl(int index) => this.Children[index];

    public void RemoveChildAt(int index)
    {
      SyntaxNode child = this.Children[index];
      this.Children.RemoveAt(index);
      child.Parent = (SyntaxNode) null;
    }

    public void RemoveChild(TSyntaxNode node)
    {
      if ((object) node == null)
        throw new ArgumentNullException(nameof (node));
      if (node.Parent != this)
        throw new InvalidOperationException("The node is not part of this list");
      this.Children.Remove((SyntaxNode) node);
      node.Parent = (SyntaxNode) null;
    }

    public SyntaxList<
    #nullable disable
    TSyntaxNode>.Enumerator GetEnumerator() => new SyntaxList<TSyntaxNode>.Enumerator(this.Children);


    #nullable enable
    IEnumerator<TSyntaxNode> IEnumerable<TSyntaxNode>.GetEnumerator() => (IEnumerator<TSyntaxNode>) this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public struct Enumerator : IEnumerator<TSyntaxNode>, IEnumerator, IDisposable
    {
      private readonly List<SyntaxNode> _nodes;
      private int _index;

      public Enumerator(List<SyntaxNode> nodes)
      {
        this._nodes = nodes;
        this._index = -1;
      }

      public bool MoveNext()
      {
        if (this._index + 1 == this._nodes.Count)
          return false;
        ++this._index;
        return true;
      }

      public void Reset() => this._index = -1;

      public TSyntaxNode Current
      {
        get
        {
          if (this._index < 0)
            throw new InvalidOperationException("MoveNext must be called before accessing Current");
          return (TSyntaxNode) this._nodes[this._index];
        }
      }

      object IEnumerator.Current => (object) this.Current;

      public void Dispose()
      {
      }
    }
  }
}
