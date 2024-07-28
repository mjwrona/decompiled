// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ForNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class ForNode : IterationStatement
  {
    private AstNode m_initializer;
    private AstNode m_condition;
    private AstNode m_incrementer;

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

    public AstNode Incrementer
    {
      get => this.m_incrementer;
      set
      {
        this.m_incrementer.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_incrementer = value;
        this.m_incrementer.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context Separator1Context { get; set; }

    public Context Separator2Context { get; set; }

    public BlockScope BlockScope { get; set; }

    public override Context TerminatingContext => base.TerminatingContext ?? this.Body.IfNotNull<Block, Context>((Func<Block, Context>) (b => b.TerminatingContext));

    public ForNode(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    internal override bool EncloseBlock(EncloseBlockType type) => this.Body != null && this.Body.Count != 0 && this.Body.EncloseBlock(type);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Initializer, this.Condition, this.Incrementer, (AstNode) this.Body);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Initializer == oldNode)
      {
        this.Initializer = newNode;
        return true;
      }
      if (this.Condition == oldNode)
      {
        this.Condition = newNode;
        return true;
      }
      if (this.Incrementer == oldNode)
      {
        this.Incrementer = newNode;
        return true;
      }
      if (this.Body != oldNode)
        return false;
      this.Body = AstNode.ForceToBlock(newNode);
      return true;
    }
  }
}
