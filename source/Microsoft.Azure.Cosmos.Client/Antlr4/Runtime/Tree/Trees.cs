// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Trees
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime.Tree
{
  internal class Trees
  {
    public static string ToStringTree(ITree t) => Trees.ToStringTree(t, (IList<string>) null);

    public static string ToStringTree(ITree t, Parser recog)
    {
      string[] ruleNames1 = recog?.RuleNames;
      IList<string> ruleNames2 = ruleNames1 != null ? Arrays.AsList<string>(ruleNames1) : (IList<string>) null;
      return Trees.ToStringTree(t, ruleNames2);
    }

    public static string ToStringTree(ITree t, IList<string> ruleNames)
    {
      string stringTree = Utils.EscapeWhitespace(Trees.GetNodeText(t, ruleNames), false);
      if (t.ChildCount == 0)
        return stringTree;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(");
      string str = Utils.EscapeWhitespace(Trees.GetNodeText(t, ruleNames), false);
      stringBuilder.Append(str);
      stringBuilder.Append(' ');
      for (int i = 0; i < t.ChildCount; ++i)
      {
        if (i > 0)
          stringBuilder.Append(' ');
        stringBuilder.Append(Trees.ToStringTree(t.GetChild(i), ruleNames));
      }
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    public static string GetNodeText(ITree t, Parser recog)
    {
      string[] ruleNames1 = recog?.RuleNames;
      IList<string> ruleNames2 = ruleNames1 != null ? Arrays.AsList<string>(ruleNames1) : (IList<string>) null;
      return Trees.GetNodeText(t, ruleNames2);
    }

    public static string GetNodeText(ITree t, IList<string> ruleNames)
    {
      if (ruleNames != null)
      {
        switch (t)
        {
          case RuleContext _:
            int ruleIndex = ((RuleContext) t).RuleIndex;
            string ruleName = ruleNames[ruleIndex];
            int altNumber = ((RuleContext) t).getAltNumber();
            return altNumber != 0 ? ruleName + ":" + altNumber.ToString() : ruleName;
          case IErrorNode _:
            return t.ToString();
          case ITerminalNode _:
            IToken symbol = ((ITerminalNode) t).Symbol;
            if (symbol != null)
              return symbol.Text;
            break;
        }
      }
      object payload = t.Payload;
      return payload is IToken ? ((IToken) payload).Text : t.Payload.ToString();
    }

    public static IList<ITree> GetChildren(ITree t)
    {
      IList<ITree> children = (IList<ITree>) new List<ITree>();
      for (int i = 0; i < t.ChildCount; ++i)
        children.Add(t.GetChild(i));
      return children;
    }

    [return: NotNull]
    public static IList<ITree> GetAncestors(ITree t)
    {
      if (t.Parent == null)
        return (IList<ITree>) Antlr4.Runtime.Sharpen.Collections.EmptyList<ITree>();
      IList<ITree> ancestors = (IList<ITree>) new List<ITree>();
      for (t = t.Parent; t != null; t = t.Parent)
        ancestors.Insert(0, t);
      return ancestors;
    }

    public static ICollection<IParseTree> FindAllTokenNodes(IParseTree t, int ttype) => (ICollection<IParseTree>) Trees.FindAllNodes(t, ttype, true);

    public static ICollection<IParseTree> FindAllRuleNodes(IParseTree t, int ruleIndex) => (ICollection<IParseTree>) Trees.FindAllNodes(t, ruleIndex, false);

    public static IList<IParseTree> FindAllNodes(IParseTree t, int index, bool findTokens)
    {
      IList<IParseTree> nodes = (IList<IParseTree>) new List<IParseTree>();
      Trees._findAllNodes(t, index, findTokens, nodes);
      return nodes;
    }

    private static void _findAllNodes(
      IParseTree t,
      int index,
      bool findTokens,
      IList<IParseTree> nodes)
    {
      if (findTokens && t is ITerminalNode)
      {
        if (((ITerminalNode) t).Symbol.Type == index)
          nodes.Add(t);
      }
      else if (!findTokens && t is ParserRuleContext && ((RuleContext) t).RuleIndex == index)
        nodes.Add(t);
      for (int i = 0; i < t.ChildCount; ++i)
        Trees._findAllNodes(t.GetChild(i), index, findTokens, nodes);
    }

    public static IList<IParseTree> Descendants(IParseTree t)
    {
      List<IParseTree> parseTreeList = new List<IParseTree>();
      parseTreeList.Add(t);
      int childCount = t.ChildCount;
      for (int i = 0; i < childCount; ++i)
        parseTreeList.AddRange((IEnumerable<IParseTree>) Trees.Descendants(t.GetChild(i)));
      return (IList<IParseTree>) parseTreeList;
    }

    private Trees()
    {
    }
  }
}
