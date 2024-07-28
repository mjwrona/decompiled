// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.IfNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class IfNode : AstNode
  {
    private AstNode m_condition;
    private Block m_trueBlock;
    private Block m_falseBlock;

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

    public Block TrueBlock
    {
      get => this.m_trueBlock;
      set
      {
        this.m_trueBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_trueBlock = value;
        this.m_trueBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Block FalseBlock
    {
      get => this.m_falseBlock;
      set
      {
        this.m_falseBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_falseBlock = value;
        this.m_falseBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context ElseContext { get; set; }

    public override Context TerminatingContext => base.TerminatingContext ?? (this.FalseBlock == null ? this.TrueBlock.IfNotNull<Block, Context>((Func<Block, Context>) (b => b.TerminatingContext)) : this.FalseBlock.TerminatingContext);

    public IfNode(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public void SwapBranches()
    {
      Block trueBlock = this.m_trueBlock;
      this.m_trueBlock = this.m_falseBlock;
      this.m_falseBlock = trueBlock;
    }

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Condition, (AstNode) this.TrueBlock, (AstNode) this.FalseBlock);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Condition == oldNode)
      {
        this.Condition = newNode;
        return true;
      }
      if (this.TrueBlock == oldNode)
      {
        this.TrueBlock = AstNode.ForceToBlock(newNode);
        return true;
      }
      if (this.FalseBlock != oldNode)
        return false;
      this.FalseBlock = AstNode.ForceToBlock(newNode);
      return true;
    }

    internal override bool EncloseBlock(EncloseBlockType type)
    {
      if (this.FalseBlock != null && (this.FalseBlock.ForceBraces || this.FalseBlock.Count > 0))
        return this.FalseBlock.EncloseBlock(type);
      if (type == EncloseBlockType.IfWithoutElse)
        return true;
      return this.TrueBlock != null && this.TrueBlock.EncloseBlock(type);
    }
  }
}
