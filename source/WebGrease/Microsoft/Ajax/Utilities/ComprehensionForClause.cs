// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ComprehensionForClause
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ComprehensionForClause : ComprehensionClause
  {
    private AstNode m_binding;
    private AstNode m_expression;

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

    public bool IsInOperation { get; set; }

    public Context OfContext { get; set; }

    public AstNode Expression
    {
      get => this.m_expression;
      set
      {
        this.m_expression.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_expression = value;
        this.m_expression.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public ComprehensionForClause(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Binding, this.Expression);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Binding == oldNode)
      {
        this.Binding = newNode;
        return true;
      }
      if (this.Expression != oldNode)
        return false;
      this.Expression = newNode;
      return true;
    }
  }
}
