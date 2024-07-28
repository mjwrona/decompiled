// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ConditionalCompilationSet
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ConditionalCompilationSet : ConditionalCompilationStatement
  {
    private AstNode m_value;

    public AstNode Value
    {
      get => this.m_value;
      set
      {
        this.m_value.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_value = value;
        this.m_value.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public string VariableName { get; set; }

    public ConditionalCompilationSet(Context context)
      : base(context)
    {
    }

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Value);

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Value != oldNode)
        return false;
      this.Value = newNode;
      return true;
    }
  }
}
