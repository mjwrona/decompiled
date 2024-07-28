// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Member
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class Member : Expression
  {
    private AstNode m_root;

    public AstNode Root
    {
      get => this.m_root;
      set
      {
        this.m_root.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_root = value;
        this.m_root.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public string Name { get; set; }

    public Context NameContext { get; set; }

    public Member(Context context)
      : base(context)
    {
    }

    public override OperatorPrecedence Precedence => OperatorPrecedence.FieldAccess;

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override bool IsEquivalentTo(AstNode otherNode) => otherNode is Member member && string.CompareOrdinal(this.Name, member.Name) == 0 && this.Root.IsEquivalentTo(member.Root);

    internal override string GetFunctionGuess(AstNode target) => this.Root.GetFunctionGuess((AstNode) this) + (object) '.' + this.Name;

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Root);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Root != oldNode)
        return false;
      this.Root = newNode;
      return true;
    }

    public override AstNode LeftHandSide => this.Root.LeftHandSide;
  }
}
