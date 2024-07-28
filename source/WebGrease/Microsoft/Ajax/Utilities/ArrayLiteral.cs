// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ArrayLiteral
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class ArrayLiteral : Expression
  {
    private AstNodeList m_elements;

    public AstNodeList Elements
    {
      get => this.m_elements;
      set
      {
        this.m_elements.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_elements = value;
        this.m_elements.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public bool MayHaveIssues { get; set; }

    public int Length
    {
      get
      {
        int length = 0;
        foreach (AstNode element in this.m_elements)
        {
          if (!element.IsConstant)
            return -1;
          if (element is UnaryOperator unaryOperator && unaryOperator.OperatorToken == JSToken.RestSpread)
          {
            int num = (unaryOperator.Operand as ArrayLiteral).IfNotNull<ArrayLiteral, int>((Func<ArrayLiteral, int>) (a => a.Length), -1);
            if (num < 0)
              return -1;
            length += num;
          }
          else
            ++length;
        }
        return length;
      }
    }

    public ArrayLiteral(Context context)
      : base(context)
    {
    }

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.Elements);

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (oldNode == this.Elements)
      {
        if (newNode == null)
        {
          this.Elements = (AstNodeList) null;
          return true;
        }
        if (newNode is AstNodeList astNodeList)
        {
          this.Elements = astNodeList;
          return true;
        }
      }
      return false;
    }

    public override bool IsConstant => this.Elements == null || this.Elements.IsConstant;
  }
}
