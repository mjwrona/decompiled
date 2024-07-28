// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ImportExportStatement
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public abstract class ImportExportStatement : 
    AstNode,
    IEnumerable<AstNode>,
    IEnumerable,
    IModuleReference
  {
    private List<AstNode> m_list;

    public Context KeywordContext { get; set; }

    public Context OpenContext { get; set; }

    public Context CloseContext { get; set; }

    public Context FromContext { get; set; }

    public Context ModuleContext { get; set; }

    public string ModuleName { get; set; }

    public ModuleScope ReferencedModule { get; set; }

    public override bool IsDeclaration => true;

    protected ImportExportStatement(Context context)
      : base(context)
    {
      this.m_list = new List<AstNode>();
    }

    public int Count => this.m_list.Count;

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes<AstNode>((IList<AstNode>) this.m_list);

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

    public ImportExportStatement Append(AstNode node)
    {
      if (node is ImportExportStatement importExportStatement)
      {
        for (int index = 0; index < importExportStatement.Count; ++index)
          this.Append(importExportStatement[index]);
      }
      else if (node != null)
      {
        node.Parent = (AstNode) this;
        this.m_list.Add(node);
        this.Context.UpdateWith(node.Context);
      }
      return this;
    }

    public ImportExportStatement Insert(int position, AstNode node)
    {
      if (node is ImportExportStatement importExportStatement)
      {
        for (int index = 0; index < importExportStatement.Count; ++index)
          this.Insert(position + index, importExportStatement[index]);
      }
      else if (node != null)
      {
        node.Parent = (AstNode) this;
        this.m_list.Insert(position, node);
        this.Context.UpdateWith(node.Context);
      }
      return this;
    }

    public void RemoveAt(int position)
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
