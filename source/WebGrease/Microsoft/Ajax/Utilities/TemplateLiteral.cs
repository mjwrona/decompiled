// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.TemplateLiteral
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class TemplateLiteral : Expression
  {
    private Lookup m_function;
    private AstNodeList m_expressions;

    public Lookup Function
    {
      get => this.m_function;
      set
      {
        this.m_function.IfNotNull<Lookup, AstNode>((Func<Lookup, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_function = value;
        this.m_function.IfNotNull<Lookup, AstNode>((Func<Lookup, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public string Text { get; set; }

    public Context TextContext { get; set; }

    public AstNodeList Expressions
    {
      get => this.m_expressions;
      set
      {
        this.m_expressions.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_expressions = value;
        this.m_expressions.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public TemplateLiteral(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.m_function, (AstNode) this.m_expressions);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Function == oldNode)
        return (newNode as Lookup).IfNotNull<Lookup, bool>((Func<Lookup, bool>) (lookup =>
        {
          this.Function = lookup;
          return true;
        }));
      return this.Expressions == oldNode && (newNode as AstNodeList).IfNotNull<AstNodeList, bool>((Func<AstNodeList, bool>) (list =>
      {
        this.Expressions = list;
        return true;
      }));
    }
  }
}
