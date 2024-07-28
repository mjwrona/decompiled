// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.StyleSheetNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class StyleSheetNode : AstNode
  {
    public StyleSheetNode(
      string charSet,
      ReadOnlyCollection<ImportNode> imports,
      ReadOnlyCollection<NamespaceNode> namespaces,
      ReadOnlyCollection<StyleSheetRuleNode> styleSheetRules)
    {
      this.CharSetString = charSet ?? string.Empty;
      this.Imports = imports ?? new List<ImportNode>(0).AsReadOnly();
      this.Namespaces = namespaces ?? new List<NamespaceNode>(0).AsReadOnly();
      this.StyleSheetRules = styleSheetRules ?? new List<StyleSheetRuleNode>(0).AsReadOnly();
    }

    public string CharSetString { get; private set; }

    public ReadOnlyCollection<ImportNode> Imports { get; private set; }

    public ReadOnlyCollection<NamespaceNode> Namespaces { get; private set; }

    public ReadOnlyCollection<StyleSheetRuleNode> StyleSheetRules { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitStyleSheetNode(this);
  }
}
