// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ObjectLiteralProperty
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ObjectLiteralProperty : AstNode
  {
    private ObjectLiteralField m_propertyName;
    private AstNode m_propertyValue;

    public ObjectLiteralField Name
    {
      get => this.m_propertyName;
      set
      {
        this.m_propertyName.IfNotNull<ObjectLiteralField, AstNode>((Func<ObjectLiteralField, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_propertyName = value;
        this.m_propertyName.IfNotNull<ObjectLiteralField, AstNode>((Func<ObjectLiteralField, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public AstNode Value
    {
      get => this.m_propertyValue;
      set
      {
        this.m_propertyValue.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_propertyValue = value;
        this.m_propertyValue.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public override bool IsConstant => this.Value == null || this.Value.IsConstant;

    public ObjectLiteralProperty(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.Name, this.Value);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Name == oldNode)
      {
        ObjectLiteralField objectLiteralField = newNode as ObjectLiteralField;
        if (newNode == null || objectLiteralField != null)
          this.Name = objectLiteralField;
        return true;
      }
      if (this.Value != oldNode)
        return false;
      this.Value = newNode;
      return true;
    }

    internal override string GetFunctionGuess(AstNode target) => this.Name.IfNotNull<ObjectLiteralField, string>((Func<ObjectLiteralField, string>) (n => n.ToString()));
  }
}
