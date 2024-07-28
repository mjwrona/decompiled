// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Engine.RelevanceEngine
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Engine
{
  public static class RelevanceEngine
  {
    public static void ProcessRules(IExpression expression, IEnumerable<RelevanceRule> rules) => RelevanceEngine.ProcessRulesHelper((RelevanceExpression) expression, rules, true);

    public static void ProcessChildRules(IExpression expression, IEnumerable<RelevanceRule> rules) => RelevanceEngine.ProcessRulesHelper((RelevanceExpression) expression, rules, true, true);

    private static void ProcessRulesHelper(
      RelevanceExpression expression,
      IEnumerable<RelevanceRule> rules,
      bool isRootNode,
      bool isChildRule = false)
    {
      foreach (RelevanceRule rule in rules)
      {
        switch (rule.Target)
        {
          case Target.Unknown:
            throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Encountered rule with target 'UNKNOWN', Bailing out. Rule : [{0}]", (object) rule.Description)));
          case Target.AllNodes:
            RelevanceEngine.AddRelevanceRuleAsExpression(expression, rule, isChildRule);
            continue;
          case Target.AndNodes:
            if (expression is AndExpression)
            {
              RelevanceEngine.AddRelevanceRuleAsExpression(expression, rule, isChildRule);
              continue;
            }
            continue;
          case Target.OrNodes:
            if (expression is OrExpression)
            {
              RelevanceEngine.AddRelevanceRuleAsExpression(expression, rule, isChildRule);
              continue;
            }
            continue;
          case Target.NotNodes:
            if (expression is NotExpression)
            {
              RelevanceEngine.AddRelevanceRuleAsExpression(expression, rule, isChildRule);
              continue;
            }
            continue;
          case Target.RootNode:
            if (isRootNode)
            {
              RelevanceEngine.AddRelevanceRuleAsExpression(expression, rule, isChildRule);
              continue;
            }
            continue;
          case Target.LeafNodes:
            if (expression.IsLeafNode())
            {
              RelevanceEngine.AddRelevanceRuleAsExpression(expression, rule, isChildRule);
              continue;
            }
            continue;
          default:
            continue;
        }
      }
      if (expression.IsLeafNode())
        return;
      foreach (RelevanceExpression child in expression.Children)
        RelevanceEngine.ProcessRulesHelper(child, rules, false, isChildRule);
    }

    private static void AddRelevanceRuleAsExpression(
      RelevanceExpression expression,
      RelevanceRule rule,
      bool isChildRule)
    {
      if (isChildRule)
        expression.AddChildDocumentRelevanceExpression(rule.Evaluate((IExpression) expression));
      else
        expression.AddRelevanceExpression(rule.Evaluate((IExpression) expression));
    }
  }
}
