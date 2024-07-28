// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Pattern.ParseTreePattern
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree.Xpath;
using System.Collections.Generic;

namespace Antlr4.Runtime.Tree.Pattern
{
  internal class ParseTreePattern
  {
    private readonly int patternRuleIndex;
    [NotNull]
    private readonly string pattern;
    [NotNull]
    private readonly IParseTree patternTree;
    [NotNull]
    private readonly ParseTreePatternMatcher matcher;

    public ParseTreePattern(
      ParseTreePatternMatcher matcher,
      string pattern,
      int patternRuleIndex,
      IParseTree patternTree)
    {
      this.matcher = matcher;
      this.patternRuleIndex = patternRuleIndex;
      this.pattern = pattern;
      this.patternTree = patternTree;
    }

    [return: NotNull]
    public virtual ParseTreeMatch Match(IParseTree tree) => this.matcher.Match(tree, this);

    public virtual bool Matches(IParseTree tree) => this.matcher.Match(tree, this).Succeeded;

    [return: NotNull]
    public virtual IList<ParseTreeMatch> FindAll(IParseTree tree, string xpath)
    {
      ICollection<IParseTree> all1 = XPath.FindAll(tree, xpath, this.matcher.Parser);
      IList<ParseTreeMatch> all2 = (IList<ParseTreeMatch>) new List<ParseTreeMatch>();
      foreach (IParseTree tree1 in (IEnumerable<IParseTree>) all1)
      {
        ParseTreeMatch parseTreeMatch = this.Match(tree1);
        if (parseTreeMatch.Succeeded)
          all2.Add(parseTreeMatch);
      }
      return all2;
    }

    [NotNull]
    public virtual ParseTreePatternMatcher Matcher => this.matcher;

    [NotNull]
    public virtual string Pattern => this.pattern;

    public virtual int PatternRuleIndex => this.patternRuleIndex;

    [NotNull]
    public virtual IParseTree PatternTree => this.patternTree;
  }
}
