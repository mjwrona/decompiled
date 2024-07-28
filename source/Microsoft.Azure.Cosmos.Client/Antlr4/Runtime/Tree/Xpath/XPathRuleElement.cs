// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Xpath.XPathRuleElement
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Tree.Xpath
{
  internal class XPathRuleElement : XPathElement
  {
    protected internal int ruleIndex;

    public XPathRuleElement(string ruleName, int ruleIndex)
      : base(ruleName)
    {
      this.ruleIndex = ruleIndex;
    }

    public override ICollection<IParseTree> Evaluate(IParseTree t)
    {
      IList<IParseTree> parseTreeList = (IList<IParseTree>) new List<IParseTree>();
      foreach (ITree child in (IEnumerable<ITree>) Trees.GetChildren((ITree) t))
      {
        if (child is ParserRuleContext)
        {
          ParserRuleContext parserRuleContext = (ParserRuleContext) child;
          if (parserRuleContext.RuleIndex == this.ruleIndex && !this.invert || parserRuleContext.RuleIndex != this.ruleIndex && this.invert)
            parseTreeList.Add((IParseTree) parserRuleContext);
        }
      }
      return (ICollection<IParseTree>) parseTreeList;
    }
  }
}
