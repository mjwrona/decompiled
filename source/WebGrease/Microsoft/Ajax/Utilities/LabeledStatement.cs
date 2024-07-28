// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.LabeledStatement
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class LabeledStatement : AstNode
  {
    private AstNode m_statement;

    public AstNode Statement
    {
      get => this.m_statement;
      set
      {
        this.m_statement.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_statement = value;
        this.m_statement.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public string Label { get; set; }

    public Context LabelContext { get; set; }

    public LabelInfo LabelInfo { get; set; }

    public Context ColonContext { get; set; }

    public LabeledStatement(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override AstNode LeftHandSide => this.Statement == null ? (AstNode) null : this.Statement.LeftHandSide;

    internal override bool EncloseBlock(EncloseBlockType type) => this.Statement != null && this.Statement.EncloseBlock(type);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Statement);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Statement != oldNode)
        return false;
      this.Statement = newNode;
      return true;
    }
  }
}
