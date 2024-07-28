// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ConditionalCompilationComment
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ConditionalCompilationComment : AstNode
  {
    private Block m_statements;

    public Block Statements
    {
      get => this.m_statements;
      set
      {
        this.m_statements.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_statements = value;
        this.m_statements.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public ConditionalCompilationComment(Context context)
      : base(context)
    {
      this.Statements = new Block(context.FlattenToStart());
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public void Append(AstNode statement)
    {
      if (statement == null)
        return;
      this.Context.UpdateWith(statement.Context);
      this.Statements.Append(statement);
    }

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.Statements);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Statements != oldNode)
        return false;
      this.Statements = AstNode.ForceToBlock(newNode);
      return true;
    }
  }
}
