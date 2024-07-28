// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.IterationStatement
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace Microsoft.Ajax.Utilities
{
  public abstract class IterationStatement : AstNode
  {
    private Block m_body;

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

    protected IterationStatement(Context context)
      : base(context)
    {
    }
  }
}
