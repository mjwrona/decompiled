// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Pattern.ParseTreePatternMatcher
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Tree.Pattern
{
  internal class ParseTreePatternMatcher
  {
    private readonly Lexer lexer;
    private readonly Parser parser;
    protected internal string start = "<";
    protected internal string stop = ">";
    protected internal string escape = "\\";

    public ParseTreePatternMatcher(Lexer lexer, Parser parser)
    {
      this.lexer = lexer;
      this.parser = parser;
    }

    public virtual void SetDelimiters(string start, string stop, string escapeLeft)
    {
      if (string.IsNullOrEmpty(start))
        throw new ArgumentException("start cannot be null or empty");
      if (string.IsNullOrEmpty(stop))
        throw new ArgumentException("stop cannot be null or empty");
      this.start = start;
      this.stop = stop;
      this.escape = escapeLeft;
    }

    public virtual bool Matches(IParseTree tree, string pattern, int patternRuleIndex)
    {
      ParseTreePattern pattern1 = this.Compile(pattern, patternRuleIndex);
      return this.Matches(tree, pattern1);
    }

    public virtual bool Matches(IParseTree tree, ParseTreePattern pattern)
    {
      MultiMap<string, IParseTree> labels = new MultiMap<string, IParseTree>();
      return this.MatchImpl(tree, pattern.PatternTree, labels) == null;
    }

    public virtual ParseTreeMatch Match(IParseTree tree, string pattern, int patternRuleIndex)
    {
      ParseTreePattern pattern1 = this.Compile(pattern, patternRuleIndex);
      return this.Match(tree, pattern1);
    }

    [return: NotNull]
    public virtual ParseTreeMatch Match(IParseTree tree, ParseTreePattern pattern)
    {
      MultiMap<string, IParseTree> labels = new MultiMap<string, IParseTree>();
      IParseTree mismatchedNode = this.MatchImpl(tree, pattern.PatternTree, labels);
      return new ParseTreeMatch(tree, pattern, labels, mismatchedNode);
    }

    public virtual ParseTreePattern Compile(string pattern, int patternRuleIndex)
    {
      CommonTokenStream input = new CommonTokenStream((ITokenSource) new ListTokenSource(this.Tokenize(pattern)));
      ParserInterpreter parserInterpreter = new ParserInterpreter(this.parser.GrammarFileName, this.parser.Vocabulary, (IEnumerable<string>) Arrays.AsList<string>(this.parser.RuleNames), this.parser.GetATNWithBypassAlts(), (ITokenStream) input);
      IParseTree patternTree;
      try
      {
        parserInterpreter.ErrorHandler = (IAntlrErrorStrategy) new BailErrorStrategy();
        patternTree = (IParseTree) parserInterpreter.Parse(patternRuleIndex);
      }
      catch (ParseCanceledException ex)
      {
        throw (RecognitionException) ex.InnerException;
      }
      catch (RecognitionException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new ParseTreePatternMatcher.CannotInvokeStartRule(ex);
      }
      if (input.LA(1) != -1)
        throw new ParseTreePatternMatcher.StartRuleDoesNotConsumeFullPattern();
      return new ParseTreePattern(this, pattern, patternRuleIndex, patternTree);
    }

    [NotNull]
    public virtual Lexer Lexer => this.lexer;

    [NotNull]
    public virtual Parser Parser => this.parser;

    [return: Nullable]
    protected internal virtual IParseTree MatchImpl(
      IParseTree tree,
      IParseTree patternTree,
      MultiMap<string, IParseTree> labels)
    {
      if (tree == null)
        throw new ArgumentException("tree cannot be null");
      if (patternTree == null)
        throw new ArgumentException("patternTree cannot be null");
      switch (tree)
      {
        case ITerminalNode _ when patternTree is ITerminalNode:
          ITerminalNode terminalNode1 = (ITerminalNode) tree;
          ITerminalNode terminalNode2 = (ITerminalNode) patternTree;
          IParseTree parseTree1 = (IParseTree) null;
          if (terminalNode1.Symbol.Type == terminalNode2.Symbol.Type)
          {
            if (terminalNode2.Symbol is TokenTagToken)
            {
              TokenTagToken symbol = (TokenTagToken) terminalNode2.Symbol;
              labels.Map(symbol.TokenName, tree);
              if (symbol.Label != null)
                labels.Map(symbol.Label, tree);
            }
            else if (!terminalNode1.GetText().Equals(terminalNode2.GetText(), StringComparison.Ordinal) && parseTree1 == null)
              parseTree1 = (IParseTree) terminalNode1;
          }
          else if (parseTree1 == null)
            parseTree1 = (IParseTree) terminalNode1;
          return parseTree1;
        case ParserRuleContext _ when patternTree is ParserRuleContext:
          ParserRuleContext parserRuleContext = (ParserRuleContext) tree;
          ParserRuleContext t = (ParserRuleContext) patternTree;
          IParseTree parseTree2 = (IParseTree) null;
          RuleTagToken ruleTagToken = this.GetRuleTagToken((IParseTree) t);
          if (ruleTagToken != null)
          {
            if (parserRuleContext.RuleIndex == t.RuleIndex)
            {
              labels.Map(ruleTagToken.RuleName, tree);
              if (ruleTagToken.Label != null)
                labels.Map(ruleTagToken.Label, tree);
            }
            else if (parseTree2 == null)
              parseTree2 = (IParseTree) parserRuleContext;
            return parseTree2;
          }
          if (parserRuleContext.ChildCount != t.ChildCount)
          {
            if (parseTree2 == null)
              parseTree2 = (IParseTree) parserRuleContext;
            return parseTree2;
          }
          int childCount = parserRuleContext.ChildCount;
          for (int i = 0; i < childCount; ++i)
          {
            IParseTree parseTree3 = this.MatchImpl(parserRuleContext.GetChild(i), patternTree.GetChild(i), labels);
            if (parseTree3 != null)
              return parseTree3;
          }
          return parseTree2;
        default:
          return tree;
      }
    }

    protected internal virtual RuleTagToken GetRuleTagToken(IParseTree t)
    {
      if (t is IRuleNode)
      {
        IRuleNode ruleNode = (IRuleNode) t;
        if (ruleNode.ChildCount == 1 && ruleNode.GetChild(0) is ITerminalNode)
        {
          ITerminalNode child = (ITerminalNode) ruleNode.GetChild(0);
          if (child.Symbol is RuleTagToken)
            return (RuleTagToken) child.Symbol;
        }
      }
      return (RuleTagToken) null;
    }

    public virtual IList<IToken> Tokenize(string pattern)
    {
      IList<Chunk> chunkList = this.Split(pattern);
      IList<IToken> tokenList = (IList<IToken>) new List<IToken>();
      foreach (Chunk chunk in (IEnumerable<Chunk>) chunkList)
      {
        if (chunk is TagChunk)
        {
          TagChunk tagChunk = (TagChunk) chunk;
          if (char.IsUpper(tagChunk.Tag[0]))
          {
            int tokenType = this.parser.GetTokenType(tagChunk.Tag);
            if (tokenType == 0)
              throw new ArgumentException("Unknown token " + tagChunk.Tag + " in pattern: " + pattern);
            TokenTagToken tokenTagToken = new TokenTagToken(tagChunk.Tag, tokenType, tagChunk.Label);
            tokenList.Add((IToken) tokenTagToken);
          }
          else
          {
            if (!char.IsLower(tagChunk.Tag[0]))
              throw new ArgumentException("invalid tag: " + tagChunk.Tag + " in pattern: " + pattern);
            int ruleIndex = this.parser.GetRuleIndex(tagChunk.Tag);
            if (ruleIndex == -1)
              throw new ArgumentException("Unknown rule " + tagChunk.Tag + " in pattern: " + pattern);
            int bypassTokenType = this.parser.GetATNWithBypassAlts().ruleToTokenType[ruleIndex];
            tokenList.Add((IToken) new RuleTagToken(tagChunk.Tag, bypassTokenType, tagChunk.Label));
          }
        }
        else
        {
          this.lexer.SetInputStream((ICharStream) new AntlrInputStream(((TextChunk) chunk).Text));
          for (IToken token = this.lexer.NextToken(); token.Type != -1; token = this.lexer.NextToken())
            tokenList.Add(token);
        }
      }
      return tokenList;
    }

    internal virtual IList<Chunk> Split(string pattern)
    {
      int startIndex = 0;
      int length = pattern.Length;
      IList<Chunk> list = (IList<Chunk>) new List<Chunk>();
      IList<int> intList1 = (IList<int>) new List<int>();
      IList<int> intList2 = (IList<int>) new List<int>();
      while (startIndex < length)
      {
        if (startIndex == pattern.IndexOf(this.escape + this.start, startIndex))
          startIndex += this.escape.Length + this.start.Length;
        else if (startIndex == pattern.IndexOf(this.escape + this.stop, startIndex))
          startIndex += this.escape.Length + this.stop.Length;
        else if (startIndex == pattern.IndexOf(this.start, startIndex))
        {
          intList1.Add(startIndex);
          startIndex += this.start.Length;
        }
        else if (startIndex == pattern.IndexOf(this.stop, startIndex))
        {
          intList2.Add(startIndex);
          startIndex += this.stop.Length;
        }
        else
          ++startIndex;
      }
      if (intList1.Count > intList2.Count)
        throw new ArgumentException("unterminated tag in pattern: " + pattern);
      if (intList1.Count < intList2.Count)
        throw new ArgumentException("missing start tag in pattern: " + pattern);
      int count = intList1.Count;
      for (int index = 0; index < count; ++index)
      {
        if (intList1[index] >= intList2[index])
          throw new ArgumentException("tag delimiters out of order in pattern: " + pattern);
      }
      if (count == 0)
      {
        string text = Antlr4.Runtime.Sharpen.Runtime.Substring(pattern, 0, length);
        list.Add((Chunk) new TextChunk(text));
      }
      if (count > 0 && intList1[0] > 0)
      {
        string text = Antlr4.Runtime.Sharpen.Runtime.Substring(pattern, 0, intList1[0]);
        list.Add((Chunk) new TextChunk(text));
      }
      for (int index = 0; index < count; ++index)
      {
        string str = Antlr4.Runtime.Sharpen.Runtime.Substring(pattern, intList1[index] + this.start.Length, intList2[index]);
        string tag = str;
        string label = (string) null;
        int endOffset = str.IndexOf(':');
        if (endOffset >= 0)
        {
          label = Antlr4.Runtime.Sharpen.Runtime.Substring(str, 0, endOffset);
          tag = Antlr4.Runtime.Sharpen.Runtime.Substring(str, endOffset + 1, str.Length);
        }
        list.Add((Chunk) new TagChunk(label, tag));
        if (index + 1 < count)
        {
          string text = Antlr4.Runtime.Sharpen.Runtime.Substring(pattern, intList2[index] + this.stop.Length, intList1[index + 1]);
          list.Add((Chunk) new TextChunk(text));
        }
      }
      if (count > 0)
      {
        int beginOffset = intList2[count - 1] + this.stop.Length;
        if (beginOffset < length)
        {
          string text = Antlr4.Runtime.Sharpen.Runtime.Substring(pattern, beginOffset, length);
          list.Add((Chunk) new TextChunk(text));
        }
      }
      for (int index = 0; index < list.Count; ++index)
      {
        Chunk chunk = list[index];
        if (chunk is TextChunk)
        {
          TextChunk textChunk = (TextChunk) chunk;
          string text = textChunk.Text.Replace(this.escape, string.Empty);
          if (text.Length < textChunk.Text.Length)
            list.Set<Chunk>(index, (Chunk) new TextChunk(text));
        }
      }
      return list;
    }

    [Serializable]
    internal class CannotInvokeStartRule : Exception
    {
      public CannotInvokeStartRule(Exception e)
        : base(e.Message, e)
      {
      }
    }

    [Serializable]
    internal class StartRuleDoesNotConsumeFullPattern : Exception
    {
    }
  }
}
