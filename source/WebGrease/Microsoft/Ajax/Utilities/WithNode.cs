// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.WithNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class WithNode : AstNode
  {
    private AstNode m_withObject;
    private Block m_body;

    public AstNode WithObject
    {
      get => this.m_withObject;
      set
      {
        this.m_withObject.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_withObject = value;
        this.m_withObject.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Block Body
    {
      get => this.m_body;
      set
      {
        this.m_body.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_body = value;
        this.m_body.IfNotNull<Block, AstNode>((Func<Block, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public override Context TerminatingContext => base.TerminatingContext ?? this.Body.IfNotNull<Block, Context>((Func<Block, Context>) (b => b.TerminatingContext));

    public WithNode(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.WithObject, (AstNode) this.Body);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.WithObject == oldNode)
      {
        this.WithObject = newNode;
        return true;
      }
      if (this.Body != oldNode)
        return false;
      this.Body = AstNode.ForceToBlock(newNode);
      return true;
    }

    internal override bool EncloseBlock(EncloseBlockType type) => this.Body != null && this.Body.EncloseBlock(type);
  }
}
