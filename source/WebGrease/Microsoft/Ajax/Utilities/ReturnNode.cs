// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ReturnNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class ReturnNode : AstNode
  {
    private AstNode m_operand;

    public AstNode Operand
    {
      get => this.m_operand;
      set
      {
        this.m_operand.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_operand = value;
        this.m_operand.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public ReturnNode(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Operand);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Operand != oldNode)
        return false;
      this.Operand = newNode;
      return true;
    }
  }
}
