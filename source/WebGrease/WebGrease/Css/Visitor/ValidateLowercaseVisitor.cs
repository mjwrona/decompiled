// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.ValidateLowercaseVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Globalization;
using WebGrease.Activities;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Ast.Selectors;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.Visitor
{
  public sealed class ValidateLowercaseVisitor : NodeVisitor
  {
    public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
    {
      if (styleSheet == null)
        throw new ArgumentNullException(nameof (styleSheet));
      ValidateLowercaseVisitor.ValidateForLowerCase(styleSheet.CharSetString);
      styleSheet.Imports.ForEach<ImportNode>((Action<ImportNode>) (importNode => ValidateLowercaseVisitor.ValidateForLowerCase(importNode.MinifyPrint())));
      styleSheet.StyleSheetRules.ForEach<StyleSheetRuleNode>((Action<StyleSheetRuleNode>) (styleSheetRule => styleSheetRule.Accept((NodeVisitor) this)));
      return (AstNode) styleSheet;
    }

    public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
    {
      if (rulesetNode == null)
        throw new ArgumentNullException(nameof (rulesetNode));
      try
      {
        rulesetNode.SelectorsGroupNode.SelectorNodes.ForEach<SelectorNode>((Action<SelectorNode>) (selectorNode => ValidateLowercaseVisitor.ValidateForLowerCase(selectorNode.MinifyPrint())));
        rulesetNode.Declarations.ForEach<DeclarationNode>((Action<DeclarationNode>) (declarationNode => declarationNode.Accept((NodeVisitor) this)));
      }
      catch (BuildWorkflowException ex)
      {
        throw new WorkflowException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.CssLowercaseValidationParentNodeError, new object[1]
        {
          (object) rulesetNode.PrettyPrint()
        }), (Exception) ex);
      }
      return (AstNode) rulesetNode;
    }

    public override AstNode VisitMediaNode(MediaNode mediaNode)
    {
      if (mediaNode == null)
        throw new ArgumentNullException(nameof (mediaNode));
      try
      {
        mediaNode.MediaQueries.ForEach<MediaQueryNode>((Action<MediaQueryNode>) (mediaQuery => ValidateLowercaseVisitor.ValidateForLowerCase(mediaQuery.MinifyPrint())));
        mediaNode.Rulesets.ForEach<RulesetNode>((Action<RulesetNode>) (rulesetNode => rulesetNode.Accept((NodeVisitor) this)));
      }
      catch (BuildWorkflowException ex)
      {
        throw new WorkflowException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.CssLowercaseValidationParentNodeError, new object[1]
        {
          (object) mediaNode.PrettyPrint()
        }), (Exception) ex);
      }
      return (AstNode) mediaNode;
    }

    public override AstNode VisitPageNode(PageNode pageNode)
    {
      if (pageNode == null)
        throw new ArgumentNullException(nameof (pageNode));
      try
      {
        ValidateLowercaseVisitor.ValidateForLowerCase(pageNode.PseudoPage);
        pageNode.Declarations.ForEach<DeclarationNode>((Action<DeclarationNode>) (declarationNode => declarationNode.Accept((NodeVisitor) this)));
      }
      catch (BuildWorkflowException ex)
      {
        throw new WorkflowException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.CssLowercaseValidationParentNodeError, new object[1]
        {
          (object) pageNode.PrettyPrint()
        }), (Exception) ex);
      }
      return (AstNode) pageNode;
    }

    public override AstNode VisitDeclarationNode(DeclarationNode declarationNode)
    {
      if (declarationNode == null)
        throw new ArgumentNullException(nameof (declarationNode));
      ValidateLowercaseVisitor.ValidateForLowerCase(declarationNode.Property);
      return (AstNode) declarationNode;
    }

    private static void ValidateForLowerCase(string textToValidate)
    {
      textToValidate = ResourcesResolver.LocalizationResourceKeyRegex.Replace(textToValidate, string.Empty);
      if (!string.IsNullOrWhiteSpace(textToValidate) && string.CompareOrdinal(textToValidate, textToValidate.ToLower(CultureInfo.InvariantCulture)) != 0)
        throw new BuildWorkflowException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, CssStrings.CssLowercaseValidationError, new object[1]
        {
          (object) textToValidate
        }));
    }
  }
}
