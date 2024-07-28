// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.AstNodeList
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public sealed class AstNodeList : AstNode, IEnumerable<AstNode>, IEnumerable
  {
    private List<AstNode> m_list;

    public override Context TerminatingContext
    {
      get
      {
        Context terminatingContext = base.TerminatingContext;
        if (terminatingContext != null)
          return terminatingContext;
        return this.m_list.Count <= 0 ? (Context) null : this.m_list[this.m_list.Count - 1].TerminatingContext;
      }
    }

    public AstNodeList(Context context)
      : base(context)
    {
      this.m_list = new List<AstNode>();
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override OperatorPrecedence Precedence => OperatorPrecedence.Comma;

    public int Count => this.m_list.Count;

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes<AstNode>((IList<AstNode>) this.m_list);

    public void ForEach<TItem>(Action<TItem> action) where TItem : AstNode
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      foreach (AstNode astNode in this.m_list)
      {
        if (astNode is TItem obj)
          action(obj);
      }
    }

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      for (int index = 0; index < this.m_list.Count; ++index)
      {
        if (this.m_list[index] == oldNode)
        {
          oldNode.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
          if (newNode == null)
          {
            this.m_list.RemoveAt(index);
          }
          else
          {
            this.m_list[index] = newNode;
            newNode.Parent = (AstNode) this;
          }
          return true;
        }
      }
      return false;
    }

    public override bool IsEquivalentTo(AstNode otherNode)
    {
      bool flag = false;
      if (otherNode is AstNodeList astNodeList && this.m_list.Count == astNodeList.Count)
      {
        flag = true;
        for (int index = 0; index < this.m_list.Count; ++index)
        {
          if (!this.m_list[index].IsEquivalentTo(astNodeList[index]))
          {
            flag = false;
            break;
          }
        }
      }
      return flag;
    }

    internal AstNodeList Append(AstNode node)
    {
      if (node is AstNodeList astNodeList)
      {
        for (int index = 0; index < astNodeList.Count; ++index)
          this.Append(astNodeList[index]);
      }
      else if (node != null)
      {
        node.Parent = (AstNode) this;
        this.m_list.Add(node);
        this.Context.UpdateWith(node.Context);
      }
      return this;
    }

    public AstNodeList Insert(int position, AstNode node)
    {
      if (node is AstNodeList astNodeList)
      {
        for (int index = 0; index < astNodeList.Count; ++index)
          this.Insert(position + index, astNodeList[index]);
      }
      else if (node != null)
      {
        node.Parent = (AstNode) this;
        this.m_list.Insert(position, node);
        this.Context.UpdateWith(node.Context);
      }
      return this;
    }

    internal void RemoveAt(int position)
    {
      this.m_list[position].IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
      this.m_list.RemoveAt(position);
    }

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

    public bool IsSingleConstantArgument(string argumentValue) => this.m_list.Count == 1 && this.m_list[0] is ConstantWrapper constantWrapper && string.CompareOrdinal(argumentValue, constantWrapper.Value.ToString()) == 0;

    public string SingleConstantArgument
    {
      get
      {
        string constantArgument = (string) null;
        if (this.m_list.Count == 1 && this.m_list[0] is ConstantWrapper constantWrapper)
          constantArgument = constantWrapper.ToString();
        return constantArgument;
      }
    }

    public override bool IsConstant
    {
      get
      {
        foreach (AstNode astNode in this.m_list)
        {
          if (astNode != null && !astNode.IsConstant)
            return false;
        }
        return true;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.m_list.Count > 0)
      {
        stringBuilder.Append(this.m_list[0].ToString());
        for (int index = 1; index < this.m_list.Count; ++index)
        {
          stringBuilder.Append(" , ");
          stringBuilder.Append(this.m_list[index].ToString());
        }
      }
      return stringBuilder.ToString();
    }

    public IEnumerator<AstNode> GetEnumerator() => (IEnumerator<AstNode>) this.m_list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_list.GetEnumerator();
  }
}
