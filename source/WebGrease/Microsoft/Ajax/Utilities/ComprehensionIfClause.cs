// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ComprehensionIfClause
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ComprehensionIfClause : ComprehensionClause
  {
    private AstNode m_condition;

    public AstNode Condition
    {
      get => this.m_condition;
      set
      {
        this.m_condition.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_condition = value;
        this.m_condition.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public ComprehensionIfClause(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Condition);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Condition != oldNode)
        return false;
      this.Condition = newNode;
      return true;
    }
  }
}
