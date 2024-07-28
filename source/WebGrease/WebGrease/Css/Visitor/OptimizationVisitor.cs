// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.OptimizationVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.Visitor
{
  internal class OptimizationVisitor : NodeVisitor
  {
    internal IEnumerable<string> NonMergeRuleSetSelectors { get; set; }

    internal bool ShouldMergeMediaQueries { get; set; }

    internal bool ShouldMergeBasedOnCommonDeclarations { get; set; }

    internal bool ShouldPreventOrderBasedConflict { get; set; }

    public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
    {
      if (styleSheet == null)
        return (AstNode) null;
      List<StyleSheetRuleNode> list = this.GetMergedNodeDictionary((IEnumerable<StyleSheetRuleNode>) styleSheet.StyleSheetRules).Values.Cast<StyleSheetRuleNode>().ToList<StyleSheetRuleNode>();
      return (AstNode) new StyleSheetNode(styleSheet.CharSetString, styleSheet.Imports, styleSheet.Namespaces, list.AsSafeReadOnly<StyleSheetRuleNode>());
    }

    private static void OptimizeRulesetNode(
      RulesetNode currentRuleSet,
      OrderedDictionary ruleSetMediaPageDictionary,
      OrderedDictionary rulesetHashKeysDictionary,
      bool shouldPreventOrderBasedConflict)
    {
      string key = currentRuleSet.PrintSelector();
      string str = currentRuleSet.PrintSelector();
      if (rulesetHashKeysDictionary.Contains((object) key))
      {
        str = ((IEnumerable<string>) rulesetHashKeysDictionary[(object) key]).Last<string>();
      }
      else
      {
        rulesetHashKeysDictionary.Add((object) key, (object) new List<string>());
        (rulesetHashKeysDictionary[(object) key] as List<string>).Add(key);
      }
      if (OptimizationVisitor.ShouldCollapseTheNewRuleset(str, ruleSetMediaPageDictionary, currentRuleSet, shouldPreventOrderBasedConflict))
      {
        RulesetNode rulesetNode = OptimizationVisitor.MergeDeclarations((RulesetNode) ruleSetMediaPageDictionary[(object) str], currentRuleSet);
        ruleSetMediaPageDictionary.Remove((object) str);
        ruleSetMediaPageDictionary.Add((object) str, (object) rulesetNode);
      }
      else
      {
        RulesetNode rulesetNode = OptimizationVisitor.OptimizeRuleset(currentRuleSet);
        if (rulesetNode == null)
          return;
        while (ruleSetMediaPageDictionary.Contains((object) str))
          str = OptimizationVisitor.GenerateRandomkey();
        ruleSetMediaPageDictionary.Add((object) str, (object) rulesetNode);
        (rulesetHashKeysDictionary[(object) key] as List<string>).Add(str);
      }
    }

    private static void MergeBasedOnCommonDeclarations(
      RulesetNode currentRuleSet,
      OrderedDictionary ruleSetMediaPageDictionary)
    {
      string key1 = currentRuleSet.PrintSelector();
      IEnumerator enumerator = ruleSetMediaPageDictionary.Keys.GetEnumerator();
      object ruleSetMediaPage1 = ruleSetMediaPageDictionary[(object) key1];
      OrderedDictionary orderedDictionary = new OrderedDictionary();
      bool flag = false;
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
        object ruleSetMediaPage2 = ruleSetMediaPageDictionary[current];
        if (ruleSetMediaPageDictionary.Contains((object) key1) && ruleSetMediaPage1.GetType().IsAssignableFrom(ruleSetMediaPage2.GetType()) && !current.Equals((object) key1))
        {
          RulesetNode otherRulesetNode = (RulesetNode) ruleSetMediaPage2;
          RulesetNode rulesetNode = (RulesetNode) ruleSetMediaPage1;
          if (otherRulesetNode.ShouldMergeWith(rulesetNode))
          {
            RulesetNode mergedRulesetNode = rulesetNode.GetMergedRulesetNode(otherRulesetNode);
            string key2 = mergedRulesetNode.PrintSelector();
            if (rulesetNode.Declarations.Count < 1)
              flag = true;
            if (otherRulesetNode.Declarations.Count < 1)
              orderedDictionary.Add(current, (object) null);
            while (ruleSetMediaPageDictionary.Contains((object) key2) || orderedDictionary.Contains((object) key2))
              key2 = OptimizationVisitor.GenerateRandomkey();
            orderedDictionary.Add((object) key2, (object) mergedRulesetNode);
          }
        }
      }
      if (flag)
        orderedDictionary.Add((object) key1, (object) null);
      foreach (object key3 in (IEnumerable) orderedDictionary.Keys)
      {
        if (orderedDictionary[key3] == null)
          ruleSetMediaPageDictionary.Remove(key3);
        else
          ruleSetMediaPageDictionary.Add(key3, orderedDictionary[key3]);
      }
    }

    private static string GenerateRandomkey()
    {
      string element = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      Random random = new Random();
      return new string(Enumerable.Repeat<string>(element, 16).Select<string, char>((Func<string, char>) (s => s[random.Next(s.Length)])).ToArray<char>());
    }

    private static bool ShouldCollapseTheNewRuleset(
      string hashKey,
      OrderedDictionary ruleSetMediaPageDictionary,
      RulesetNode currentRuleSet,
      bool shouldPreventOrderBasedConflict)
    {
      if (!ruleSetMediaPageDictionary.Contains((object) hashKey))
        return false;
      if (!shouldPreventOrderBasedConflict)
        return true;
      OrderedDictionary declarationDictionary = new OrderedDictionary();
      List<StyleSheetRuleNode> list = ruleSetMediaPageDictionary.Values.Cast<StyleSheetRuleNode>().ToList<StyleSheetRuleNode>();
      list.Reverse();
      foreach (StyleSheetRuleNode styleSheetRuleNode in list)
      {
        if (currentRuleSet.GetType().IsAssignableFrom(styleSheetRuleNode.GetType()))
        {
          RulesetNode rulesetNode = styleSheetRuleNode as RulesetNode;
          if (rulesetNode.PrintSelector().Equals(currentRuleSet.PrintSelector()))
            return true;
          foreach (DeclarationNode declaration in rulesetNode.Declarations)
          {
            string property = declaration.Property;
            if (!declarationDictionary.Contains((object) property))
              declarationDictionary[(object) property] = (object) declaration;
          }
          if (((RulesetNode) ruleSetMediaPageDictionary[(object) hashKey]).HasConflictingDeclaration(declarationDictionary))
            return false;
        }
      }
      return true;
    }

    private static RulesetNode MergeDeclarations(
      RulesetNode sourceRuleset,
      RulesetNode destinationRuleset)
    {
      OrderedDictionary orderedDictionary = OptimizationVisitor.UniqueDeclarations(destinationRuleset);
      OrderedDictionary uniqueSourceDeclarations = OptimizationVisitor.UniqueDeclarations(sourceRuleset);
      foreach (DeclarationNode newDeclaration in (IEnumerable) orderedDictionary.Values)
        OptimizationVisitor.AddDeclaration(uniqueSourceDeclarations, newDeclaration);
      List<DeclarationNode> list = uniqueSourceDeclarations.Values.Cast<DeclarationNode>().ToList<DeclarationNode>();
      return new RulesetNode(destinationRuleset.SelectorsGroupNode, list.AsReadOnly(), sourceRuleset.ImportantComments);
    }

    private static void AddDeclaration(
      OrderedDictionary uniqueSourceDeclarations,
      DeclarationNode newDeclaration)
    {
      string uniquePropertyKey = OptimizationVisitor.GetUniquePropertyKey(newDeclaration);
      if (uniqueSourceDeclarations.Contains((object) uniquePropertyKey))
      {
        if (OptimizationVisitor.HasImportantFlag(uniqueSourceDeclarations[(object) uniquePropertyKey] as DeclarationNode) && !OptimizationVisitor.HasImportantFlag(newDeclaration))
          return;
        uniqueSourceDeclarations.Remove((object) uniquePropertyKey);
      }
      uniqueSourceDeclarations.Add((object) uniquePropertyKey, (object) newDeclaration);
    }

    private static bool HasImportantFlag(DeclarationNode declarationNode) => declarationNode.Prio.Equals("!important");

    private static string GetUniquePropertyKey(DeclarationNode declarationNode)
    {
      string property = declarationNode.Property;
      if (!string.IsNullOrWhiteSpace(OptimizationVisitor.GetVendorPrefix(property)))
        return property;
      string stringBasedValue = declarationNode.ExprNode.TermNode.StringBasedValue;
      if (!string.IsNullOrWhiteSpace(stringBasedValue))
      {
        string vendorPrefix = OptimizationVisitor.GetVendorPrefix(stringBasedValue);
        if (!string.IsNullOrWhiteSpace(vendorPrefix))
          return vendorPrefix + property;
      }
      return property;
    }

    private static string GetVendorPrefix(string stringBasedValue)
    {
      if (stringBasedValue.StartsWith("-", StringComparison.OrdinalIgnoreCase))
      {
        int num = stringBasedValue.IndexOf("-", 2, StringComparison.OrdinalIgnoreCase);
        if (num < stringBasedValue.Length - 1)
          return stringBasedValue.Substring(0, num + 1);
      }
      return (string) null;
    }

    private static OrderedDictionary UniqueDeclarations(RulesetNode rulesetNode)
    {
      OrderedDictionary uniqueSourceDeclarations = new OrderedDictionary();
      foreach (DeclarationNode declaration in rulesetNode.Declarations)
        OptimizationVisitor.AddDeclaration(uniqueSourceDeclarations, declaration);
      return uniqueSourceDeclarations;
    }

    private static RulesetNode OptimizeRuleset(RulesetNode rulesetNode)
    {
      if (rulesetNode.Declarations.Count == 0)
        return (RulesetNode) null;
      List<DeclarationNode> list = OptimizationVisitor.UniqueDeclarations(rulesetNode).Values.Cast<DeclarationNode>().ToList<DeclarationNode>();
      return new RulesetNode(rulesetNode.SelectorsGroupNode, list.AsReadOnly(), rulesetNode.ImportantComments);
    }

    private OrderedDictionary GetMergedNodeDictionary(
      IEnumerable<StyleSheetRuleNode> styleSheetRuleNodes)
    {
      OrderedDictionary ruleSetMediaPageDictionary = new OrderedDictionary();
      OrderedDictionary rulesetHashKeysDictionary = new OrderedDictionary();
      foreach (StyleSheetRuleNode styleSheetRuleNode in styleSheetRuleNodes)
      {
        RulesetNode rulesetNode = styleSheetRuleNode as RulesetNode;
        if (rulesetNode != null && (this.NonMergeRuleSetSelectors == null || !this.NonMergeRuleSetSelectors.Any<string>((Func<string, bool>) (r => r.Equals(rulesetNode.PrintSelector(), StringComparison.OrdinalIgnoreCase)))))
          OptimizationVisitor.OptimizeRulesetNode(rulesetNode, ruleSetMediaPageDictionary, rulesetHashKeysDictionary, this.ShouldPreventOrderBasedConflict);
        else if (this.ShouldMergeMediaQueries && styleSheetRuleNode is MediaNode mediaNode)
        {
          this.OptimizeMediaQuery(mediaNode, ruleSetMediaPageDictionary);
        }
        else
        {
          string key = styleSheetRuleNode.MinifyPrint();
          if (!ruleSetMediaPageDictionary.Contains((object) key))
            ruleSetMediaPageDictionary.Add((object) key, (object) styleSheetRuleNode);
          else
            ruleSetMediaPageDictionary[(object) key] = (object) styleSheetRuleNode;
        }
      }
      if (this.ShouldMergeBasedOnCommonDeclarations)
      {
        foreach (StyleSheetRuleNode styleSheetRuleNode in styleSheetRuleNodes)
        {
          if (styleSheetRuleNode is RulesetNode currentRuleSet)
            OptimizationVisitor.MergeBasedOnCommonDeclarations(currentRuleSet, ruleSetMediaPageDictionary);
        }
      }
      return ruleSetMediaPageDictionary;
    }

    private void OptimizeMediaQuery(
      MediaNode mediaNode,
      OrderedDictionary ruleSetMediaPageDictionary)
    {
      string key = mediaNode.PrintSelector();
      List<PageNode> list1 = mediaNode.PageNodes.ToList<PageNode>();
      List<RulesetNode> list2 = mediaNode.Rulesets.ToList<RulesetNode>();
      if (ruleSetMediaPageDictionary.Contains((object) key))
      {
        if (ruleSetMediaPageDictionary[(object) key] is MediaNode ruleSetMediaPage)
        {
          list1 = ruleSetMediaPage.PageNodes.Concat<PageNode>((IEnumerable<PageNode>) list1).ToList<PageNode>();
          list2 = ruleSetMediaPage.Rulesets.Concat<RulesetNode>((IEnumerable<RulesetNode>) list2).ToList<RulesetNode>();
        }
        ruleSetMediaPageDictionary.Remove((object) key);
      }
      ruleSetMediaPageDictionary.Add((object) key, (object) new MediaNode(mediaNode.MediaQueries, this.GetMergedNodeDictionary((IEnumerable<StyleSheetRuleNode>) list2).Values.Cast<RulesetNode>().ToList<RulesetNode>().AsSafeReadOnly<RulesetNode>(), list1.ToSafeReadOnlyCollection<PageNode>()));
    }
  }
}
