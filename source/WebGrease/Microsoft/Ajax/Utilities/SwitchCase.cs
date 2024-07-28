// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.SwitchCase
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class SwitchCase : AstNode
  {
    private AstNode m_caseValue;
    private Block m_statements;

    public AstNode CaseValue
    {
      get => this.m_caseValue;
      set
      {
        this.m_caseValue.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_caseValue = value;
        this.m_caseValue.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

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

    internal bool IsDefault => this.CaseValue == null;

    public Context ColonContext { get; set; }

    public SwitchCase(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.CaseValue, (AstNode) this.Statements);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.CaseValue == oldNode)
      {
        this.CaseValue = newNode;
        return true;
      }
      if (this.Statements == oldNode)
      {
        Block block = newNode as Block;
        if (newNode == null || block != null)
        {
          this.Statements = block;
          return true;
        }
      }
      return false;
    }
  }
}
