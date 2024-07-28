// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CallNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class CallNode : Expression
  {
    private AstNode m_function;
    private AstNodeList m_arguments;

    public AstNode Function
    {
      get => this.m_function;
      set
      {
        this.m_function.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_function = value;
        this.m_function.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public AstNodeList Arguments
    {
      get => this.m_arguments;
      set
      {
        this.m_arguments.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_arguments = value;
        this.m_arguments.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public bool IsConstructor { get; set; }

    public bool InBrackets { get; set; }

    public CallNode(Context context)
      : base(context)
    {
    }

    public override OperatorPrecedence Precedence => OperatorPrecedence.FieldAccess;

    public override bool IsExpression => !(this.Function is Member function) || !function.Name.StartsWith("on", StringComparison.Ordinal) || this.Arguments.Count <= 0;

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Function, (AstNode) this.Arguments);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Function == oldNode)
      {
        this.Function = newNode;
        return true;
      }
      if (this.Arguments == oldNode)
      {
        if (newNode == null)
        {
          this.Arguments = (AstNodeList) null;
          return true;
        }
        if (newNode is AstNodeList astNodeList)
        {
          this.Arguments = astNodeList;
          return true;
        }
      }
      return false;
    }

    public override AstNode LeftHandSide => this.Function.LeftHandSide;

    public override bool IsEquivalentTo(AstNode otherNode) => otherNode is CallNode callNode && this.InBrackets == callNode.InBrackets && this.IsConstructor == callNode.IsConstructor && this.Function.IsEquivalentTo(callNode.Function) && this.Arguments.IsEquivalentTo((AstNode) callNode.Arguments);
  }
}
