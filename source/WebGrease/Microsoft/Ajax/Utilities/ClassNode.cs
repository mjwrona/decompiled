// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ClassNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ClassNode : AstNode
  {
    private AstNode m_binding;
    private AstNode m_heritage;
    private AstNodeList m_elements;

    public Context ClassContext { get; set; }

    public AstNode Binding
    {
      get => this.m_binding;
      set
      {
        this.m_binding.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_binding = value;
        this.m_binding.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context ExtendsContext { get; set; }

    public AstNode Heritage
    {
      get => this.m_heritage;
      set
      {
        this.m_heritage.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_heritage = value;
        this.m_heritage.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context OpenBrace { get; set; }

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

    public Context CloseBrace { get; set; }

    public ClassType ClassType { get; set; }

    public override bool IsExpression => this.ClassType != ClassType.Declaration;

    public BlockScope Scope { get; set; }

    public override bool IsDeclaration => this.ClassType == ClassType.Declaration;

    public ClassNode(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.m_binding, this.m_heritage, (AstNode) this.m_elements);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Binding == oldNode)
      {
        this.Binding = (AstNode) (newNode as BindingIdentifier);
        return true;
      }
      if (this.Heritage == oldNode)
      {
        this.Heritage = newNode;
        return true;
      }
      if (this.Elements != oldNode)
        return false;
      this.Elements = newNode as AstNodeList;
      return true;
    }
  }
}
