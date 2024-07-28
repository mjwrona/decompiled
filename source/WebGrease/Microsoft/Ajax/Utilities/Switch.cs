// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Switch
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class Switch : AstNode
  {
    private AstNode m_expression;
    private AstNodeList m_cases;

    public AstNode Expression
    {
      get => this.m_expression;
      set
      {
        this.m_expression.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_expression = value;
        this.m_expression.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public AstNodeList Cases
    {
      get => this.m_cases;
      set
      {
        this.m_cases.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_cases = value;
        this.m_cases.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public bool BraceOnNewLine { get; set; }

    public Context BraceContext { get; set; }

    public ActivationObject BlockScope { get; set; }

    public Switch(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Expression, (AstNode) this.Cases);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Expression == oldNode)
      {
        this.Expression = newNode;
        return true;
      }
      if (this.Cases == oldNode)
      {
        AstNodeList astNodeList = newNode as AstNodeList;
        if (newNode == null || astNodeList != null)
        {
          this.Cases = astNodeList;
          return true;
        }
      }
      return false;
    }
  }
}
