// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.InitializerNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class InitializerNode : AstNode
  {
    private AstNode m_binding;
    private AstNode m_initializer;

    public AstNode Binding
    {
      get => this.m_binding;
      set
      {
        this.m_binding.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_binding = value;
        this.m_binding.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context AssignContext { get; set; }

    public AstNode Initializer
    {
      get => this.m_initializer;
      set
      {
        this.m_initializer.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_initializer = value;
        this.m_initializer.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public override bool IsConstant => this.Binding.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (v => v.IsConstant), true);

    public InitializerNode(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Binding, this.Initializer);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Binding == oldNode)
      {
        this.Binding = newNode;
        return true;
      }
      if (this.Initializer != oldNode)
        return false;
      this.Initializer = newNode;
      return true;
    }

    internal override string GetFunctionGuess(AstNode target) => this.Binding.IfNotNull<AstNode, string>((Func<AstNode, string>) (b => b.GetFunctionGuess(target)));

    public override bool IsEquivalentTo(AstNode otherNode)
    {
      bool flag1 = true;
      IList<BindingIdentifier> bindingIdentifierList = BindingsVisitor.Bindings(this.Binding);
      foreach (Lookup reference in (IEnumerable<Lookup>) BindingsVisitor.References(otherNode))
      {
        bool flag2 = false;
        foreach (AstNode astNode in (IEnumerable<BindingIdentifier>) bindingIdentifierList)
        {
          if (astNode.IsEquivalentTo((AstNode) reference))
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2)
        {
          flag1 = false;
          break;
        }
      }
      return flag1;
    }
  }
}
