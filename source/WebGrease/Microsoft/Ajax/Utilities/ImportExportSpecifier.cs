// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ImportExportSpecifier
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ImportExportSpecifier : AstNode, INameDeclaration
  {
    private AstNode m_localIdentifier;

    public Context NameContext { get; set; }

    public string ExternalName { get; set; }

    public Context AsContext { get; set; }

    public AstNode LocalIdentifier
    {
      get => this.m_localIdentifier;
      set
      {
        this.m_localIdentifier.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_localIdentifier = value;
        this.m_localIdentifier.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public string Name => this.ExternalName;

    public AstNode Initializer => (AstNode) null;

    public bool IsParameter => false;

    public bool RenameNotAllowed => true;

    public JSVariableField VariableField { get; set; }

    public ImportExportSpecifier(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.m_localIdentifier);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.LocalIdentifier != oldNode)
        return false;
      this.LocalIdentifier = newNode;
      return true;
    }
  }
}
