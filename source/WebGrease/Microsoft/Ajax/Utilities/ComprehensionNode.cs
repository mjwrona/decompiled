// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ComprehensionNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ComprehensionNode : Microsoft.Ajax.Utilities.Expression
  {
    private AstNode m_expression;
    private AstNodeList m_clauses;

    public ComprehensionType ComprehensionType { get; set; }

    public bool MozillaOrdering { get; set; }

    public Context OpenDelimiter { get; set; }

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

    public AstNodeList Clauses
    {
      get => this.m_clauses;
      set
      {
        this.m_clauses.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_clauses = value;
        this.m_clauses.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context CloseDelimiter { get; set; }

    public BlockScope BlockScope { get; set; }

    public ComprehensionNode(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.m_clauses, this.m_expression);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Expression == oldNode)
      {
        this.Expression = newNode;
        return true;
      }
      return this.Clauses == oldNode && (newNode as AstNodeList).IfNotNull<AstNodeList, bool>((Func<AstNodeList, bool>) (list =>
      {
        this.Clauses = list;
        return true;
      }));
    }
  }
}
