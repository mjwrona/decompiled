// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.TryNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class TryNode : AstNode
  {
    private Block m_tryBlock;
    private Block m_catchBlock;
    private Block m_finallyBlock;
    private ParameterDeclaration m_catchParameter;

    public Block TryBlock
    {
      get => this.m_tryBlock;
      set
      {
        this.m_tryBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_tryBlock = value;
        this.m_tryBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Block CatchBlock
    {
      get => this.m_catchBlock;
      set
      {
        this.m_catchBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_catchBlock = value;
        this.m_catchBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Block FinallyBlock
    {
      get => this.m_finallyBlock;
      set
      {
        this.m_finallyBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_finallyBlock = value;
        this.m_finallyBlock.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public ParameterDeclaration CatchParameter
    {
      get => this.m_catchParameter;
      set
      {
        this.m_catchParameter.IfNotNull<ParameterDeclaration, AstNode>((Func<ParameterDeclaration, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_catchParameter = value;
        this.m_catchParameter.IfNotNull<ParameterDeclaration, AstNode>((Func<ParameterDeclaration, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context CatchContext { get; set; }

    public Context FinallyContext { get; set; }

    public TryNode(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.TryBlock, (AstNode) this.CatchParameter, (AstNode) this.CatchBlock, (AstNode) this.FinallyBlock);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.TryBlock == oldNode)
      {
        this.TryBlock = AstNode.ForceToBlock(newNode);
        return true;
      }
      if (this.CatchParameter == oldNode)
        return (newNode as ParameterDeclaration).IfNotNull<ParameterDeclaration, bool>((Func<ParameterDeclaration, bool>) (p =>
        {
          this.CatchParameter = p;
          return true;
        }));
      if (this.CatchBlock == oldNode)
      {
        this.CatchBlock = AstNode.ForceToBlock(newNode);
        return true;
      }
      if (this.FinallyBlock != oldNode)
        return false;
      this.FinallyBlock = AstNode.ForceToBlock(newNode);
      return true;
    }
  }
}
