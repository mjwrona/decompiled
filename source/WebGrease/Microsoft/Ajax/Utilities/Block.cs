// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Block
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class Block : AstNode, IEnumerable<AstNode>, IEnumerable
  {
    private List<AstNode> m_list;

    public AstNode this[int index]
    {
      get => this.m_list[index];
      set
      {
        this.m_list[index].IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        if (value != null)
        {
          this.m_list[index] = value;
          this.m_list[index].Parent = (AstNode) this;
        }
        else
          this.m_list.RemoveAt(index);
      }
    }

    public int Count => this.m_list.Count;

    public bool BraceOnNewLine { get; set; }

    public bool IsModule { get; set; }

    public override Context TerminatingContext
    {
      get
      {
        Context terminatingContext = base.TerminatingContext;
        if (terminatingContext != null)
          return terminatingContext;
        return this.m_list.Count != 1 ? (Context) null : this.m_list[0].TerminatingContext;
      }
    }

    public bool ForceBraces { get; set; }

    public bool IsConcise { get; set; }

    public override bool IsExpression => this.m_list.Count == 1 && this.m_list[0].IsExpression;

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes<AstNode>((IList<AstNode>) this.m_list);

    public Block(Context context)
      : base(context)
    {
      this.m_list = new List<AstNode>();
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public void Clear()
    {
      foreach (AstNode astNode in this.m_list)
        astNode.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
      this.m_list.Clear();
      this.IsConcise = false;
    }

    internal override bool EncloseBlock(EncloseBlockType type) => this.m_list.Count == 1 && this.m_list[0].EncloseBlock(type);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      for (int index = this.m_list.Count - 1; index >= 0; --index)
      {
        if (this.m_list[index] == oldNode)
        {
          this.m_list[index].IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
          if (newNode == null)
          {
            this.IsConcise = false;
            this.m_list.RemoveAt(index);
            this.IsConcise = false;
          }
          else if (newNode is Block block)
          {
            this.m_list.RemoveAt(index);
            this.InsertRange(index, (IEnumerable<AstNode>) block.m_list);
          }
          else
          {
            this.m_list[index] = newNode;
            newNode.Parent = (AstNode) this;
            if (this.IsConcise && !newNode.IsExpression)
              this.IsConcise = false;
          }
          return true;
        }
      }
      return false;
    }

    public void Append(AstNode item)
    {
      if (item == null)
        return;
      if (this.IsConcise)
        this.Unconcise();
      item.Parent = (AstNode) this;
      this.m_list.Add(item);
      this.Context.UpdateWith(item.Context);
    }

    public int IndexOf(AstNode item) => this.m_list.IndexOf(item);

    public void InsertAfter(AstNode after, AstNode item)
    {
      if (item == null)
        return;
      int num = this.m_list.IndexOf(after);
      if (num < 0)
        return;
      if (this.IsConcise)
        this.Unconcise();
      if (item is Block block)
      {
        this.InsertRange(num + 1, block.Children);
      }
      else
      {
        item.Parent = (AstNode) this;
        this.m_list.Insert(num + 1, item);
      }
    }

    public void Insert(int index, AstNode item)
    {
      if (item == null)
        return;
      if (this.IsConcise)
        this.Unconcise();
      if (item is Block block)
      {
        this.InsertRange(index, block.Children);
      }
      else
      {
        item.Parent = (AstNode) this;
        this.m_list.Insert(index, item);
      }
    }

    public void RemoveLast()
    {
      this.IsConcise = false;
      this.RemoveAt(this.m_list.Count - 1);
    }

    public void RemoveAt(int index)
    {
      if (0 > index || index >= this.m_list.Count)
        return;
      this.IsConcise = false;
      this.m_list[index].IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
      this.m_list.RemoveAt(index);
    }

    public void InsertRange(int index, IEnumerable<AstNode> newItems)
    {
      if (newItems == null)
        return;
      if (this.IsConcise)
        this.Unconcise();
      this.m_list.InsertRange(index, newItems);
      foreach (AstNode newItem in newItems)
        newItem.Parent = (AstNode) this;
    }

    private void Unconcise()
    {
      this.IsConcise = false;
      if (this.m_list.Count != 1)
        return;
      AstNode astNode = this.m_list[0];
      if (!astNode.IsExpression)
        return;
      List<AstNode> list = this.m_list;
      ReturnNode returnNode1 = new ReturnNode(astNode.Context);
      returnNode1.Operand = astNode;
      returnNode1.Parent = (AstNode) this;
      ReturnNode returnNode2 = returnNode1;
      list[0] = (AstNode) returnNode2;
    }

    public IEnumerator<AstNode> GetEnumerator() => (IEnumerator<AstNode>) this.m_list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_list.GetEnumerator();
  }
}
