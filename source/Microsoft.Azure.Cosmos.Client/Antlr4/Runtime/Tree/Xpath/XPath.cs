// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Xpath.XPath
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antlr4.Runtime.Tree.Xpath
{
  internal class XPath
  {
    public const string Wildcard = "*";
    public const string Not = "!";
    protected internal string path;
    protected internal XPathElement[] elements;
    protected internal Parser parser;

    public XPath(Parser parser, string path)
    {
      this.parser = parser;
      this.path = path;
      this.elements = this.Split(path);
    }

    public virtual XPathElement[] Split(string path)
    {
      AntlrInputStream baseArg1;
      try
      {
        baseArg1 = new AntlrInputStream((TextReader) new StringReader(path));
      }
      catch (IOException ex)
      {
        throw new ArgumentException("Could not read path: " + path, (Exception) ex);
      }
      XPathLexer xpathLexer = (XPathLexer) new XPath._XPathLexer_87((ICharStream) baseArg1);
      xpathLexer.RemoveErrorListeners();
      xpathLexer.AddErrorListener((IAntlrErrorListener<int>) new XPathLexerErrorListener());
      CommonTokenStream commonTokenStream = new CommonTokenStream((ITokenSource) xpathLexer);
      try
      {
        commonTokenStream.Fill();
      }
      catch (LexerNoViableAltException ex)
      {
        throw new ArgumentException("Invalid tokens or characters at index " + xpathLexer.Column.ToString() + " in path '" + path + "'", (Exception) ex);
      }
      IList<IToken> tokens = commonTokenStream.GetTokens();
      IList<XPathElement> source = (IList<XPathElement>) new List<XPathElement>();
      int count = tokens.Count;
      int index1 = 0;
      while (index1 < count)
      {
        IToken wordToken1 = tokens[index1];
        switch (wordToken1.Type)
        {
          case -1:
            goto label_13;
          case 1:
          case 2:
          case 5:
            source.Add(this.GetXPathElement(wordToken1, false));
            ++index1;
            continue;
          case 3:
          case 4:
            bool anywhere = wordToken1.Type == 3;
            int index2 = index1 + 1;
            IToken wordToken2 = tokens[index2];
            bool flag = wordToken2.Type == 6;
            if (flag)
            {
              ++index2;
              wordToken2 = tokens[index2];
            }
            XPathElement xpathElement = this.GetXPathElement(wordToken2, anywhere);
            xpathElement.invert = flag;
            source.Add(xpathElement);
            index1 = index2 + 1;
            continue;
          default:
            throw new ArgumentException("Unknowth path element " + wordToken1?.ToString());
        }
      }
label_13:
      return source.ToArray<XPathElement>();
    }

    protected internal virtual XPathElement GetXPathElement(IToken wordToken, bool anywhere)
    {
      string str = wordToken.Type != -1 ? wordToken.Text : throw new ArgumentException("Missing path element at end of path");
      int tokenType = this.parser.GetTokenType(str);
      int ruleIndex = this.parser.GetRuleIndex(str);
      switch (wordToken.Type)
      {
        case 1:
        case 8:
          if (tokenType == 0)
            throw new ArgumentException(str + " at index " + wordToken.StartIndex.ToString() + " isn't a valid token name");
          return !anywhere ? (XPathElement) new XPathTokenElement(str, tokenType) : (XPathElement) new XPathTokenAnywhereElement(str, tokenType);
        case 5:
          return !anywhere ? (XPathElement) new XPathWildcardElement() : (XPathElement) new XPathWildcardAnywhereElement();
        default:
          if (ruleIndex == -1)
            throw new ArgumentException(str + " at index " + wordToken.StartIndex.ToString() + " isn't a valid rule name");
          return !anywhere ? (XPathElement) new XPathRuleElement(str, ruleIndex) : (XPathElement) new XPathRuleAnywhereElement(str, ruleIndex);
      }
    }

    public static ICollection<IParseTree> FindAll(IParseTree tree, string xpath, Parser parser) => new XPath(parser, xpath).Evaluate(tree);

    public virtual ICollection<IParseTree> Evaluate(IParseTree t)
    {
      ICollection<IParseTree> parseTrees1 = (ICollection<IParseTree>) new ParserRuleContext[1]
      {
        new ParserRuleContext()
        {
          children = (IList<IParseTree>) Antlr4.Runtime.Sharpen.Collections.SingletonList<IParseTree>(t)
        }
      };
      int index = 0;
      while (index < this.elements.Length)
      {
        HashSet<IParseTree> parseTreeSet = new HashSet<IParseTree>();
        ICollection<IParseTree> parseTrees2 = (ICollection<IParseTree>) new List<IParseTree>();
        foreach (IParseTree t1 in (IEnumerable<IParseTree>) parseTrees1)
        {
          if (t1.ChildCount > 0)
          {
            foreach (IParseTree parseTree in (IEnumerable<IParseTree>) this.elements[index].Evaluate(t1))
            {
              if (parseTreeSet.Add(parseTree))
                parseTrees2.Add(parseTree);
            }
          }
        }
        ++index;
        parseTrees1 = parseTrees2;
      }
      return parseTrees1;
    }

    private sealed class _XPathLexer_87 : XPathLexer
    {
      public _XPathLexer_87(ICharStream baseArg1)
        : base(baseArg1)
      {
      }

      public override void Recover(LexerNoViableAltException e) => throw e;
    }
  }
}
