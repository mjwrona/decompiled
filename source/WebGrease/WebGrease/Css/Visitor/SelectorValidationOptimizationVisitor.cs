// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.SelectorValidationOptimizationVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Ast.Selectors;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.Visitor
{
  public sealed class SelectorValidationOptimizationVisitor : NodeVisitor
  {
    private readonly HashSet<string> selectorsToValidateOrRemove;
    private readonly bool shouldMatchExactly;
    private readonly bool validate;

    public SelectorValidationOptimizationVisitor(
      HashSet<string> selectorsToValidateOrRemove,
      bool shouldMatchExactly,
      bool validate)
    {
      this.validate = validate;
      this.shouldMatchExactly = shouldMatchExactly;
      this.selectorsToValidateOrRemove = selectorsToValidateOrRemove ?? new HashSet<string>();
    }

    public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
    {
      if (styleSheet == null)
        throw new ArgumentNullException(nameof (styleSheet));
      List<StyleSheetRuleNode> updatedStyleSheetRules = new List<StyleSheetRuleNode>();
      styleSheet.StyleSheetRules.ForEach<StyleSheetRuleNode>((Action<StyleSheetRuleNode>) (ruleSetMediaPageNode =>
      {
        StyleSheetRuleNode styleSheetRuleNode = (StyleSheetRuleNode) ruleSetMediaPageNode.Accept((NodeVisitor) this);
        if (styleSheetRuleNode == null)
          return;
        updatedStyleSheetRules.Add(styleSheetRuleNode);
      }));
      return (AstNode) new StyleSheetNode(styleSheet.CharSetString, styleSheet.Imports, styleSheet.Namespaces, updatedStyleSheetRules.AsReadOnly());
    }

    public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
    {
      string str1 = rulesetNode.PrintSelector();
      string str2 = string.Empty;
      bool flag = false;
      if (this.shouldMatchExactly)
      {
        flag = this.selectorsToValidateOrRemove.Contains(str1);
        str2 = str1;
      }
      else
      {
        foreach (string str3 in this.selectorsToValidateOrRemove)
        {
          if (str1.Contains(str3))
          {
            flag = true;
            str2 = str3;
            break;
          }
        }
      }
      if (!flag)
        return (AstNode) rulesetNode;
      if (this.validate)
        throw new BuildWorkflowException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.CssSelectorHackError, new object[1]
        {
          (object) str2
        }));
      if (rulesetNode.SelectorsGroupNode.SelectorNodes.Count > 1)
      {
        List<SelectorNode> list = rulesetNode.SelectorsGroupNode.SelectorNodes.Where<SelectorNode>((Func<SelectorNode, bool>) (sn => !this.selectorsToValidateOrRemove.Any<string>((Func<string, bool>) (sr => sn.MinifyPrint().Contains(sr))))).ToList<SelectorNode>();
        if (list.Any<SelectorNode>())
          return (AstNode) new RulesetNode(new SelectorsGroupNode(new ReadOnlyCollection<SelectorNode>((IList<SelectorNode>) list)), rulesetNode.Declarations, rulesetNode.ImportantComments);
      }
      return (AstNode) null;
    }

    public override AstNode VisitMediaNode(MediaNode mediaNode)
    {
      if (mediaNode == null)
        throw new ArgumentNullException(nameof (mediaNode));
      List<RulesetNode> updatedRulesetNodes = new List<RulesetNode>();
      List<PageNode> updatePageNodes = new List<PageNode>();
      mediaNode.Rulesets.ForEach<RulesetNode>((Action<RulesetNode>) (rulesetNode =>
      {
        RulesetNode rulesetNode1 = (RulesetNode) rulesetNode.Accept((NodeVisitor) this);
        if (rulesetNode1 == null)
          return;
        updatedRulesetNodes.Add(rulesetNode1);
      }));
      mediaNode.PageNodes.ForEach<PageNode>((Action<PageNode>) (page =>
      {
        PageNode pageNode = (PageNode) page.Accept((NodeVisitor) this);
        if (pageNode == null)
          return;
        updatePageNodes.Add(pageNode);
      }));
      return (AstNode) new MediaNode(mediaNode.MediaQueries, updatedRulesetNodes.AsReadOnly(), updatePageNodes.AsReadOnly());
    }
  }
}
