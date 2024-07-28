// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ModuleDeclaration
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ModuleDeclaration : AstNode, IModuleReference
  {
    private BindingIdentifier m_binding;
    private Block m_body;

    public BindingIdentifier Binding
    {
      get => this.m_binding;
      set
      {
        this.m_binding.IfNotNull<BindingIdentifier, AstNode>((Func<BindingIdentifier, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_binding = value;
        this.m_binding.IfNotNull<BindingIdentifier, AstNode>((Func<BindingIdentifier, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context FromContext { get; set; }

    public string ModuleName { get; set; }

    public Context ModuleContext { get; set; }

    public ModuleScope ReferencedModule { get; set; }

    public bool IsImplicit { get; set; }

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

    public override bool IsDeclaration => true;

    public ModuleDeclaration(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes((AstNode) this.m_binding, (AstNode) this.m_body);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Binding == oldNode)
        return (newNode as BindingIdentifier).IfNotNull<BindingIdentifier, bool>((Func<BindingIdentifier, bool>) (b =>
        {
          this.Binding = b;
          return true;
        }));
      if (this.Body != oldNode)
        return false;
      this.Body = AstNode.ForceToBlock(newNode);
      return true;
    }
  }
}
