// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Declaration
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public abstract class Declaration : AstNode, IEnumerable<VariableDeclaration>, IEnumerable
  {
    private List<VariableDeclaration> m_list;

    public JSToken StatementToken { get; set; }

    public Context KeywordContext { get; set; }

    public int Count => this.m_list.Count;

    public VariableDeclaration this[int index]
    {
      get => this.m_list[index];
      set
      {
        this.m_list[index].IfNotNull<VariableDeclaration, AstNode>((Func<VariableDeclaration, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        if (value != null)
        {
          this.m_list[index] = value;
          this.m_list[index].Parent = (AstNode) this;
        }
        else
          this.m_list.RemoveAt(index);
      }
    }

    public ActivationObject Scope { get; set; }

    public override bool IsDeclaration => true;

    protected Declaration(Context context)
      : base(context)
    {
      this.m_list = new List<VariableDeclaration>();
    }

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes<VariableDeclaration>((IList<VariableDeclaration>) this.m_list);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      for (int index = 0; index < this.m_list.Count; ++index)
      {
        if (this.m_list[index] == oldNode)
        {
          if (newNode == null)
          {
            this.m_list[index].IfNotNull<VariableDeclaration, AstNode>((Func<VariableDeclaration, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
            this.m_list.RemoveAt(index);
            break;
          }
          VariableDeclaration variableDeclaration = newNode as VariableDeclaration;
          if (newNode == null || variableDeclaration != null)
          {
            this.m_list[index].IfNotNull<VariableDeclaration, AstNode>((Func<VariableDeclaration, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
            this.m_list[index] = variableDeclaration;
            variableDeclaration.Parent = (AstNode) this;
            return true;
          }
          break;
        }
      }
      return false;
    }

    public void Append(AstNode element)
    {
      switch (element)
      {
        case VariableDeclaration variableDeclaration:
          if (!this.HandleDuplicates(variableDeclaration.Binding) && variableDeclaration.Initializer == null)
            break;
          variableDeclaration.Parent = (AstNode) this;
          this.m_list.Add(variableDeclaration);
          this.UpdateWith(variableDeclaration.Context);
          break;
        case Declaration declaration:
          for (int index = 0; index < declaration.m_list.Count; ++index)
            this.Append((AstNode) declaration.m_list[index]);
          break;
      }
    }

    public void InsertAt(int index, AstNode element)
    {
      switch (element)
      {
        case VariableDeclaration variableDeclaration:
          if (!this.HandleDuplicates(variableDeclaration.Binding) && variableDeclaration.Initializer == null)
            break;
          variableDeclaration.Parent = (AstNode) this;
          this.m_list.Insert(index, variableDeclaration);
          break;
        case Declaration declaration:
          for (int index1 = declaration.m_list.Count - 1; index1 >= 0; --index1)
            this.InsertAt(index, (AstNode) declaration.m_list[index1]);
          break;
      }
    }

    private bool HandleDuplicates(AstNode binding)
    {
      bool flag = true;
      string str = (binding as BindingIdentifier).IfNotNull<BindingIdentifier, string>((Func<BindingIdentifier, string>) (b => b.Name));
      if (!str.IsNullOrWhiteSpace())
      {
        for (int index = this.m_list.Count - 1; index >= 0; --index)
        {
          VariableDeclaration variableDeclaration = this.m_list[index];
          if (variableDeclaration.Binding is BindingIdentifier binding1 && string.CompareOrdinal(binding1.Name, str) == 0)
          {
            if (variableDeclaration.Initializer == null)
            {
              variableDeclaration.Parent = (AstNode) null;
              this.m_list.RemoveAt(index);
            }
            else
              flag = false;
          }
        }
      }
      return flag;
    }

    public void RemoveAt(int index)
    {
      if (!(0 <= index & index < this.m_list.Count))
        return;
      this.m_list[index].IfNotNull<VariableDeclaration, AstNode>((Func<VariableDeclaration, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
      this.m_list.RemoveAt(index);
    }

    public void Remove(VariableDeclaration variableDeclaration)
    {
      if (variableDeclaration == null || !this.m_list.Remove(variableDeclaration) || variableDeclaration.Parent != this)
        return;
      variableDeclaration.Parent = (AstNode) null;
    }

    public bool Contains(string name)
    {
      if (!name.IsNullOrWhiteSpace())
      {
        foreach (AstNode node in this.m_list)
        {
          foreach (BindingIdentifier binding in (IEnumerable<BindingIdentifier>) BindingsVisitor.Bindings(node))
          {
            if (string.CompareOrdinal(name, binding.Name) == 0)
              return true;
          }
        }
      }
      return false;
    }

    public override bool ContainsInOperator
    {
      get
      {
        foreach (VariableDeclaration variableDeclaration in this.m_list)
        {
          if (variableDeclaration.Initializer != null && variableDeclaration.Initializer.ContainsInOperator)
            return true;
        }
        return false;
      }
    }

    public IEnumerator<VariableDeclaration> GetEnumerator() => (IEnumerator<VariableDeclaration>) this.m_list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_list.GetEnumerator();
  }
}
