// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.DocumentQueryNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class DocumentQueryNode : StyleSheetRuleNode
  {
    public DocumentQueryNode(
      string matchFunctionName,
      string documentSymbol,
      ReadOnlyCollection<RulesetNode> rulesets)
    {
      this.Rulesets = rulesets;
      this.MatchFunctionName = matchFunctionName;
      this.DocumentSymbol = documentSymbol;
    }

    public string MatchFunctionName { get; private set; }

    public string DocumentSymbol { get; private set; }

    public ReadOnlyCollection<RulesetNode> Rulesets { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitDocumentQueryNode(this);
  }
}
