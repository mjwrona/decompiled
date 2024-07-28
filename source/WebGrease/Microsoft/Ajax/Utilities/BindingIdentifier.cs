// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.BindingIdentifier
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace Microsoft.Ajax.Utilities
{
  public class BindingIdentifier : AstNode, INameDeclaration, IRenameable
  {
    public string Name { get; set; }

    public JSVariableField VariableField { get; set; }

    public bool RenameNotAllowed { get; set; }

    public ScopeType ScopeType { get; set; }

    public AstNode Initializer => (this.Parent as InitializerNode).IfNotNull<InitializerNode, AstNode>((Func<InitializerNode, AstNode>) (v => v.Initializer));

    public bool IsParameter { get; set; }

    public string OriginalName => this.Name;

    public bool WasRenamed => this.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (f => !f.CrunchedName.IsNullOrWhiteSpace()));

    public BindingIdentifier(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override bool IsEquivalentTo(AstNode otherNode)
    {
      switch (otherNode)
      {
        case BindingIdentifier bindingIdentifier:
          return bindingIdentifier.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v == this.VariableField));
        case Lookup lookup:
          return lookup.VariableField.IfNotNull<JSVariableField, bool>((Func<JSVariableField, bool>) (v => v == this.VariableField));
        default:
          return false;
      }
    }

    public override string ToString() => this.Name;
  }
}
