// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ObjectLiteral
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class ObjectLiteral : Expression
  {
    private AstNodeList m_properties;

    public AstNodeList Properties
    {
      get => this.m_properties;
      set
      {
        this.m_properties.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_properties = value;
        this.m_properties.IfNotNull<AstNodeList, AstNode>((Func<AstNodeList, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public override bool IsConstant
    {
      get
      {
        if (this.Properties != null)
        {
          foreach (AstNode property in this.Properties)
          {
            if (!property.IsConstant)
              return false;
          }
        }
        return true;
      }
    }

    public ObjectLiteral(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.m_properties);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (oldNode == this.m_properties)
      {
        AstNodeList astNodeList = newNode as AstNodeList;
        if (newNode == null || astNodeList != null)
          this.Properties = astNodeList;
      }
      return false;
    }
  }
}
